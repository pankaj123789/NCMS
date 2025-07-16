using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNaati.Ui.Helpers
{
    public static class Extensions
    {
        public static T SingleOrNull<T>(this IEnumerable<T> source) where T : class
        {
            try
            {
                return source.Single();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public static T SingleOrNull<T>(this IEnumerable<T> source, Func<T, bool> predicate) where T : class
        {
            try
            {
                return source.Single(predicate);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}
