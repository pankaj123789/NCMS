using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models.Accounting
{
    public class ApplicationInvoiceCreateRequestModel : InvoiceCreateRequestModel
    {
        public SystemActionTypeName? PaymentCompletionAction { get; set; }
        public SystemActionTypeName? InvoiceCompletionAction { get; set; }
        public int ProductSpecificationId { get; set; }
        public int CredentialApplicationId { get; set; }
        public int CredentialRequestId { get; set; }
        public int CredentialFeeProductId { get; set; }
    }
}