using System;
using System.Collections.Generic;

namespace Ncms.Contracts.Models.Accounting
{
    public class InvoiceModel
    {
        public int OfficeId { get; set; }
        public string Office { get; set; }
        public int? TransactionId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? AmountDue { get; set; }
        public decimal Total { get; set; }
        public InvoiceTypeModel Type { get; set; }
        public InvoiceStatusModel Status { get; set; }
        public decimal Payment { get; set; }
        public decimal Balance { get; set; }
        public int? NaatiNumber { get; set; }
        public string Customer { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? Date { get; set; }
        public string WiiseReference { get; set; }
        public string Theme { get; set; }
        public IEnumerable<PaymentModel> Payments { get; set; }
    }

    public enum InvoiceTypeModel
    {
        Invoice = 1,
        CreditNote = 2,
        Bill = 3,
    }

    public enum FinanceInfoLocationModel
    {
        Xero = 2,
    }

    public enum InvoiceStatusModel
    {
        Draft,
        Open,
        Paid,
        Canceled,
    }
}
