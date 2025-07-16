using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    [Obsolete("Use GetInvoicePdfByIdRequest Instead")]
    public class GetInvoicePdfRequest
    {
        [Obsolete("Use GetInvoicePdfByIdRequest Instead")]
        public string InvoiceNumber { get; set; }
        public InvoiceType Type { get; set; }
    }
}