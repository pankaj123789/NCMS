using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
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
}