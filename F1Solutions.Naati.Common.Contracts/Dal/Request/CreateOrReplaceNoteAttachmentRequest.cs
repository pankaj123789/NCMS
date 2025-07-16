using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateOrReplaceNoteAttachmentRequest
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int NoteId { get; set; }
        public string StoragePath { get; set; }
        public int StoredFileId { get; set; }
        public string Title { get; set; }
        public StoredFileType Type { get; set; }
        public int UploadedByUserId { get; set; }
    }
}