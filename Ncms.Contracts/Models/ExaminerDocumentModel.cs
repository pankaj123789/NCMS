using System;

namespace Ncms.Contracts.Models
{
    public class ExaminerDocumentModel
    {
        public int StoredFileId { get; set; }
        public int DocumentTypeId { get; set; }
        public long FileSize { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string DocumentTypeDisplayName { get; set; }
        public string UploadedByPersonName { get; set; }
        public string UploadedByUserName { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public bool EportalDownload { get; set; }
        public string DocumentSource { get; set; }
        public DateTime? SoftDeleteDate { get; set; }
    }
}
