using System.Collections.Generic;
using System.Runtime.Serialization;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    [KnownType(typeof(LookupTypeDetailedDto))]
    [KnownType(typeof(FormLookupTypeDto))]
    public class LookupTypeResponse: BaseResponse
    {
        public IEnumerable<LookupTypeDto> Results { get; set; }
    }
}