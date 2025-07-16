using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class AttendeeTestSpecificationAttachmentDto
    {
        public StoredFileDto StoredFile { get; set; }
        public string Title { get; set; }
        public bool Deleted { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public string UploadedBy { get; set; }
        public int TestSpecificationId { get; set; }
        public int Id { get; set; }
        public bool EportalDownload { get; set; }
        public bool MergeDocument { get; set; }
    }
}