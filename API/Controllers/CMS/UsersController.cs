using Agrishare.API;
using Agrishare.Core;
using System;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class UsersController : BaseApiController
    {
        [Route("users/me")]
        [AcceptVerbs("GET")]
        public object Me()
        {
            return Success(new
            {
                User = CurrentUser.Json()
            });
        }

        [Route("users/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Query = "")
        {
            var recordCount = Entities.User.Count(Keywords: Query);
            var list = Entities.User.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query);

            int total = 0, active = 0, male = 0, female = 0, deleted = 0, lockedout = 0;
            if (PageIndex == 0)
            {
                total = Entities.User.Count();
                male = Entities.User.Count(Gender: Entities.Gender.Male);
                female = Entities.User.Count(Gender: Entities.Gender.Female);
                deleted = Entities.User.Count(Deleted: true);
                lockedout = Entities.User.Count(FailedLoginAttempts: 1);
                active = Entities.Counter.Count(UniqueUser: true);
            }

            var data = new
            {
                Count = recordCount,
                Sort = Entities.User.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Users",
                Summary = new
                {
                    Total = total,
                    Active = active,
                    Male =  male,
                    Female = female,
                    Deleted = deleted,
                    LockedOut = lockedout
                }
            };

            return Success(data);
        }

        [Route("users/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.User.Find(Id: Id).AdminJson(),
                Roles = EnumInfo.ToList<Entities.Role>(),
                Genders = EnumInfo.ToList<Entities.Gender>(),
                Languages = EnumInfo.ToList<Entities.Language>()
            };

            return Success(data);
        }

        [Route("users/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.User User)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (User.DateOfBirth.HasValue)
                User.DateOfBirth = User.DateOfBirth.Value.AddMinutes(UTCOffset);

            if (User.Id > 0)
                User.Password = Entities.User.Find(Id: User.Id).Password;

            if (User.Save())
                return Success(new
                {
                    Entity = User.AdminJson()
                });

            return Error();
        }

        [Route("users/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var user = Entities.User.Find(Id: Id);

            var bookings = Entities.Booking.Count(UserId: user.Id, StartDate: DateTime.Now, Upcoming: true);
            if (bookings > 0)
                return Error("Can not delete account - user has upcoming bookings");

            var listings = Entities.Listing.List(UserId: user.Id);
            foreach (var listing in listings)
            {
                bookings = Entities.Booking.Count(ListingId: listing.Id, StartDate: DateTime.Now, Upcoming: true);
                if (bookings > 0)
                    return Error("Can not delete account - user has upcoming bookings");
            }
            foreach (var listing in listings)
                listing.Delete();

            var devices = Entities.Device.List(UserId: user.Id);
            foreach (var device in devices)
                device.Delete();

            user.Delete();

            return Success(new
            {
                Entity = user.AdminJson()
            });
        }

        /* Deleted */

        [Route("users/deleted/list")]
        [AcceptVerbs("GET")]
        public object DeletedList(int PageIndex = 0, int PageSize = 25, string Query = "")
        {
            var recordCount = Entities.User.Count(Keywords: Query, Deleted: true);
            var list = Entities.User.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query, Deleted: true);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.User.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Deleted Users"
            };

            return Success(data);
        }

        [Route("users/deleted/find")]
        [AcceptVerbs("GET")]
        public object FindDeleted(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.User.Find(Id: Id, Deleted: true).AdminJson(),
                Roles = EnumInfo.ToList<Entities.Role>(),
                Genders = EnumInfo.ToList<Entities.Gender>()
            };

            return Success(data);
        }
    }
}
