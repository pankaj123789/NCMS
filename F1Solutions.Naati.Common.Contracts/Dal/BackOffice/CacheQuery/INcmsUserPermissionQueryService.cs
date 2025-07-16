using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery
{
    public interface INcmsUserPermissionQueryService : ICacheQueryService
    {
        IReadOnlyList<UserPermissionsDto> GetUserPermissionsByUserName(string userName);
        void RefreshUserCache(string userName);
    }
}
