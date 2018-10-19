using Agrishare.API;
using Agrishare.Core;
using System;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CmsDashboardController : BaseApiController
    {
        [Route("dashboard/summary")]
        [AcceptVerbs("GET")]
        public object Summary()
        {
            // TODO cache these results for 1 hour

            var startDate = DateTime.Now.AddDays(30).StartOfDay();
            var endDate = DateTime.Now.EndOfDay();

            var totalBookingAmount = Entities.Booking.TotalAmountPaid(startDate, endDate);
            var agrishareCommission = totalBookingAmount * Entities.Transaction.AgriShareCommission;

            var data = new
            {
                activeListingCount = Entities.Listing.Count(Status: Entities.ListingStatus.Live),
                totalListingCount = Entities.Listing.Count(),
                userCount = Entities.User.Count(),
                activeUsers = Entities.Counter.ActiveUsers(startDate, endDate),
                completeBookingCount = Entities.Booking.Count(Status: Entities.BookingStatus.Complete),
                totalBookingCount = Entities.Booking.Count(),
                totalBookingAmount,
                agrishareCommission,
                searchCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Search, Gender: Entities.Gender.Male),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Search, Gender: Entities.Gender.Female)
                },
                matchCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Match, Gender: Entities.Gender.Male),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Match, Gender: Entities.Gender.Female)
                },
                bookingCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Book, Gender: Entities.Gender.Male),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Book, Gender: Entities.Gender.Female)
                },
                confirmCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, Gender: Entities.Gender.Male),
                    Female = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, Gender: Entities.Gender.Female)
                },
                paidCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, Gender: Entities.Gender.Male),
                    Female = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, Gender: Entities.Gender.Female)
                },
                completeCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, Gender: Entities.Gender.Male),
                    Female = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, Gender: Entities.Gender.Female)
                }
            };

            return Success(data);
        }
    }
}
