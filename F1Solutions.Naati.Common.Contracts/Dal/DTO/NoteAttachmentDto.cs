using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class NoteAttachmentDto
    {
        public int NoteId { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public DateTime? SoftDeleteDate { get; set; }
    }
}