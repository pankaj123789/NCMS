using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise
{
    public interface IWiiseAccountingApi
    {
        void ConfigureWiise(string WiiseBaseUrl, string WiiseResource, string WiiseAuthRedirectUri, string WiiseAuthClientId, string WiiseClientSecret);

        Task<ApiResponse<Invoices>> CreateInvoicesAsync(string accessToken, string tenantId, Invoices invoices);
        Task<ApiResponse<Invoices>> GetInvoicesAsync(string accessToken, string tenantId, string where = null, List<Guid> contactIDs = null, List<string> statuses = null, int? pageSize = null, int? page = null, string invoiceId = null);
        //Not used as create payment would post the invoice 
        Task<ApiResponse<Invoices>> PostInvoicesAsync(string accessToken, string tenantId, Invoices invoices);
        Task<ApiResponse<Stream>> GetInvoiceAsPdfAsync(string accessToken, string tenantId, Guid invoiceId);


        Task<ApiResponse<CreditNotes>> CreateCreditNotesAsync(string accessToken, string tenantId, CreditNotes creditNotes);
        Task<ApiResponse<CreditNotes>> GetCreditNotesAsync(string accessToken, string tenantId, string where = null);
        Task<ApiResponse<Stream>> GetCreditNoteAsPdfAsync(string accessToken, string tenantId, Guid creditNoteId);

        Task<ApiResponse<Payments>> CreatePaymentsAsync(string accessToken, string tenantId, Payments payments, string journalNumber);
        Task<ApiResponse<Payments>> GetPaymentsAsync(string accessToken, string tenantId, string where = null);

        Task<ApiResponse<Invoices>> GetPurchaseInvoicesAsync(string value, string tenant, List<string> statuses, List<Guid> contactIDs, string where);

        Task<ApiResponse<Contacts>> CreateContactsAsync(string accessToken, string tenantId, Contacts contacts);
        Task<ApiResponse<Contacts>> GetContactsAsync(string accessToken, string tenantId, string where = null, int? pageSize = null, int? page = null);
        Task<ApiResponse<Invoices>> GetAllInvoicesAsync(string value, string tenant, List<string> statuses, List<Guid> contactIDs, string where);
        Task<ApiResponse<Contacts>> GetAllContactsAsync(string value, string tenant, string where);

        Task<ApiResponse<Contacts>> GetVendorsAsync(string accessToken, string tenantId, DateTime? ifModifiedSince = null, string where = null, string order = null, List<Guid> iDs = null, int? page = null, bool? includeArchived = null);
        Task<ApiResponse<BrandingThemes>> GetBrandingThemesAsync(string value, string tenant);

        Task<ApiResponse<Accounts>> GetAccountsAsync(string value, string tenant, string where = null);
    }
}
