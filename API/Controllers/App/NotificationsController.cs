using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    [@Authorize(Roles="User")]
    public class NotificationsController : BaseApiController
    {
        [Route("notifications/offering")]
        [AcceptVerbs("GET")]
        public object OfferingList(int PageIndex = 0, int PageSize = 25)
        {
            var list = Entities.Notification.List(PageIndex: PageIndex, PageSize: PageSize, UserId: CurrentUser.Id, GroupId: Entities.NotificationGroup.Offering);
            return Success(new
            {
                List = list.Select(e => e.Json())
            });
        }

        [Route("notifications/seeking")]
        [AcceptVerbs("GET")]
        public object SeekingList(int PageIndex = 0, int PageSize = 25)
        {
            var list = Entities.Notification.List(PageIndex: PageIndex, PageSize: PageSize, UserId: CurrentUser.Id, GroupId: Entities.NotificationGroup.Seeking);
            return Success(new
            {
                List = list.Select(e => e.Json())
            });
        }
    }
}
