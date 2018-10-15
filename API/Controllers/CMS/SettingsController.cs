using Agrishare.API;
using Agrishare.Core;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class SettingsController : BaseApiController
    {
        [Route("settings/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Query = "")
        {
            var recordCount = Entities.Config.Count(Keywords: Query);
            var list = Entities.Config.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Config.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Settings"
            };

            return Success(data);
        }

        [Route("settings/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Config.Find(Id: Id).Json()
            };

            return Success(data);
        }

        [Route("settings/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Config Setting)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Setting.Save())
                return Success(new
                {
                    Entity = Setting.Json()
                });

            return Error();
        }
    }
}
