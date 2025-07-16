namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class StoredFileMarterialDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int DocumentTypeId { get; set; }
        public string Title { get; set; }
        public int TestMaterialId { get; set; }
        public int TestSpecificationId { get; set; }
        public bool EportalDownload { get; set; }
        public bool MergeDocument { get; set; }
    }
}