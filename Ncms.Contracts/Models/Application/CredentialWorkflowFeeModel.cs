using System;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.Accounting;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialWorkflowFeeModel
    {
        public int Id { get; set; }
        public int CredentialApplicationId { get; set; }
        public int? CredentialRequestId { get; set; }
        public SystemActionTypeName? OnInvoiceActionType { get; set; }
        public SystemActionTypeName? OnPaymentActionType { get; set; }
        public string InvoiceNumber { get; set; }
        public string PaymentReference { get; set; }
        public Guid? InvoiceId { get; set; }
        public InvoiceModel Invoice { get; set; }
        public ProductSpecificationModel ProductSpecification { get; set; }
 
        public DateTime? PaymentActionProcessedDate { get; set; }
        public DateTime? InvoiceActionProcessedDate { get; set; }

        public string TransactionId { get; set; }
        public string OrderNumber { get; set; }
        public CredentialApplicationRefundPolicyData CredentialApplicationRefundPolicy { get; set; }
    }
}