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

namespace Agri.API.Controllers
{
    [@Authorize(Roles="User")]
    public class BookingsController : BaseApiController
    {
        [Route("bookings/add")]
        [AcceptVerbs("POST")]
        public object AddBooking(BookingModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var booking = new Entities.Booking
            {
                ForId = Model.ForId,
                IncludeFuel = Model.IncludeFuel,
                Latitude = Model.Latitude,
                Location = Model.Location,
                Longitude = Model.Longitude,
                Quantity = Model.Quantity,
                Service = Entities.Service.Find(Id: Model.ServiceId),
                StartDate = Model.StartDate,
                StatusId = Entities.BookingStatus.Pending,
                User = CurrentUser
            };

            if (booking.Service == null)
                return Error("Invalid service selected");

            booking.Listing = booking.Service.Listing;

            if (booking.Service.Mobile)
            {
                var origin = Geometry.CreatePoint(booking.Listing.Latitude, booking.Listing.Longitude);
                var destination = Geometry.CreatePoint(booking.Latitude, booking.Longitude);
                booking.Distance = (decimal)(origin.Distance(destination) ?? 0) * 2;
            }
            else
                booking.Distance = 0;

            var days = Math.Ceiling(booking.Service.TimePerQuantityUnit * booking.Quantity / 8) - 1;
            booking.EndDate = booking.StartDate.AddDays((double)days);

            booking.HireCost = booking.Quantity * booking.Service.PricePerQuantityUnit;
            booking.FuelCost = booking.Quantity * booking.Service.FuelPerQuantityUnit * booking.Service.FuelPrice;
            booking.TransportCost = booking.Service.Mobile ? booking.Distance * booking.Service.PricePerDistanceUnit : 0;
            booking.Price = booking.HireCost + booking.FuelCost + booking.TransportCost;

            if (booking.Save())
            {
                booking.BookingUsers = new List<Entities.BookingUser>();
                foreach (var user in Model.Users)
                {
                    var bookingUser = new Entities.BookingUser
                    {
                        Booking = booking,
                        Name = user.Name,
                        Ratio = user.Quantity / booking.Quantity,
                        StatusId = Entities.BookingUserStatus.Pending,
                        Telephone = user.Telephone,
                        VerificationCode = Entities.User.GeneratePIN(4),
                        VerificationCodeExpiry = DateTime.Now.AddDays(1)
                    };

                    if (bookingUser.Save())
                        bookingUser.SendVerificationCode();

                    booking.BookingUsers.Add(bookingUser);
                }

                new Entities.Notification
                {
                    BookingId = booking.Id,
                    TypeId = Entities.NotificationType.NewBooking,
                    UserId = booking.Listing.UserId
                }.Save(Notify: true);

                return Success(new
                {
                    Booking = booking.Json()
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/confirm")]
        [AcceptVerbs("GET")]
        public object ConfirmBooking(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || booking.Listing.UserId != CurrentUser.Id)
                return Error("Booking not found");

            if (booking.StatusId != Entities.BookingStatus.Pending)
                return Error("This booking has already been updated");

            booking.StatusId = Entities.BookingStatus.Approved;
            if (booking.Save())
            {
                new Entities.Notification
                {
                    BookingId = booking.Id,
                    TypeId = Entities.NotificationType.BookingConfirmed,
                    UserId = booking.UserId
                }.Save(Notify: true);

                return Success(new
                {
                    Booking = new
                    {
                        Id = BookingId
                    }
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/decline")]
        [AcceptVerbs("GET")]
        public object DeclineBooking(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || booking.Listing.UserId != CurrentUser.Id)
                return Error("Booking not found");

            if (booking.StatusId != Entities.BookingStatus.Pending)
                return Error("This booking has already been updated");

            booking.StatusId = Entities.BookingStatus.Declined;
            if (booking.Save())
            {
                new Entities.Notification
                {
                    BookingId = booking.Id,
                    TypeId = Entities.NotificationType.BookingCancelled,
                    UserId = booking.UserId
                }.Save(Notify: true);

                return Success(new
                {
                    Booking = new
                    {
                        Id = BookingId
                    }
                });
            }

            return Error("An unknown error occurred");
        }
    }
}
