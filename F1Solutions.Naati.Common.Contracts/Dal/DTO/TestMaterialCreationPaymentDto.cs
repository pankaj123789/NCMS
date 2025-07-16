using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestMaterialCreationPaymentDto
    {
        public int TestMaterialId { get; set; }
        public DateTime? MaterialCreationSubmittedDate { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime? PaymentApprovedDate { get; set; }
        public DateTime? PaymentProcessedDate { get; set; }
        public string InvoiceNo { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TotalInvoice { get; set; }
        public string Skill { get; set; }
        public string CredentialType { get; set; }
    }
}