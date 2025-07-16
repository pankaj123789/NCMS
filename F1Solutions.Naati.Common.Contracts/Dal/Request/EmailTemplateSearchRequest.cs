using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class EmailTemplateSearchRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public IEnumerable<EmailTemplateSearchCriteria> Filter { get; set; }
    }
}