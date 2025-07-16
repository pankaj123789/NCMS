using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestAttendanceAssetSearchDto
    {
        public int TestAttendanceDocumentId { get; set; }
        public int TestAttendanceId { get; set; }
        public int TestMaterialId { get; set; }
        public int MaterialId { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string DocumentType { get; set; }
        public string Type { get; set; }
        public int NaatiNumber { get; set; }
        public string Title { get; set; }
        public int? UploadedByUserId { get; set; }
        public string UploadedByUserName { get; set; }
        public int? UploadedByPersonId { get; set; }
        public int? UploadedByPersonNaatiNo { get; set; }
        public string UploadedByPersonName { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public int FileSize { get; set; }
        public int? JobId { get; set; }
        public DateTime TestSatDate { get; set; }
        public int TestOfficeId { get; set; }
        public bool Deleted { get; set; }
        public bool ExaminerMarksRemoved { get; set; }
        public bool EportalDownload { get; set; }
        public DateTime? SubmittedDate { get; set; }
    }
}