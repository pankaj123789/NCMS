namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class TestMaterialRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? LanguageId { get; set; }
        public int CredentialTypeId { get; set; }
        public int TestComponentTypeId { get; set; }
        public int TestMaterialDomainId { get; set; }
        public int? SkillId { get; set; }
        public bool HasFile { get; set; }
        public bool Available { get; set; }
        public string Notes { get; set; }
        public bool IsTestMaterialTypeSource { get; set; }
    }
}