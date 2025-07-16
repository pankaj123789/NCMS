using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.Common;
using System;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialApplicationAttachmentModel:AttachmentModel
    {
        public int CredentialApplicationAttachmentId { get; set; }
        public int CredentialApplicationId { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public bool? PrerequisiteApplicationDocument { get; set; }
        public new StoredFileType Type { get; set; }
    }
}
