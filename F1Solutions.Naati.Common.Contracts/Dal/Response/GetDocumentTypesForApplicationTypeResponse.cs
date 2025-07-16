using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetDocumentTypesForApplicationTypeResponse
    {
        public IEnumerable<string> Results { get; set; }
    }
}