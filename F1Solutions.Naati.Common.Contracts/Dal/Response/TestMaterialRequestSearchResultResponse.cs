using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class TestMaterialRequestSearchResultResponse
    {
        public IEnumerable<TestMaterialRequestSearchDto> Results { get; set; }
    }
}