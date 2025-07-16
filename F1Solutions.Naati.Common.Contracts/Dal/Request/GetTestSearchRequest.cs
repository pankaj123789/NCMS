using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTestSearchRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public IEnumerable<TestSearchCriteria> Filters { get; set; }
    }
}