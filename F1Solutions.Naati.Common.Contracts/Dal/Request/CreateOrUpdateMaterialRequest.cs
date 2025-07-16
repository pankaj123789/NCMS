namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateOrUpdateMaterialRequest
    {
        public int MaterialId { get; set; }
        public int TestMaterialId { get; set; }
        public int StoredFileId { get; set; }
        public string Title { get; set; }
        public bool Deleted { get; set; }
    }
}