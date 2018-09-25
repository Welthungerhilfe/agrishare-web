using Agrishare.Core.Utils;
using Newtonsoft.Json;
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
        public static string DefaultSort = "DateCreated DESC";
        public string Title => Id.ToString().PadLeft(8, '0');
        public string Status => $"{StatusId}".ExplodeCamelCase();

        public static Transaction Find(int Id = 0)
        {
            if (Id == 0)
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
        private static bool EcoCashLog => Config.Find(Key: "EcoCash Log").Value.Equals("True", StringComparison.InvariantCultureIgnoreCase);

        public void RequestEcoCashPayment()
        {
            Save();

            var resourceUri = $"{EcoCashUrl}transactions/amount";
            
            var json = JsonConvert.SerializeObject(new
            {
                clientCorrelator = Id,
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
                        channel = "Web"
                    }
                },
                referenceCode = Title,
                transactionOperationStatus = "Charged"
            });

            if (EcoCashLog)
                Log += resourceUri + Environment.NewLine + json;


            //bool success = false;
            //string response = Utilities.JsonRequestBack("POST", resourceUri, json, out success);
            //Audit += Environment.NewLine + response;

            //if (!Config.ApplicationLive)
            //    GlooEventHistory.Log("EcoCash Payment (response)", response);

            //if (success)
            //{
            //    dynamic jsonObject = JsonConvert.DeserializeObject(response);

            //    if (jsonObject.text != null)
            //    {
            //        Status = GlooTransactionStatus.Failed;
            //        EcoCashErrorMessage = jsonObject.text;
            //    }
            //    else
            //    {
            //        Status = GlooTransactionStatus.PendingSubscriberValidation;
            //        EcoCashReference = jsonObject.serverReferenceCode;
            //    }

            //}
            //else
            //{
            //    Status = GlooTransactionStatus.Failed;
            //    EcoCashErrorMessage = "Unable to send details to EcoCash server";
            //}

            Save();
        }

        public void RequestEcoCashStatus()
        {
            var previousStatusId = StatusId;

            var resourceUri = $"{EcoCashUrl}{BookingUser.Telephone}/transactions/amount/{Id}";

            //if (!Config.ApplicationLive)
            //    GlooEventHistory.Log("EcoCash Status (request)", resource_uri);

            //bool success = false;
            //string response = Utilities.JsonRequestBack("GET", resource_uri, String.Empty, out success);

            //if (!Config.ApplicationLive)
            //    GlooEventHistory.Log("EcoCash Status (response)", response);

            //if (success)
            //{
            //    dynamic jsonObject = JsonConvert.DeserializeObject(response);

            //    if (jsonObject.transactionOperationStatus == "Charged" || jsonObject.transactionOperationStatus == "COMPLETED")
            //        Status = GlooTransactionStatus.Paid;
            //    else if (jsonObject.transactionOperationStatus == "Processing")
            //        Status = GlooTransactionStatus.Processing;
            //    else if (jsonObject.transactionOperationStatus == "Refunded")
            //        Status = GlooTransactionStatus.Refunded;
            //    else if (jsonObject.transactionOperationStatus == "Denied")
            //        Status = GlooTransactionStatus.Denied;
            //    else if (jsonObject.transactionOperationStatus == "Refused")
            //        Status = jsonObject.transactionOperationStatus.Refused;
            //}

            Save();

            if (previousStatusId != TransactionStatus.Paid && StatusId == TransactionStatus.Paid)
                SendPaymentNotification();
        }

        public void RequestEcoCashRefund()
        {
            Id = 0;
            StatusId = TransactionStatus.Pending;

            var resourceUri = $"{EcoCashUrl}transactions/amount";

            string JSON = JsonConvert.SerializeObject(new
            {
                clientCorrelator = Id,
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
                        channel = "Web"
                    }
                },
                referenceCode = Title,
                transactionOperationStatus = "Refunded"
            });


            //if (!Config.ApplicationLive)
            //    GlooEventHistory.Log("EcoCash Refund (request)", resource_uri + Environment.NewLine + JSON);

            //bool success = false;
            //string response = Utilities.JsonRequestBack("POST", resource_uri, JSON, out success);
            //Audit += Environment.NewLine + response;

            //if (!Config.ApplicationLive)
            //    GlooEventHistory.Log("EcoCash Refund (request)", response);

            //if (success)
            //{
            //    dynamic jsonObject = JsonConvert.DeserializeObject(response);
            //    if (jsonObject.transactionOperationStatus == "Refunded")
            //        Status = GlooTransactionStatus.Refunded;
            //}

            Save();
        }

        #endregion
    }
}
