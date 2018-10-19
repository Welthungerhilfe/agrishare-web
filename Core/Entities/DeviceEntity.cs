using Agrishare.Core.Utils;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Agrishare.Core.Entities
{
    public partial class Device : IEntity
    {
        public static string DefaultSort = "DateCreated DESC";
        public string Title => User?.Title ?? "Device";

        public static Device Find(int Id = 0, string Token = "")
        {
            if (Id == 0 && Token.IsEmpty())
                return new Device
                {
                    DateCreated = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                };

            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Devices.Include(o => o.User).Where(o => !o.Deleted);

                if (Id > 0)
                    query = query.Where(e => e.Id == Id);

                if (!Token.IsEmpty())
                    query = query.Where(e => e.Token == Token);

                return query.FirstOrDefault();
            }
        }

        public static List<Device> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", string Keywords = "", string StartsWith = "", int UserId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Devices.Include(o => o.User).Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Token.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Token.ToLower().StartsWith(Keywords.ToLower()));

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                query = query.OrderBy(Sort.Coalesce(DefaultSort));

                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public static int Count(string Keywords = "", string StartsWith = "", int UserId = 0)
        {
            using (var ctx = new AgrishareEntities())
            {
                var query = ctx.Devices.Where(o => !o.Deleted);

                if (!Keywords.IsEmpty())
                    query = query.Where(o => o.Token.ToLower().Contains(Keywords.ToLower()));

                if (!StartsWith.IsEmpty())
                    query = query.Where(o => o.Token.ToLower().StartsWith(Keywords.ToLower()));

                if (UserId > 0)
                    query = query.Where(o => o.UserId == UserId);

                return query.Count();
            }
        }

        public bool Save()
        {
            var success = false;

            var user = User;
            if (user != null)
                UserId = user.Id;
            User = null;

            if (EndpointARN.IsEmpty() && !Token.IsEmpty())
                if (SNS.AddDevice(Token, out var arn))
                    EndpointARN = arn;
                else
                    return false;

            if (Id == 0)
                success = Add();
            else
                success = Update();

            User = user;

            return success;
        }

        private bool Add()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Devices.Attach(this);
                ctx.Entry(this).State = EntityState.Added;
                return ctx.SaveChanges() > 0;
            }
        }

        private bool Update()
        {
            using (var ctx = new AgrishareEntities())
            {
                ctx.Devices.Attach(this);
                ctx.Entry(this).State = EntityState.Modified;
                return ctx.SaveChanges() > 0;
            }
        }

        public bool Delete()
        {
            if (Id == 0)
                return false;

            if (SNS.RemoveDevice(EndpointARN))
            {
                Deleted = true;
                return Update();
            }

            return false;
        }

        public object Json()
        {
            return new
            {
                Id,
                User = User.Json(),
                Token,
                EndpointARN,
                DateCreated
            };
        }

        public bool SendMessage(string Message, string Category = "", Dictionary<string, object> Params = null)
        {
            return SNS.SendMessage(EndpointARN, Message, Category, Params);
        }
    }
}
