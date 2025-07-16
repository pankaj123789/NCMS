using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class ProfessionalDevelopmentCategoryRequirementListResponse
    {
        public IEnumerable<ProfessionalDevelopmentCategoryResponse> ProfessionalDevelopmentCategory { get; set; }
        public IEnumerable<ProfessionalDevelopmentRequirementResponse> ProfessionalDevelopmentRequirement { get; set; }
    }
}