using Agrishare.API;
using Agrishare.Core;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CmsListingsController : BaseApiController
    {
        [Route("listings/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Query = "", int UserId = 0)
        {
            var recordCount = Entities.Listing.Count(Keywords: Query, UserId: UserId);
            var list = Entities.Listing.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query, UserId: UserId);
            var title = "Listings";
            
            if (UserId > 0)
            {
                var user = Entities.User.Find(Id: UserId);
                title = $"{user.FirstName} {user.LastName}";
            }

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Listing.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = title
            };

            return Success(data);
        }

        [Route("listings/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Listing.Find(Id: Id).Json(IncludeUser: true),
                Categories = Entities.Category.List().Select(e => e.Json()),
                Conditions = EnumInfo.ToList<Entities.ListingCondition>(),
                Statuses = EnumInfo.ToList<Entities.ListingStatus>()
            };

            return Success(data);
        }

        [Route("listings/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Listing Listing)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Listing.Save())
                return Success(new
                {
                    Entity = Listing.Json()
                });

            return Error();
        }
    }
}
