using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetPractitionerCountRequest
    {
        public IEnumerable<PractitionerFilterSearchCriteria> Filters { get; set; }
    }

    public class GetAPiPublicPractitionerCountRequest
    {
        public IEnumerable<ApiPublicPractitionerFilterSearchCriteria> Filters { get; set; }
    }
}