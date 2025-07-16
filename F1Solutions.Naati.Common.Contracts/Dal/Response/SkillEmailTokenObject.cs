using System.Collections.Generic;
using System.Runtime.Serialization;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class SkillEmailTokenObject
    {
        public string DisplayName { get; set; }
        public int CredentialId { get; set; }
        public string Externalname { get; set; }
    }
}