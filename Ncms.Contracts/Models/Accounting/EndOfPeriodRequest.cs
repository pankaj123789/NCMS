using System;

namespace Ncms.Contracts.Models.Accounting
{
    public class EndOfPeriodRequest
    {
        public int[] NaatiNumber { get; set; }
        public DateTime? DateCreatedFrom { get; set; }
        public DateTime? DateCreatedTo { get; set; }
        public int[] Office { get; set; }
        public int[] EftMachine { get; set; }
        public string[] InvoiceNumber { get; set; }
        public string[] PaidToAccount { get; set; }
        public string[] PaymentType { get; set; }
    }

    public class InvoiceRequest : EndOfPeriodRequest
    {
        public bool IncludeFullPaymentInfo { get; set; } = true;
        public bool ExcludePayables { get; set; } = true;
        public bool ExcludeCreditNotes { get; set; }
    }
}
