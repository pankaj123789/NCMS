namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetInvoicePdfResponse : FinanceServiceResponse
    {
        public byte[] FileContent { get; set; }
    }
}