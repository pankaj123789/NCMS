using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using System;

namespace F1Solutions.Naati.Common.Contracts
{
    public class CredentialApplicationAttachmentDalModel
    {
        public int CredentialApplicationAttachmentId { get; set; }
        public int CredentialApplicationId { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string StoragePath { get; set; }
        public string Title { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public StoredFileType Type { get; set; }
        public bool? PrerequisiteApplicationDocument { get; set; }
    }
}
