using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Bl.Services;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl
{
    public class NcmsPodsIntegrationService : IntegrationService, INcmsPodsIntegrationService
    {
        private readonly ISystemQueryService _systemQueryService;

        protected override string BaseAddress { get; }

        protected override string AuthenticationScheme => SecuritySettings.NcmsPrivateApiScheme;

        protected override string GetPrivateKey()
        {
            var ncmsPrivateKey = _systemQueryService.GetSystemValue(new GetSystemValueRequest {ValueKey = SecuritySettings.NcmsPrivateKeyValue, ForceRefresh = true}).Value;
            ncmsPrivateKey = HmacCalculatorHelper.UnProtectKey(ncmsPrivateKey);
            return ncmsPrivateKey;
        }

        protected override string GetPublicKey()
        {
            var ncmsPublicKey = _systemQueryService.GetSystemValue(new GetSystemValueRequest { ValueKey = SecuritySettings.NcmsPublicKeyValue, ForceRefresh = true }).Value;
            ncmsPublicKey = HmacCalculatorHelper.UnProtectKey(ncmsPublicKey);
            return ncmsPublicKey;
        }

        public NcmsPodsIntegrationService(ISystemQueryService systemQueryService)
        {
            _systemQueryService = systemQueryService;
            BaseAddress = ConfigurationManager.AppSettings["NcmsPodUrl"];
        }

        public Task<BusinessServiceResponse> RefreshUserCacheAsync(string podIp, IEnumerable<NcmsUserRefreshDto> users)
        {
            var request = $"{podIp}/" + NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.RefreshUserCache;
            return SendPostRequestAsync<object, BusinessServiceResponse>(new RefreshUsersRequest{ Users = users}, request);
        }

        public Task<BusinessServiceResponse> ExecuteRefreshPendingUserTaskAsync(string podIp)
        {
            var request = $"{podIp}/" + NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.ExecuteRefreshPendingUsersTask;
            return SendPostRequestAsync<object, BusinessServiceResponse>(new { }, request);
        }

        public Task<BusinessServiceResponse> RefreshAllUsersCacheAsync(string podIp)
        {
            var request = $"{podIp}/"+ NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.RefreshAllUsersCache;
            var response = SendPostRequestAsync<object, BusinessServiceResponse>(new { }, request);
            return response;
        }

        public Task<BusinessServiceResponse> RefreshAllInvalidCookiesAsync(string podIp)
        {
            var request = $"{podIp}/" + NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.RefreshAllInvalidCookies;
            var response = SendPostRequestAsync<object, BusinessServiceResponse>(new { }, request);
            return response;
        }

        public Task<BusinessServiceResponse> RefreshSystemCacheAsync(string podIp)
        {
            var request = $"{podIp}/" + NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.RefreshSystemCache;
            return SendPostRequestAsync<object, BusinessServiceResponse>(new { }, request);
        }
        public Task<BusinessServiceResponse> RefreshUserNotifications(string podIp, IEnumerable<string> userNames)
        {
            var request = $"{podIp}/" + NcmsIntegrationSettings.NcmsRoutePrefix + "/" + NcmsIntegrationSettings.RefreshUserNotifications;
            return SendPostRequestAsync<object, BusinessServiceResponse>(new RefreshUserNotificationsRequest { UserNames = userNames }, request);
        }
    }
}
