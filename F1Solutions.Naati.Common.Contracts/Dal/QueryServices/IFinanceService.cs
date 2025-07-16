using System;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IFinanceService : IQueryService
    {
        
        GetBankAccountsResponse GetBankAccounts();

        
        GetInvoiceBrandingThemesResponse GetInvoiceBrandingThemes();

        
        GetInvoiceBrandingThemeResponse GetInvoiceBrandingTheme(Guid brandingThemeId);
        GetInvoicesResponse GetPurchaseInvoices(GetInvoicesRequest request);

        GetInvoicesResponse GetInvoices(GetInvoicesRequest request);

        GetInvoicesResponse GetUnraisedInvoices(int naatiNumber);

        CreateInvoiceResponse CreateInvoice(CreateInvoiceRequest request);

        
        CreateInvoiceResponse CreateApplicationInvoice(CreateApplicationInvoiceRequest request);

        CreateCreditNoteResponse CreateApplicationRefund(CreateApplicationRefundRequest request);

        CreateInvoiceResponse CreateInvoiceFromQueue(SubmitQueuedOperationRequest request);

        
        CreatePaymentResponse CreatePayment(CreatePaymentRequest request);

        
        CreatePaymentResponse CreatePaymentFromQueue(SubmitQueuedOperationRequest request);

        
        GetOfficesResponse GetOffices();

        
        GetEftMachinesResponse GetEftMachines();

        
        [Obsolete("Use GetInvoicePdfById Instead")]
        GetInvoicePdfResponse GetInvoicePdf(GetInvoicePdfRequest request);

        
        GetInvoicePdfResponse GetInvoicePdfById(GetInvoicePdfByIdRequest request);

        
        GetPaymentsResponse GetPayments(GetPaymentsRequest request);

        
        FinanceServiceResponse CancelOperation(int operationId);

        
        GetQueuedOperationsResponse GetQueuedOperations(GetQueuedOperationsRequest request);

        
        GetOnliceNaatiOfficeAbbrAndEftMachineTermianlResponse GetOnlineNaatiAbbrOfficeAndEftMachineTerminal(GetOnlineNaatiAbbrOfficeAndEftMachineTerminalRequest request);

        
        FinanceBatchProcessResponse PerformBatchOperations(PerformBatchOperationsRequest request);

        string GetInvoiceNumber(Guid invoiceId);

        Guid GetExternalAccountIdByCode(string code);

        /// <summary>
        /// Used to move Credit Notes forward that have processed in Wiise but Wiise never got around to telling us about it.
        /// Hopefully future versions of Wiise will be better and backout it something goes wrong before the transaction is complete
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="credentialRequestId"></param>
        /// <param name="refund"></param>
        /// <returns></returns>
        GenericResponse<bool> ProgressCreditNote(int applicationId, int credentialRequestId, RefundDto refund);

        /// <summary>
        /// Used to move Invoices forward that have processed in Wiise but Wiise never got around to telling us about it.
        /// Hopefully future versions of Wiise will be better and backout it something goes wrong before the transaction is complete
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="credentialRequestId"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceId"></param>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        GenericResponse<bool> ProgressInvoice(int applicationId, int credentialRequestId,string invoiceNo, string invoiceId, string paymentId);

        /// <summary>
        /// Get what is happening with the Invoice
        /// </summary>
        /// <param name="credentialRequestId"></param>
        /// <returns>GenericResponse<CredentialWorkflowFeeDto></returns>

        GenericResponse<CredentialWorkflowFeeDto> FindCredentialWorkflowFee(int credentialRequestId);
    }
}
