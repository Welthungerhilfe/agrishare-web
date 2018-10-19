using Agrishare.Core.Entities;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace Agrishare.API.Controllers.App
{
    public class UploadController : BaseApiController
    {
        [@Authorize(Roles="Administrator, User")]
        [Route("upload")]
        [AcceptVerbs("POST")]
        public object Post()
        {
            var list = new List<string>();
            foreach(HttpPostedFile file in HttpContext.Current.Request.Files)
                using (var ms = new System.IO.MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    list.Add(File.Save(file.FileName, ms.ToArray()));
                }
            return Success(list);
        }

    }
}
