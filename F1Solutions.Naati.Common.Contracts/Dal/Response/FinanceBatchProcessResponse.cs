using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class FinanceBatchProcessResponse : FinanceServiceResponse
    {
        public IList<FinanceBatchInvoiceOperationResult> Invoices { get; set; } = new List<FinanceBatchInvoiceOperationResult>();
        public IList<FinanceBatchPaymentOperationResult> Payments { get; set; } = new List<FinanceBatchPaymentOperationResult>();
        public IList<FinanceBatchCreditNoteOperationResult> CreditNotes { get; set; } = new List<FinanceBatchCreditNoteOperationResult>();
    }
}