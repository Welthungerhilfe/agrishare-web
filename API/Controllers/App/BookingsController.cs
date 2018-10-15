using Agrishare.API.Models;
using Agrishare.Core;
using Agrishare.Core.Utils;
using System;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
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
                DestinationLatitude = Model.DestinationLatitude,
                Destination = Model.Destination,
                DestinationLongitude = Model.DestinationLongitude,
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
                var distance = Location.GetDistance(Convert.ToDouble(booking.Listing.Latitude), Convert.ToDouble(booking.Listing.Longitude), Convert.ToDouble(booking.Latitude), Convert.ToDouble(booking.Longitude));
                booking.Distance = (decimal)distance / 1000;
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
                new Entities.Notification
                {
                    Booking = booking,
                    TypeId = Entities.NotificationType.NewBooking,
                    User = Entities.User.Find(Id: booking.Listing.UserId)
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
                    Booking = booking,
                    TypeId = Entities.NotificationType.BookingConfirmed,
                    User = Entities.User.Find(Id: booking.UserId)
                }.Save(Notify: true);

                return Success(new
                {
                    Booking = new
                    {
                        booking.Id,
                        booking.StatusId,
                        booking.Status
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
                    Booking = booking,
                    TypeId = Entities.NotificationType.BookingCancelled,
                    User = Entities.User.Find(Id: booking.UserId)
                }.Save(Notify: true);

                return Success(new
                {
                    Booking = new
                    {
                        booking.Id,
                        booking.StatusId,
                        booking.Status
                    }
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/complete")]
        [AcceptVerbs("GET")]
        public object CompleteBooking(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Booking not found");

            if (booking.StatusId == Entities.BookingStatus.Complete)
                return Error("This booking has already been updated");

            booking.StatusId = Entities.BookingStatus.Complete;
            if (booking.Save())
            {
                new Entities.Notification
                {
                    Booking = booking,
                    TypeId = Entities.NotificationType.ServiceComplete,
                    User = Entities.User.Find(Id: booking.Listing.UserId)
                }.Save(Notify: true);

                return Success(new
                {
                    Booking = new
                    {
                        booking.Id,
                        booking.StatusId,
                        booking.Status
                    }
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("bookings/seeking")]
        [AcceptVerbs("GET")]
        public object SeekingList(int PageIndex = 0, int PageSize = 25)
        {
            var startDate = DateTime.Today.StartOfDay().AddDays(-(DateTime.Today.Day - 1));
            var monthlySpend = Entities.Booking.SeekingSummary(CurrentUser.Id, startDate);
            var totalSpend = Entities.Booking.SeekingSummary(CurrentUser.Id);

            var bookings = Entities.Booking.List(PageIndex: PageIndex, PageSize: PageSize, UserId: CurrentUser.Id);

            return Success(new
            {
                Bookings = bookings.Select(e => e.Json()),
                Summary = new
                {
                    Month = monthlySpend,
                    Total = totalSpend
                }
            });
        }

        [Route("bookings/offering")]
        [AcceptVerbs("GET")]
        public object OfferingList(int PageIndex = 0, int PageSize = 25)
        {
            var startDate = DateTime.Today.StartOfDay().AddDays(-(DateTime.Today.Day - 1));
            var monthlySpend = Entities.Booking.SeekingSummary(CurrentUser.Id, startDate);
            var totalSpend = Entities.Booking.SeekingSummary(CurrentUser.Id);
            
            var bookings = Entities.Booking.List(PageIndex: PageIndex, PageSize: PageSize, SupplierId: CurrentUser.Id);

            return Success(new
            {
                Bookings = bookings.Select(e => e.Json()),
                Summary = new
                {
                    Month = monthlySpend,
                    MonthCommission = monthlySpend * Entities.Transaction.AgriShareCommission,
                    Total = totalSpend,
                    TotalCommission = totalSpend * Entities.Transaction.AgriShareCommission
                }
            });
        }

        [Route("bookings/listing")]
        [AcceptVerbs("GET")]
        public object ListingBookings(int ListingId, int PageIndex = 0, int PageSize = 25)
        {
            var listing = Entities.Listing.Find(Id: ListingId);
            if (listing == null || listing.Id == 0 || listing.UserId != CurrentUser.Id)
                return Error("Listing not found");

            var bookings = Entities.Booking.List(PageIndex: PageIndex, PageSize: PageSize, ListingId: ListingId);

            return Success(new
            {
                Bookings = bookings.Select(e => e.Json())
            });
        }

        [Route("bookings/detail")]
        [AcceptVerbs("GET")]
        public object BookingDetail(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || (booking.UserId != CurrentUser.Id && booking.Listing.UserId != CurrentUser.Id))
                return Error("Booking does not exist");

            var ratingCount = Entities.Rating.Count(ListingId: booking.ListingId, UserId: CurrentUser.Id);

            var bookingUsers = Entities.BookingUser.List(BookingId: booking.Id);

            return Success(new
            {
                Booking = booking.Json(),
                Users = bookingUsers.Select(e => e.Json()),
                Rated = ratingCount > 0
            });
        }

        [Route("bookings/users/verify")]
        [AcceptVerbs("GET")]
        public object VerifyUser(int BookingUserId, string VerificationCode)
        {
            var bookingUser = Entities.BookingUser.Find(Id: BookingUserId);
            if (bookingUser == null || bookingUser.Id == 0)
                return Error("User does not exist");

            var booking = Entities.Booking.Find(Id: bookingUser.BookingId);
            if (booking == null || booking.Id == 0 || booking.UserId != CurrentUser.Id)
                return Error("Booking does not exist");

            if (bookingUser.VerificationCode == VerificationCode)
            {
                bookingUser.StatusId = Entities.BookingUserStatus.Verified;
                bookingUser.Save();

                return Success(new
                {
                    BookingUser = bookingUser.Json()
                });
            }

            return Error("Invalid code");
        }
    }
}
