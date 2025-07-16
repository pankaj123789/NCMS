using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class StoredFile : EntityBase
    {
        // ExternalFileId is a string because the implementation of the file storage system is interchangeable. 
        // it could be numeric identifier, or a path, or a foofoo, etc.
        public virtual string ExternalFileId { get; set; }
        public virtual DocumentType DocumentType { get; set; }
        public virtual string FileName { get; set; }
        public virtual long FileSize { get; set; }
        public virtual Person UploadedByPerson { get; set; }
        public virtual User UploadedByUser { get; set; }
        public virtual DateTime UploadedDateTime { get; set; }
        public virtual StoredFileStatusType StoredFileStatusType { get; set; }
        public virtual DateTime StoredFileStatusChangeDate { get; set; }
    }
}
