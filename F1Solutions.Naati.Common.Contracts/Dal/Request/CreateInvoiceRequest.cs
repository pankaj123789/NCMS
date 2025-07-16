using System;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateInvoiceRequest : CreatPayableRequest
    {
        public string InvoiceNumber { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public int UserId { get; set; }
    }
}