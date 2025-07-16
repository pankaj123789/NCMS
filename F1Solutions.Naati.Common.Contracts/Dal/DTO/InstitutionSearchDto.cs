namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class InstitutionSearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NaatiNumber { get; set; }
        public int? CountryId { get; set; }
        public bool HasCourseOnly { get; set; }
        public bool HasNoCourse { get; set; }
        public int OrganisationWithCourse { get; set; }
        public string CourseName { get; set; }
        public int? EducationLevelId { get; set; }
        public int? LanguageId { get; set; }
        public int? AccreditationCategoryId { get; set; }
        public int? AccreditationLevelId { get; set; }
        public int? ApprovalStatusId { get; set; }
    }
}