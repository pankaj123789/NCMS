using System.Collections.Generic;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Contracts
{
    public interface INcmsPodsIntegrationService
    {
        Task<BusinessServiceResponse> RefreshUserCacheAsync(string podIp, IEnumerable<NcmsUserRefreshDto> users);
        Task<BusinessServiceResponse> RefreshAllUsersCacheAsync(string podIp);
        Task<BusinessServiceResponse> RefreshAllInvalidCookiesAsync(string podIp);
        Task<BusinessServiceResponse> RefreshSystemCacheAsync(string podIp);
        Task<BusinessServiceResponse> ExecuteRefreshPendingUserTaskAsync(string podIp);

        Task<BusinessServiceResponse> RefreshUserNotifications(string podIp, IEnumerable<string> userNames);
    }
}
