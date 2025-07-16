using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class TestMaterialsSummaryRequest
    {
        public int[] TestSessionIds { get; set; }
        public int SkillId { get; set; }
        public int TestSpecificationId { get; set; }
        public IEnumerable<TestMaterialAssignmentDto> TestMaterialAssignments { get; set; }
    }
}