using Agrishare.API;
using Agrishare.Core;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CMSSettlementController : BaseApiController
    {
        [Route("settlement/report")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage SettlementReport()
        {
            var bookings = Entities.Booking.List(StartDate: DateTime.Now.AddDays(-7), EndDate: DateTime.Now, Status: Entities.BookingStatus.Complete);

            var csv = new StringBuilder();
            csv.AppendLine("Date,Booking,Supplier,Telephone,Amount");
            foreach (var booking in bookings)
            {
                csv.Append(booking.StartDate.ToString("yyyy-MM-dd") + ",");
                csv.Append(booking.Id.ToString() + ",");
                csv.Append("\"" + booking.User.Title.Replace("\"", "\\\"") + "\",");
                csv.Append(booking.User.Telephone + ",");
                csv.Append((booking.Price - booking.AgriShareCommission).ToString());
                csv.AppendLine();
            }

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StringContent(csv.ToString());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = $"Settlement Report {DateTime.Now.ToString("yyyy-MM-dd")}.csv" };
            return result;
        }
    }
}
