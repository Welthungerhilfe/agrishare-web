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
                //BookingUsers
                Distance = Model.Distance,
                //EndDate
                ForId = Model.ForId,
                IncludeFuel = Model.IncludeFuel,
                Latitude = Model.Latitude,
                //Listing
                Location = Model.Location,
                Longitude = Model.Longitude,
                //Price
                Quantity = Model.Quantity,
                //Service 
                StartDate = Model.StartDate,
                StatusId = Entities.BookingStatus.Pending,
                User = CurrentUser
            };

            if (booking.Save())
            {
                // TODO notifications

                return Success(new
                {
                    Booking = booking.Json()
                });
            }

            return Error("An unknown error occurred");
        }
    }
}
