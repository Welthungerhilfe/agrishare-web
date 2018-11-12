using Agrishare.API;
using Agrishare.Core;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class LogController : BaseApiController
    {
        [Route("log/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Query = "")
        {
            var recordCount = Entities.Log.Count(Keywords: Query);
            var list = Entities.Log.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Log.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Log"
            };

            return Success(data);
        }

        [Route("log/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Log.Find(Id: Id).DetailJson()
            };

            return Success(data);
        }
    }
}
