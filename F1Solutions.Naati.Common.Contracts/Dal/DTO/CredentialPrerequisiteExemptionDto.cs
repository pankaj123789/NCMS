using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialPrerequisiteExemptionDto
    {
        public int CredentialPrerequisiteExemptionId { get; set; }
        public string CredentialTypeName { get; set; }
        public int CredentialTypeId { get; set; }
        public string SkillName { get; set; }
        public int SkillId { get; set; }
        public int PersonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedUser { get; set; }
        public int ModifiedUserId { get; set; }
    }
}
