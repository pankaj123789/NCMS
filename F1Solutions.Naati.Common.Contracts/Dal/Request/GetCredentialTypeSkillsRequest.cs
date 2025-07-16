using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetCredentialTypeSkillsRequest
    {
        public int ApplicationId { get; set; }
        public IEnumerable<int> CredentialTypeIds { get; set; }
        public int NAATINumber { get; set; }
        public int CredentialRequestPathTypeId { get; set; }
    }
}