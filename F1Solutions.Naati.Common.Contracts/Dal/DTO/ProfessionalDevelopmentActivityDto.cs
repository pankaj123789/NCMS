using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ProfessionalDevelopmentActivityDto
    {
        public DateTime DateCompleted { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public int ProfessionalDevelopmentCategoryId { get; set; }
        public int? ProfessionalDevelopmentCategoryGroupId { get; set; }
        public int ProfessionalDevelopmentRequirementId { get; set; }
        public int Id { get; set; }
        public string ProfessionalDevelopmentCategoryName { get; set; }
        public string ProfessionalDevelopmentCategoryGroupName { get; set; }
        public int Points { get; set; }
        public ProfessionalDevelopmentRequirementResponse ProfessionalDevelopmentRequirement { get; set; }
        public ProfessionalDevelopmentCategoryResponse ProfessionalDevelopmentCategory { get; set; }
        public IEnumerable<int> SectionIds { get; set; }
    }
}