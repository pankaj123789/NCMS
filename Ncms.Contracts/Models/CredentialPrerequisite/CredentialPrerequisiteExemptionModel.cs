using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Contracts.Models.CredentialPrerequisite
{
    public class CredentialPrerequisiteExemptionModel
    {
        public int PrerequisiteExemptionId { get; set; }
        public string PrerequisiteCredentialName { get; set; }
        public string PrerequisiteSkill { get; set; }
        public string ExemptedCredentialName { get; set; }
        public int ExemptedCredentialTypeId { get; set; }
        public string ExemptedCredentialSkill { get; set; }
        public int ExemptedCredentialSkillId { get; set; }
        public int PersonId { get; set; }
        public DateTime? ExemptionStartDate { get; set; }
        public DateTime? ExemptionEndDate { get; set; }
        public string ExemptionGrantedByUser { get; set; }
        public int ExemptionGrantedByUserId { get; set; }
        public bool Checked => ExemptionStartDate.IsNotNull();        
    }
}
