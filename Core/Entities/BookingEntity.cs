using Agrishare.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Agrishare.Core.Entities
{
    public partial class Booking : IEntity
    {
        public static string DefaultSort = "StartDate DESC";
        public string Title => Id.ToString() ?? "00000";
        public string For => $"{ForId}".ExplodeCamelCase();
        public string Status => $"{StatusId}".ExplodeCamelCase();
        public decimal AgriShareCommission => Transaction.AgriShareCommission * Price;

        public static Booking Find(int Id = 0)
        {
            if (Id == 0)
                return new Booking
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Include(o => o.User).Include(o => o.Service).Include(o => o.Listing).Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Booking> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", int ListingId = 0, int UserId = 0, 
            int SupplierId = 0, DateTime? StartDate = null, DateTime? EndDate = null, BookingStatus Status = BookingStatus.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Include(o => o.User).Include(o => o.Service).Include(o => o.Listing).Where(o => !o.Deleted);

                if (ListingId > 0)
                    query = query.Where(o => o.ListingId == ListingId);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (SupplierId > 0)
                    query = query.Where(o => o.Listing.UserId == SupplierId);

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.EndDate >= startDate);
                }

                if (EndDate.HasValue)
                {
                    var endDate = EndDate.Value.StartOfDay();
                    query = query.Where(o => o.StartDate <= endDate);
                }

                if (Status != BookingStatus.None)
                    query = query.Where(e => e.StatusId == Status);

                query = query.OrderBy(Sort.Coalesce(DefaultSort));

                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(int ListingId = 0, int UserId = 0, int SupplierId = 0, DateTime? StartDate = null, DateTime? EndDate = null, BookingStatus Status = BookingStatus.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Include(o => o.Listing).Where(o => !o.Deleted);

                if (ListingId > 0)
                    query = query.Where(o => o.ListingId == ListingId);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (SupplierId > 0)
                    query = query.Where(o => o.Listing.UserId == SupplierId);

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.EndDate >= startDate);
                }

                if (EndDate.HasValue)
                {
                    var endDate = EndDate.Value.StartOfDay();
                    query = query.Where(o => o.StartDate <= endDate);
                }

                if (Status != BookingStatus.None)
                    query = query.Where(e => e.StatusId == Status);

                return query.Count();
            }
        }

        public static decimal SeekingSummary(int UserId, DateTime? StartDate = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Include(o => o.Listing)
                        .Where(o => !o.Deleted && o.UserId == UserId && (o.StatusId == BookingStatus.Approved || o.StatusId == BookingStatus.Complete || o.StatusId == BookingStatus.InProgress));

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.StartDate >= startDate);
                }

                return query.Select(o => o.Price).DefaultIfEmpty(0).Sum();
            }

        }

        public static decimal OfferingSummary(int UserId, DateTime? StartDate = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Include(o => o.Listing)
                        .Where(o => !o.Deleted && o.Listing.UserId == UserId && (o.StatusId == BookingStatus.Approved || o.StatusId == BookingStatus.Complete || o.StatusId == BookingStatus.InProgress));

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.StartDate >= startDate);
                }

                return query.Select(o => o.Price).DefaultIfEmpty(0).Sum();
            }

        }

        public bool Save()
        {
            var success = false;

            var service = Service;
            if (service != null)
                ServiceId = service.Id;
            Service = null;

            var listing = Listing;
            if (listing != null)
                ListingId = listing.Id;
            Listing = null;

            var user = User;
            if (user != null)
                UserId = user.Id;
            User = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Service = service;
            Listing = listing;
            User = user;

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Bookings.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Bookings.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        public bool Delete()
        {
            if (Id == 0)
                return false;

            Deleted = true;
            return Update();
        }

        public object Json()
        {
            return new
            {
                Id,
                Title,
                ForId,
                For,
                User = User?.Json(),
                Listing = Listing?.Json(),
                Service = Service?.Json(),
                Location,
                Latitude,
                Longitude,
                Destination,
                DestinationLatitude,
                DestinationLongitude,
                Quantity,
                Distance,
                IncludeFuel,
                StartDate,
                EndDate,
                Price,
                AgriShareCommission,
                HireCost,
                FuelCost,
                TransportCost,
                StatusId,
                Status,
                DateCreated
            };
        }

        public static decimal TotalAmountPaid(DateTime StartDate, DateTime EndDate)
        {
            using (var ctx = new AgrishareEntities())
            {
                StartDate = StartDate.StartOfDay();
                EndDate = EndDate.EndOfDay();
                return ctx.Bookings.Where(o => !o.Deleted && o.StatusId == BookingStatus.Complete && o.StartDate <= EndDate && o.EndDate >= StartDate).Select(e => e.Price).DefaultIfEmpty(0).Sum();
            }
        }
    }
}
