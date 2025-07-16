using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.NativeModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityNativeOperations
{
    internal class VendorOperation : BaseEntityOperation
    {
        internal VendorOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient)
        {

        }
        internal async Task<ApiResponse<Vendors>> GetVendorsAsync(string accessToken, string tenantId, string filter)
        {

            // verify the required parameter 'tenantId' is set
            //if (tenantId == null)
            //    throw new ApiException(400, "Missing required parameter 'tenantId' when calling AccountingApi->GetContacts");


            //if (!String.IsNullOrEmpty(accessToken))
            //{
            //    AddAccessToken(accessToken);
            //}

            var response = await GetAsyncWithHttpInfo<Vendors>(accessToken, tenantId, $"/vendors{filter}");

            if (this.ExceptionFactory != null)
            {
                Exception exception = this.ExceptionFactory("GetVendors", response);
                if (exception != null) throw exception;
            }

            return response;
        }
    }
}
