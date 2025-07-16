using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class TestMaterialRequestSearchRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public IEnumerable<TestMaterialRequestSearchCriteria> Filters { get; set; }
    }
}