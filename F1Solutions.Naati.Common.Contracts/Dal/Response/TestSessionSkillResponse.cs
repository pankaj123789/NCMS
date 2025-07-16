using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class TestSessionSkillResponse
    {
        public IEnumerable<TestSessionSkillDto> Results { get; set; }
    }
}