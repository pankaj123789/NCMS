using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetCredentialRequestSummarySearchRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public IEnumerable<CredentialRequestSummarySearchCriteria> Filters { get; set; }
    }
}