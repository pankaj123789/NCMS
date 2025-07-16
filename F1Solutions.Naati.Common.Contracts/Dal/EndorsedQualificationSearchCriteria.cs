using System.Collections.Generic;
using F1Solutions.Global.Common;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class EndorsedQualificationSearchCriteria :  ISearchCriteria<EndorsedQualificationFilterType>
    {
        public EndorsedQualificationFilterType Filter { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}
