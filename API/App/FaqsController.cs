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

namespace Agri.API.Controllers
{
    public class FaqsController : BaseApiController
    {
        [Route("faqs/list")]
        [AcceptVerbs("GET")]
        public object List()
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            return Success(new
            {
                List = Entities.Faq.List().Select(e => e.Json())
            });
        }

        [@Authorize(Roles="User")]
        [Route("contact")]
        [AcceptVerbs("POST")]
        public object Contact(ContactModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            new Entities.Email
            {
                Message = Model.Message,
                RecipientEmail = Entities.Config.ApplicationEmailAddress,
                SenderEmail = CurrentUser.EmailAddress.Coalesce($"{CurrentUser.Telephone}@hariplay.app"),
                Subject = "Contact from AgriShare app"
            }.Send();

            return Success("Your message has been sent");
        }
    }
}
