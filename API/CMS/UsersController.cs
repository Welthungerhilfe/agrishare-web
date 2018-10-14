using Agrishare.API;
using Agrishare.Core;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agri.API.CMS
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
        public object List(int PageIndex, int PageSize, [FromUri] UserFilter Filter)
        {
            var recordCount = Entities.User.Count(Keywords: Filter.Query);
            var list = Entities.User.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Filter.Query);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.User.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Users"
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
                Genders = EnumInfo.ToList<Entities.Gender>()
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
    }

    public class UserFilter
    {
        public string Query { get; set; }
    }
}
