using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetInvoicePdfByIdRequest
    {
        public Guid InvoiceId { get; set; }
        public InvoiceType Type { get; set; }
    }
}