using System.Collections.Generic;

namespace Ncms.Contracts.Models.Accounting
{
    public class AccountingOptionsModel
    {
        public IEnumerable<BankAcountModel> PaymentAccounts { get; set; }
        public IEnumerable<InvoiceBrandingThemeModel> InvoiceBrandingThemes { get; set; }
    }
}
