using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Utils
{
    public class Location
    {
        public static double GetDistance(double OriginLongitude, double OriginLatitude, double DestinationLongitude, double DestinationLatitude)
        {
            var d1 = OriginLatitude * (Math.PI / 180.0);
            var num1 = OriginLongitude * (Math.PI / 180.0);
            var d2 = DestinationLatitude * (Math.PI / 180.0);
            var num2 = DestinationLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }
    }
}
