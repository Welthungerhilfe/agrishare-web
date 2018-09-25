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
    public partial class Config : IEntity
    {
        #region Common Settings

        private static string _applicationName { get; set; }
        public static string ApplicationName
        {
            get
            {
                if (_applicationName.IsEmpty())
                    _applicationName = Find(Key: "Application Name").Value;
                return _applicationName;
            }
        }

        private static string _applicationEmailAddress { get; set; }
        public static string ApplicationEmailAddress
        {
            get
            {
                if (_applicationEmailAddress.IsEmpty())
                    _applicationEmailAddress = Find(Key: "Application Email Address").Value;
                return _applicationEmailAddress;
            }
        }

        private static string _webURL { get; set; }
        public static string WebURL
        {
            get
            {
                if (_webURL.IsEmpty())
                    _webURL = Find(Key: "Web URL").Value;
                return _webURL;
            }
        }

        private static string _cdnURL { get; set; }
        public static string CDNURL
        {
            get
            {
                if (_cdnURL.IsEmpty())
                    _cdnURL = Find(Key: "CDN URL").Value;
                return _cdnURL;
            }
        }

        private static string _apiURL { get; set; }
        public static string APIURL
        {
            get
            {
                if (_apiURL.IsEmpty())
                    _apiURL = Find(Key: "API URL").Value;
                return _apiURL;
            }
        }

        #endregion

        public static string EncryptionPassword = "@DzaRtTxBBCTwG&53Ryh*t#x#m87Kg%$aH7P";
        public static string EncryptionSalt = "B6%X%gHW*H$&Y@7%!kvN!49GVnBxbMFkD7eY";

        public static string DefaultSort = "Key";
        public string Title => Key;

        private static string CacheKey(string Key)
        {
            return $"Config:{Key}";
        }

        public static Config Find(int Id = 0, string Key = "", bool IgnoreCache = false)
        {
            if (Id == 0 && Key.IsEmpty())
                return new Config
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

            if (!IgnoreCache && !Key.IsEmpty())
            {
                var item = Cache.Instance.Get<Config>(CacheKey(Key));
                if (item != null)
                    return item;
            }

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Configs.Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                if (!Key.IsEmpty())
                    query = query.Where(e => e.Key.Equals(Key, StringComparison.InvariantCultureIgnoreCase));

                var item = query.FirstOrDefault();

                if (item != null && !IgnoreCache)
                    Cache.Instance.Add(CacheKey(item.Key), item);

                return item;
            }
        }

        public static List<Config> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string StartsWith = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Configs.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Key.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Key.ToLower().StartsWith(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Configs.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Key.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Key.ToLower().StartsWith(Keywords.ToLower()));

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

            if (success)
                Cache.Instance.Add(CacheKey(Key), this);

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Configs.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Configs.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        public bool Delete()
        {
            if (Id == 0)
                return false;

            Deleted = true;
            if (Update())
            {
                Cache.Instance.Remove(CacheKey(Key));
                return true;
            }

            return false;
        }

        public object Json()
        {
            return new
            {
                Id,
                Title,
                Key,
                Value,
                DateCreated,
                LastModified
            };
        }

    }
}
