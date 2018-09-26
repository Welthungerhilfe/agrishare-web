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
    public partial class BookingUser : IEntity
    {
        public static string DefaultSort = "Name";
        public string Title => Name;
        public string Status => $"{StatusId}".ExplodeCamelCase();

        public static BookingUser Find(int Id = 0)
        {
            if (Id == 0)
                return new BookingUser {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.BookingUsers.Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<BookingUser> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string StartsWith = "", int BookingId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.BookingUsers.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Name.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Name.ToLower().StartsWith(Keywords.ToLower()));

                if (BookingId > 0)
                    query = query.Where(o => o.BookingId == BookingId);

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", int BookingId = 0)
        {

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.BookingUsers.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Name.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Name.ToLower().StartsWith(Keywords.ToLower()));

                if (BookingId > 0)
                    query = query.Where(o => o.BookingId == BookingId);

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var booking = Booking;
            if (booking != null) BookingId = booking.Id;
            Booking = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Booking = booking;

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.BookingUsers.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.BookingUsers.Attach(this);
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
                BookingId,
                Name,
                Telephone,
                Ratio,
                StatusId,
                Status,
                DateCreated,
                LastModified
            };
        }

        public bool SendVerificationCode()
        {
            if (VerificationCode.IsEmpty() || VerificationCodeExpiry < DateTime.UtcNow)
            {
                VerificationCode = User.GeneratePIN(4);
                VerificationCodeExpiry = DateTime.UtcNow.AddDays(1);
                Save();
            }
            var message = $"Your verification code is {VerificationCode}";
            return SMS.SendMessage(Telephone, message);
        }
    }
}
