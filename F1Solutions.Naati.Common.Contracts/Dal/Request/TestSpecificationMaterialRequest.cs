using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class TestSpecificationMaterialRequest
    {
        public int TestSpecificationId { get; set; }
        public int SkillId { get; set; }
        public IEnumerable<int> TestSessionIds { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }

        public bool? IncludeSkillTypes { get; set; }

    }
}