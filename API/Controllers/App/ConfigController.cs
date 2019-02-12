using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    public class ConfigController : BaseApiController
    {
        [Route("config/currentversion")]
        [AcceptVerbs("GET")]
        public object CurrentVersion()
        {
            return Success(new
            {
                Version = Entities.Config.Find(Key: "App Version")?.Value ?? "1.0.0"
            });
        }
    }
}
