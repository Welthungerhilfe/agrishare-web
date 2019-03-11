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
    public class SmsController : BaseApiController
    {
        [Route("sms/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var entity = new SmsModel
            {
                RecipientCount = Entities.User.Count(),
                Sent = false
            };

            var data = new
            {
                Entity = entity
            };

            return Success(data);
        }

        [Route("sms/save")]
        [AcceptVerbs("POST")]
        public object Save(SmsModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            Model.Sent = true;

            var users = Entities.User.List().Select(e => e.Telephone).ToList();

            if (Core.Utils.SMS.SendMessages(users, Model.Message))
                return Success(new
                {
                    Entity = Model,
                    Feedback = "Broadcast SMS sent"
                });

            return Error();
        }
    }

    public class SmsModel
    {
        public string Message { get; set; }
        public int RecipientCount { get; set; }
        public bool Sent { get; set; }
    }
}
