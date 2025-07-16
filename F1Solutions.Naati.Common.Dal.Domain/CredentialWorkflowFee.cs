using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialWorkflowFee : EntityBase
    {
        public virtual CredentialApplication CredentialApplication { get; set; }
        public virtual CredentialRequest CredentialRequest { get; set; }
        public virtual string InvoiceNumber { get; set; }
        public virtual Guid? InvoiceId { get; set; }
        public virtual string PaymentReference { get; set; }
        public virtual ProductSpecification ProductSpecification { get; set; }
        public virtual SystemActionType OnInvoiceCreatedSystemActionType { get; set; }
        public virtual DateTime? InvoiceActionProcessedDate { get; set; }
        public virtual SystemActionType OnPaymentCreatedSystemActionType { get; set; }
        public virtual DateTime? PaymentActionProcessedDate { get; set; }
        public virtual string TransactionId { get; set; }
        public virtual string OrderNumber { get; set; }
        public virtual CredentialApplicationRefundPolicy CredentialApplicationRefundPolicy { get; set; }
    }
}
