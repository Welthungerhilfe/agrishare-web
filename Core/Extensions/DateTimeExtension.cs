/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

using System;

namespace Agrishare.Core
{
    public static class DateTimeExtension
    {
        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        public static string TimeAgo(this DateTime date)
        {
            var timespan = DateTime.Now - date;

            if (timespan.TotalSeconds < 61)
                return "Just now";

            if (timespan.TotalMinutes < 60)
                return $"{timespan.TotalMinutes} mins ago";

            if (timespan.TotalHours == 1)
                return "1 hour ago";

            if (timespan.TotalHours < 24)
                return $"{timespan.TotalMinutes} hours ago";

            if (timespan.TotalDays == 1)
                return "1 day ago";

            if (timespan.TotalDays < 7)
                return $"{timespan.TotalDays} days ago";

            return date.ToString("dd/MM");
        }
    }
}
