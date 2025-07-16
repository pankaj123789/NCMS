using System.ServiceModel;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetTestSittingDocumentResponse
    {
        [MessageHeader]
        public TestSittingDocumentDto Document { get; set; }
    }
}