using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetInvoiceDetailsResponse : FinanceServiceResponse
    {
        public InvoiceDto Invoice { get; set; }
    }
}