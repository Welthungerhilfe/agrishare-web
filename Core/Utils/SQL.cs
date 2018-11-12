/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

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

        public static string Distance(decimal Latitude, decimal Longitude, string TableName = "")
        {
            if (!TableName.IsEmpty())
                TableName += ".";
            return  $"(6371 * ACOS(COS(RADIANS({Latitude})) * COS(RADIANS({TableName}Latitude)) * COS(RADIANS({TableName}Longitude) - " +
                    $"RADIANS({Longitude})) + SIN(RADIANS({Latitude})) * SIN(RADIANS({TableName}Latitude))))";            
        }


        public static string Distance(decimal SourceLatitude, decimal SourceLongitude, decimal DestinationLatitude, decimal DestinationLongitude)
        {
            return $"(6371 * ACOS(COS(RADIANS({SourceLatitude})) * COS(RADIANS({DestinationLatitude})) * COS(RADIANS({DestinationLongitude}) - " +
                    $"RADIANS({SourceLongitude})) + SIN(RADIANS({SourceLatitude})) * SIN(RADIANS({DestinationLatitude}))))";
        }
    }
}
