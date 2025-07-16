using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class DocumentTypesResponse
    {
        public List<DocumentTypeDto> Types { get; set; }
    }
}