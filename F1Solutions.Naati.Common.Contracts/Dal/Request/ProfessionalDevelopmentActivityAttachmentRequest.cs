namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class ProfessionalDevelopmentActivityAttachmentRequest
    {
        public int? Id { get; set; }
        public int StoredFileId { get; set; }
        public string Description { get; set; }
        public int ProfessionalDevelopmentActivityId { get; set; }
        public string StoragePath { get; set; }
        public int? UploadedByUserId { get; set; }
        public string FilePath { get; set; }
        public object FileName { get; set; }
        public string UserName { get; set; }
        public string TokenToRemoveFromFilename { get; set; }
    }
}