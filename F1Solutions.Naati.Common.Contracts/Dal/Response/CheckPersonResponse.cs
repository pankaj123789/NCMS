using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class CheckPersonResponse
    {
        public IEnumerable<CheckPersonDto> Results { get; set; }
        public string ErrorMessage { get; set; }
    }
}