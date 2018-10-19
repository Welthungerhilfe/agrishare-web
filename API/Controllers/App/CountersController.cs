using System;
using System.Globalization;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    public class CounterController : BaseApiController
    {
        [Route("counter/add")]
        [AcceptVerbs("GET")]
        public object Add(string Event, int ServiceId)
        {
            if (Entities.Counter.Hit(CurrentUser.Id, Event, ServiceId))
                return Success();

            return Error();
        }
    }
}
