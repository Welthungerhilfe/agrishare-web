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
        public double Size { get; set; }
        public double TransportDistance { get; set; }
        public double TransportCost { get; set; }
        public double FuelCost { get; set; }
        public double HireCost { get; set; }
        public List<File> Photos => PhotoPaths?.Split(',').Select(e => new File(e)).ToList();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Days { get; set; }
        public int Trips { get; set; }

        public static List<ListingSearchResult> List(int PageIndex, int PageSize, string Sort, int CategoryId, int ServiceId, decimal Latitude, 
            decimal Longitude, DateTime StartDate, int Size, bool IncludeFuel, bool Mobile, BookingFor For, decimal DestinationLatitude, 
            decimal DestinationLongitude, decimal TotalVolume)
        {
            var sort = ListingSearchResultSort.Distance;
            try { sort = (ListingSearchResultSort)Enum.Parse(typeof(ListingSearchResultSort), Sort); }
            catch { }

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

                // distances
                var distance = SQL.Distance(Latitude, Longitude, "Listings");
                var depotToPickup = SQL.Distance(Latitude, Longitude, "Listings");
                var pickupToDropoff = SQL.Distance(Latitude, Longitude, DestinationLatitude, DestinationLongitude);
                var dropoffToDepot = SQL.Distance(DestinationLatitude, DestinationLongitude, "Listings");
                var trips = $"0";
                if (CategoryId == Category.LorriesId)
                    trips = $"(CEIL({TotalVolume} / Services.TotalVolume))";

                var transportDistance = $"0";
                if (CategoryId == Category.LorriesId)
                    transportDistance = $"({depotToPickup} + {dropoffToDepot} + ({pickupToDropoff} * ({trips} - 1)))";
                else if (Mobile)
                    transportDistance = $"{distance} * 2";

                // size
                string sizeField = "";
                if (CategoryId == Category.LorriesId)
                {
                    sizeField = $"({pickupToDropoff} * {trips})";
                    sql.AppendLine($"{sizeField} AS Size,");
                }
                else
                {
                    sizeField = $"({Size})";
                    sql.AppendLine($"{sizeField} AS Size,");
                }

                // time
                var days = $"CEIL((Services.TimePerQuantityUnit * {sizeField}) / 8)";
                if (CategoryId == Category.LorriesId)
                {
                    var totalDistance = $"(({depotToPickup} + {dropoffToDepot} + ({pickupToDropoff} * (({trips} * 2) - 1))) / 100)";
                    days = $"CEIL((Services.TimePerQuantityUnit * {totalDistance}) / 8)";
                }

                // availability
                sql.AppendLine($"IF((SELECT COUNT(Id) FROM Bookings WHERE Bookings.ListingId = Listings.Id AND Bookings.StartDate < DATE_ADD(DATE('{SQL.Safe(StartDate)}'), INTERVAL {days} DAY) AND Bookings.EndDate > DATE('{SQL.Safe(StartDate)}')) = 0, 1, 0) AS Available,");

                // costs
                var hireCost = $"(Services.PricePerQuantityUnit * {Size})";
                if (CategoryId == Category.LorriesId)
                    hireCost = $"(Services.PricePerQuantityUnit * {pickupToDropoff} * {trips})";
                var fuelCost = $"0";
                if (CategoryId != Category.LorriesId && IncludeFuel)
                    fuelCost = $"(Services.FuelPerQuantityUnit * {Size} * Services.FuelPrice)";
                var transportCost = $"({transportDistance} * Services.PricePerDistanceUnit)";

                sql.AppendLine($"{transportCost} AS TransportCost,");
                sql.AppendLine($"{fuelCost} AS FuelCost,");
                sql.AppendLine($"{hireCost} AS HireCost,");
                sql.AppendLine($"{hireCost} + {fuelCost} + {transportCost} AS Price,");
                sql.AppendLine($"{transportDistance} AS TransportDistance,");
                sql.AppendLine($"{distance} AS Distance,");
                sql.AppendLine($"{trips} AS Trips,");
                sql.AppendLine($"{days} AS Days,");

                sql.AppendLine($"DATE('{SQL.Safe(StartDate)}') AS StartDate,");
                sql.AppendLine($"DATE_ADD('{SQL.Safe(StartDate)}', INTERVAL {days} DAY) AS EndDate");
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
                Price,
                Size,
                Trips,
                TransportDistance,
                TransportCost,
                FuelCost,
                HireCost,
                Available,
                StartDate,
                EndDate,
                Days
            };
        }

    }
}
