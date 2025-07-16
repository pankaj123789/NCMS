using System.ServiceModel;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class FileStorageServiceRequest
    {
        [MessageHeader]
        public int StoredFileId { get; set; }
    }
}