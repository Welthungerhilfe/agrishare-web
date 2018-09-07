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
    public partial class Log : IEntity
    {
        public static string DefaultSort = "DateCreated DESC";
        public string Level => $"{LevelId}".ExplodeCamelCase();

        public static Log Find(int Id = 0)
        {
            if (Id == 0)
                return new Log
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Logs.Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Log> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Logs.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.Title + " " + o.Description + " " + o.User).ToLower().Contains(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Logs.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.Title + " " + o.Description + " " + o.User).ToLower().Contains(Keywords.ToLower()));

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
                ctx.Logs.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Logs.Attach(this);
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
                User,
                Title,
                Description,
                LevelId,
                Level,
                DateCreated,
                LastModified
            };
        }

        public static void Error(string Title, string Description)
        {
            new Log
            {
                Description = Description,
                LevelId = LogLevel.Error,
                Title = Title
            }.Save();
        }

        public static void Error(string Title, Exception Exception)
        {
            new Log
            {
                Description = GetExceptionMessage(Exception),
                LevelId = LogLevel.Error,
                Title = Title
            }.Save();
        }

        public static void Debug(string Title, string Message)
        {
            new Log
            {
                Description = Message,
                LevelId = LogLevel.Debug,
                Title = Title
            }.Save();
        }

        public static string GetExceptionMessage(Exception ex)
        {
            var message = "";
            if (ex.InnerException != null)
                message += GetExceptionMessage(ex.InnerException);
            return
                    Environment.NewLine +
                    "****************************" +
                    Environment.NewLine +
                    "Exception: " + ex.Message + Environment.NewLine +
                    Environment.NewLine +
                    "Stack trace: " + ex.StackTrace.ToString() +
                    message;
        }
    }
}
