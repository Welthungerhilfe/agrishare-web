using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using Agrishare.API;
using Agrishare.Core;
using Entities = Agrishare.Core.Entities;
using System.Text.RegularExpressions;
using Agrishare.API.Models;
using Agrishare.Core.Utils;

namespace Agri.API.Controllers
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
