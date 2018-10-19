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
    public partial class Template : IEntity
    {
        public static string DefaultSort = "Title";
        public string Type => $"{TypeId}".ExplodeCamelCase();

        public static Template Find(int Id = 0, string Title = "")
        {
            if (Id == 0 && Title.IsEmpty())
                return new Template
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Templates.Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                if (!Title.IsEmpty())
                    query = query.Where(e => e.Title.Equals(Title, StringComparison.InvariantCultureIgnoreCase));

                return query.FirstOrDefault();
            }
        }

        public static List<Template> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Templates.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Templates.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

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
                ctx.Templates.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Templates.Attach(this);
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

        public object ListJson()
        {
            return new
            {
                Id,
                TypeId,
                Type,
                Title,
                DateCreated,
                LastModified
            };
        }

        public object DetailJson()
        {
            return new
            {
                Id,
                TypeId,
                Type,
                Title,
                HTML,
                EmailHTML = EmailHtml(),
                DateCreated,
                LastModified
            };

        }

        public bool Unique()
        {
            using (var ctx = new AgrishareEntities())
                return ctx.Templates.Where(o => o.Id != Id && o.Title.Equals(Title, StringComparison.InvariantCultureIgnoreCase)).Count() == 0;
        }

        #region Formatting Content

        public void Replace(string Placeholder, string Content)
        {
            HTML = Regex.Replace(HTML, @"{{" + Placeholder + "}}", Content ?? "");
        }

        public static string Replace(string HTML, string Placeholder, string Content)
        {
            return Regex.Replace(HTML, @"{{" + Placeholder + "}}", Content ?? "");
        }

        public string GetSectionTemplate(string Id)
        {
            var placeholder = Id.Replace(" ", "-");
            var regex = $@"^[\s\S]*(<!--{placeholder}-Start-->[\s\S]*<!--{placeholder}-End-->)[\s\S]*$";
            return Regex.Replace(HTML, regex, "$1");
        }

        public void ReplaceSectionTemplate(string Id, string Content)
        {
            var placeholder = Id.Replace(" ", "-");
            var regex = $@"(<!--{placeholder}-Start-->[\s\S]*<!--{placeholder}-End-->)";
            HTML = Regex.Replace(HTML, regex, Content);
        }

        public static string FormatCurrency(decimal Amount)
        {
            if (Amount < 0)
                return "-&dollar;" + Math.Abs(Amount).ToString("N2");
            return "&dollar;" + Amount.ToString("N2");
        }

        #endregion

        #region Email

        public string EmailHtml()
        {
            var template = Find(Title: "Email Template");
            template.Replace("Content", HTML);
            return template.HTML;
        }

        #endregion
    }
}
