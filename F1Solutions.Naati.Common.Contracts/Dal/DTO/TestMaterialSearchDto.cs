using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestMaterialSearchDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string LanguageOrSkill { get; set; }
        public int? LanguageId { get; set; }
        public string CredentialType { get; set; }
        public int CredentialTypeId { get; set; }
        public string TaskType { get; set; }
        public int TestComponentTypeId { get; set; }
        public int TestMaterialDomainId { get; set; }
        public bool HasFile { get; set; }
        public bool Available { get; set; }
        public int? SkillId { get; set; }
        public int TestSpecificationId { get; set; }
        public bool TestSpecificationActive { get; set; }
        public int StatusId { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public int? SourceMaterialId { get; set; }
        public string SourceMaterialTitle { get; set; }
    }
}