using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class EmailMessageAttachmentDto
    {
        public string GraphAttachmentId { get; set; }
        public int? GraphAttachmentSize { get; set; }
        public int EmailMessageAttachmentId { get; set; }
        public int EmailMessageId { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string StoragePath { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public byte[] GraphAttachmentBytes { get; set; }
        public bool IsInline { get; set; }
        public string ContentId { get; set; }
    }
}