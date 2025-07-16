using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetSkillsDetailsRequest
    {
        public IEnumerable<int> CredentialTypeIds { get; set; }
        public int? Language1Id { get; set; }
    }
}