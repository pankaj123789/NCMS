namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class ProfessionalDevelopmentCategoryRequirementRequest
    {
        public int? Id { get; set; }
        public int Points { get; set; }
        public int ProfessionalDevelopmentCategoryId { get; set; }
        public int ProfessionalDevelopmentRequirementId { get; set; }
    }
}