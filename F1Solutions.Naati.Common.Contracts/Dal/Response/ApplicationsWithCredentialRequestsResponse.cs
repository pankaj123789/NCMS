using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class ApplicationsWithCredentialRequestsResponse
    {
        public IEnumerable<ApplicationSearchDto> ApplicationResults { get; set; }
        public IEnumerable<Tuple<ApplicationSearchDto, CredentialRequestDto>> CredentialRequestResults { get; set; }
    }
}