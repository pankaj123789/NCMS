namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class WorkPracticeAttachmentRequest
    {
        public int? Id { get; set; }
        public string Description { get; set; }
        public int WorkPracticeId { get; set; }
        public string StoragePath { get; set; }
        public int? UploadedByUserId { get; set; }
        public string FilePath { get; set; }
        public string UserName { get; set; }
        public object FileName { get; set; }
        public string TokenToRemoveFromFilename { get; set; }
        public int StoredFileId { get; set; }
    }
}