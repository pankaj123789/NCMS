using System;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetFileInfoResponse
    {
        public string ExternalFileId { get; set; }
        public int DocumentTypeId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public int? UploadedByPersonId { get; set; }
        public int? UploadedByUserId { get; set; }
        public DateTime UploadedDateTime { get; set; }
    }
}