using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface IApiAccessRepository
    {
        ApiAccess GetApiAccess(string publicKey);
    }

    public class ApiAccessRepository : Repository<ApiAccess>, IApiAccessRepository
    {
        public ApiAccessRepository(ICustomSessionManager sessionManager) : base(sessionManager)
        {

        }

        public ApiAccess GetApiAccess(string publicKey)
        {
            return Session.Query<ApiAccess>()
                .SingleOrDefault(x => x.PublicKey == publicKey);
        }

    }
}
