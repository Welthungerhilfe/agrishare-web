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
    public class CMSBlogsController : BaseApiController
    {
        [Route("blogs/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex = 0, int PageSize = 25, string Query = "")
        {
            var recordCount = Entities.Blog.Count(Keywords: Query);
            var list = Entities.Blog.List(PageIndex: PageIndex, PageSize: PageSize, Keywords: Query);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Blog.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Blogs"
            };

            return Success(data);
        }

        [Route("blogs/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Blog.Find(Id: Id).Json()
            };

            return Success(data);
        }

        [Route("blogs/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Blog Blog)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Blog.Save())
                return Success(new
                {
                    Entity = Blog.Json()
                });

            return Error();
        }

        [Route("blogs/delete")]
        [AcceptVerbs("GET")]
        public object Delete(int Id)
        {
            var blogs = Entities.User.Find(Id: Id);

            if (blogs.Delete())
                return Success(new
                {
                    Entity = blogs.Json()
                });

            return Error();
        }
    }
}
