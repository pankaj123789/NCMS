using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class PersonCredentialRequestsResponse
    {
        public IEnumerable<PersonCredentialRequestDto> Results { get; set; }
    }
}