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

        public static List<Notification> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", int UserId = 0, NotificationGroup GroupId = NotificationGroup.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Notifications.Include(o => o.Booking).Include(o => o.Booking.Service).Include(o => o.Booking.Service.Listing).Where(o => !o.Deleted);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (GroupId != NotificationGroup.None)
                    query = query.Where(o => o.GroupId == GroupId);

                query = query.OrderBy(Sort.Coalesce(DefaultSort));

                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(int UserId = 0, NotificationGroup GroupId = NotificationGroup.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Notifications.Where(o => !o.Deleted);

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

            switch (TypeId)
            {
                case NotificationType.BookingCancelled:
                case NotificationType.BookingConfirmed:
                case NotificationType.ServiceComplete:
                    GroupId = NotificationGroup.Seeking;
                    break;
                case NotificationType.NewBooking:
                case NotificationType.NewReview:
                case NotificationType.PaymentReceived:
                    GroupId = NotificationGroup.Offering;
                    break;
            }

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
            template.Replace("Start Date", Booking.StartDate.ToString("d MMMM yyyy"));
            template.Replace("End Date", Booking.EndDate.ToString("d MMMM yyyy"));

            var subject = "Notification";
            switch (TypeId)
            {
                case NotificationType.BookingCancelled:
                    subject = $"Booking cancelled by supplier";
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
                    message = $"Booking {BookingId} cancelled by supplier";
                    break;
                case NotificationType.BookingConfirmed:
                    message = $"Booking {BookingId} confirmed by supplier";
                    break;
                case NotificationType.NewBooking:
                    message = $"New booking received: {BookingId}";
                    break;
                case NotificationType.NewReview:
                    message = $"New review received for booking {BookingId}";
                    break;
                case NotificationType.PaymentReceived:
                    message = $"Payment received: {Title}";
                    break;
                case NotificationType.ServiceComplete:
                    message = $"Booking {BookingId} completed";
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
                    message = $"Booking #{Id} cancelled by supplier";
                    break;
                case NotificationType.BookingConfirmed:
                    message = $"Booking #{Id}  confirmed by supplier";
                    break;
                case NotificationType.NewBooking:
                    message = $"New booking received (#{Id})";
                    break;
                case NotificationType.NewReview:
                    message = $"New review received (#{Id})";
                    break;
                case NotificationType.PaymentReceived:
                    message = $"Payment received: {Title}";
                    break;
                case NotificationType.ServiceComplete:
                    message = $"Service (#{Id}) complete";
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
