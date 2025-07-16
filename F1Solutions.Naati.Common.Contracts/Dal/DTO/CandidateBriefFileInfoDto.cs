namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CandidateBriefFileInfoDto
    {
        public int? CandidateBriefId { get; set; }
        public int StorageFileId { get; set; }
        public string TaskLabel { get; set; }
        public string TaskTypeLabel { get; set; }
        public int TestMaterialId { get; set; }
        public int TestMaterialAttachmentId { get; set; }
        public int TestComponentId { get; set; }
    }
}