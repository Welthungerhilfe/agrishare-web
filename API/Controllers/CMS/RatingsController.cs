using Agrishare.API;
using Agrishare.Core;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CmsRatingsController : BaseApiController
    {
        [Route("ratings/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Query = "", int ListingId = 0)
        {
            var recordCount = Entities.Rating.Count(Keywords: Query, ListingId: ListingId);
            var list = Entities.Rating.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query, ListingId: ListingId);
            var title = "Ratings";
            
            if (ListingId > 0)
            {
                var listing = Entities.Listing.Find(Id: ListingId);
                if (listing != null)
                    title = listing.Title;
            }

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Rating.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = title
            };

            return Success(data);
        }
    }
}
