using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetRubricConfigurationResponse
    {
        public int TestSpecificationId { get; set; }
        public IEnumerable<TestMarkingComponentDto> TestComponents { get; set; }
    }
}