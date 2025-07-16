namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetInvoicesRequest : EndOfPeriodRequest
    {
        public bool IncludeFullPaymentInfo { get; set; } = true;
        public bool ExcludePayables { get; set; } = true;
        public bool ExcludeCreditNotes { get; set; } 
        public bool IncludeVoidedStatus { get; set; }
    }
}