using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetInvoiceBrandingThemeResponse : FinanceServiceResponse
    {
        public InvoiceBrandingThemeDto InvoiceBrandingTheme { get; set; }
    }
}