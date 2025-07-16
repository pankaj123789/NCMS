using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CredentialRequestApplicantsRequest
    {
        public int CredentialApplicationTypeId { get; set; }
        public int CredentialTypeId { get; set; }
        public IEnumerable<int> SkillIds { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public int TestLocationId { get; set; }
    }
}