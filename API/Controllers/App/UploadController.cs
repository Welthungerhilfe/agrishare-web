using System;
using System.Collections.Generic;
using System.IO;
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
                using (var ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    list.Add(Core.Entities.File.Save(file.FileName, ms.ToArray()));
                }
            return Success(list);
        }

        [Route("upload/photo")]
        [AcceptVerbs("POST")]
        public object UploadPhoto()
        {
            var list = new List<Core.Entities.File>();

            var files = HttpContext.Current.Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                var upload = files[i];
                var extension = Path.GetExtension(upload.FileName);
                if (extension.ToLower() == ".png" || extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg")
                {
                    var fileName = Guid.NewGuid() + extension;
                    var path = Core.Entities.File.CDNAbsolutePath + fileName;
                    upload.SaveAs(path);

                    var file = new Core.Entities.File(fileName);
                    file.Resize(200, 200, file.ThumbName);
                    file.Resize(800, 800, file.ZoomName);
                    list.Add(file);
                }
            }

            return Success(list);
        }
    }
}
