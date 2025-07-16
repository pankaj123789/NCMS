using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class TestSessionLanguageDetailsRequest
    {
        public IEnumerable<int> TestSessionIds { get; set; }
        public int TestSpecificationId { get; set; }
    }
}