namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSpecificationDto
    {
        public bool Active { get; set; }
        public int CredentialTypeId { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public int? OverallPassMark { get; set; }
        public string CredentialType { get; set; }
        public bool ResultAutoCalculation { get; set; }
        public bool IsRubric { get; set; }
    }
}