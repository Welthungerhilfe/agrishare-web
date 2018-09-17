using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Utils
{
    public class SQL
    {
        public static string Safe(object Value, string DateFormatString = "yyyy-MM-dd HH:mm:ss", int? MaxLength = null)
        {
            switch (Value.GetType().Name.ToString().ToLower())
            {
                case "datetime":
                    {
                        return ((DateTime)Value).ToString(DateFormatString);
                    }
                case "string":
                    {
                        var value = Value.ToString();

                        if (MaxLength.HasValue && value.Length > MaxLength.Value)
                            value = value.Substring(0, MaxLength.Value);

                        if (!value.IsEmpty())
                        {
                            value = value.Replace("'", "''");
                            value = value.Replace("’", "''");
                            value = value.Replace("‘", "''");
                            value = value.Replace(@"\", @"\\");
                        }

                        return value;
                    }
                case "bool":
                case "boolean":
                    {
                        return (bool)Value ? "1" : "0";
                    }
                default:
                    {
                        return Value.ToString();
                    }
            }
        }

        public static string Distance(decimal Latitude, decimal Longitude)
        {
            return  $"ROUND(ACOS(COS(RADIANS(latitude)) * COS(RADIANS(longitude)) * COS(RADIANS({Latitude})) * COS(RADIANS({Longitude})) + " +
                    $"COS(RADIANS(latitude)) * SIN(RADIANS(longitude)) * COS(RADIANS({Latitude})) * SIN(RADIANS({Longitude})) + SIN(RADIANS(latitude)) * " +
                    $"SIN(RADIANS({Latitude}))) * 3963.1) AS Distance";
        }
    }
}
