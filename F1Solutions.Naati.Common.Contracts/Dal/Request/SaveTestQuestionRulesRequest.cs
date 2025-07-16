using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveTestQuestionRulesRequest
    {
        public int UserId { get; set; }
        public IEnumerable<RubricTestQuestionRuleDto> Configurations { get; set; }
        public int TestSpecificationId { get; set; }
    }
}