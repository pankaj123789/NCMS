using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialAttachmentDto
    {
        public int CredentialId { get; set; }
        public int CredentialAttachmentId { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string ApplicationTypeDisplayName { get; set; }
    }
}