using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class FileDto
    {
        public int ParentId { get; set; }
        public int StoredFileId { get; set; }
        public StoredFileType Type { get; set; }
        public bool EportalDownload { get; set; }
        public long FileSize { get; set; }
        public string FileName { get; set; }
        public int? UploadedByPersonId { get; set; }
        public string UploadedByPersonName { get; set; }
        public int? UploadedByUserId { get; set; }
        public string UploadedByUserName { get; set; }
        public DateTime UploadedDateTime { get; set; }
    }
}