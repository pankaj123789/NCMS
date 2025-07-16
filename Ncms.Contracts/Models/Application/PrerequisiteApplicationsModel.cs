using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Contracts.Models.Application
{
    public class PrerequisiteApplicationsModel
    {
        public string CurrentCredentialRequest { get; set; }
        public string CurrentCredentialRequestLanguage1 { get; set; }
        public string CurrentCredentialRequestLanguage2 { get; set; }
        public string RequiredPrerequisite { get; set; }
        public bool RequiredPrerequisiteSkillMatch { get; set; }
        public string RequiredPrerequisiteLanguage1 { get; set; }
        public string RequiredPrerequisiteLanguage2 { get; set; }
        public int ExistingApplicationId { get; set; }
        public int ExistingApplicationStatusTypeId { get; set; }
        public bool ExistingApplicationAutoCreated { get; set; }
        public string ExistingCredentialRequest { get; set; }
        public string ExistingCredentialRequestLanguage1 { get; set; }
        public string ExistingCredentialRequestLanguage2 { get; set; }
        public int ExistingCredentialRequestStatusTypeId { get; set; }
        public bool ExistingCredentialRequestAutoCreated { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
