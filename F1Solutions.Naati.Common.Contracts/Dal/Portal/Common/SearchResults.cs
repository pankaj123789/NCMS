using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal.Common
{
    public class SearchResults<T>
    {
        // TODO: Move me somewhere
        public IEnumerable<T> Results { get; set; }
        public int TotalResultsCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPageCount
        {
            get { return (int)Math.Ceiling((double)TotalResultsCount / (double)PageSize); }
        }

        public string ResultsLastUpdated { get; set; }
    }

    public class CountResults<T>
    {
        // TODO: Move me somewhere
        public IEnumerable<T[]> Results { get; set; }
        public int TotalResultsCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPageCount
        {
            get { return (int)Math.Ceiling((double)TotalResultsCount / (double)PageSize); }
        }

        public string ResultsLastUpdated { get; set; }
    }

    public enum SortDirection
    {
        Ascending,
        Descending,
        None
    }
}