using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles="User")]
    public class ServicesController : BaseApiController
    {
        [Route("services/detail")]
        [AcceptVerbs("GET")]
        public object Details(int ServiceId)
        {
            var service = Entities.Service.Find(Id: ServiceId);

            if (service == null)
                return Error("Service not found");

            return Success(new
            {
                Listing = service.Listing.Json()        
            });
        }
    }
}
