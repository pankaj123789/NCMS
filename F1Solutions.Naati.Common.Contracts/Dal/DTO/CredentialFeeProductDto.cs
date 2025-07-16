using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialFeeProductDto
    {
        public int Id { get; set; }
        public int? CredentialTypeId { get; set; }
        public int CredentialApplicationTypeId { get; set; }
        public FeeTypeName FeeType { get; set; }
        public ProductSpecificationDetailsDto ProductSpecification { get; set; }
    }
}