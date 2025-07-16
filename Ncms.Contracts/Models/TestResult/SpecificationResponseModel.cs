using System.Collections.Generic;

namespace Ncms.Contracts.Models.TestResult
{
    public class SpecificationResponseModel
    {
        public int OverallPassMark { get; set; }
        public List<TestComponentModel> Components { get; set; }
    }
}
