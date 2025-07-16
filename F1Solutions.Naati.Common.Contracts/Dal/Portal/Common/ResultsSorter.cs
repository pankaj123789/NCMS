using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal.Common
{
    public class ResultsSorter<T>
    {
        // TODO: Move me somewhere
        private IDictionary<string, Expression<Func<T, object>>[]> mSortTerms;

        public ResultsSorter()
        {
            mSortTerms = new Dictionary<string, Expression<Func<T, object>>[]>();
        }

        public void DefineSortTerm(string term, params Expression<Func<T, object>>[] keySelector)
        {
            mSortTerms[term] = keySelector;
        }

        public bool IsDefined(string term)
        {
            if (term == null)
                return false;

            return mSortTerms.ContainsKey(term);
        }

        public IQueryable<T> Sort(IQueryable<T> source, string term, SortDirection direction)
        {
            var sortExpressions = mSortTerms[term];

            IOrderedQueryable<T> query;

            if (direction == SortDirection.Ascending)
                query = source.OrderBy(sortExpressions[0]);
            else
                query = source.OrderByDescending(sortExpressions[0]);

            foreach (var sortExpression in sortExpressions.Skip(1))
            {
                if (direction == SortDirection.Ascending)
                    query = query.ThenBy(sortExpression);
                else
                    query = query.ThenByDescending((sortExpression));
            }

            return query;
        }
    }
}
