using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class PractitionerSearchResponse : BaseResponse
    {
        public IEnumerable<PractitionerSearchDto> Results { get; set; }
        public int Total { get; set; }
    }

    public class ApiPublicPractitionerSearchResponse : BaseResponse
    {
        public IEnumerable<ApiPublicPractitionerSearchDto> Results { get; set; }
    }


}