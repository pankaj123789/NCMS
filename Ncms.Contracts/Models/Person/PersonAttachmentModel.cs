using Ncms.Contracts.Models.Common;
using System;

namespace Ncms.Contracts.Models.Person
{
    public class PersonAttachmentModel:AttachmentModel
    {
        public int PersonAttachmentId { get; set; }
        public int PersonId { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
    }
}
