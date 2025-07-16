using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetSkillsForCredentialTypeRequest
    {
        public int? ApplicationId { get; set; }
        public IEnumerable<int> CredentialTypeIds { get; set; }
        public IEnumerable<int> CredentialApplicationTypeIds { get; set; }
    }
}