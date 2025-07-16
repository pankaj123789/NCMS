using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace Ncms.Bl.Mappers
{
    public static class SharedCustomMapper
    {
        public static ProductSpecificationLookupModel MapProductSpecificationDetails(ProductSpecificationDetailsDto productSpecificationDetailsRequest)
        {
            var productSpecificationLookupModel = new ProductSpecificationLookupModel
            {
                Id = productSpecificationDetailsRequest.Id,
                DisplayNameWithGlCode = $"{productSpecificationDetailsRequest.Code} ({productSpecificationDetailsRequest.GlCode})",
                CostPerUnit = productSpecificationDetailsRequest.CostPerUnit,
                Inactive = productSpecificationDetailsRequest.Inactive
            };

            return productSpecificationLookupModel;
        }
    }
}
