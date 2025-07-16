using System.Collections.Generic;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class ApplicationSearchCriteria : ISearchCriteria<ApplicationFilterType>
    {
        public ApplicationFilterType Filter { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}