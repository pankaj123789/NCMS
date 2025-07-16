using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class TestSpecificationDetailsRequest
    {
        public IEnumerable<int> TestSessionIds { get; set; }
    }
}