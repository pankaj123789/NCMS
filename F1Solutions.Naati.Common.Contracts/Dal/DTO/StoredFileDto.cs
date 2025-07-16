using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class StoredFileDto
    {
        public  virtual  int Id { get; set; }
        public virtual string ExternalFileId { get; set; }
        public virtual DocumentTypeDto DocumentType { get; set; }
        public virtual string FileName { get; set; }
        public virtual long FileSize { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public int StoredFileStatusType { get; set; }
        public DateTime StoredFileStatusChangedDate { get; set; }
    }
}
