using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class InvoiceDto
    {
        public bool IsNaatiSponsored { get; set; }
        public int? OfficeId { get; set; }
        public string Office { get; set; }
        public int? TransactionId { get; set; }
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Total { get; set; }
        public decimal? AmountDue { get; set; }
        public decimal Payment { get; set; }
        public decimal Balance { get; set; }
        public int? NaatiNumber { get; set; }
        public string Customer { get; set; }
        public InvoiceStatus Status { get; set; }
        public InvoiceType Type { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? Date { get; set; }
        public string WiiseReference { get; set; }
        public Guid? ThemeId { get; set; }
        public PaymentDto[] Payments { get; set; }
        public LineItemDto[] LineItems { get; set; }
        public decimal? TotalTax { get; set; }
        public int? CredentialApplicationTypeId { get; set; }
        public string CredentialApplicationTypeDisplayName { get; set; }
        public int? CredentialApplicationId { get; set; }
    }
}