using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetProductSpecificationLookupListResponse
    {
        public IEnumerable<ProductSpecificationDetailsDto> ProductSpecificationDetails { get; set; }
    }
}