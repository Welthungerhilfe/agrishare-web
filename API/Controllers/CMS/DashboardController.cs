using Agrishare.API;
using Agrishare.Core;
using Agrishare.Core.Utils;
using System;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator, Dashboard")]
    [RoutePrefix("cms")]
    public class CmsDashboardController : BaseApiController
    {
        [Route("dashboard/summary")]
        [AcceptVerbs("GET")]
        public object Summary(string Type = "", DateTime? StartDate = null, DateTime? EndDate = null, int Category = 0)
        {
            var startDate = (StartDate ?? DateTime.MinValue).StartOfDay();
            var endDate = (EndDate ?? DateTime.MaxValue).EndOfDay();
            var uniqueUser = Type.Equals("User");

            var totalBookingAmount = Entities.Booking.TotalAmountPaid(startDate, endDate);
            var agrishareCommission = totalBookingAmount * Entities.Transaction.AgriShareCommission;

            var locations = Entities.Booking.List(StartDate: startDate, EndDate: endDate);

            var data = new
            {
                activeListingCount = Entities.Listing.Count(Status: Entities.ListingStatus.Live),
                activeUsers = Entities.Counter.Count(UniqueUser: true),
                completeBookingCount = Entities.Booking.Count(Status: Entities.BookingStatus.Complete),
                totalBookingAmount,
                agrishareCommission,
                searchCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Search, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Search, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate)
                },
                matchCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Match, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Match, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate)
                },
                bookingCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Book, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Book, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate)
                },
                confirmCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate),
                    Female = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate)
                },
                paidCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate),
                    Female = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate)
                },
                completeCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate),
                    Female = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category, StartDate: startDate, EndDate: endDate)
                },
                locations = locations.Select(o => new { o.Latitude, o.Longitude }),
                smsBalance = SMS.GetBalance()
            };

            return Success(data);
        }
    }
}
