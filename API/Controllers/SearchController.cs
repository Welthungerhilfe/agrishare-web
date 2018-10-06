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
    public class SearchController : BaseApiController
    {
        [Route("search")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Sort, int CategoryId, int ServiceId, decimal Latitude, decimal Longitude, DateTime StartDate, int Size, 
            bool IncludeFuel, bool Mobile, Entities.BookingFor For = Entities.BookingFor.Me, decimal DestinationLatitude = 0, decimal DestinationLongitude = 0)
        {
            var list = Entities.ListingSearchResult.List(PageIndex, PageSize, Sort, CategoryId, ServiceId, Latitude, Longitude, StartDate, Size, IncludeFuel, Mobile, For, DestinationLatitude, DestinationLongitude);
            return Success(new
            {
                List = list.Select(e => e.Json())
            });
        }
    }
}
