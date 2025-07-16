namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class ProfessionalDevelopmentCategoryRequest
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProfessionalDevelopmentSectionId { get; set; }
    }
}