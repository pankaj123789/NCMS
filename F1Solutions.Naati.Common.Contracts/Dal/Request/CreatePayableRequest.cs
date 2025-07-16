using System;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreatPayableRequest
    {
        public int NaatiNumber { get; set; }
        public int EntityId { get; set; }
        public bool IsInstitution { get; set; }
        public string Reference { get; set; }
        public DateTime DueDate { get; set; }
        public Guid? BrandingThemeId { get; set; }
        public InvoiceLineItemDto[] LineItems { get; set; }
        public string Description { get; set; }
        public CreatePaymentModel[] Payments { get; set; }
        public bool CancelOperationIfError { get; set; }
        public bool BatchProcess { get; set; }
    }
}