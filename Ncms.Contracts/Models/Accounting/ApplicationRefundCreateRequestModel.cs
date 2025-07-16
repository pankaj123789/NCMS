using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using System;

namespace Ncms.Contracts.Models.Accounting
{
    public class ApplicationRefundCreateRequestModel : InvoiceCreateRequestModel
    {
        public int CredentialApplicationRefundId { get; set; }
        public SystemActionTypeName? CreditNotePaymentCompletionAction { get; set; }
        public SystemActionTypeName? CreditNoteCreateCompletionAction { get; set; }

        public ApplicationRefundCreateRequestModel()
        {
            InvoiceType = InvoiceType.CreditNote;
        }
    }
}