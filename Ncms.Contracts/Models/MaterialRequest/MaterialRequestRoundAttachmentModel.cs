using System;

namespace Ncms.Contracts.Models.MaterialRequest
{
    public class MaterialRequestRoundAttachmentModel
    {
        public int MaterialRequestRoundAttachmentId { get; set; }
        public int MaterialRequestRoundId { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string StoragePath { get; set; }
        public string Title { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public int? UploadedByUserId { get; set; }
        public int? UploadedByPersonId { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string Type { get; set; }
        public bool EportalDownload { get; set; }
        public bool NcmsAvailable { get; set; }
        public DateTime? SoftDeleteDate { get; set; }
    }
}
