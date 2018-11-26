using Agrishare.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    public class TransactionsController : BaseApiController
    {
        [Route("transactions/ecocash/notify")]
        [AcceptVerbs("POST")]
        public object EcoCashNotification(EcoCashModel Model)
        {
            if (Model.TransactionOperationStatus == "COMPLETED")
                Entities.Transaction.Find(ClientCorrelator: Model.ClientCorrelator).RequestEcoCashStatus();

            return Success(Model);
        }

        [@Authorize(Roles = "User")]
        [Route("transactions/ecocash/poll")]
        [AcceptVerbs("GET")]
        public object PollEcoCash(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Transaction not found");

            var transactions = Entities.Transaction.List(BookingId: booking.Id);
            foreach (var transaction in transactions)
                transaction.RequestEcoCashStatus();

            return Success(new
            {
                Transactions = transactions.Select(e => e.Json())
            });
        }

        [@Authorize(Roles = "User")]
        [Route("transactions/create")]
        [AcceptVerbs("POST")]
        public object CreateTransaction(TransactionModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var booking = Entities.Booking.Find(Id: Model.BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Booking not found");

            booking.BookingUsers = new List<Entities.BookingUser>();
            foreach (var user in Model.Users)
            {
                if (user.Quantity == 0)
                    user.Quantity = 1;
                if (booking.Quantity == 0)
                    booking.Quantity = 1;

                var bookingUser = new Entities.BookingUser
                {
                    Booking = booking,
                    Name = user.Name,
                    Ratio = user.Quantity / booking.Quantity,
                    StatusId = Entities.BookingUserStatus.Pending,
                    Telephone = user.Telephone
                };

                var registeredUser = Entities.User.Find(Telephone: user.Telephone);
                if (registeredUser.Id > 0)
                    bookingUser.User = registeredUser;

                bookingUser.Save();
                booking.BookingUsers.Add(bookingUser);
            }

            var transactions = new List<Entities.Transaction>();

            foreach (var bookingUser in booking.BookingUsers)
            {
                var transaction = new Entities.Transaction
                {
                    Amount = booking.Price * bookingUser.Ratio,
                    Booking = booking,
                    BookingUser = bookingUser,
                    StatusId = Entities.TransactionStatus.Pending
                };

                transaction.Save();
                transaction.RequestEcoCashPayment();
                transactions.Add(transaction);

                Entities.Counter.Hit(bookingUser.UserId ?? 0, Entities.Counters.InitiatePayment, booking.Service.CategoryId);
            }

            // BS: temporary fake success
            /*******************************/

            //booking.StatusId = Entities.BookingStatus.InProgress;
            //booking.Save();

            //new Entities.Notification
            //{
            //    Booking = booking,
            //    GroupId = Entities.NotificationGroup.Offering,
            //    TypeId = Entities.NotificationType.PaymentReceived,
            //    User = Entities.User.Find(Id: booking.Listing.UserId)
            //}.Save(Notify: true);

            //new Entities.Notification
            //{
            //    Booking = booking,
            //    GroupId = Entities.NotificationGroup.Seeking,
            //    TypeId = Entities.NotificationType.PaymentReceived,
            //    User = CurrentUser
            //}.Save(Notify: false);

            /*******************************/

            return Success(new
            {
                Transactions = transactions.Select(e => e.Json())
            });
        }

        [@Authorize(Roles = "User")]
        [Route("transactions")]
        [AcceptVerbs("GET")]
        public object TransactionList(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Transaction not found");

            var transactions = Entities.Transaction.List(BookingId: booking.Id);

            return Success(new
            {
                Transactions = transactions.Select(e => e.Json())
            });
        }
    }
}
