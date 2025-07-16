using System;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class ProfessionalDevelopmentActivityRequest
    {
        public int? Id { get; set; }
        public int NaatiNumber { get; set; }
        public DateTime DateCompleted { get; set; }
        [LegalCharacters('\n', '\r', '\t')]
        public string Description { get; set; }
        [LegalCharacters('\n', '\r', '\t')]
        public string Notes { get; set; }
        public int ProfessionalDevelopmentCategoryId { get; set; }
        public int ProfessionalDevelopmentRequirementId { get; set; }
        public int? CredentialApplicationId { get; set; }
    }
}