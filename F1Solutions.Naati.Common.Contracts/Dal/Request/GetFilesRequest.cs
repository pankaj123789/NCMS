using System.ServiceModel;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetFilesRequest
    {
        public int[] StoredFileIds { get; set; }
        [MessageHeader]
        public string TempFileStorePath { get; set; }
    }
}