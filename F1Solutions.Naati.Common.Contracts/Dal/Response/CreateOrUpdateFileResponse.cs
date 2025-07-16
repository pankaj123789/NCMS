using System.ServiceModel;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class CreateOrUpdateFileResponse
    {
        [MessageHeader]
        public int StoredFileId { get; set; }
    }
}