using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetEndorsedQualificationSearchRequest : ISearchPagingRequest
    {
        public IEnumerable<EndorsedQualificationSearchCriteria> Filters { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }

    public interface ISearchPagingRequest
    {
        int? Skip { get; set; }
        int? Take { get; set; }
    }
}
