namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class FinanceBatchOperationResult 
    {
        public int CredentialWorkflowFeeId { get; set; }
        public int CredentialApplicationRefundId { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceReference { get; set; }
        public string CreditNoteReference { get; set; }
        public string CreditNoteNumber { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }
    }
}