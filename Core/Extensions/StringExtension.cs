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

        public static string AddQueryStringVariable(this string Url, string QueryStringPairs)
        {
            // Common variables
            char[] Separator = new char[1];
            string Path;
            string QueryString;
            string NewQueryString = "";

            // Split url into path and querystring
            if (Url.IndexOf("?") > -1)
            {
                Separator[0] = '?';
                string[] UrlParts = Url.Split(Separator, 100);
                Path = UrlParts[0];
                QueryString = UrlParts[1];
            }
            else
            {
                Path = Url;
                QueryString = "";
            }

            // Split QueryStrings into pairs
            Separator[0] = '&';
            string[] NewQueryStringPairs = QueryStringPairs.Split(Separator);
            string[] OldQueryStringPairs = QueryString.Split(Separator);

            // Loop through old querystring
            for (int i = 0; i < OldQueryStringPairs.Length; i++)
            {
                // Get name/value pairs
                Separator[0] = '=';
                string[] OldNameValuePair = OldQueryStringPairs[i].Split(Separator);

                // Loop through new querystring looking for same name
                bool PairExists = false;
                for (int j = 0; j < NewQueryStringPairs.Length; j++)
                {
                    // Get name/value pairs
                    string[] NewNameValuePair = NewQueryStringPairs[j].Split(Separator);

                    if (NewNameValuePair[0] == OldNameValuePair[0])
                    {
                        PairExists = true;
                    }
                }

                // Add non-existent pair to new QueryString
                if (!PairExists)
                {
                    NewQueryString += OldQueryStringPairs[i] + "&";
                }
            }

            // Add new querystring
            NewQueryString += QueryStringPairs;

            // Remove leading ampersand (&)
            if (NewQueryString.IndexOf("&") == 0)
            {
                NewQueryString = NewQueryString.Substring(1, NewQueryString.Length - 1);
            }

            // Return new Url
            return Path + "?" + NewQueryString;
        }

        public static string UrlPath(this string str)
        {
            return Regex.Replace(Regex.Replace(str.ToLower(), "[^a-z0-9]+", "-"), @"[-]+$", "");
        }
    }
}
