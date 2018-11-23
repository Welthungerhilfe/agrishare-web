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

            var service = Entities.Service.Find(Id: Model.ServiceId);
            if (service == null)
                return Error("Invalid service selected");

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
                Service = service,
                Listing = service.Listing,
                StartDate = Model.StartDate,
                EndDate = Model.EndDate,
                StatusId = Entities.BookingStatus.Pending,
                User = CurrentUser,
                AdditionalInformation = Model.AdditionalInformation,
                TotalVolume = Model.TotalVolume,
                HireCost = Model.HireCost,
                FuelCost = Model.FuelCost,
                TransportCost = Model.TransportCost,
                Price = Model.HireCost + Model.FuelCost + Model.TransportCost,
                Distance = Model.Distance
            };       

            if (booking.Save())
            {
                Entities.Counter.Hit(CurrentUser.Id, Entities.Counters.Book, booking.Service.CategoryId);

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Offering,
                    TypeId = Entities.NotificationType.NewBooking,
                    User = Entities.User.Find(Id: booking.Listing.UserId)
                }.Save(Notify: true);

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Seeking,
                    TypeId = Entities.NotificationType.NewBooking,
                    User = CurrentUser
                }.Save(Notify: false);

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
                var notifications = Entities.Notification.List(BookingId: booking.Id, Type: Entities.NotificationType.NewBooking);
                foreach(var notification in notifications)
                {
                    notification.StatusId = Entities.NotificationStatus.Complete;
                    notification.Save();
                }

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Seeking,
                    TypeId = Entities.NotificationType.BookingConfirmed,
                    User = Entities.User.Find(Id: booking.UserId)
                }.Save(Notify: true);

                Entities.Counter.Hit(booking.UserId, Entities.Counters.ConfirmBooking, booking.Service.CategoryId);

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
                var notifications = Entities.Notification.List(BookingId: booking.Id, Type: Entities.NotificationType.NewBooking);
                foreach (var notification in notifications)
                {
                    notification.StatusId = Entities.NotificationStatus.Complete;
                    notification.Save();
                }

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Seeking,
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
                    GroupId = Entities.NotificationGroup.Offering,
                    TypeId = Entities.NotificationType.ServiceComplete,
                    User = Entities.User.Find(Id: booking.Listing.UserId)
                }.Save(Notify: true);

                Entities.Counter.Hit(booking.UserId, Entities.Counters.CompleteBooking, booking.Service.CategoryId);

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

        [Route("bookings/incomplete")]
        [AcceptVerbs("GET")]
        public object IncompleteBooking(int BookingId, string Message)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || booking.UserId != CurrentUser.Id)
                return Error("Booking not found");

            if (booking.StatusId == Entities.BookingStatus.Complete)
                return Error("This booking has already been updated");

            booking.StatusId = Entities.BookingStatus.Incomplete;
            if (booking.Save())
            {
                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Offering,
                    Message = Message,
                    TypeId = Entities.NotificationType.ServiceIncomplete,
                    User = Entities.User.Find(Id: booking.Listing.UserId)
                }.Save(Notify: true);

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Seeking,
                    Message = Message,
                    TypeId = Entities.NotificationType.ServiceIncomplete,
                    User = Entities.User.Find(Id: booking.UserId)
                }.Save(Notify: true);

                Entities.Counter.Hit(booking.UserId, Entities.Counters.IncompleteBooking, booking.Service.CategoryId);

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
            var monthlySpend = Entities.Booking.OfferingSummary(CurrentUser.Id, startDate);
            var totalSpend = Entities.Booking.OfferingSummary(CurrentUser.Id);
            
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

        [Route("bookings/cancel")]
        [AcceptVerbs("GET")]
        public object CancelBooking(int BookingId)
        {
            var booking = Entities.Booking.Find(Id: BookingId);
            if (booking == null || (booking.Listing.UserId != CurrentUser.Id && booking.UserId != CurrentUser.Id))
                return Error("Booking not found");

            if (booking.StatusId == Entities.BookingStatus.Complete)
                return Error("You can not cancel a completed booking");

            if (booking.StatusId == Entities.BookingStatus.InProgress)
                return Error("You can not cancel a paid booking that is not complete");

            if (DateTime.Now.AddDays(7) >= booking.StartDate)
                return Error("You can not cancel a booking within 7 days of the start date");

            booking.StatusId = Entities.BookingStatus.Cancelled;
            if (booking.Save())
            {
                // TODO refund users

                var notifications = Entities.Notification.List(BookingId: booking.Id, Type: Entities.NotificationType.BookingCancelled);
                foreach (var notification in notifications)
                {
                    notification.StatusId = Entities.NotificationStatus.Complete;
                    notification.Save();
                }

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Seeking,
                    TypeId = Entities.NotificationType.BookingCancelled,
                    User = Entities.User.Find(Id: booking.UserId)
                }.Save(Notify: true);

                new Entities.Notification
                {
                    Booking = booking,
                    GroupId = Entities.NotificationGroup.Offering,
                    TypeId = Entities.NotificationType.BookingCancelled,
                    User = Entities.User.Find(Id: booking.Listing.UserId)
                }.Save(Notify: true);

                Entities.Counter.Hit(booking.UserId, Entities.Counters.CancelBooking, booking.Service.CategoryId);

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
    }
}
