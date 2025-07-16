using System.ServiceModel;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetTestAssetsFileResponse
    {
        [MessageHeader]
        public string FileName { get; set; }
        public string[] FilePaths { get; set; }
    }
}