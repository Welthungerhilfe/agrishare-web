using Agrishare.API.Models;
using Agrishare.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles="User")]
    public class ListingsController : BaseApiController
    {
        [Route("listings")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25)
        {
            var list = Entities.Listing.List(UserId: CurrentUser.Id, PageIndex: PageIndex, PageSize: PageSize);
            return Success(new
            {
                List = list.Select(e => e.Json())
            });
        }

        [Route("listings/detail")]
        [AcceptVerbs("GET")]
        public object Detail(int ListingId)
        {
            var listing = Entities.Listing.Find(Id: ListingId);

            if (listing == null || listing.Id == 0)
                return Error("Listing not found");

            var upcomingBookings = Entities.Booking.Count(UserId: CurrentUser.Id, StartDate: DateTime.Now);

            return Success(new
            {
                Listing = listing.Json(),
                UpcomingBookings = upcomingBookings
            });
        }

        [Route("listings/add")]
        [AcceptVerbs("POST")]
        public object Add(ListingModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var listing = new Entities.Listing
            {
                AvailableWithoutFuel = Model.AvailableWithoutFuel,
                Brand = Model.Brand,
                CategoryId = Model.CategoryId,
                ConditionId = Model.ConditionId,
                Latitude = Model.Latitude,
                Longitude = Model.Longitude,
                Description = Model.Description,
                GroupServices = Model.GroupServices,
                HorsePower = Model.HorsePower,
                Location = Model.Location,
                StatusId = Entities.ListingStatus.Live,
                Title = Model.Title,
                UserId = CurrentUser.Id,
                Year = Model.Year
            };

            listing.Services = new List<Entities.Service>();
            foreach (var service in Model.Services)
                listing.Services.Add(new Entities.Service
                {
                    DistanceUnitId = service.DistanceUnitId,
                    FuelPrice = service.FuelPrice,
                    FuelPerQuantityUnit = service.FuelPerQuantityUnit,
                    MaximumDistance = service.MaximumDistance,
                    MinimumQuantity = service.MinimumQuantity,
                    Mobile = service.Mobile || listing.CategoryId == Entities.Category.TractorsId || listing.CategoryId == Entities.Category.LorriesId,
                    PricePerDistanceUnit = service.Mobile ? service.PricePerDistanceUnit : 0,
                    PricePerQuantityUnit = service.PricePerQuantityUnit,
                    QuantityUnitId = service.QuantityUnitId,
                    CategoryId = service.CategoryId,
                    TimePerQuantityUnit = service.TimePerQuantityUnit,
                    TimeUnitId = service.TimeUnitId,
                    TotalVolume = service.TotalVolume
                });

            if (listing.Services.Count == 0)
                return Error("You must enable at least on service");

            var photos = new List<string>();
            if (Model.Photos != null)
                foreach (var photo in Model.Photos)
                {
                    var filename = Entities.File.SaveBase64Image(photo.Base64);
                    var file = new Entities.File(filename);
                    file.Resize(200, 200, file.ThumbName);
                    file.Resize(800, 800, file.ZoomName);
                    photos.Add(filename);
                }
            if (photos.Count > 0)
                listing.PhotoPaths = string.Join(",", photos.ToArray());

            if (listing.Save())
            {
                listing = Entities.Listing.Find(listing.Id);
                return Success(new
                {
                    Listing = listing.Json()
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("listings/edit")]
        [AcceptVerbs("POST")]
        public object Edit(ListingModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var listing = Entities.Listing.Find(Id: Model.Id);
            if (listing.UserId != CurrentUser.Id)
                return Error("Listing not found");

            listing.AvailableWithoutFuel = Model.AvailableWithoutFuel;
            listing.Brand = Model.Brand;
            listing.Category = null;
            listing.CategoryId = Model.CategoryId;
            listing.ConditionId = Model.ConditionId;
            listing.Latitude = Model.Latitude;
            listing.Longitude = Model.Longitude;
            listing.Description = Model.Description;
            listing.GroupServices = Model.GroupServices;
            listing.HorsePower = Model.HorsePower;
            listing.Location = Model.Location;
            listing.Title = Model.Title;
            listing.Year = Model.Year;

            listing.Services = new List<Entities.Service>();
            foreach (var service in Model.Services)
                listing.Services.Add(new Entities.Service
                {
                    Id = service.Id,
                    DistanceUnitId = service.DistanceUnitId,
                    FuelPerQuantityUnit = service.FuelPerQuantityUnit,
                    MaximumDistance = service.MaximumDistance,
                    MinimumQuantity = service.MinimumQuantity,
                    Mobile = service.Mobile,
                    PricePerDistanceUnit = service.PricePerDistanceUnit,
                    PricePerQuantityUnit = service.PricePerQuantityUnit,
                    QuantityUnitId = service.QuantityUnitId,
                    CategoryId = service.CategoryId,
                    TimePerQuantityUnit = service.TimePerQuantityUnit,
                    TimeUnitId = service.TimeUnitId,
                    TotalVolume = service.TotalVolume
                });

            var photos = new List<string>();
            if (Model.Photos != null)
                foreach (var photo in Model.Photos)
                {
                    if (photo.Filename.IsEmpty())
                        photos.Add(Entities.File.SaveBase64Image(photo.Base64));
                    else
                        photos.Add(photo.Filename);
                }
            if (photos.Count > 0)
                listing.PhotoPaths = string.Join(",", photos.ToArray());

            if (listing.Save())
            {
                listing = Entities.Listing.Find(listing.Id);
                return Success(new
                {
                    Listing = listing.Json()
                });
            }

            return Error("An unknown error occurred");
        }

        [Route("listings/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int ListingId)
        {
            var listing = Entities.Listing.Find(Id: ListingId);
            if (listing?.Id == 0 || listing?.UserId != CurrentUser.Id)
                return Error("Listing not found");

            if (listing.Delete())
                return Success(new
                {
                    listing.Id
                });

            return Error("An unknown error occurred");
        }

        [Route("listings/availability")]
        [AcceptVerbs("GET")]
        public object Availability(int ListingId, DateTime StartDate, DateTime EndDate)
        {
            var listing = Entities.Listing.Find(Id: ListingId);
            if (listing == null || listing.Id == 0)
                return Error("Listing not found");

            var bookings = Entities.Booking.List(ListingId: ListingId, StartDate: StartDate, EndDate: EndDate);

            var list = new List<object>();

            var date = StartDate;
            while (date <= EndDate)
            {
                var available = bookings.Where(o => o.StartDate <= date.StartOfDay() && o.EndDate >= date.EndOfDay()).Count() == 0;

                list.Add(new
                {
                    Date = date,
                    Available = available
                });

                date = date.AddDays(1);
            }

            return Success(new
            {
                Calendar = list
            });
        }

        [Route("listings/hide")]
        [AcceptVerbs("GET")]
        public object Hide(int ListingId)
        {
            var listing = Entities.Listing.Find(Id: ListingId);
            if (listing == null || listing.Id == 0 || listing.UserId != CurrentUser.Id)
                return Error("Listing not found");

            listing.StatusId = Entities.ListingStatus.Hidden;
            if (listing.Save())
                return Success(new
                {
                    Listing = listing.Json()
                });

            return Error("An unknown error occurred");
        }

        [Route("listings/show")]
        [AcceptVerbs("GET")]
        public object Show(int ListingId)
        {
            var listing = Entities.Listing.Find(Id: ListingId);
            if (listing == null || listing.Id == 0 || listing.UserId != CurrentUser.Id)
                return Error("Listing not found");

            listing.StatusId = Entities.ListingStatus.Live;
            if (listing.Save())
                return Success(new
                {
                    Listing = listing.Json()
                });

            return Error("An unknown error occurred");
        }
    }
}
