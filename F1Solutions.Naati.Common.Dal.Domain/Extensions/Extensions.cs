using System;

namespace F1Solutions.Naati.Common.Dal.Domain.Extensions
{
    public static class Extensions
    {
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparisonType)
        {
            newValue = newValue ?? string.Empty;
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(oldValue) || oldValue.Equals(newValue, comparisonType))
            {
                return str;
            }
            int foundAt;
            while ((foundAt = str.IndexOf(oldValue, 0, comparisonType)) != -1)
            {
                str = str.Remove(foundAt, oldValue.Length).Insert(foundAt, newValue);
            }
            return str;
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNotNullOrEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        public static bool IsNull(this object o)
        {
            return (o == null);
        }

        public static bool IsNotNull(this object o)
        {
            return (o != null);
        }

        public static bool IsTrue(this bool b)
        {
            return b;
        }

        public static bool IsFalse(this bool b)
        {
            return !b;
        }
    }
}
