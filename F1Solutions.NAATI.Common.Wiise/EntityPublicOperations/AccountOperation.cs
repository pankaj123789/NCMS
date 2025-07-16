using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityPublicOperations
{
    internal class AccountOperation : BaseEntityOperation
    {
        internal AccountOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient) { }

        internal async Task<ApiResponse<Accounts>> GetAccountsAsync(string accessToken, string tenantId)
        {
            var nativeResponse = await new EntityNativeOperations.AccountOperation(this.AsynchronousClient).GetAccountsAsync(accessToken, tenantId);
            return new ApiResponse<Accounts>(nativeResponse.StatusCode, nativeResponse.Data.ToPublicModelAccounts());
        }

    }
}
