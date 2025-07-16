using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialWorkflowFeeDto
    {
        public int Id { get; set; }
        public int CredentialApplicationId { get; set; }
        public int? CredentialRequestId { get; set; }
        public string InvoiceNumber { get; set; }
        public string PaymentReference { get; set; }
        public InvoiceDto Invoice { get; set; }
        public ProductSpecificationDetailsDto ProductSpecification { get; set; }
        public virtual SystemActionTypeName? OnInvoiceActionType { get; set; }
        public virtual DateTime? InvoiceActionProcessedDate { get; set; }
        public SystemActionTypeName? OnPaymentActionType { get; set; }
        public DateTime? PaymentActionProcessedDate { get; set; }
        public Guid? InvoiceId { get; set; }
        public string OrderNumber { get; set; }
        public string TransactionId { get; set; }
        public CredentialApplicationRefundPolicyData CredentialApplicationRefundPolicy { get; set; }
    }
}