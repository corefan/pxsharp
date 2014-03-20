using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxSharp.ApiGen {
    public static class Utils {

        public static string Join (this IEnumerable<string> v, string separator) {
            return String.Join(separator, v.ToArray());
        }

        public static string StripComments (this string value) {
            value = Regex.Replace(value, @"(?<!/)/\*.*?\*/", "", RegexOptions.Singleline);
            value = Regex.Replace(value, @"(//|#).*", "");
            return value;
        }

        public static string Grab (this string value, string regex, RegexOptions options = RegexOptions.IgnoreCase) {
            return Regex.Match(value, regex, options).Groups[0].Value.Trim();
        }

        public static string Strip (this string value, string regex, RegexOptions options = RegexOptions.IgnoreCase) {
            return Regex.Replace(value, regex, "", options);
        }

        public static bool HasValue (this string value) {
            return String.IsNullOrWhiteSpace(value) == false;
        }

        public static IEnumerable<Match> MatchAll (this string value, string regex, RegexOptions options = RegexOptions.None) {
            return Regex.Matches(value, regex, options).Cast<Match>();
        }


        public static bool Matches (this string value, string regex, RegexOptions options = RegexOptions.None) {
            return Regex.IsMatch(value, regex, options);
        }

        public static IEnumerable<Match> MatchAll (this string value, Regex regex) {
            return regex.Matches(value).Cast<Match>();
        }

        public static bool HasGroup (this Match match, string group) {
            return String.IsNullOrEmpty(match.Groups[group].Value) == false;
        }

        public static string GroupValue (this Match match, string group) {
            return match.Groups[group].Value.Trim();
        }

        public static string UpperCaseFirst (this string v) {
            return v[0].ToString().ToUpperInvariant() + v.Substring(1);
        }

        public static bool IsAny (this string value, params string[] args) {
            foreach (string a in args) {
                if (a == value)
                    return true;
            }

            return false;
        }

        public static string RemoveEnd (this string value, int length) {
            return value.Substring(0, value.Length - length);
        }

        public static bool ContainsAny (this string value, params string[] args) {
            foreach (string a in args) {
                if (value.Contains(a))
                    return true;
            }

            return false;
        }
    }
}
