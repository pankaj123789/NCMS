using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class VenueSearchRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public IEnumerable<VenueSearchCriteria> Filters { get; set; }
    }
}