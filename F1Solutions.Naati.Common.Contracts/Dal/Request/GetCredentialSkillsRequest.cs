using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetCredentialSkillsRequest
    {
        public IEnumerable<int> CredentialIds { get; set; }
    }
}