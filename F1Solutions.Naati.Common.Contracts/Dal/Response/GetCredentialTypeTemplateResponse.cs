using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetCredentialTypeTemplateResponse
    {
        public IEnumerable<CredentialTypeTemplateDto> Results { get; set; }
    }
}