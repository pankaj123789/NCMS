using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.NativeModels;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityNativeOperations
{
    internal class AccountOperation : BaseEntityOperation
    {
        internal AccountOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient)
        {

        }

        internal async Task<ApiResponse<Accounts>> GetAccountsAsync(string accessToken, string tenantId)
        {
            return await GetAsyncWithHttpInfo<Accounts>(accessToken, tenantId, $"/accounts");
        }
    }
}
