using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using MyNaati.Contracts;
using MyNaati.Contracts.BackgroundTask;

namespace MyNaati.Bl.BackgroundTasks
{
    public class MyNaatiRefreshCookieTask : BaseBackgroundTask, IMyNaatiRefreshCookieTask
    {
        private readonly ICookieQueryService _cookieQueryService;
        private readonly IMyNaatiPodsIntegrationService _podsIntegrationService;
        private readonly IUtilityQueryService _utilityQueryService;

        public MyNaatiRefreshCookieTask(ISystemQueryService systemQueryService,
            IBackgroundTaskLogger backgroundTaskLogger,
            IMyNaatiPodsIntegrationService podsIntegrationService,
            ICookieQueryService cookieQueryService, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _podsIntegrationService = podsIntegrationService;
            _cookieQueryService = cookieQueryService;
            _utilityQueryService = utilityQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            RefreshAllServersInvalidCookies();
        }

        public void RefreshAllInvalidLocalCookies()
        {
            var ip = _utilityQueryService.GetSystemIp();
            TaskLogger.WriteInfo("Refreshing all invalid cookies on server {serverIp}", ip);
            _cookieQueryService.RefreshAllCache();
        }

        private void RefreshAllServersInvalidCookies()
        {
            TaskLogger.WriteInfo("Removing expired cookies...");
            _cookieQueryService.RemoveExpiredInvalidCookies();
            RefreshAllInvalidLocalCookies();
            ExecuteRemoteServersAction(_podsIntegrationService.RefreshAllInvalidCookiesAsync);
        }
    }
}
