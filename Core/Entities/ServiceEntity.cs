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
    public partial class Service : IEntity
    {
        public static string DefaultSort = "DateCreated DESC";
        public string Title => Category?.Title ?? "Service";
        public string QuantityUnit => $"{QuantityUnitId}".ExplodeCamelCase();
        public string TimeUnit => $"{TimeUnitId}".ExplodeCamelCase();
        public string DistanceUnit => $"{DistanceUnitId}".ExplodeCamelCase();

        private Category _category;
        public Category Category
        {
            get
            {
                if (_category == null)
                    _category = Category.Find(CategoryId);
                return _category;
            }
            set
            {
                _category = value;
                if (value != null)
                    CategoryId = value.Id;
            }
        }

        public static Service Find(int Id = 0)
        {
            if (Id == 0)
                return new Service
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Services.Include(o => o.Listing).Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Service> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string StartsWith = "", int ListingId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Services.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                if (ListingId > 0)
                    query = query.Where(o => o.ListingId == ListingId);

                query = query.OrderBy(Sort.Coalesce(DefaultSort));

                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", int ListingId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Services.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                if (ListingId > 0)
                    query = query.Where(o => o.ListingId == ListingId);

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var category = Category;
            if (category != null)
                CategoryId = category.Id;
            Category = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Category = category;

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Services.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Services.Attach(this);
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
                Category = Category?.Json(),
                Mobile,
                TotalVolume,
                QuantityUnitId,
                QuantityUnit,
                TimeUnitId,
                TimeUnit,
                DistanceUnitId,
                DistanceUnit,
                MinimumQuantity,
                MaximumDistance,
                PricePerQuantityUnit,
                FuelPerQuantityUnit,
                TimePerQuantityUnit,
                PricePerDistanceUnit,
                FuelPrice,
                DateCreated
            };
        }
    }
}
