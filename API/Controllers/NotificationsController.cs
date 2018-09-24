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

        [Route("notifications/read")]
        [AcceptVerbs("GET")]
        public object MarkAsRead(int NotificationId)
        {
            var notification = Entities.Notification.Find(Id: NotificationId);
            notification.StatusId = Entities.NotificationStatus.Read;

            if (notification.Save())
                return Success(new
                {
                    Notification = notification.Json()
                });

            return Error("An unknown error occurred");
        }
    }
}
