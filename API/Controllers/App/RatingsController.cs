using Agrishare.API.Models;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles="User")]
    public class RatingsController : BaseApiController
    {
        [Route("ratings")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, int ListingId)
        {
            var ratings = Entities.Rating.List(PageIndex: PageIndex, PageSize: PageSize, ListingId: ListingId);

            return Success(new
            {
                List = ratings.Select(e => e.Json())
            });
        }

        [Route("ratings/add")]
        [AcceptVerbs("POST")]
        public object Add(RatingModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var booking = Entities.Booking.Find(Id: Model.BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Booking not found");

            var rating = new Entities.Rating
            {
                Comments = Model.Comments,
                ListingId = booking.ListingId,
                Stars = Model.Rating,
                User = CurrentUser
            };

            if (rating.Save())
            {
                var listing = Entities.Listing.Find(Id: rating.ListingId);
                listing.AverageRating = ((listing.AverageRating * listing.RatingCount) + rating.Stars)  / (listing.RatingCount + 1);
                listing.RatingCount += 1;
                listing.Save();

                var notifications = Entities.Notification.List(Type: Entities.NotificationType.ServiceComplete, UserId: booking.UserId);
                foreach(var notification in notifications)
                {
                    notification.StatusId = Entities.NotificationStatus.Complete;
                    notification.Save();
                }

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Offering,
                    TypeId = Entities.NotificationType.NewReview,
                    User = Entities.User.Find(Id: listing.UserId)
                }.Save(Notify: true);

                return Success(new
                {
                    Rating = rating.Json()
                });
            }

            return Error("An unknown error ocurred");
        }
    }
}
