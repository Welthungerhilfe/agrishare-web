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
using System.Globalization;

namespace Agri.API.Controllers
{
    public class CounterController : BaseApiController
    {
        [Route("counter/update")]
        [AcceptVerbs("GET")]
        public object Update(string Event, string Category, DateTime Date, int Hits)
        {
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            Event = textInfo.ToTitleCase(Event.ToLower());
            Category = textInfo.ToTitleCase(Category.ToLower());

            var counter = Entities.Counter.Find(Event: Event, Category: Category, Date: Date);
            if (counter?.Id > 0)
                counter.Hits += Hits;
            else
                counter = new Entities.Counter
                {
                    Category = Category,
                    Date = Date,
                    Event = Event,
                    Hits = Hits
                };

            if (counter.Save())
                return Success("OK");

            return Error("An unknown error occurred");
        }
    }
}
