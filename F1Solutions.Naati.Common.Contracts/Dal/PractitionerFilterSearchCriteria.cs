using System.Collections.Generic;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class PractitionerFilterSearchCriteria : ISearchCriteria<PractitionerFilterType>
    {
        public PractitionerFilterType Filter { get; set; }
        public IEnumerable<string> Values { get; set; }
    }

    public class ApiPublicPractitionerFilterSearchCriteria : ISearchCriteria<ApiPublicPractitionerFilterType>
    {
        public ApiPublicPractitionerFilterType Filter { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}