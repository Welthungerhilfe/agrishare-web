using Agrishare.Core.Utils;
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
    public partial class Blog : IEntity
    {
        public static string DefaultSort = "Id DESC";
        public string UrlPath => $"/blog/{Id}/{Title.UrlPath()}";
        private File _photo;
        public File Photo
        {
            get
            {
                if (_photo == null && !PhotoPath.IsEmpty())
                    _photo = new File(PhotoPath);
                return _photo;
            }
            set
            {
                _photo = value;
                if (_photo != null)
                    PhotoPath = _photo.Name + _photo.Extension;
            }
        }

        public static Blog Find(int Id = 0, string UrlPath = "")
        {
            if (!UrlPath.IsEmpty() && Regex.IsMatch(UrlPath, @"^/blog/[\d]+/.+$"))
                Id = Convert.ToInt32(Regex.Replace(UrlPath, @"^/blog/([\d]+)/.+$", "$1"));

            if (Id == 0)
                return new Blog {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Blogs.Where(o => o.Deleted == false);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Blog> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string StartsWith = "", bool Deleted = false)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Blogs.Where(o => o.Deleted == Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", bool Deleted = false)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Blogs.Where(o => o.Deleted == Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().StartsWith(Keywords.ToLower()));

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var photo = Photo;
            if (photo != null) PhotoPath = photo.Name + photo.Extension;
            Photo = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Photo = photo;

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Blogs.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Blogs.Attach(this);
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
                Photo = Photo?.JSON(),
                Content,
                DateCreated
            };
        }
    }
}
