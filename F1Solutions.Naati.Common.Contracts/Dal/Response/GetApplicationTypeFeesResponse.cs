using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetApplicationTypeFeesResponse
    {
        public IList<CredentialFeeProductDto> FeeProducts { get; set; }
    }
}