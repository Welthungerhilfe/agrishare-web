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
    public partial class User : IEntity
    {
        public static string AuthCookieName = "agrishare";
        public static string DefaultSort = "FirstName";
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Title => FullName;
        public string ClearPassword { get; set; }
        public string Interest => $"{InterestId}".ExplodeCamelCase();
        public string Gender => $"{GenderId}".ExplodeCamelCase();
        public string Status => $"{StatusId}".ExplodeCamelCase();
        public string Language => $"{LanguageId}".ExplodeCamelCase();

        private List<Role> roles { get; set; }
        public List<Role> Roles
        {
            get
            {
                if (roles == null)
                    roles = RoleList?.Split(',').Where(e => !e.Trim().IsEmpty()).Select(e => (Role)Enum.Parse(typeof(Role), e.Trim(), true)).ToList();
                return roles ?? new List<Role>();
            }
            set
            {
                roles = value;
                RoleList = string.Join(",", roles);
            }
        }

        private static string CacheKey(string AuthToken)
        {
            return $"User:{AuthToken}";
        }

        public static User Find(int Id = 0, string EmailAddress = "", string Telephone = "", string AuthToken = "", bool Deleted = false)
        {
            if (!AuthToken.IsEmpty())
            {
                var item = Cache.Instance.Get<User>(CacheKey(AuthToken));
                if (item != null)
                    return item;
            }

            if (Id == 0 && EmailAddress.IsEmpty() && Telephone.IsEmpty() && AuthToken.IsEmpty())
                return new User {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Users.Where(o => o.Deleted == Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                if (!EmailAddress.IsEmpty())
                    query = query.Where(e => e.EmailAddress.Equals(EmailAddress, StringComparison.InvariantCultureIgnoreCase));

                if (!Telephone.IsEmpty())
                    query = query.Where(e => e.Telephone.Equals(Telephone, StringComparison.InvariantCultureIgnoreCase));

                if (!AuthToken.IsEmpty())
                    query = query.Where(e => e.AuthToken.Equals(AuthToken));

                return query.FirstOrDefault();
            }
        }

        public static List<User> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string StartsWith = "", bool Deleted = false)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Users.Where(o => o.Deleted == Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().StartsWith(Keywords.ToLower()));

                return query.OrderBy(Sort.Coalesce(DefaultSort)).Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", bool Deleted = false)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Users.Where(o => o.Deleted == Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => (o.FirstName + " " + o.LastName).ToLower().StartsWith(Keywords.ToLower()));

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            if (!ClearPassword.IsEmpty())
            {
                Salt = Utils.Encryption.GetSalt();
                Password = Utils.Encryption.GetSHAHash(ClearPassword, Salt);
            }

            if (Id == 0)
                success = Add();
            else
                success = Update();

            if (!AuthToken.IsEmpty())
                Cache.Instance.Add(CacheKey(AuthToken), this);

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Users.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Users.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        public bool Delete()
        {
            if (Id == 0)
                return false;

            if (!AuthToken.IsEmpty())
                Cache.Instance.Remove(CacheKey(AuthToken));

            Deleted = true;
            return Update();
        }

        public object Json()
        {
            return new
            {
                Id,
                Title,
                FirstName,
                LastName,
                Telephone,
                Language
            };
        }

        public string ProfileJsonString()
        {
            return JsonConvert.SerializeObject(ProfileJson(), Formatting.None);
        }

        public object ProfileJson()
        {   
            return new
            {
                Id,
                FirstName,
                LastName,
                EmailAddress,
                Telephone,
                DateOfBirth,
                GenderId,
                Gender,
                AuthToken,
                NotificationPreferences = new
                {
                    SMS = (NotificationPreferences & (int)Agrishare.Core.Entities.NotificationPreferences.SMS) > 0,
                    PushNotifications = (NotificationPreferences & (int)Agrishare.Core.Entities.NotificationPreferences.PushNotifications) > 0,
                    Email = (NotificationPreferences & (int)Agrishare.Core.Entities.NotificationPreferences.Email) > 0
                },
                InterestId,
                Interest,
                LanguageId,
                Language
            };
        }

        public object AdminJson()
        {
            return new
            {
                Id,
                FirstName,
                LastName,
                EmailAddress,
                Telephone,
                DateOfBirth,
                GenderId,
                Gender,
                AuthToken,
                FailedLoginAttempts,
                VerificationCode,
                VerificationCodeExpiry,
                NotificationPreferences,
                InterestId,
                Interest,
                StatusId,
                Status,
                LanguageId,
                Language,
                Roles,
                DateCreated,
                LastModified
            };
        }

        public static int CleanDeletedAccounts()
        {
            using (var ctx = new AgrishareEntities())
            {
                var oneYearAgo = DateTime.Now.AddYears(-1);
                var users = ctx.Users.Where(o => o.Deleted && o.LastModified > oneYearAgo);
                foreach(var user in users)
                    ctx.Entry(user).State = EntityState.Deleted;
                return ctx.SaveChanges();
            }
        }

        #region Authorisation

        public bool UniqueTelephone()
        {
            using (var ctx = new AgrishareEntities())
                return ctx.Users.Count(o => !o.Deleted && o.Id != Id && o.Telephone == Telephone) == 0;
        }

        public bool UniqueEmailAddress()
        {
            using (var ctx = new AgrishareEntities())
                return ctx.Users.Count(o => !o.Deleted && o.Id != Id && o.EmailAddress == EmailAddress) == 0;
        }

        public bool ValidatePassword(string ClearPassword)
        {
            string encryptedPassowrd = Utils.Encryption.GetSHAHash(ClearPassword, Salt);
            return encryptedPassowrd == Password;
        }

        public bool SendVerificationCode()
        {
            if (VerificationCode.IsEmpty() || VerificationCodeExpiry < DateTime.UtcNow)
            {
                VerificationCode = GeneratePIN(4);
                VerificationCodeExpiry = DateTime.UtcNow.AddDays(1);
                Save();
            }
            var message = string.Format(Translations.Translate("Verification Code", LanguageId), VerificationCode);
            return SMS.SendMessage(Telephone, message);
        }

        #endregion

        #region Utils

        private static Random random = new Random();

        public static string GeneratePIN(int length)
        {  
            var randomString = "";
            for (int i = 0; i < length; i++)
                randomString = randomString + (char)random.Next(48, 58);
            return randomString;
        }

        #endregion
    }
}
