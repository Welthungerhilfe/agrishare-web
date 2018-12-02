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
    public class CMSFaqsController : BaseApiController
    {
        [Route("faqs/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Query = "")
        {
            var recordCount = Entities.Faq.Count(Keywords: Query);
            var list = Entities.Faq.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Faq.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Faqs"
            };

            return Success(data);
        }

        [Route("faqs/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Faq.Find(Id: Id).Json()
            };

            return Success(data);
        }

        [Route("faqs/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Faq Faq)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Faq.Save())
                return Success(new
                {
                    Entity = Faq.Json()
                });

            return Error();
        }

        [Route("faqs/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var faqs = Entities.User.Find(Id: Id);

            if (faqs.Delete())
                return Success(new
                {
                    Entity = faqs.Json()
                });

            return Error();
        }
    }
}
