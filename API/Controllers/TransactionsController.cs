using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using Agrishare.API;
using Agrishare.Core;
using Entities = Agrishare.Core.Entities;
using System.Text.RegularExpressions;
using Agrishare.API.Models;
using Agrishare.Core.Utils;
using Newtonsoft.Json;

namespace Agri.API.Controllers
{
    [@Authorize(Roles = "User")]
    public class TransactionsController : BaseApiController
    {
        [Route("transactions/ecocash/notify")]
        [AcceptVerbs("POST")]
        public void EcoCashNotification([FromBody] string JsonString)
        {
            dynamic jsonObject = JsonConvert.DeserializeObject(JsonString);
            if (jsonObject.transactionOperationStatus == "COMPLETED")
            {
                var id = Convert.ToInt32(jsonObject.clientCorrelator.ToString());
                Entities.Transaction.Find(Id: id).RequestEcoCashStatus();
            }
        }

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

        [Route("transactions/create")]
        [AcceptVerbs("GET")]
        public object CreateTransaction(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Booking not found");

            var transactions = new List<Entities.Transaction>();

            var bookingUsers = Entities.BookingUser.List(BookingId: booking.Id);
            foreach (var bookingUser in bookingUsers)
            {
                var transaction = new Entities.Transaction
                {
                    Amount = booking.Price * bookingUser.Ratio,
                    Booking = booking,
                    BookingUser = bookingUser
                };

                transaction.Save();
                transaction.RequestEcoCashPayment();
                transactions.Add(transaction);
            }

            return Success(new
            {
                Transactions = transactions.Select(e => e.Json())
            });
        }

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
