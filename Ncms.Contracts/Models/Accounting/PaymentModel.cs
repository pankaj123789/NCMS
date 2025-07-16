using System;

namespace Ncms.Contracts.Models.Accounting
{
    public class PaymentModel
    {
        public string InvoiceNumber { get; set; }
        public InvoiceTypeModel InvoiceType { get; set; }
        public int? OfficeId { get; set; }
        public string Office { get; set; }
        public int? NaatiNumber { get; set; }
        public string Customer { get; set; }
        public DateTime DatePaid { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentType { get; set; }
        public string BSB { get; set; }
        public string ChequeNumber { get; set; }
        public string BankName { get; set; }
        public int? EftMachineId { get; set; }
        public string EftMachine { get; set; }
        public string Reference { get; set; }
        public string PaymentAccount { get; set; }
    }
}
