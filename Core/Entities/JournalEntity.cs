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
    public partial class Journal : IEntity
    {
        public static string DefaultSort = "DateCreated DESC";
        public string Type => $"{TypeId}".ExplodeCamelCase();
        public decimal Balance { get; set; }
        public decimal Debit => Amount < 0 ? Math.Abs(Amount) : 0;
        public decimal Credit => Amount > 0 ? Math.Abs(Amount) : 0;

        public static Journal Find(int Id = 0)
        {
            if (Id == 0)
                return new Journal
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Journals.Include(o => o.User).Include(o => o.Booking).Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                return query.FirstOrDefault();
            }
        }

        public static List<Journal> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", int BookingId = 0, int UserId = 0, 
            DateTime? StartDate = null, DateTime? EndDate = null, JournalType Type = JournalType.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Journals.Include(o => o.User).Include(o => o.Booking).Where(o => !o.Deleted);

                if (BookingId > 0)
                    query = query.Where(o => o.BookingId == BookingId);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated >= startDate);
                }

                if (EndDate.HasValue)
                {
                    var endDate = EndDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated <= endDate);
                }

                if (Type != JournalType.None)
                    query = query.Where(e => e.TypeId == Type);

                query = query.OrderBy(Sort.Coalesce(DefaultSort));

                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(int BookingId = 0, int UserId = 0, DateTime? StartDate = null, DateTime? EndDate = null, JournalType Type = JournalType.None)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Journals.Where(o => !o.Deleted);

                if (BookingId > 0)
                    query = query.Where(o => o.BookingId == BookingId);

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                if (StartDate.HasValue)
                {
                    var startDate = StartDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated >= startDate);
                }

                if (EndDate.HasValue)
                {
                    var endDate = EndDate.Value.StartOfDay();
                    query = query.Where(o => o.DateCreated <= endDate);
                }

                if (Type != JournalType.None)
                    query = query.Where(e => e.TypeId == Type);

                return query.Count();
            }
        }

        public static decimal BalanceAt(DateTime Date)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Journals.Where(o => !o.Deleted);
                query = query.Where(o => o.DateCreated <= Date);
                try
                {
                    return query.Select(o => o.Amount).DefaultIfEmpty(0).Sum();
                }
                catch
                {
                    return 0;
                }
            }

        }

        public static decimal BalanceAt(int Id)
        {
            if (Id == 0)
                return 0;

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Journals.Where(o => !o.Deleted);
                query = query.Where(o => o.Id <= Id);
                try
                {
                    return query.Select(o => o.Amount).DefaultIfEmpty(0).Sum();
                }
                catch
                {
                    return 0;
                }
            }
        }

        public bool Save()
        {
            var success = false;

            var booking = Booking;
            if (booking != null)
                BookingId = booking.Id;
            Booking = null;

            var user = User;
            if (user != null)
                UserId = user.Id;
            User = null;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            Booking = booking;
            User = user;

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Journals.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Journals.Attach(this);
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
                User = User?.Json(),
                Booking = Booking?.Json(),
                Amount,
                Credit,
                Debit,
                Reconciled,
                EcoCashReference,
                TypeId,
                Type,
                DateCreated,
                LastModified,
                Deleted,
                Balance
            };
        }
    }

    public partial class JournalImport : ImportExcel
    {
        private List<Booking> Bookings;

        public JournalImport()
        {
            Bookings = Booking.List(Status: BookingStatus.Complete);
        }

        public override bool ValidateCell(int Column, string Data, out string Error)
        {
            Error = string.Empty;

            switch (Column)
            {
                case 1: // Date
                    Error += Required(Data);
                    Error += Date(Data);
                    break;
                case 2: // Booking ID
                    if (!Data.IsEmpty())
                    {
                        Error = Integer(Data);
                        if (Bookings.Where(o => o.Id == Convert.ToInt32(Data)).Count() == 0)
                            Error = "Booking does not exist";
                    }
                    break;
                case 3: // Supplier Name
                    Error += Required(Data);
                    Error += MaxLength(Data, 128);
                    break;
                case 4: // Supplier Telephone
                    Error += Required(Data);
                    Error += MaxLength(Data, 128);
                    break;
                case 5: // Amount
                    Error += Required(Data);
                    Error += Decimal(Data);
                    break;
                case 6: // EcoCash Reference
                    Error += Required(Data);
                    Error += MaxLength(Data, 128);
                    break;
            }

            return string.IsNullOrEmpty(Error);
        }

        public override void ExtractRow(List<string> Row)
        {
            var booking = Bookings.FirstOrDefault(o => o.Id == Convert.ToInt32(Row[1]));
            if (booking != null)
            {
                booking.PaidOut = true;
                booking.Save();

                new Journal
                {
                    Amount = Convert.ToDecimal(Row[4]),
                    Booking = booking,
                    EcoCashReference = Row[5],
                    Reconciled = false,
                    Title = $"Pay out to {Row[2]} {Row[3]}",
                    TypeId = JournalType.Settlement,
                    UserId = booking.Listing.UserId
                }.Save();
            }
        }
    }
}
