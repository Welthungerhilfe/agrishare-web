﻿using System;
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
    public class TransactionsController : BaseApiController
    {
        [Route("transactions/ecocash/notify")]
        [AcceptVerbs("POST")]
        public object EcoCashNotification(EcoCashModel Model)
        {
            if (Model.TransactionOperationStatus == "COMPLETED")
                Entities.Transaction.Find(Id: Model.ClientCorrelator).RequestEcoCashStatus();

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
                var bookingUser = new Entities.BookingUser
                {
                    Booking = booking,
                    Name = user.Name,
                    Ratio = user.Quantity / booking.Quantity,
                    StatusId = Entities.BookingUserStatus.Pending,
                    Telephone = user.Telephone
                };

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
                //transaction.RequestEcoCashPayment();
                transactions.Add(transaction);
            }

            // BS: temporary fake success
            booking.StatusId = Entities.BookingStatus.InProgress;
            booking.Save();

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