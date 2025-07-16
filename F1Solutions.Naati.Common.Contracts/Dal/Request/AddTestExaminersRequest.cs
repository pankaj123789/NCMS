using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class AddTestExaminersRequest: UpdateJobExaminersRequest
    {
        public IEnumerable<TestDataRequest> TestDataRequests { get; set; }
    }
}