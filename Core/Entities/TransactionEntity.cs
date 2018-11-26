using Agrishare.Core.Utils;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Agrishare.Core.Entities
{
    public partial class Transaction : IEntity
    {
        public static decimal AgriShareCommission => Convert.ToDecimal(Config.Find(Key: "AgriShare Commission").Value);

        public static string DefaultSort = "DateCreated DESC";
        public string Title => Id.ToString().PadLeft(8, '0');
        public string Status => $"{StatusId}".ExplodeCamelCase();

        public static Transaction Find(int Id = 0, string ClientCorrelator = "")
        {
            if (Id == 0 && ClientCorrelator.IsEmpty())
                return new Transaction
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Transactions.Include(o => o.BookingUser).Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);
                if (!ClientCorrelator.IsEmpty())
                    query = query.Where(e => e.ClientCorrelator == ClientCorrelator);

                return query.FirstOrDefault();
            }
        }

        public static List<Transaction> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", 
            int BookingId = 0, int BookingUserId = 0, TransactionStatus StatusId = TransactionStatus.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Transactions.Include(o => o.BookingUser).Where(o => !o.Deleted);

                if (BookingId > 0)
                    query = query.Where(o => o.BookingId == BookingId);

                if (BookingUserId > 0)
                    query = query.Where(o => o.BookingUserId == BookingUserId);

                if (StatusId != TransactionStatus.None)
                    query = query.Where(o => o.StatusId == StatusId);

                query = query.OrderBy(Sort.Coalesce(DefaultSort));

                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(int BookingId = 0, int BookingUserId = 0, TransactionStatus StatusId = TransactionStatus.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Transactions.Where(o => !o.Deleted);

                if (BookingId > 0)
                    query = query.Where(o => o.BookingId == BookingId);

                if (BookingUserId > 0)
                    query = query.Where(o => o.BookingUserId == BookingUserId);

                if (StatusId != TransactionStatus.None)
                    query = query.Where(o => o.StatusId == StatusId);

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var booking = Booking;
            if (booking != null) BookingId = booking.Id;
            Booking = null;

            var bookingUser = BookingUser;
            if (bookingUser != null) BookingUserId = bookingUser.Id;
            BookingUser = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Booking = booking;
            BookingUser = bookingUser;

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Transactions.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Transactions.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        public bool Delete()
        {
            if (Id == 0)
                return false;

            Deleted = true;
            return Update();
        }

        public object Json()
        {
            return new
            {
                Id,
                BookingId,
                BookingUser = BookingUser?.Json(),
                Reference,
                Amount,
                StatusId,
                Status,
                DateCreated
            };
        }

        private void SendPaymentNotification()
        {
            //TODO send payment notification
        }

        #region EcoCash

        private static string EcoCashUrl => Config.Find(Key: "EcoCash URL").Value;
        private static string EcoCashMerchantCode => Config.Find(Key: "EcoCash Merchant Code").Value;
        private static string EcoCashMerchantPin => Config.Find(Key: "EcoCash Merchant Pin").Value;
        private static string EcoCashMerchantNumber => Config.Find(Key: "EcoCash Merchant Number").Value;
        private static string EcoCashUsername => Config.Find(Key: "EcoCash Username").Value;
        private static string EcoCashPassword => Config.Find(Key: "EcoCash Password").Value;
        private static bool EcoCashLog => Config.Find(Key: "EcoCash Log").Value.Equals("True", StringComparison.InvariantCultureIgnoreCase);

        public bool RequestEcoCashPayment()
        {
            ClientCorrelator = Guid.NewGuid().ToString();
            Save();

            var resourceUri = $"{EcoCashUrl}transactions/amount";
            
            var json = JsonConvert.SerializeObject(new
            {
                clientCorrelator = ClientCorrelator,
                endUserId = BookingUser.Telephone,
                merchantCode = EcoCashMerchantCode,
                merchantPin = EcoCashMerchantPin,
                merchantNumber = EcoCashMerchantNumber,
                notifyUrl = $"{Config.APIURL}transactions/ecocash/notify",
                paymentAmount = new
                {
                    charginginformation = new
                    {
                        amount = Amount,
                        currency = "USD",
                        description = Booking?.Title ?? "Booking"
                    },
                    chargeMetaData = new
                    {
                        onBeHalfOf = Config.ApplicationName,
                        purchaseCategoryCode = "Booking",
                        channel = "WEB"
                    }
                },
                referenceCode = Title,
                transactionOperationStatus = "Charged"
            });

            if (EcoCashLog)
                Log += resourceUri + Environment.NewLine + json;

            var client = new RestClient(resourceUri)
            {
                Authenticator = new HttpBasicAuthenticator(EcoCashUsername, EcoCashPassword)
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", json, ParameterType.RequestBody);
            var response = client.Execute<dynamic>(request);

            if (EcoCashLog)
                Log += Environment.NewLine + Environment.NewLine + JsonConvert.SerializeObject(response);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (response.Data.messageId != null)
                {
                    StatusId = TransactionStatus.Error;
                    Save();
                    return false;
                }
                else
                {
                    Reference = response.Data.serverReferenceCode;
                    StatusId = TransactionStatus.PendingSubscriberValidation;
                    Save();
                    return true;
                }
            }
            else
            {
                StatusId = TransactionStatus.Failed;
                Save();
                return false;
            }
        }

        public void RequestEcoCashStatus()
        {
            var previousStatusId = StatusId;

            var resourceUri = $"{EcoCashUrl}{BookingUser.Telephone}/transactions/amount/{ClientCorrelator}";

            var client = new RestClient(resourceUri)
            {
                Authenticator = new HttpBasicAuthenticator(EcoCashUsername, EcoCashPassword)
            };

            var request = new RestRequest(Method.GET);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            var response = client.Execute<dynamic>(request);

            if (EcoCashLog)
                Log += Environment.NewLine + Environment.NewLine + JsonConvert.SerializeObject(response);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (response.Data.messageId != null)
                {
                    StatusId = TransactionStatus.Error;
                    Save();
                    return;
                }

                if (response.Data.transactionOperationStatus == "Charged" || response.Data.transactionOperationStatus == "COMPLETED")
                    StatusId = TransactionStatus.Paid;
                else if (response.Data.transactionOperationStatus == "PENDING SUBSCRIBER VALIDATION")
                        StatusId = TransactionStatus.PendingSubscriberValidation;
                else
                    StatusId = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), response.Data.transactionOperationStatus);

                Reference = response.Data.serverReferenceCode;
            }
            else
                StatusId = TransactionStatus.Failed;

            Save();

            if (previousStatusId != TransactionStatus.Paid && StatusId == TransactionStatus.Paid)
            {
                var booking = Booking.Find(BookingId);
                booking.StatusId = BookingStatus.InProgress;
                booking.Save();

                new Notification
                {
                    Booking = booking,
                    GroupId = NotificationGroup.Offering,
                    TypeId = NotificationType.PaymentReceived,
                    User = User.Find(Id: booking.Listing.UserId)
                }.Save(Notify: true);

                new Notification
                {
                    Booking = booking,
                    GroupId = NotificationGroup.Seeking,
                    TypeId = NotificationType.PaymentReceived,
                    User = User.Find(Id: booking.UserId)
                }.Save(Notify: false);

                Counter.Hit(BookingUser.UserId ?? 0, Counters.CompletePayment, Booking.ServiceId);
                SendPaymentNotification();
            }
        }

        public bool RequestEcoCashRefund()
        {
            Id = 0;
            StatusId = TransactionStatus.Pending;

            var resourceUri = $"{EcoCashUrl}transactions/refund";

            var json = JsonConvert.SerializeObject(new
            {
                clientCorrelator = ClientCorrelator,
                endUserId = BookingUser.Telephone,
                merchantCode = EcoCashMerchantCode,
                merchantPin = EcoCashMerchantPin,
                merchantNumber = EcoCashMerchantNumber,
                notifyUrl = $"{Config.APIURL}transactions/ecocash/notify",
                paymentAmount = new
                {
                    charginginformation = new
                    {
                        amount = Amount,
                        currency = "USD",
                        description = Booking?.Title ?? "Booking"
                    },
                    chargeMetaData = new
                    {
                        onBeHalfOf = Config.ApplicationName,
                        purchaseCategoryCode = "Booking",
                        channel = "WEB"
                    }
                },
                referenceCode = Title,
                transactionOperationStatus = "Refunded",
                tranType = "REF"
            });

            if (EcoCashLog)
                Log += resourceUri + Environment.NewLine + json;

            var client = new RestClient(resourceUri)
            {
                Authenticator = new HttpBasicAuthenticator(EcoCashUsername, EcoCashPassword)
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", json, ParameterType.RequestBody);
            var response = client.Execute<dynamic>(request);

            if (EcoCashLog)
                Log += Environment.NewLine + Environment.NewLine + JsonConvert.SerializeObject(response);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (response.Data.messageId != null)
                {
                    StatusId = TransactionStatus.Error;
                    Save();
                    return false;
                }

                if (response.Data.transactionOperationStatus == "Charged" || response.Data.transactionOperationStatus == "COMPLETED")
                    StatusId = TransactionStatus.Paid;
                else if (response.Data.transactionOperationStatus == "PENDING SUBSCRIBER VALIDATION")
                    StatusId = TransactionStatus.PendingSubscriberValidation;
                else
                    StatusId = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), response.Data.transactionOperationStatus);
                Save();

                return true;
            }
            else
            {
                StatusId = TransactionStatus.Failed;
                Save();

                return false;
            }
        }

        #endregion
    }
}
