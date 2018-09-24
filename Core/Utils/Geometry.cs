using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Utils
{
    public class Geometry
    {
        public static DbGeometry CreatePoint(decimal latitude, decimal longitude)
        {
            var point = string.Format(CultureInfo.InvariantCulture.NumberFormat, "POINT({0} {1})", longitude, latitude);
            // 4326 is most common coordinate system used by GPS/Maps
            return DbGeometry.PointFromText(point, 4326);
        }
    }
}
