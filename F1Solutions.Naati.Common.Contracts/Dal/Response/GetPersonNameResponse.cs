using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetPersonNameResponse
    {
        public IEnumerable<PersonNameDto> Names { get; set; }
    }
}