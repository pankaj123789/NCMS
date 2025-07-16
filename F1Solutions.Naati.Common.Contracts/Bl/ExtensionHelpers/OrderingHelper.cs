using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Bl.ExtensionHelpers
{
    public static class OrderingHelper
    {
        public static IEnumerable<TSource> OrderUsing<TSource, TKeySelector>(this IEnumerable<TSource> source, Func<TSource, TKeySelector> keySelector,  SortDirection sorDirection)
        {
            var orderedSource = source as IOrderedEnumerable<TSource>;
            if (sorDirection == SortDirection.Ascending)
            {
                
                return orderedSource != null ? GetThenByAscendingOrder<TSource, TKeySelector>(orderedSource)(keySelector) : GetAscendingOrder<TSource, TKeySelector>(source)(keySelector);
            }

            return orderedSource != null ? GetThenByDescendingOrder<TSource, TKeySelector>(orderedSource)(keySelector) : GetDescendingOrder<TSource, TKeySelector>(source)(keySelector);
        }

        private static Func<Func<TSource, TKeySelector>, IOrderedEnumerable<TSource>> GetDescendingOrder<TSource, TKeySelector>(IEnumerable<TSource> source)
        {
            Func<Func<TSource, TKeySelector>, IOrderedEnumerable<TSource>> function =
                source.OrderByDescending;
            return function;
        }

        private static Func<Func<TSource, TKeySelector>, IOrderedEnumerable<TSource>> GetThenByDescendingOrder<TSource, TKeySelector>(IOrderedEnumerable<TSource> source)
        {
            Func<Func<TSource, TKeySelector>, IOrderedEnumerable<TSource>> function =
                source.ThenByDescending;
            return function;
        }

        private static Func<Func<TSource, TKeySelector>, IOrderedEnumerable<TSource>> GetAscendingOrder<TSource, TKeySelector>(IEnumerable<TSource> source)
        {
            Func<Func<TSource, TKeySelector>, IOrderedEnumerable<TSource>> function =
                source.OrderBy;
            return function;
        }

        private static Func<Func<TSource, TKeySelector>, IOrderedEnumerable<TSource>> GetThenByAscendingOrder<TSource, TKeySelector>(IOrderedEnumerable<TSource> source)
        {
            Func<Func<TSource, TKeySelector>, IOrderedEnumerable<TSource>> function =
                source.ThenBy;
            return function;
        }
    }
}
