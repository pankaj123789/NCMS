namespace Ncms.Contracts.Models.MaterialRequest
{
    public class TestMaterialLinkModel
    {
        public int Id { get; set; }
        public int FromTestMaterialId { get; set; }
        public int ToTestMaterialId { get; set; }

       public int TypeId { get; set; }
    }
}
