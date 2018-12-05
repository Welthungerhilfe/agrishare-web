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
    public partial class Notification : IEntity
    {
        public static string DefaultSort = "DateCreated DESC";
        public string Title => Type;
        public string Type => $"{TypeId}".ExplodeCamelCase();
        public string Status => $"{StatusId}".ExplodeCamelCase();
        public string Group => $"{GroupId}".ExplodeCamelCase();

        public static Notification Find(int Id = 0)
        {
            if (Id == 0)
                return new Notification
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Notifications.Include(o => o.Booking).Include(o => o.Booking.Service).Include(o => o.Booking.Service.Listing).Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Notification> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", int UserId = 0, NotificationGroup GroupId = NotificationGroup.None, int BookingId = 0, NotificationType Type = NotificationType.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Notifications.Include(o => o.Booking).Include(o => o.Booking.Service).Include(o => o.Booking.Service.Listing).Where(o => !o.Deleted && !o.Booking.Deleted && !o.Booking.Service.Deleted && !o.Booking.Service.Listing.Deleted);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (GroupId != NotificationGroup.None)
                    query = query.Where(o => o.GroupId == GroupId);

                if (BookingId > 0 && Type != NotificationType.None)
                    query = query.Where(e => e.BookingId == BookingId && e.TypeId == Type);

                query = query.OrderBy(Sort.Coalesce(DefaultSort));

                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(int UserId = 0, NotificationGroup GroupId = NotificationGroup.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Notifications.Include(o => o.Booking).Include(o => o.Booking.Service).Include(o => o.Booking.Service.Listing).Where(o => !o.Deleted && !o.Booking.Deleted && !o.Booking.Service.Deleted && !o.Booking.Service.Listing.Deleted);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (GroupId != NotificationGroup.None)
                    query = query.Where(o => o.GroupId == GroupId);

                return query.Count();
            }
        }

        public bool Save(bool Notify = false)
        {
            var success = false;

            var user = User;
            if (user != null) UserId = user.Id;
            User = null;

            var booking = Booking;
            if (booking != null) BookingId = booking.Id;
            Booking = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            User = user;
            Booking = booking;

            if (Notify)
            {
                if ((User.NotificationPreferences & (int)NotificationPreferences.Email) > 0)
                    SendEmail();

                if ((User.NotificationPreferences & (int)NotificationPreferences.PushNotifications) > 0)
                {
                    var devices = Device.List(UserId: UserId);
                    foreach (var device in devices)
                        SendPushNotification(device.EndpointARN);
                }

                if ((User.NotificationPreferences & (int)NotificationPreferences.SMS) > 0)
                    SendSMS();
            }

            return success;
        }

        private bool Add()
        {
            StatusId = NotificationStatus.Pending;

            using (var ctx = new AgrishareEntities())
            {
                ctx.Notifications.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Notifications.Attach(this);
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
                User = User?.Json(),
                Booking = Booking?.Json(),
                Title,
                TypeId,
                Type,
                StatusId,
                Status,
                GroupId,
                Group,
                DateCreated
            };
        }

        private void SendEmail()
        {
            var template = Template.Find(Title: Type);
            template.Replace("Service", Booking.Service.Category.Title);
            template.Replace("Supplier", Booking.Service.Listing.Title);
            template.Replace("Message", Message);
            template.Replace("Start Date", Booking.StartDate.ToString("d MMMM yyyy"));
            template.Replace("End Date", Booking.EndDate.ToString("d MMMM yyyy"));

            template.Replace("Payment Amount", Template.FormatCurrency(Booking.Price));
            template.Replace("Payment Fees", Template.FormatCurrency(Booking.AgriShareCommission));
            template.Replace("Payment Total", Template.FormatCurrency(Booking.Price - Booking.AgriShareCommission));

            var subject = "Notification";
            switch (TypeId)
            {
                case NotificationType.BookingCancelled:
                    subject = $"Booking cancelled";
                    break;
                case NotificationType.BookingConfirmed:
                    subject = $"Booking confirmed by supplier";
                    break;
                case NotificationType.NewBooking:
                    subject = $"New booking received";
                    break;
                case NotificationType.NewReview:
                    subject = $"New review received";
                    break;
                case NotificationType.PaymentReceived:
                    subject = $"Payment received";
                    break;
                case NotificationType.ServiceComplete:
                    subject = $"Service complete";
                    break;
                case NotificationType.ServiceIncomplete:
                    subject = $"Service not complete";
                    break;
            }

            subject = $"{subject} (#{Id})";

            new Email
            {
                Message = template.EmailHtml(),
                RecipientEmail = User.EmailAddress,
                RecipientName = User.FullName,
                SenderEmail = Config.ApplicationEmailAddress,
                Subject = subject
            }.Send();
        }

        private void SendSMS()
        {
            var message = "Notification";
            switch (TypeId)
            {
                case NotificationType.BookingCancelled:
                    message = string.Format(Translations.Translate("Booking Cancelled", User.LanguageId), BookingId);
                    break;
                case NotificationType.BookingConfirmed:
                    message = string.Format(Translations.Translate("Booking Confirmed", User.LanguageId), BookingId);
                    break;
                case NotificationType.NewBooking:
                    message = string.Format(Translations.Translate("Booking Received", User.LanguageId), BookingId);
                    break;
                case NotificationType.NewReview:
                    message = string.Format(Translations.Translate("New Review", User.LanguageId), BookingId);
                    break;
                case NotificationType.PaymentReceived:
                    message = string.Format(Translations.Translate("Payment Received", User.LanguageId), BookingId);
                    break;
                case NotificationType.ServiceComplete:
                    message = string.Format(Translations.Translate("Booking Completed", User.LanguageId), BookingId);
                    break;
                case NotificationType.ServiceIncomplete:
                    message = string.Format(Translations.Translate("Booking Not Completed", User.LanguageId), BookingId);
                    break;
            }

            SMS.SendMessage(User.Telephone, message);
        }

        private void SendPushNotification(string DeviceARN)
        {
            var message = "Notification";
            switch (TypeId)
            {
                case NotificationType.BookingCancelled:
                    message = string.Format(Translations.Translate("Booking Cancelled", User.LanguageId), BookingId);
                    break;
                case NotificationType.BookingConfirmed:
                    message = string.Format(Translations.Translate("Booking Confirmed", User.LanguageId), BookingId);
                    break;
                case NotificationType.NewBooking:
                    message = string.Format(Translations.Translate("Booking Received", User.LanguageId), BookingId);
                    break;
                case NotificationType.NewReview:
                    message = string.Format(Translations.Translate("New Review", User.LanguageId), BookingId);
                    break;
                case NotificationType.PaymentReceived:
                    message = string.Format(Translations.Translate("Payment Received", User.LanguageId), BookingId);
                    break;
                case NotificationType.ServiceComplete:
                    message = string.Format(Translations.Translate("Booking Completed", User.LanguageId), BookingId);
                    break;
                case NotificationType.ServiceIncomplete:
                    message = string.Format(Translations.Translate("Booking Not Completed", User.LanguageId), BookingId);
                    break;
            }

            var args = new Dictionary<string, object>
            {
                { "UserId", UserId },
                { "BookingId", BookingId }
            };
            SNS.SendMessage(DeviceARN, message, $"app.agrishare.category.{TypeId}", args);
        }
    }
}
