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
    public partial class Faq : IEntity
    {
        public static string DefaultSort = "SortOrder";
        public string Title => Question;

        public static Faq Find(int Id = 0)
        {
            if (Id == 0)
                return new Faq
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Faqs.Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Faq> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Faqs.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.Question + " " + o.Answer).ToLower().Contains(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Faqs.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.Question + " " + o.Answer).ToLower().Contains(Keywords.ToLower()));

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
                try { SortOrder = ctx.Faqs.Max(o => o.SortOrder) + 1; }
                catch { SortOrder = 1; }

                ctx.Faqs.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Faqs.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        public bool UpdateOrder()
        {
            var success = false;

            using (var ctx = new AgrishareEntities())
            {
                if (SortOrder > 0)
                    ctx.Faqs.Where(o => o.SortOrder >= SortOrder).ToList().ForEach(o => { o.SortOrder += 1; });
                success = ctx.SaveChanges() > 0;
            }

            using (var ctx = new AgrishareEntities())
            {
                ctx.Faqs.Attach(this);
                if (SortOrder == 0)
                    SortOrder = ctx.Faqs.Max(o => o.SortOrder) + 1;
                ctx.Entry(this).State = EntityState.Modified;
                success = ctx.SaveChanges() > 0;
            }

            return success;
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
                Question,
                Answer,
                SortOrder,
                DateCreated,
                LastModified
            };
        }
    }
}
