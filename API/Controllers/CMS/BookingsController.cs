using Agrishare.API;
using Agrishare.Core;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CmsBookingsController : BaseApiController
    {
        [Route("bookings/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Query = "")
        {
            var recordCount = Entities.Booking.Count();
            var list = Entities.Booking.List(PageIndex: PageIndex, PageSize: PageSize);

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Booking.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = "Bookings"
            };

            return Success(data);
        }

        [Route("bookings/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = Entities.Booking.Find(Id: Id).Json()
            };

            return Success(data);
        }

        [Route("bookings/save")]
        [AcceptVerbs("POST")]
        public object Save(Entities.Booking Booking)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            if (Booking.Save())
                return Success(new
                {
                    Entity = Booking.Json()
                });

            return Error();
        }
    }
}
