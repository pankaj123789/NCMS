using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSpecificationAttachmentDto
    {
        public StoredFileType Type { get; set; }
        public int TestSpecificationAttachmentId { get; set; }
        public int TestSpecificationId { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public string UploadedByPersonName { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public bool MergeDocument { get; set; }
        public bool ExaminerToolsDownload { get; set; }
        public DateTime? StoredFileStatusChangeDate { get; set; }
        public int StoredFileStatusTypeId { get; set; }
    }
}