using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Entities
{
    class ListingSearchResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public int ConditionId { get; set; }
        public double AverageRating { get; set; }
        public string Photos { get; set; }
        public double Distance { get; set; }
        public bool Available { get; set; }
        public double Price { get; set; }

        public static List<Listing> List(int PageIndex = 0, int PageSize = int.MaxValue, string Sort = "", int CategoryId = 0, int SubcategoryId = 0, double Latitude = 0, double Longitude = 0)
        {
            // TODO calculate distance
            // TODO calculate price
            // TODO determine if available

            using (var ctx = new AgrishareEntities())
            {
                var sql = @"SELECT 
                                Services.Id AS Id, 
                                Listings.Title AS Title,
                                Listings.Year AS Year,
                                Listings.ConditionId AS ConditionId,
                                Listings.AverageRating AS AverageRating,
                                Listings.Photos AS Photos,
                                {DISTANCE FORMULA} AS Distance,
                                {AVAILABLE} AS Available,
                                {PRICE} AS Price
                            FROM 
                                Listings 
                            INNER JOIN 
                                Services 
                            ON Listings.Id = Services.ListingId";

                // Category Id == 1: field size * cost per field size + field size * fuel per field size
            }

            throw new NotImplementedException();
        }

    }
}
