using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateOrReplacePersonAttachmentRequest
    {
        public int PersonAttachmentId { get; set; }
        public int PersonId { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string StoragePath { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public StoredFileType Type { get; set; }
        public string TokenToRemoveFromFilename { get; set; }
    }
}