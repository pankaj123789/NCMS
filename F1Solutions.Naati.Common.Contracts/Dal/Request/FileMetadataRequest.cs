using System;
using System.ServiceModel;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class FileMetadataRequest
    {
        [MessageHeader]
        public int? UpdateStoredFileId { get; set; }
        [MessageHeader]
        public string UpdateFileName { get; set; }
        [MessageHeader]
        public StoredFileType Type { get; set; }
        // this should be the file name with optional path (relative path only, no drive spec,
        // as the base path is an implementation detail of the service)
        [MessageHeader]
        public string StoragePath { get; set; }        
        [MessageHeader]
        public int? UploadedByPersonId { get; set; }
        [MessageHeader]
        public int? UploadedByUserId { get; set; }
        [MessageHeader]
        public DateTime UploadedDateTime { get; set; }
    }
}