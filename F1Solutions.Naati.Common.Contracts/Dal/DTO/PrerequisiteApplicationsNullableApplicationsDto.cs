using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PrerequisiteApplicationsNullableApplicationsDto
    {
        public string PrerequisiteCredentialName { get; set; }
        public string PrerequisiteCredentialLanguage1 { get; set; }
        public string PrerequisiteCredentialLanguage2 { get; set; }
        public string PrerequistieDirection { get; set; }
        public bool PrerequisiteSkillMatch { get; set; }
        public int? ExistingApplicationId { get; set; }
        public int? ExistingApplicationStatusTypeId { get; set; }
        public string ExistingApplicationStatusName { get; set; }
        public string ExistingApplicationTypeName { get; set; }
        public string MatchingCredentialName { get; set; }
        public string MatchingCredentialLanguage1 { get; set; }
        public string MatchingCredentialLanguage2 { get; set; }
        public int? MatchingCredentialStatusTypeId { get; set; }
        public string MatchingCredentialStatusName { get; set; }
        public string MatchingCredentialDirection { get; set; }
        public int ApplicationTypePrerequisiteId { get; set; }
        public int PrerequisiteSkillId { get; set; }
    }
}
