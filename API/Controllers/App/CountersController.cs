using System;
using System.Globalization;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.App
{
    public class CounterController : BaseApiController
    {
        [Route("counter/update")]
        [AcceptVerbs("GET")]
        public object Update(string Event, string Category, DateTime Date, int Hits, string Subcategory = "")
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
                    Subcategory = Subcategory,
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
