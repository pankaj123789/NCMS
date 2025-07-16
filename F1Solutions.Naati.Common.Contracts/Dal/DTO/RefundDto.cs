using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class RefundDto
    {
        public int Id { get; set; }
        public int CredentialWorkflowFeeId { get; set; }
        public int CredentialApplicationId { get; set; }
        public int CredentialRequestId { get; set; }
        public string OrderNumber { get; set; }
        public string CreditNoteNumber { get; set; }
        public Guid? CreditNoteId { get; set; }
        public string PaymentReference { get; set; }
        public decimal? InitialPaidAmount { get; set; }
        public decimal? RefundAmount { get; set; }
        public double? RefundPercentage { get; set; }
        public string RefundTransactionId { get; set; }
        public int RefundMethodTypeId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? RefundedDate { get; set; }
        public DateTime? CreditNoteProcessedDate { get; set; }
        public DateTime? CreditNotePaymentProcessedDate { get; set; }
        public int? OnCreditNoteCreatedSystemActionTypeId { get; set; }
        public int? OnPaymentCreatedSystemActionTypeId { get; set; }
        public int ObjectStatusId { get; set; }
        public bool? DisallowProcessing { get; set; }
        public bool? IsRejected { get; set; }
        public decimal? InitialPaidTax { get; set; }
        public string Comments { get; set; }
        public string BankDetails { get; set; }
        public int ProductCategoryId { get; set; }
    }
}
