using System;
using System.Text.RegularExpressions;
using DataDynamics.ActiveReports;

namespace MyNaati.Ui.Common
{
    public static class DisplayExtensionMethods
    {
        public static string ToStringWithFormat<T>(this Nullable<T> val, string format, string valueIfNull)
            where T : struct, IFormattable
        {
            if (val == null)
                return valueIfNull;

            return val.Value.ToString(format, null);
        }

        public static string ToNullableString(this Field field, string valueIfNull = "")
        {
            if (field == null || field.Value == null)
                return valueIfNull;

            return field.Value.ToString();
        }

        public static string ToStringWithLookup<T>(this Nullable<T> val, Func<T, string> doLookup, string valueIfNull)
            where T : struct
        {
            if (val == null)
                return valueIfNull;

            return doLookup(val.Value);
        }

        public static string ToNetDateTimeFormat(this string jsDateTimeFormat)
        {
            //Worst. Method. Ever.
            switch (jsDateTimeFormat)
            {
                case "dd M yy":
                    return "dd MMM yyyy";
                default:
                    return "dd MMM yyyy";
            }
        }

        public static string TrimDuplicateSpaces(this string input)
        {
            Regex findDuplicateSpaces = new Regex(@"[\s]+", RegexOptions.Multiline);
            return findDuplicateSpaces.Replace(input, " ").Trim();
        }
    }
}