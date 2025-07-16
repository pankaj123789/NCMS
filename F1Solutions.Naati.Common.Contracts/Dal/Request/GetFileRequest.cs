using System.ServiceModel;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetFileRequest
    {
        [MessageHeader]
        public int StoredFileId { get; set; }
        [MessageHeader]
        public string TempFileStorePath { get; set; }
    }
}