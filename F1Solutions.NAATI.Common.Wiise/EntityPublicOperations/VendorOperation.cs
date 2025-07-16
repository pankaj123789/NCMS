using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityPublicOperations
{
    internal class VendorOperation : BaseEntityOperation
    {
        internal VendorOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient) { }

        internal async Task<ApiResponse<Contacts>> GetVendorsAsync(string accessToken, string tenantId, DateTime? ifModifiedSince, string where, string order, List<Guid> iDs, int? page, bool? includeArchived)
        {
            var filter = where.ToContactFilter();
            return await GetVendorsAsync(accessToken, tenantId,filter);
        }

        internal async Task<ApiResponse<Contacts>> GetVendorsAsync(string accessToken, string tenantId,string filter)
        {
            var nativeResponse = await new EntityNativeOperations.VendorOperation(this.AsynchronousClient).GetVendorsAsync(accessToken, tenantId, filter);
            return new ApiResponse<Contacts>(nativeResponse.StatusCode, nativeResponse.Data.ToPublicModelContacts());
        }
    }
}
