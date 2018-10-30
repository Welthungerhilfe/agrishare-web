using Agrishare.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Entities
{
    public enum ListingSearchResultSort
    {
        Distance,
        Price
    }

    public class ListingSearchResult
    {
        public int ListingId { get; set; }
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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Days { get; set; }

        public static List<ListingSearchResult> List(int PageIndex, int PageSize, string Sort, int CategoryId, int ServiceId, decimal Latitude, 
            decimal Longitude, DateTime StartDate, int Size, bool IncludeFuel, bool Mobile, BookingFor For, decimal DestinationLatitude, decimal DestinationLongitude)
        {
            var sort = ListingSearchResultSort.Distance;
            try { sort = (ListingSearchResultSort)Enum.Parse(typeof(ListingSearchResultSort), Sort); }
            catch { }

            var distance = SQL.Distance(Latitude, Longitude, "Listings");

            using (var ctx = new AgrishareEntities())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT");
                sql.AppendLine("Services.Id AS ServiceId,");
                sql.AppendLine("Listings.Id AS ListingId,");
                sql.AppendLine("Listings.Title AS Title,");
                sql.AppendLine("Listings.Year AS YEAR,");
                sql.AppendLine("Listings.ConditionId AS ConditionId,");
                sql.AppendLine("Listings.AverageRating AS AverageRating,");
                sql.AppendLine("Listings.Photos AS PhotoPaths,");

                sql.AppendLine($"IF((SELECT COUNT(Id) FROM Bookings WHERE Bookings.ListingId = Listings.Id AND Bookings.StartDate < DATE_ADD(DATE('{SQL.Safe(StartDate)}'), INTERVAL CEIL((Services.TimePerQuantityUnit * {Size}) / 8) DAY) AND Bookings.EndDate > DATE('{SQL.Safe(StartDate)}')) = 0, 1, 0) AS Available,");

                if (CategoryId == Category.LorriesId)
                {
                    var pickupToDropoff = SQL.Distance(Latitude, Longitude, DestinationLatitude, DestinationLongitude);
                    sql.AppendLine($"(Services.PricePerQuantityUnit * {pickupToDropoff})");
                }
                else
                {
                    sql.AppendLine($"(Services.PricePerQuantityUnit * {Size})");
                }
                
                if (CategoryId != Category.LorriesId && IncludeFuel)
                    sql.AppendLine($"+ (Services.FuelPerQuantityUnit * {Size} * Services.FuelPrice)");

                if (CategoryId == Category.LorriesId)
                {
                    var depotToPickup = SQL.Distance(Latitude, Longitude, "Listings");
                    var pickupToDropoff = SQL.Distance(Latitude, Longitude, DestinationLatitude, DestinationLongitude);
                    var dropoffToDepot = SQL.Distance(DestinationLatitude, DestinationLongitude, "Listings");
                    sql.AppendLine($"+ (({depotToPickup} + {pickupToDropoff} + {dropoffToDepot}) * Services.PricePerDistanceUnit)");
                }
                else
                    sql.AppendLine($"+ (Services.PricePerDistanceUnit * {distance} * 2)");

                sql.AppendLine("AS Price,");


                if (CategoryId == Category.TractorsId || CategoryId == Category.ProcessingId)
                {
                    sql.AppendLine($"{distance} AS Distance,");
                    sql.AppendLine($"CEIL((Services.TimePerQuantityUnit * {Size}) / 8) AS Days,");
                }
                else
                {
                    var depotToPickup = SQL.Distance(Latitude, Longitude, "Listings");
                    var pickupToDropoff = SQL.Distance(Latitude, Longitude, DestinationLatitude, DestinationLongitude);
                    var dropoffToDepot = SQL.Distance(DestinationLatitude, DestinationLongitude, "Listings");

                    sql.AppendLine($"({depotToPickup} + {pickupToDropoff} + {dropoffToDepot}) AS Distance,");
                    sql.AppendLine($"CEIL((Services.TimePerQuantityUnit * ({depotToPickup} + {pickupToDropoff} + {dropoffToDepot})) / 8) AS Days,");
                }

                sql.AppendLine($"DATE('{SQL.Safe(StartDate)}') AS StartDate,");
                sql.AppendLine($"DATE_ADD('{SQL.Safe(StartDate)}', INTERVAL CEIL((Services.TimePerQuantityUnit * {Size}) / 8) DAY) AS EndDate");
                sql.AppendLine("FROM Listings");
                sql.AppendLine("INNER JOIN Services ON Listings.Id = Services.ListingId");
                sql.AppendLine("WHERE Listings.Deleted = 0 AND Services.Deleted = 0 AND Listings.StatusId = 1");
                sql.AppendLine($"AND Listings.CategoryId = {CategoryId}");
                sql.AppendLine($"AND Services.Mobile = {SQL.Safe(Mobile)}");
                sql.AppendLine($"AND Services.CategoryId = {ServiceId}");
                sql.AppendLine($"AND {Size} >= MinimumQuantity");

                if (Mobile)
                    sql.AppendLine($"AND {distance} <= Services.MaximumDistance");

                if (For == BookingFor.Group)
                    sql.AppendLine($"AND Listings.GroupServices = 1");

                if (!IncludeFuel)
                    sql.AppendLine($"AND Listings.AvailableWithoutFuel = 1");

                sql.AppendLine($"GROUP BY Services.Id ORDER BY {sort} LIMIT {PageIndex * PageSize}, {PageSize}");

                #if DEBUG
                Log.Debug("Search SQL", sql.ToString());
                #endif

                return ctx.Database.SqlQuery<ListingSearchResult>(sql.ToString()).ToList();
            }
        }

        public object Json()
        {
            return new
            {
                ServiceId,
                ListingId,
                Title,
                Year,
                Condition,
                AverageRating,
                Photos = Photos?.Select(e => e.JSON()),
                Distance,
                Available,
                Price,
                StartDate,
                EndDate,
                Days
            };
        }

    }
}
