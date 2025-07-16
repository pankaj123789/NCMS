using System.Collections.Generic;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.Request;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class CredentialRequestSummarySearchCriteria : ISearchCriteria<CredentialRequestSummaryFilterType>
    {
        public CredentialRequestSummaryFilterType Filter { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}