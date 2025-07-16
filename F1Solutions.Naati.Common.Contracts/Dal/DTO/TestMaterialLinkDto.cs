namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestMaterialLinkDto
    {
        public int Id { get; set; }
        public int FromTestMaterialId { get; set; }
        public int ToTestMaterialId { get; set; }
        public int TypeId { get; set; }
    }
}
