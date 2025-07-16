using System.ServiceModel;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetCredentialAttachmentFileResponse
    {
        [MessageHeader]
        public string FileName { get; set; }
        public string[] FilePaths { get; set; }
        public int? PersonHash { get; set; }
        public string ExternalField { get; set; }
    }
}