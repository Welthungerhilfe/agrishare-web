using Agrishare.API;
using Agrishare.Core;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CmsBookingsController : BaseApiController
    {
        [Route("bookings/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Query = "", int UserId = 0)
        {
            var recordCount = Entities.Booking.Count(UserId: UserId);
            var list = Entities.Booking.List(PageIndex: PageIndex, PageSize: PageSize, UserId: UserId);
            var title = "Bookings";

            if (UserId > 0)
            {
                var user = Entities.User.Find(Id: UserId);
                if (user != null)
                    title = $"{user.FirstName} {user.LastName}";
            }

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Booking.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = title
            };

            return Success(data);
        }

        [Route("bookings/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Booking.Find(Id: Id).Json(),
                Transactions = Entities.Transaction.List(BookingId: Id).Select(e => e.Json())
            };

            return Success(data);
        }

        [Route("bookings/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Booking Booking)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Booking.Save())
                return Success(new
                {
                    Entity = Booking.Json()
                });

            return Error();
        }

        [Route("bookings/transactions/poll")]
        [AcceptVerbs("GET")]
        public object PollEcoCash(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Transaction not found");

            var transactions = Entities.Transaction.List(BookingId: booking.Id);
            foreach (var transaction in transactions)
                transaction.RequestEcoCashStatus();

            var data = new
            {
                Entity = booking.Json(),
                Transactions = Entities.Transaction.List(BookingId: booking.Id).Select(e => e.Json()),
                Feedback = $"Finished updating transaction statuses"
            };

            return Success(data);
        }

        [Route("bookings/cancel")]
        [AcceptVerbs("GET")]
        public object CancelBooking(int Id = 0)
        {
            var booking = Entities.Booking.Find(Id: Id);
            if (booking == null || booking.Id == 0)
                return Error("Booking not found");

            if (booking.StatusId == Entities.BookingStatus.Complete)
                return Error("You can not cancel a completed booking");

            if (booking.StatusId == Entities.BookingStatus.InProgress)
                return Error("You can not cancel a paid booking that is not complete");

            booking.StatusId = Entities.BookingStatus.Cancelled;
            if (booking.Save())
            {
                var transactions = Entities.Transaction.List(BookingId: booking.Id);
                foreach (var transaction in transactions)
                    transaction.RequestEcoCashRefund();

                var notifications = Entities.Notification.List(BookingId: booking.Id, Type: Entities.NotificationType.BookingCancelled);
                foreach (var notification in notifications)
                {
                    notification.StatusId = Entities.NotificationStatus.Complete;
                    notification.Save();
                }

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Seeking,
                    TypeId = Entities.NotificationType.BookingCancelled,
                    User = Entities.User.Find(Id: booking.UserId)
                }.Save(Notify: true);

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Offering,
                    TypeId = Entities.NotificationType.BookingCancelled,
                    User = Entities.User.Find(Id: booking.Listing.UserId)
                }.Save(Notify: false);

                Entities.Counter.Hit(booking.UserId, Entities.Counters.CancelBooking, booking.Service.CategoryId);

                var data = new
                {
                    Entity = booking.Json(),
                    Transactions = Entities.Transaction.List(BookingId: booking.Id).Select(e => e.Json()),
                    Feedback = $"Booking was cancelled"
                };

                return Success(data);
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/transactions/refund")]
        [AcceptVerbs("GET")]
        public object RefundTransaction(int Id = 0)
        {
            var transaction = Entities.Transaction.Find(Id: Id);

            if (transaction.RequestEcoCashRefund())
            {
                var data = new
                {
                    Entity = Entities.Booking.Find(Id: Id).Json(),
                    Transactions = Entities.Transaction.List(BookingId: Id).Select(e => e.Json())
                };

                return Success(data);
            }
            else
                return Error("Unable to process refund");
        }
    }
}
