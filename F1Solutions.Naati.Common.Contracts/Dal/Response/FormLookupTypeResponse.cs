using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class FormLookupTypeResponse
    {
        public IEnumerable<FormLookupTypeDto> Results { get; set; }
    }
}