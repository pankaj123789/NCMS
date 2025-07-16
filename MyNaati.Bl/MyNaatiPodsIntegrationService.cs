using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Bl.Services;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using MyNaati.Contracts;
using MyNaati.Contracts.BackgroundTask;
using MyNaati.Contracts.Portal.Users;

namespace MyNaati.Bl
{
    public class MyNaatiPodsIntegrationService : IntegrationService, IMyNaatiPodsIntegrationService
    {
        private readonly ISystemQueryService _systemQueryService;

        protected override string BaseAddress { get; }

        protected override string AuthenticationScheme => SecuritySettings.MyNaatiPrivateApiScheme;

        protected override string GetPrivateKey()
        {
            var ncmsPrivateKey = _systemQueryService.GetSystemValue(new GetSystemValueRequest { ValueKey = SecuritySettings.MyNaatiPrivateKeyValue, ForceRefresh = true }).Value;
            ncmsPrivateKey = HmacCalculatorHelper.UnProtectKey(ncmsPrivateKey);
            return ncmsPrivateKey;
        }

        protected override string GetPublicKey()
        {
            var ncmsPublicKey = _systemQueryService.GetSystemValue(new GetSystemValueRequest { ValueKey = SecuritySettings.MyNaatiPublicKeyValue, ForceRefresh = true }).Value;
            ncmsPublicKey = HmacCalculatorHelper.UnProtectKey(ncmsPublicKey);
            return ncmsPublicKey;
        }

        public MyNaatiPodsIntegrationService(ISystemQueryService systemQueryService)
        {
            _systemQueryService = systemQueryService;
            BaseAddress = ConfigurationManager.AppSettings["MyNaatiPodUrl"];
        }

        public Task<BusinessServiceResponse> RefreshUserCacheAsync(string podIp, IEnumerable<MyNaatiUserRefreshDto> users)
        {
            var request = $"{podIp}/" + MyNaatiIntegrationSettings.MyNaatiRoutePrefix + "/" + MyNaatiIntegrationSettings.RefreshUserCache;
            return SendPostRequestAsync<object, BusinessServiceResponse>(new RefreshUsersRequest { Users = users }, request);
            ;
        }
        public Task<BusinessServiceResponse> ExecuteRefreshPendingUserTaskAsync(string podIp)
        {
            var request = $"{podIp}/" + MyNaatiIntegrationSettings.MyNaatiRoutePrefix + "/" + MyNaatiIntegrationSettings.ExecuteRefreshPendingUsersTask;
            return SendPostRequestAsync<object, BusinessServiceResponse>(new { }, request);
            ;
        }
        public Task<BusinessServiceResponse> RefreshAllUsersCacheAsync(string podIp)
        {
            var request = $"{podIp}/" + MyNaatiIntegrationSettings.MyNaatiRoutePrefix + "/" + MyNaatiIntegrationSettings.RefreshAllUsersCache;
            return SendPostRequestAsync<object, BusinessServiceResponse>(new { }, request);
        }

        public Task<BusinessServiceResponse> RefreshAllInvalidCookiesAsync(string podIp)
        {
            var request = $"{podIp}/" + MyNaatiIntegrationSettings.MyNaatiRoutePrefix + "/" + MyNaatiIntegrationSettings.RefreshAllInvalidCookies;
            return SendPostRequestAsync<object, BusinessServiceResponse>(new { }, request);
        }

        public Task<BusinessServiceResponse> RefreshSystemCacheAsync(string podIp)
        {
            var request = $"{podIp}/" + MyNaatiIntegrationSettings.MyNaatiRoutePrefix + "/" + MyNaatiIntegrationSettings.RefreshSystemCache;
            return SendPostRequestAsync<object, BusinessServiceResponse>(new { }, request);
        }
    }
}
