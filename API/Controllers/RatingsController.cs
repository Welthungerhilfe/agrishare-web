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
    public class RatingsController : BaseApiController
    {
        [Route("ratings")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, int ListingId)
        {
            var ratings = Entities.Rating.List(PageIndex: PageIndex, PageSize: PageSize, ListingId: ListingId);

            return Success(new
            {
                List = ratings.Select(e => e.Json())
            });
        }

        [Route("ratings/add")]
        [AcceptVerbs("POST")]
        public object Add(RatingModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            var rating = new Entities.Rating
            {
                Comments = Model.Comments,
                ListingId = Model.ListingId,
                Stars = Model.Rating,
                User = CurrentUser
            };

            if (rating.Save())
            {
                var listing = Entities.Listing.Find(Id: rating.ListingId);
                listing.AverageRating = ((listing.AverageRating * listing.RatingCount) + rating.Stars)  / (listing.RatingCount + 1);
                listing.RatingCount += 1;
                listing.Save();

                return Success(new
                {
                    Rating = rating.Json()
                });
            }

            return Error("An unknown error ocurred");
        }
    }
}
