using Agrishare.API;
using Agrishare.Core;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class TemplatesController : BaseApiController
    {
        [Route("templates/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Query = "")
        {
            var recordCount = Entities.Template.Count(Keywords: Query);
            var list = Entities.Template.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Template.DefaultSort,
                List = list.Select(e => e.ListJson()),
                Title = "Templates"
            };

            return Success(data);
        }

        [Route("templates/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Template.Find(Id: Id).DetailJson(),
                Types = EnumInfo.ToList<Entities.TemplateType>()
            };

            return Success(data);
        }

        [Route("templates/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Template Template)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Template.Save())
                return Success(new
                {
                    Entity = Template.DetailJson()
                });

            return Error();
        }
    }
}
