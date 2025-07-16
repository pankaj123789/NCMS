using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Contracts.Models
{
    public class OnHoldCredentialsToBeIssuedModel
    {
        public int CredentialRequestId { get; set; }
        public string CredentialTypeName { get; set; }
        public string SkillDisplayName { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public string CredentialRequestStatusDisplayName { get; set; }
        public int CredentialApplicationId { get; set; }
        public int CredentialApplicationStatusTypeId { get; set; }
        public string CredentialApplicationStatusDisplayName { get; set; }
        public string CredentialApplicationTypeDisplayName { get; set; }
        public string CredentialTypeAndSkillDisplayText => $"{CredentialTypeName} ({SkillDisplayName})";
    }
}
