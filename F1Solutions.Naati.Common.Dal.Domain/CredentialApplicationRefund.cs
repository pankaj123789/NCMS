using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationRefund : EntityBase
    {
        public virtual CredentialWorkflowFee CredentialWorkflowFee { get; set; }
        public virtual string CreditNoteNumber { get; set; }
        public virtual Guid? CreditNoteId { get; set; }
        public virtual string PaymentReference { get; set; }
        public virtual decimal? InitialPaidAmount { get; set; }
        public virtual decimal? RefundAmount { get; set; }
        public virtual double? RefundPercentage { get; set; }
        public virtual string RefundTransactionId { get; set; }
        public virtual RefundMethodType RefundMethodType { get; set; }
        public virtual User User { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime? RefundedDate { get; set; }
        public virtual DateTime? CreditNoteProcessedDate { get; set; }
        public virtual DateTime? CreditNotePaymentProcessedDate { get; set; }
        public virtual SystemActionType OnCreditNoteCreatedSystemActionType { get; set; }
        public virtual SystemActionType OnPaymentCreatedSystemActionType { get; set; }
        public virtual bool DisallowProcessing { get; set; }
        public virtual bool IsRejected { get; set; }
        public virtual decimal? InitialPaidTax { get; set; }
        public virtual string Comments { get; set; }
        public virtual string BankDetails { get; set; }
    }
}
