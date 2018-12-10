using Agrishare.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Web;

namespace Agrishare.Core.Entities
{
    public enum Counters
    {
        None = 0,
        Launch = 1,
        Register = 2,
        Login = 3,
        ForgotPIN = 4,
        ResetPIN = 5,
        Search = 6,
        Match = 7,
        Book = 8,
        ConfirmBooking = 9,
        InitiatePayment = 10,
        CompletePayment = 11,
        CompleteBooking = 12,
        CancelBooking = 13,
        IncompleteBooking = 14
    }

    public partial class Counter : IEntity
    {
        public static string DefaultSort = "Date";
        public string Title => $"{Event}";

        public static int Count(Counters Event = Counters.None, DateTime? StartDate = null, DateTime? EndDate = null, Gender Gender = Gender.None, int ServiceId = 0, bool UniqueUser = false, int CategoryId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Counters.Include(o => o.User).Include(o => o.Service).Where(o => !o.Deleted);

                if (Event != Counters.None)
                {
                    var eventString = $"{Event}".ExplodeCamelCase();
                    query = query.Where(e => e.Event == eventString);
                }

                if (Gender != Gender.None)
                    query = query.Where(e => e.User.GenderId == Gender);

                if (StartDate.HasValue)
                {
                    var start = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated >= start);
                }

                if (EndDate.HasValue)
                {
                    var end = StartDate.Value.EndOfDay();
                    query = query.Where(o => o.DateCreated <= end);
                }

                if (CategoryId > 0)
                    query = query.Where(o => o.Service.ParentId == CategoryId);

                if (UniqueUser)
                    return query.Select(e => e.UserId).Distinct().Count();

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var user = User;
            if (user != null) UserId = user.Id;
            User = null;

            var service = Service;
            if (service != null) ServiceId = service.Id;
            Service = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            User = user;
            Service = service;

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
                Service = Service?.Json(),
                User = User?.Json(),
                DateCreated,
                LastModified
            };
        }

        public static int ActiveUsers(DateTime StartDate, DateTime EndDate)
        {
            using (var ctx = new AgrishareEntities())
            {
                var sql = $"SELECT COUNT(DISTINCT(UserId)) FROM Counters WHERE DATE(DateCreated) >= DATE('{SQL.Safe(StartDate)}') AND DATE(DateCreated) <= ('{SQL.Safe(EndDate)}')";
                return ctx.Database.SqlQuery<int>(sql).DefaultIfEmpty(0).FirstOrDefault();
            }
        }

        // TODO update this method to use caching so that we only write to the db once every minute instead of once every hit
        // Using caching will limit the disk IO which will become an issue
        public static bool Hit(int UserId, Counters Event, int ServiceId = 0)
        {
            return Hit(UserId, $"{Event}".ExplodeCamelCase(), ServiceId);
        }
        public static bool Hit(int UserId, string Event, int ServiceId = 0)
        {
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            Event = textInfo.ToTitleCase(Event.ToLower());

            var hit = new Counter { Event = Event };
            if (UserId > 0)
                hit.UserId = UserId;
            if (ServiceId > 0)
                hit.ServiceId = ServiceId;

            return hit.Save();
        }
    }
}
