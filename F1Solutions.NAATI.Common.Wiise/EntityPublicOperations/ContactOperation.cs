using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityPublicOperations
{
    internal class ContactOperation : BaseEntityOperation
    {
        internal ContactOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient) { }

        internal async Task<ApiResponse<Contacts>> GetContactsAsync(string accessToken, string tenantId, string where, int? pageSize = null, int? page = null)
        {
            var filter = where.ToContactFilter();
            var nativeResponse = await new EntityNativeOperations.ContactOperation(this.AsynchronousClient).GetContactsAsync(accessToken, tenantId, filter);
            return new ApiResponse<Contacts>(nativeResponse.StatusCode, nativeResponse.Data.ToPublicModelContacts());
        }

        internal async Task<ApiResponse<PublicModels.Contacts>> CreateContactsAsync(string accessToken, string tenantId, PublicModels.Contacts contacts)
        {
            var contactsCreated = await new EntityNativeOperations.ContactOperation(this.AsynchronousClient).CreateContactsAsync(accessToken, tenantId, contacts.ToNativeModelContacts());
            return new ApiResponse<Contacts>(contactsCreated.StatusCode, contactsCreated.Data.ToPublicModelContacts());
        }
    }
}
