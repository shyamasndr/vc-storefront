﻿using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VirtoCommerce.Storefront.Model.Common
{
    public static class StringExtensions
    {
        /// <summary>
        /// http://stackoverflow.com/questions/484085/an-algorithm-to-spacify-camelcased-strings
        /// </summary>
        /// <param name="str"></param>
        /// <param name="spacer"></param>
        /// <returns></returns>
        public static string Decamelize(this string str, char spacer = '_')
        {
            if (string.IsNullOrEmpty(str))
                return str;

            /* Note that the .ToString() is required, otherwise the char is implicitly
             * converted to an integer and the wrong overloaded ctor is used */
            var sb = new StringBuilder(str[0].ToString());
            for (var i = 1; i < str.Length; i++)
            {
                if (char.IsUpper(str, i))
                    sb.Append(spacer);
                sb.Append(str[i]);
            }
            return sb.ToString().ToLowerInvariant();
        }

        /// <summary>
        /// Equals invariant
        /// </summary>
        /// <param name="str1">The STR1.</param>
        /// <param name="str2">The STR2.</param>
        /// <returns></returns>
        public static bool EqualsInvariant(this string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool FitsMask(this string fileName, string fileMask)
        {
            var mask = new Regex("^" + fileMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", ".") + "$", RegexOptions.IgnoreCase);
            return mask.IsMatch(fileName);
        }

        public static int? ToNullableInt(this string str)
        {
            int retVal;
            if (int.TryParse(str, out retVal))
            {
                return retVal;
            }
            return null;
        }

        public static Tuple<string, string, string> SplitIntoTuple(this string input, char separator)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var pieces = input.Split(separator);
            return Tuple.Create(pieces.FirstOrDefault(), pieces.Skip(1).FirstOrDefault(), pieces.Skip(2).FirstOrDefault());
        }

        public static string RemoveAccent(this string txt)
        {
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }

        public static string Handelize(this string phrase)
        {
            var retVal = phrase;
            if (phrase != null)
            {
                retVal = phrase.RemoveAccent().ToLower();

                retVal = Regex.Replace(retVal, @"[^a-z0-9\s-]", ""); // invalid chars           
                retVal = Regex.Replace(retVal, @"\s+", " ").Trim(); // convert multiple spaces into one space   
                retVal = retVal.Substring(0, retVal.Length <= 240 ? retVal.Length : 240).Trim(); // cut and trim it   
                retVal = Regex.Replace(retVal, @"\s", "-"); // hyphens   
            }
            return retVal;
        }

        //http://www.ietf.org/rfc/rfc3986.txt
        //section 4.2 Relative reference
        //Remove leading protocol scheme from uri. http://host/path -> //host/path
        public static string RemoveLeadingUriScheme(this string str)
        {
            if (!string.IsNullOrEmpty(str) && Uri.IsWellFormedUriString(str, UriKind.Absolute))
            {
                //remove scheme from image url
                //http://www.ietf.org/rfc/rfc3986.txt
                //section 4.2
                var uri = new Uri(str);
                str = "//" + uri.Authority + uri.PathAndQuery;
            }
            return str;
        }
    }
}
