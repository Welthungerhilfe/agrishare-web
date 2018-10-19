/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

using System;
using System.Text.RegularExpressions;

namespace Agrishare.Core
{
    public static class StringExtension
    {
        public static string Coalesce(this string str, string @default)
        {
            if (string.IsNullOrEmpty(str))
                return @default;
            return str;
        }

        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string Truncate(this string str, int Length)
        {
            if (str.Length > Length && Length > 3)
                return str.Substring(0, Length - 3) + "...";
            else if (str.Length > Length)
                return str.Substring(0, Length);
            return str;
        }

        public static string ExplodeCamelCase(this string str)
        {
            return string.Join(" ", Regex.Split(str, @"(?<!^)(?=[A-Z][^A-Z])"));
        }
    }
}
