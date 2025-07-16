using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetDocumentsResponse
    {
        public IEnumerable<TestSittingDocumentDto> Documents { get; set; }
    }
}