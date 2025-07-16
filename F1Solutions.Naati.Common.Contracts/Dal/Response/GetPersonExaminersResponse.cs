using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetPersonExaminersResponse
    {
        public IEnumerable<PersonExaminerDto> Results { get; set; }
    }
}