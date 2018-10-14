using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Web;

namespace Agrishare.Core.Entities
{
    public partial class Counter : IEntity
    {
        public static string DefaultSort = "Date";
        public string Title => $"{Event}: {Category}";

        public static Counter Find(int Id = 0, string Event = "", string Category = "", string Subcategory = "", DateTime? Date = null)
        {
            if (Id == 0 && Event.IsEmpty() && Category.IsEmpty() && !Date.HasValue)
                return new Counter
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Counters.Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                if (!Event.IsEmpty())
                    query = query.Where(o => o.Event.Equals(Event, StringComparison.InvariantCultureIgnoreCase));

                if (!Category.IsEmpty())
                    query = query.Where(o => o.Category.Equals(Category, StringComparison.InvariantCultureIgnoreCase));

                if (!Subcategory.IsEmpty())
                    query = query.Where(o => o.Subcategory.Equals(Subcategory, StringComparison.InvariantCultureIgnoreCase));

                if (Date.HasValue)
                {
                    var start = Date.Value.StartOfDay();
                    var end = Date.Value.EndOfDay();
                    query = query.Where(o => o.Date >= start && o.Date <= end);
                }

                return query.FirstOrDefault();
            }
        }

        public static List<Counter> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string Event = "", string Category = "", string Subcategory = "", DateTime? StartDate = null, DateTime? EndDate = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Counters.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.Event + " " + o.Category).ToLower().Contains(Keywords.ToLower()));

                if (!Event.IsEmpty())
                    query = query.Where(o => o.Event.Equals(Event, StringComparison.InvariantCultureIgnoreCase));

                if (!Category.IsEmpty())
                    query = query.Where(o => o.Category.Equals(Category, StringComparison.InvariantCultureIgnoreCase));

                if (!Subcategory.IsEmpty())
                    query = query.Where(o => o.Subcategory.Equals(Subcategory, StringComparison.InvariantCultureIgnoreCase));

                if (StartDate.HasValue)
                {
                    var start = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.Date >= start);
                }

                if (EndDate.HasValue)
                {
                    var end = StartDate.Value.EndOfDay();
                    query = query.Where(o => o.Date <= end);
                }

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string Event = "", string Category = "", string Subcategory = "", DateTime? StartDate = null, DateTime? EndDate = null)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Counters.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.Event + " " + o.Category).ToLower().Contains(Keywords.ToLower()));

                if (!Event.IsEmpty())
                    query = query.Where(o => o.Event.Equals(Event, StringComparison.InvariantCultureIgnoreCase));

                if (!Category.IsEmpty())
                    query = query.Where(o => o.Category.Equals(Category, StringComparison.InvariantCultureIgnoreCase));

                if (!Subcategory.IsEmpty())
                    query = query.Where(o => o.Subcategory.Equals(Subcategory, StringComparison.InvariantCultureIgnoreCase));

                if (StartDate.HasValue)
                {
                    var start = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.Date >= start);
                }

                if (EndDate.HasValue)
                {
                    var end = StartDate.Value.EndOfDay();
                    query = query.Where(o => o.Date <= end);
                }

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Counters.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Counters.Attach(this);
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
                Event,
                Category,
                Subcategory,
                Date,
                Hits,
                DateCreated,
                LastModified
            };
        }
    }
}
