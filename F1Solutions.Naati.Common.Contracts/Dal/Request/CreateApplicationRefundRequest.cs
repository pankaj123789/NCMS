using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateApplicationRefundRequest : CreatPayableRequest
    {
        public int CredentialApplicationRefundId { get; set; }

        public InvoiceType InvoiceType { get; set; } = InvoiceType.CreditNote;
        public string InvoiceNumber { get; set; }
        public SystemActionTypeName? CreditNotePaymentCompletionAction { get; set; }
        public SystemActionTypeName? CreditNoteCreateCompletionAction { get; set; }
    }
}