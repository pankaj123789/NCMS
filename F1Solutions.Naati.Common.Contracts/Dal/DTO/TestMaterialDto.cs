using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestMaterialDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string CredentialType { get; set; }
        public string TestComponentTypeName { get; set; }
        public bool HasFile { get; set; }
        public bool Available { get; set; }
        public int? LanguageId { get; set; }
        public int CredentialTypeId { get; set; }
        public int TestComponentTypeId { get; set; }
        public int TestMaterialDomainId { get; set; }
        public int? SkillId { get; set; }
      
        public string SkillName { get; set; }
        public string Notes { get; set; }
        public int TestSittingTestMaterialId { get; set; }
        public int TestSittingId { get; set; }
        public int TestSpecificationId { get; set; }
        public bool TestSpecificationActive { get; set; }

        public int TestMaterialTypeId { get; set; }
        public double DefaultMaterialRequestHours { get; set; }
        public int? SourceTestMaterialId { get; set; }

        public IList<TestMaterialLinkDto> Links { get; set; }
    }
}