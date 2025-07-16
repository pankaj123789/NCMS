using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTestSessionSearchRequest : ISearchPagingRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public IEnumerable<TestSessionSearchCriteria> Filters { get; set; }
        public bool IncludeInactiveVenueFlag { get; set; }
    }
}