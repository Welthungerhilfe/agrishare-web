using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Entities
{
    public class ListingSearchResult
    {
        public int ServiceId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public ListingCondition ConditionId { get; set; }
        public string Condition => $"{ConditionId}".ExplodeCamelCase();
        public double AverageRating { get; set; }
        public string PhotoPaths { get; set; }
        public double Distance { get; set; }
        public bool Available { get; set; }
        public double Price { get; set; }
        public List<File> Photos => PhotoPaths?.Split(',').Select(e => new File(e)).ToList();

        public static List<ListingSearchResult> List(int PageIndex, int PageSize, string Sort, int CategoryId, int ServiceId, double Latitude, double Longitude, DateTime StartDate, int Size)
        {
            // TODO calculate distance
            // TODO calculate price
            // TODO determine if available

            using (var ctx = new AgrishareEntities())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT");
                sql.AppendLine("Services.Id AS ServiceId,");
                sql.AppendLine("Listings.Title AS Title,");
                sql.AppendLine("Listings.Year AS Year,");
                sql.AppendLine("Listings.ConditionId AS ConditionId,");
                sql.AppendLine("Listings.AverageRating AS AverageRating,");
                sql.AppendLine("Listings.Photos AS PhotoPaths,");
                sql.AppendLine("99 AS Price,");
                sql.AppendLine("100 AS Distance");
                sql.AppendLine("FROM Listings");
                sql.AppendLine("INNER JOIN Services ON Listings.Id = Services.ListingId");
                sql.AppendLine("WHERE Listings.Deleted = 0 AND Services.Deleted = 0");

                if (CategoryId > 0)
                    sql.AppendLine($"AND Listings.CategoryId = {CategoryId}");

                if (ServiceId > 0)
                    sql.AppendLine($"AND Services.CategoryId = {ServiceId}");

                sql.AppendLine($"GROUP BY Services.Id ORDER BY {Sort.Coalesce("Distance")} LIMIT {PageIndex * PageSize}, {PageSize}");

                return ctx.Database.SqlQuery<ListingSearchResult>(sql.ToString()).ToList();
            }
        }

        public object Json()
        {
            return new
            {
                ServiceId,
                Title,
                Year,
                Condition,
                AverageRating,
                Photos = Photos?.Select(e => e.JSON()),
                Distance,
                Available,
                Price
            };
        }

    }
}
