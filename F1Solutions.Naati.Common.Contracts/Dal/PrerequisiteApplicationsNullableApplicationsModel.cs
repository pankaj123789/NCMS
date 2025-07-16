using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class PrerequisiteApplicationsNullableApplicationsModel
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

        public string CredentialPrerequisiteDisplayName => GetPrerequisiteCredentialDisplayName();
        public string MatchingCredentialDisplayName => GetMatchingCredentialDisplayName();

        private string GetPrerequisiteCredentialDisplayName()
        {
            StringBuilder finalDisplayName = new StringBuilder();

            finalDisplayName.Append(PrerequisiteCredentialName);
            finalDisplayName.Append(" ");

            if (PrerequisiteSkillMatch)
            {
                finalDisplayName.Append(PrerequistieDirection);
                finalDisplayName.Replace("[Language 1]", "(" + PrerequisiteCredentialLanguage1 + " ");
                finalDisplayName.Replace("[Language 2]", " " + PrerequisiteCredentialLanguage2 + ")");
                return finalDisplayName.ToString();
            }

            finalDisplayName.Append("[Language 1]");
            finalDisplayName.Replace("[Language 1]", "(" + PrerequisiteCredentialLanguage1 + ")");

            return finalDisplayName.ToString();
        }

        private string GetMatchingCredentialDisplayName()
        {
            if (ExistingApplicationId == null)
            {
                return "-";
            }

            StringBuilder finalDisplayName = new StringBuilder();

            finalDisplayName.Append(MatchingCredentialName);
            finalDisplayName.Append(" ");
            finalDisplayName.Append(MatchingCredentialDirection);

            finalDisplayName.Replace("[Language 1]", "(" + MatchingCredentialLanguage1 + ")");

            if (PrerequisiteSkillMatch)
            {
                finalDisplayName.Replace("[Language 2]", "(" + MatchingCredentialLanguage2 + ")");
            }

            return finalDisplayName.ToString();
        }
    }
}

