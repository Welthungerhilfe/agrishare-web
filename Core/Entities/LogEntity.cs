using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Agrishare.Core.Entities
{
    public class LogDTO
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Title { get; set; }
        public LogLevel LevelId { get; set; }
        public DateTime DateCreated { get; set; }

        public object Json()
        {
            return new
            {
                Id,
                User,
                Title,
                LevelId,
                Level = $"{LevelId}".ExplodeCamelCase(),
                DateCreated,
            };
        }        

    }

    public partial class Log : IEntity
    {
        public static string DefaultSort = "Id DESC";
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

        public static List<LogDTO> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Logs.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()) || o.Description.ToLower().Contains(Keywords.ToLower()) || o.User.ToLower().Contains(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).Select(e => new LogDTO
                {
                    Id = e.Id,
                    User = e.User,
                    Title = e.Title,
                    LevelId = e.LevelId,
                    DateCreated = e.DateCreated
                }).ToList();
            }
        }

        public static int Count(string Keywords = "")
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Logs.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Title.ToLower().Contains(Keywords.ToLower()) && o.Description.ToLower().Contains(Keywords.ToLower()) && o.User.ToLower().Contains(Keywords.ToLower()));

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

        public object ListJson()
        {
            return new
            {
                Id,
                User,
                Title,
                LevelId,
                Level,
                DateCreated,
                LastModified
            };
        }

        public object DetailJson()
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
            return Environment.NewLine +
                    "****************************" +
                    Environment.NewLine +
                    "Exception: " + ex.Message + Environment.NewLine +
                    Environment.NewLine +
                    "Stack trace: " + ex.StackTrace.ToString() +
                    message +
                    Environment.NewLine +
                    GetValidationMessage(ex);
        }

        private static string GetValidationMessage(Exception InnerException)
        {
            var innerException = InnerException as DbEntityValidationException;
            if (innerException == null)
                return string.Empty;

            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            foreach (var eve in innerException.EntityValidationErrors)
            {
                sb.AppendLine(string.Format("- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().FullName, eve.Entry.State));
                foreach (var ve in eve.ValidationErrors)
                {
                    object value;
                    if (ve.PropertyName.Contains("."))
                    {
                        var propertyChain = ve.PropertyName.Split('.');
                        var complexProperty = eve.Entry.CurrentValues.GetValue<DbPropertyValues>(propertyChain.First());
                        value = GetComplexPropertyValue(complexProperty, propertyChain.Skip(1).ToArray());
                    }
                    else
                    {
                        value = eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName);
                    }
                    sb.AppendLine(string.Format("-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                        ve.PropertyName,
                        value,
                        ve.ErrorMessage));
                }
            }
            sb.AppendLine();

            return sb.ToString();
        }

        private static object GetComplexPropertyValue(DbPropertyValues propertyValues, string[] propertyChain)
        {
            var propertyName = propertyChain.First();
            return propertyChain.Count() == 1
                ? propertyValues[propertyName]
                : GetComplexPropertyValue((DbPropertyValues)propertyValues[propertyName], propertyChain.Skip(1).ToArray());
        }
    }
    }
