using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityPublicOperations
{
    internal class PurchaseInvoiceOperation : BaseEntityOperation
    {
        internal PurchaseInvoiceOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient) { }
        public async Task<ApiResponse<Invoices>> GetPurchaseInvoicesAsync(string accessToken, string tenantId, DateTime? ifModifiedSince = null, string where = null, string order = null, List<Guid> iDs = null, List<Guid> contactIDs = null, List<string> statuses = null, int? page = null, bool? includeArchived = null, bool? createdByMyApp = null, int? unitdp = null)
        {
            var filter = where.ToPurchaseInvoiceFilter();
            var nativeResponse = await new EntityNativeOperations.PurchaseInvoiceOperation(this.AsynchronousClient).GetPurchaseInvoicesAsync(accessToken, tenantId, filter);
            var publicResponse = new ApiResponse<Invoices>(nativeResponse.StatusCode, nativeResponse.Data.ToPublicInvoices());
            return publicResponse;
        }
    }
}
