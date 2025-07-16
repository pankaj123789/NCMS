using System;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class CreatePaymentModel
    {
        public string InvoiceNumber { get; set; }
        public string CreditNoteNumber { get; set; }
        public PaymentTypeDto PaymentType { get; set; }
        public string Reference { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string EftMachine { get; set; }
        public string BSB { get; set; }
        public string ChequeNumber { get; set; }
        public string ChequeBankName { get; set; }
        public string OrderNumber { get; set; }
    }
}