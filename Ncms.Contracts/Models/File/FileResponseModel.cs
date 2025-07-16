using System;
using Ncms.Contracts.Models.Common;

namespace Ncms.Contracts.Models.File
{
    public class FileResponseModel : FileModel
    {
        public int ParentId { get; set; }
        public int StoredFileId { get; set; }
        public string Type { get; set; }
        public bool EportalDownload { get; set; }
        public long FileSize { get; set; }
        public int? UploadedByPersonId { get; set; }
        public string UploadedByPersonName { get; set; }
        public int? UploadedByUserId { get; set; }
        public string UploadedByUserName { get; set; }
        public DateTime UploadedDateTime { get; set; }
    }

    public class FileInfoModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int StoredFileTypeId { get; set; }
    }
}
