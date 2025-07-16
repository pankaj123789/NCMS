using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateOrUpdateDocumentRequest
    {
        public CreateOrReplaceTestSittingDocumentDto Document { get; set; }
    }
}