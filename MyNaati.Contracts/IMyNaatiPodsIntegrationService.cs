using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using MyNaati.Contracts.BackgroundTask;

namespace MyNaati.Contracts
{
    public interface IMyNaatiPodsIntegrationService
    {
        Task<BusinessServiceResponse> RefreshUserCacheAsync(string podIp, IEnumerable<MyNaatiUserRefreshDto> users);

        Task<BusinessServiceResponse> RefreshAllUsersCacheAsync(string podIp);
        Task<BusinessServiceResponse> RefreshAllInvalidCookiesAsync(string podIp);
        Task<BusinessServiceResponse> RefreshSystemCacheAsync(string podIp);
        Task<BusinessServiceResponse> ExecuteRefreshPendingUserTaskAsync(string podIp);
    }
}
