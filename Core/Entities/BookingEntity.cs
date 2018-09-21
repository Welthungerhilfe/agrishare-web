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
        public static string DefaultSort = "DateCreated DESC";
        public string Title => Id.ToString() ?? "00000";
        public string For => $"{ForId}".ExplodeCamelCase();
        public string Status => $"{StatusId}".ExplodeCamelCase();

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
                var query = ctx.Bookings.Include(o => o.Service).Include(o => o.Listing).Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Booking> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", int ListingId = 0, int UserId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Include(o => o.Service).Include(o => o.Listing).Where(o => !o.Deleted);

                if (ListingId > 0)
                    query = query.Where(o => o.ListingId == ListingId);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                query = query.OrderBy(Sort.Coalesce(DefaultSort));

                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(int ListingId = 0, int UserId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Bookings.Where(o => !o.Deleted);

                if (ListingId > 0)
                    query = query.Where(o => o.ListingId == ListingId);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                return query.Count();
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

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Service = service;
            Listing = listing;

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
                ForId,
                For,
                UserId,
                Listing = Listing?.Json(),
                Service = Service?.Json(),
                Location,
                Latitude,
                Longitude,
                Quantity,
                Distance,
                IncludeFuel,
                StartDate,
                EndDate,
                Price,
                StatusId,
                Status,
                DateCreated
            };
        }
    }
}
