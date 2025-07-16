using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveQuestionPassRulesRequest
    {
        public int UserId { get; set; }
        public IEnumerable<RubricQuestionPassRuleDto> Configurations { get; set; }
        public int TestSpecificationId { get; set; }
    }
}