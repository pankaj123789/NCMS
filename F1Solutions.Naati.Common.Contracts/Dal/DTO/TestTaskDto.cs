namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestTaskDto
    {
        public bool HasTestMaterialAttachment { get; set; }
        public int? TestMaterialId { get; set; }
        public int? TestMaterialDomainId { get; set; }
        public int? MaterialComponentTypeId { get; set; }
        public int TestComponentId { get; set; }
        public string TestComponentName { get; set; }
        public string Label { get; set; }
        public string TestComponentTypeLabel { get; set; }
        public int TaskComponentTypeId { get; set; }

    }
}