using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetEmailSearchRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public IEnumerable<EmailSearchCriteria> Filters { get; set; }
        public IEnumerable<EmailMessageSortingOption> SortingOptions { get; set; }
    }
}