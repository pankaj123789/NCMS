using System;
using System.Collections.Generic;

namespace Ncms.Contracts.Models.Accounting
{
    public class InvoicePreviewModel {
        public string InvoiceTo { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(3);
        public string BrandingTheme { get; set; }
        public IEnumerable<InvoiceLineItemModel> LineItems { get; set; }
        public string UserOfficeAbbreviation { get; set; }
        public bool DoNotInvoice { get; set; }
    }
}