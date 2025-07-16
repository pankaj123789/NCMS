using System;
using System.Collections.Generic;
using Ncms.Contracts.Models.Common;

namespace Ncms.Contracts.Models
{
    public class DocumentModel : FileModel
    {
        public int FileSize { get; set; }
        public int StoredFileId { get; set; }
        public string Title { get; set; }
        public bool Deleted { get; set; }
    }
    public class AssetModel : DocumentModel
    {
        public int TestAttendanceId { get; set; }
        public int TestAttendanceAssetId { get; set; }
        public bool EportalDownload { get; set; }
    }
    public class MaterialModel : DocumentModel
    {
        public int TestMaterialId { get; set; }
        public int MaterialId { get; set; }
    }

    public class TestAttendanceAssetSearchResultModel
    {
        public int TestAttendanceId { get; set; }
        public int TestMaterialId { get; set; }
        public int MaterialId { get; set; }
        public int TestAttendanceAssetId { get; set; }
        public int StoredFileId { get; set; }
        public string Title { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public int FileSize { get; set; }
        public int NaatiNumber { get; set; }
        public string FileType { get; set; }
        public bool ExaminerMarksRemoved { get; set; }
        public bool UploadeByExaminer { get; set; }
        public bool EportalDownload { get; set; }
    }

    public class TestAttendanceAssetSearchRequestModel
    {
        public IEnumerable<int> TestAttendanceId { get; set; }
        public IEnumerable<int> TestMaterialId { get; set; }
        public IEnumerable<int> JobId { get; set; }
        public IEnumerable<int> NaatiNumber { get; set; }
        public IEnumerable<int> OfficeId { get; set; }
        public IEnumerable<string> TestAttendanceAssetType { get; set; }
        public IEnumerable<int> UploadedByUserId { get; set; }
        public IEnumerable<int> UploadedByPersonNaatiNo { get; set; }
        public DateTime? SatDateFrom { get; set; }
        public DateTime? SatDateTo { get; set; }
    }

    public class BulkFileDownloadModel : FileModel { }
}
