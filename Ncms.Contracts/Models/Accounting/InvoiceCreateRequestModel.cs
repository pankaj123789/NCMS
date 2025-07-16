using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models.Accounting
{
    public class InvoiceCreateRequestModel
    {
        public string InvoiceNumber { get; set; }
        public InvoiceType InvoiceType { get; set; } = InvoiceType.Invoice;
        public int EntityId { get; set; }
        public int NaatiNumber { get; set; }
        public bool IsInstitution { get; set; }
        public string Reference { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(3);
        public Guid? BrandingThemeId { get; set; }
        public IEnumerable<InvoiceLineItemModel> LineItems { get; set; }
        public bool CancelOperationIfError { get; set; }

        public InvoicePaymentCreateModel[] Payments { get; set; }

        public bool BatchProcess { get; set; }
    }
    public class InvoicePaymentCreateModel
    {
        public string InvoiceNumber { get; set; }
        public PaymentTypeDto PaymentType { get; set; }
        public string Reference { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string EftMachine { get; set; }
        public string BSB { get; set; }
        public string ChequeNumber { get; set; }
        public string ChequeBankName { get; set; }
        public string OrderNumber { get; set; }
    }
  
}
