using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    // DTO used to return data from the PrerequisiteApplications SP
    public class PrerequisiteApplicationsDto
    {
        public string CurrentCredentialRequestCredentialTypeName { get; set; }
        public string CurrentCredentialRequestLanguage1 { get; set; }
        public string CurrentCredentialRequestLanguage2 { get; set; }
        public string CurrentCredentialRequestSkillDirection { get; set; }
        public string RequiredPrerequisiteCredentialType { get; set; }
        public string RequiredPrerequisiteLanguage1 { get; set; }
        public string RequiredPrerequisiteLanguage2 { get; set; }
        public bool RequiredPrerequisiteSkillMatch { get; set; }
        public int ExistingApplicationId { get; set; }
        public int ExistingApplicationStatusTypeId { get; set; }
        public string ExistingApplicationStatusName { get; set; }
        public string ExistingApplicationTypeName { get; set; }
        public bool? ExistingApplicationAutoCreated { get; set; }
        public string ExistingCredentialRequestCredentialTypeName { get; set; }
        public int ExistingCredentialRequestSkillId { get; set; }
        public string ExistingCredentialRequestSkillDirection { get; set; }
        public string ExistingCredentialRequestLanguage1 { get; set; }
        public string ExistingCredentialRequestLanguage2 { get; set; }
        public int ExistingCredentialRequestStatusTypeId { get; set; }
        public string ExsitingCredentialRequestStatusName { get; set; }
        public bool? ExistingCredentialRequestAutoCreated { get; set; }
        public DateTime? ExistingApplicationCreatedDate { get; set; }
    }
}
