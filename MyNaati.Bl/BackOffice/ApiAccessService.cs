using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.BackOffice;

namespace MyNaati.Bl.BackOffice
{
   
    public class ApiAccessService : IApiAccessService
    {
        private readonly IApiAccessRepository mApiAccessRepository;

        public ApiAccessService(IApiAccessRepository apiAccessRepository)
        {
            mApiAccessRepository = apiAccessRepository;
        }

        public ApiAccessResponse GetApiAccess(string publicKey)
        {
            var response = ToApiAccessResposne(mApiAccessRepository.GetApiAccess(publicKey));

            return response;
        }

        private static ApiAccessResponse ToApiAccessResposne(ApiAccess model)
        {
            if (model == null) return null;
            return new ApiAccessResponse
            {
                PrivateKey = model.PrivateKey,
                CreatedDate = model.CreatedDate,
                Inactive = model.Inactive,
                InstitutionId = model.Institution.Id,
                InstitutionName = model.Institution.InstitutionName,
                ModifiedDate = model.ModifiedDate,
                ModifiedUserId = model.ModifiedUser.Id,
                Permissions = model.Permissions
            };
        }
    }
}