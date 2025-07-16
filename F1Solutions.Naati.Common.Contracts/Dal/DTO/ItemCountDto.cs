using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ItemCountDto
    {
        public PractitionerLookupType Type { get; set; }
        public IEnumerable<ItemCountValueDto> Values { get; set; }
    }

    public class ApiPublicItemCountDto
    {
        public ApiPublicPractitionerCountLookupType Type { get; set; }
        public IEnumerable<ItemCountValueDto> Values { get; set; }
    }
}