using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Common;

namespace Ncms.Contracts
{
    public interface IAccountingService
    {
        GenericResponse<AccountingOptionsModel> GetAccountingOptions();
        GenericResponse<IEnumerable<InvoiceBrandingThemeModel>> GetInvoiceBrandingThemes();
        GenericResponse<IEnumerable<InvoiceModel>> GetInvoices(InvoiceRequest request);
        GenericResponse<IEnumerable<InvoiceModel>> GetPurchaseInvoices(InvoiceRequest request);
        GenericResponse<IEnumerable<PaymentModel>> GetPayments(EndOfPeriodRequest request);
        GenericResponse<IEnumerable<BankAcountModel>> GetBankAccounts();
        GenericResponse<IEnumerable<OfficeModel>> GetOffices();
        GenericResponse<IEnumerable<EftMachineModel>> GetEftMachines();
        [Obsolete("Use GetInvoicePdfById instead")]
        FileModel GetInvoicePdf(string invoiceNumber, InvoiceTypeModel type);
        FileModel GetInvoicePdfById(Guid invoiceId, string invoiceNumber, InvoiceTypeModel type);
        GenericResponse<InvoiceCreateResponseModel> CreateInvoice(InvoiceCreateRequestModel model);
        GenericResponse<InvoiceCreateResponseModel> CreateApplicationInvoice(ApplicationInvoiceCreateRequestModel model);
        GenericResponse<RefundCreateResponseModel> CreateApplicationRefund(ApplicationRefundCreateRequestModel model);
        GenericResponse<InvoiceCreateResponseModel> RetryCreateInvoice(int operationId);
        GenericResponse<PaymentCreateResponseModel> CreatePayment(IEnumerable<PaymentCreateRequestModel> models);
        GenericResponse<PaymentCreateResponseModel> RetryCreatePayment(int operationId);
        BusinessServiceResponse CancelOperation(OperationCancelRequestModel model);
        FileModel ExportEndOfPeriod(InvoiceRequest invoicesRequest);
        IEnumerable<object> GetQueuedOperations(string request);

        /// <summary>
        /// When WIISE completes an operation but doesnt tell us about it then we need to get the system going again
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        GenericResponse<bool> ProgressCreditNote(ProgressCreditNoteModel model);

        /// <summary>
        /// When WIISE completes an operation but doesnt tell us about it then we need to get the system going again
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        GenericResponse<bool> ProgressInvoice(ProgressInvoiceModel model);

    }
}
