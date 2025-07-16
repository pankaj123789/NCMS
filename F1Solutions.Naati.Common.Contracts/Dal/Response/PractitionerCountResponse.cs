using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class PractitionerCountResponse 
    {
        public IEnumerable<ItemCountDto> Results { get; set; }
    }

    public class ApiPublicPractitionerCountServiceResponse
    {
        public IEnumerable<ApiPublicItemCountDto> Results { get; set; }
    }

    public class ApiPublicPractitionerCountResponse : BaseResponse
    {
        public IEnumerable<ItemCountValueDto> ByCredentialTypeId { get; set; }
        public IEnumerable<ItemCountValueDto> ByCountryId { get; set; }
        public IEnumerable<ItemCountValueDto> ByStateId { get; set; }
        public int TotalPractitinerCount { get; set; }
    }
}