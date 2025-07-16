using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SupplementaryTestRequest
    {
        public IEnumerable<int> TestSessionIds { get; set; }
    }
}