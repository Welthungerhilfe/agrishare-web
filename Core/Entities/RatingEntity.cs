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
    public partial class Rating : IEntity
    {
        public static string DefaultSort = "DateCreated DESC";
        public string Title => User?.Title ?? "User";

        public static Rating Find(int Id = 0, int BookingId = 0)
        {
            if (Id == 0 && BookingId == 0)
                return new Rating
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Ratings.Include(o => o.User).Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                if (BookingId > 0)
                    query = query.Where(e => e.BookingId == BookingId);

                return query.FirstOrDefault();
            }
        }

        public static List<Rating> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", 
            string Keywords = "", string StartsWith = "", int UserId = 0, int ListingId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Ratings.Include(o => o.User).Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (ListingId > 0)
                    query = query.Where(o => o.ListingId == ListingId);

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", int UserId = 0, int ListingId = 0, int Stars = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Ratings.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (ListingId > 0)
                    query = query.Where(o => o.ListingId == ListingId);

                if (Stars > 0)
                    query = query.Where(o => o.Stars == Stars);

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var user = User;
            if (user != null)
                UserId = user.Id;
            User = null;

            var listing = Listing;
            if (listing != null)
                ListingId = listing.Id;
            Listing = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            User = user;
            Listing = listing;

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Ratings.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Ratings.Attach(this);
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
                ListingId,
                BookingId,
                User = User?.Json(),
                Title,
                Comments,
                Stars,
                DateCreated
            };
        }

        public static decimal AverageRating()
        {
            using (var ctx = new AgrishareEntities())
                return ctx.Ratings.Where(o => !o.Deleted).Select(o => o.Stars).DefaultIfEmpty(0).Average();
        }
    }
}
