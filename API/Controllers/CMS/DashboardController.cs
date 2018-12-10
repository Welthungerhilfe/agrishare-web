using Agrishare.API;
using Agrishare.Core;
using Agrishare.Core.Utils;
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
        public object Summary(string Type = "", int Timespan = 0, int Category = 0)
        {
            var startDate = Timespan == 0 ? DateTime.MinValue : DateTime.Now.AddDays(-Timespan).StartOfDay();
            var endDate = DateTime.Now.EndOfDay();
            var uniqueUser = Type.Equals("User");

            var totalBookingAmount = Entities.Booking.TotalAmountPaid(startDate, endDate);
            var agrishareCommission = totalBookingAmount * Entities.Transaction.AgriShareCommission;

            var data = new
            {
                activeListingCount = Entities.Listing.Count(Status: Entities.ListingStatus.Live),
                activeUsers = Entities.Counter.Count(UniqueUser: true),
                completeBookingCount = Entities.Booking.Count(Status: Entities.BookingStatus.Complete),
                totalBookingAmount,
                agrishareCommission,
                searchCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Search, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Search, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category)
                },
                matchCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Match, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Match, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category)
                },
                bookingCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.Book, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category),
                    Female = Entities.Counter.Count(Event: Entities.Counters.Book, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category)
                },
                confirmCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category),
                    Female = Entities.Counter.Count(Event: Entities.Counters.ConfirmBooking, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category)
                },
                paidCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category),
                    Female = Entities.Counter.Count(Event: Entities.Counters.CompletePayment, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category)
                },
                completeCount = new
                {
                    Male = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, Gender: Entities.Gender.Male, UniqueUser: uniqueUser, CategoryId: Category),
                    Female = Entities.Counter.Count(Event: Entities.Counters.CompleteBooking, Gender: Entities.Gender.Female, UniqueUser: uniqueUser, CategoryId: Category)
                },
                smsBalance = SMS.GetBalance()
            };

            return Success(data);
        }
    }
}
