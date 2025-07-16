using F1Solutions.Naati.Common.Wiise.EntityPublicOperations;
using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using F1Solutions.Global.Common.Logging;
using System.IO;
using System.Net;
using System.Linq;

namespace F1Solutions.Naati.Common.Wiise
{
    public class WiiseAccountingApi : IWiiseAccountingApi
    {
        private IAsynchronousClient _apiClient { get; set; }
        public IAsynchronousClient ApiClient
        {
            get
            {
                if(_apiClient == null)
                {
                    _apiClient = new ApiClient();
                }
                return _apiClient;
            }
        }

        public async Task<ApiResponse<Invoices>> CreateInvoicesAsync(string accessToken, string tenantId, Invoices invoices)
        {
            try
            {
                return await new SalesInvoiceOperation(ApiClient).CreateInvoicesAsync(accessToken, tenantId, invoices);
            }
            catch (Exception ex)
            {
                var invoiceArray = invoices._Invoices.Select(x=>x.InvoiceNumber).ToArray();
                var invoiceList = String.Join(", ", invoiceArray);
                var apiEntryLevelException = new Exception($"CreateInvoicesAsync Failed: {invoiceList}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException; 
            }
        }

        public async Task<ApiResponse<Invoices>> GetInvoicesAsync(string accessToken, string tenantId, string where = null, List<Guid> contactIDs = null, List<string> statuses = null, int? pageSize = null, int? page = null,string invoiceId = null)
        {
            try
            {
                return await new SalesInvoiceOperation(ApiClient).GetInvoicesAsync(accessToken, tenantId, where, contactIDs, statuses, page, invoiceId);
            }
            catch(Exception ex)
            {
                var apiEntryLevelException = new Exception($"GetInvoicesAsync Failed: InvoiceId:{invoiceId} Where:{where}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<CreditNotes>> GetCreditNotesAsync(string accessToken, string tenantId, string where = null, List<Guid> contactIDs = null, List<string> statuses = null, int? pageSize = null, int? page = null)
        {
            try
            {
                return await new CreditNoteOperation(ApiClient).GetCreditNotesAsync(accessToken, tenantId, where);
            }
            catch (Exception ex)
            {
                var apiEntryLevelException = new Exception($"GetCreditNotesAsync Failed: Where:{where}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<Invoices>> GetPurchaseInvoicesAsync(string value, string tenant, List<string> statuses, List<Guid> contactIDs, string where)
        {
            try
            {
                return await new PurchaseInvoiceOperation(ApiClient).GetPurchaseInvoicesAsync(value, tenant, null, where, null, null, contactIDs, statuses);
            }
            catch (Exception ex)
            {
                var contactArray = contactIDs.ToArray();
                var contactList = String.Join(", ", contactArray);
                var apiEntryLevelException = new Exception($"GetPurchaseInvoicesAsync Failed: {contactList}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<CreditNotes>> CreateCreditNotesAsync(string accessToken, string tenantId, CreditNotes creditNotes)
        {
            try
            {
                return await new CreditNoteOperation(ApiClient).CreateCreditNotesAsync(accessToken, tenantId, creditNotes);
            }
            catch (Exception ex)
            {
                var creditNoteArray = creditNotes._CreditNotes.Select(x=>x.Reference).ToArray();
                var creditNoteList = String.Join(", ", creditNoteArray);
                var apiEntryLevelException = new Exception($"CreateCreditNotesAsync Failed: {creditNoteList}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<CreditNotes>> GetCreditNotesAsync(string accessToken, string tenantId, string where = null)
        {
            try
            {
                return await new CreditNoteOperation(ApiClient).GetCreditNotesAsync(accessToken, tenantId, where);
            }
            catch (Exception ex)
            {
                var apiEntryLevelException = new Exception($"GetCreditNotesAsync Failed: Where:{where}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }


        public async Task<ApiResponse<Payments>> CreatePaymentsAsync(string accessToken, string tenantId, Payments payments, string journalNumber)
        {
            try
            {
                return await new PaymentOperation(ApiClient).CreatePaymentsAsync(accessToken, tenantId, payments, journalNumber);
            }
            catch (Exception ex)
            {
                var paymentsArray = payments._Payments.Select(x => x.InvoiceNumber).ToArray();
                var paymentsList = String.Join(", ", paymentsArray);
                var apiEntryLevelException = new Exception($"CreateCreditNotesAsync Failed: {paymentsList} Journal Number: {journalNumber}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<Payments>> GetPaymentsAsync(string accessToken, string tenantId, string where = null)
        {
            try
            {
                return await new PaymentOperation(ApiClient).GetPaymentsAsync(accessToken, tenantId, where);
            }
            catch (Exception ex)
            {
                var apiEntryLevelException = new Exception($"GetPaymentsAsync Failed: Where:{where}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<Contacts>> CreateContactsAsync(string accessToken, string tenantId, Contacts contacts)
        {
            try
            {
                return await new ContactOperation(ApiClient).CreateContactsAsync(accessToken, tenantId, contacts);
            }
            catch (Exception ex)
            {
                var contactsArray = contacts._Contacts.Select(x => x.EmailAddress).ToArray();
                var contactsList = String.Join(", ", contactsArray);
                var apiEntryLevelException = new Exception($"CreateContactsAsync Failed: {contactsList}", ex);

                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<Contacts>> GetContactsAsync(string accessToken, string tenantId, string where = null, int? pageSize = null, int? page = null)
        {
            try
            {
                return await new ContactOperation(ApiClient).GetContactsAsync(accessToken, tenantId, where, page);
            }
            catch (Exception ex)
            {
                var apiEntryLevelException = new Exception($"GetContactsAsync Failed: Where:{where}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<Invoices>> PostInvoicesAsync(string accessToken, string tenantId, Invoices invoices)
        {
            try
            {
                return await new SalesInvoiceOperation(ApiClient).PostInvoicesAsync(accessToken, tenantId, invoices);
            }
            catch (Exception ex)
            {
                var invoiceArray = invoices._Invoices.Select(x => x.InvoiceNumber).ToArray();
                var invoiceList = String.Join(", ", invoiceArray);
                var apiEntryLevelException = new Exception($"PostInvoicesAsync Failed: {invoiceList}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<Invoices>> GetAllInvoicesAsync(string accessToken, string tenant, List<string> statuses, List<Guid> contactIDs, string where)
        {
            try
            {
                return await GetInvoicesAsync(accessToken, tenant, where, contactIDs, statuses);
            }
            catch (Exception ex)
            {
                var apiEntryLevelException = new Exception($"GetAllInvoicesAsync Failed: Where:{where}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<Contacts>> GetAllContactsAsync(string accessToken, string tenant, string where)
        {
            try
            {
                return await GetContactsAsync(accessToken, tenant, where, 100, 1);
            }
            catch (Exception ex)
            {
                var apiEntryLevelException = new Exception($"GetAllContactsAsync Failed: Where:{where}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<Contacts>> GetVendorsAsync(string accessToken, string tenantId, DateTime? ifModifiedSince = null, string where = null, string order = null, List<Guid> iDs = null, int? page = null, bool? includeArchived = null)
        {
            try
            {
                return await new VendorOperation(ApiClient).GetVendorsAsync(accessToken, tenantId, ifModifiedSince, where, order, iDs, page, includeArchived);
            }
            catch (Exception ex)
            {
                var apiEntryLevelException = new Exception($"GetVendorsAsync Failed: Where:{where}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<Stream>> GetInvoiceAsPdfAsync(string accessToken, string tenantId, Guid invoiceId)
        {
            try
            {
                return await new SalesInvoiceOperation(ApiClient).GetInvoiceAsPdfAsync(accessToken, tenantId, invoiceId);
            }
            catch (Exception ex)
            {
                var apiEntryLevelException = new Exception($"GetInvoiceAsPdfAsync Failed: InvoiceId:{invoiceId}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        public async Task<ApiResponse<Stream>> GetCreditNoteAsPdfAsync(string accessToken, string tenantId, Guid creditNoteId)
        {
            try
            {
                return await new CreditNoteOperation(ApiClient).GetCreditNoteAsPdfAsync(accessToken, tenantId, creditNoteId);
            }
            catch (Exception ex)
            {
                var apiEntryLevelException = new Exception($"GetCreditNoteAsPdfAsync Failed: CreditNoteId:{creditNoteId}", ex);
                LoggingHelper.LogException(apiEntryLevelException);
                throw apiEntryLevelException;
            }
        }

        /// <summary>
        /// this is a placeholder only. Branding themes dont have relevance in Wiise.
        /// </summary>
        public async Task<ApiResponse<BrandingThemes>> GetBrandingThemesAsync(string value, string tenant)
        {
            var brandingThemes = new BrandingThemes();
            brandingThemes._BrandingThemes.Add(new BrandingTheme()
            {
                BrandingThemeID = Guid.Parse("bdb254dd-0820-4b0e-8547-db6799bd2296"),
                LogoUrl = "my.naati.com.au/Content/Images/NAATIAltLogo.png",
                Name = "Default Branding Theme",
                SortOrder = 1,
                Type = BrandingTheme.TypeEnum.INVOICE
            });
            return new ApiResponse<BrandingThemes>(HttpStatusCode.OK, brandingThemes);
        }

        public async Task<ApiResponse<Accounts>> GetAccountsAsync(string accessToken, string tenant, string where = null)
        {
            //note where is not there at the moment as there is no filter on this call except id. If BANK does show up we could post process the filter
            try { 
            return await new AccountOperation(ApiClient).GetAccountsAsync(accessToken, tenant);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex);
                throw ex;
            }
        }

        public void ConfigureWiise(string WiiseBaseUrl, string WiiseResource, string WiiseAuthRedirectUri, string WiiseAuthClientId, string WiiseClientSecret)
        {
            GlobalConfiguration.ConfigureWiise(WiiseBaseUrl, WiiseResource, WiiseAuthRedirectUri, WiiseAuthClientId, WiiseClientSecret);
        }
    }
}
