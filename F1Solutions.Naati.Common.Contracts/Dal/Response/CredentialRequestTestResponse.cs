using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class CredentialRequestTestResponse
    {
        public IEnumerable<CredentialRequestTestRequestDto> Results { get; set; }
    }
}