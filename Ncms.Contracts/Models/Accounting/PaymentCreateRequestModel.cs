using System;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace Ncms.Contracts.Models.Accounting
{
    public class PaymentCreateRequestModel
    {
        public string InvoiceNumber { get; set; }
        public int OfficeId { get; set; }
        public PaymentTypeDto PaymentType { get; set; }
        public decimal Amount { get; set; }
        public DateTime DatePaid { get; set; }
        public Guid AccountId { get; set; }
        public string Reference { get; set; }
    }
}