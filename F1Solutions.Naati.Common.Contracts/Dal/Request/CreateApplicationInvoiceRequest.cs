using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateApplicationInvoiceRequest : CreateInvoiceRequest
    {
        public int CredentialApplicationId { get; set; }
        public int CredentialRequestId { get; set; }
        public SystemActionTypeName? InvoiceCompletionAction { get; set; }
        public SystemActionTypeName? PaymentCompletionAction { get; set; }
        public int ProductSpecificationId { get; set; }
        public int CredentialFeeProductId { get; set; }
    }
}