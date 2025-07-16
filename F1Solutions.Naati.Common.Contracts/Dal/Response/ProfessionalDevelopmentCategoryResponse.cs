namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class ProfessionalDevelopmentCategoryResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public ProfessionalDevelopmentCategoryGroupResponse ProfessionalDevelopmentCategoryGroup { get; set; }
    }
}