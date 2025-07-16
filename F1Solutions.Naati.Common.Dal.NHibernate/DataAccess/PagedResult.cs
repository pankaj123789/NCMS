using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.NHibernate.DataAccess
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Results { get; set; }
        public int TotalResultsCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalPageCount
        {
            get { return (int)Math.Ceiling(TotalResultsCount / (double)PageSize); }
        }

        public int LastPage { get; set; }
    }
}
