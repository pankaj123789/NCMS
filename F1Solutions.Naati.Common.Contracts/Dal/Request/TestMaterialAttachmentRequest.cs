using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class TestMaterialAttachmentRequest
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int TestMaterialId { get; set; }
        public string StoragePath { get; set; }
        public int? UploadedByUserId { get; set; }
        public string FilePath { get; set; }
        public bool Deleted { get; set; }
        public StoredFileType FileType { get; set; }
        public int? UpdateStoredFileId { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string Type { get; set; }
        public int MaterialId { get; set; }
        public bool AvailableForExaminers { get; set; }
        public bool MergeDocument { get; set; }
    }
}