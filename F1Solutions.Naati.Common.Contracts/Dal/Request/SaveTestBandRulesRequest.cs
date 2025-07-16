using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveTestBandRulesRequest
    {
        public int TestSpecificationId { get; set; }
        public int UserId { get; set; }
        public IEnumerable<RubricTestBandRuleDto> Configurations { get; set; }
    }
}