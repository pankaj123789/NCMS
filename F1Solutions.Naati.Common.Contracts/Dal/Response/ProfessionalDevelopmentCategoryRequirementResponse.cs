namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class ProfessionalDevelopmentCategoryRequirementResponse
    {
        public int Points { get; set; }
        public ProfessionalDevelopmentCategoryResponse ProfessionalDevelopmentCategory { get; set; }
        public ProfessionalDevelopmentRequirementResponse ProfessionalDevelopmentRequirement { get; set; }
        public int Id { get; set; }
    }
}