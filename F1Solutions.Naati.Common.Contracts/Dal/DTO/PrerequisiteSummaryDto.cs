using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PrerequisiteSummaryDto
    {
        public string PrerequisiteCredentialName { get; set; }
        public int? PersonId { get; set; }
        public int? ExistingApplicationId { get; set; }
        public int? ExistingCredentialRequestId { get; set; }
        public int PreRequisiteCredentialId { get; set; }
        public int? ExistingRequestCredentialTypeId { get; set; }
        public string MatchingCredentialName { get; set; }
        public string PrerequisiteCredentialLanguage1 { get; set; }
        public string PrerequisiteCredentialLanguage2 { get; set; }
        public string PrerequistieDirection { get; set; }
        public string MatchingCredentialLanguage1 { get; set; }
        public string MatchingCredentialLanguage2 { get; set; }
        public string MatchingCredentialDirection { get; set; }
        public bool Match { get; set; }
        public bool CredentialPrerequistieSkillMatch { get; set; }
        public DateTime? MatchingCredentialStartDate { get; set; }
        public DateTime? MatchingCredentialEndDate { get; set; }
        public int? MatchingCredentialCertificationPeriodId { get; set; }
    }
}
