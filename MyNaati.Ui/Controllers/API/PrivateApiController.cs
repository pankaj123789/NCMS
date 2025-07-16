using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using Microsoft.Ajax.Utilities;
using MyNaati.Contracts.BackgroundTask;
using MyNaati.Contracts.Portal;
using MyNaati.Contracts.Portal.Users;
using MyNaati.Ui.Security;
using Newtonsoft.Json;

namespace MyNaati.Ui.Controllers.API
{
    [MyNaatiPrivateApiAuthentication((EndpointCaller.Ncms))]
    [RoutePrefix(MyNaatiIntegrationSettings.MyNaatiRoutePrefix)]
    public class PrivateApiController : BaseApiController
    {
        private readonly IMembershipProviderService _membershipProviderService;
        private readonly IMyNaatiRefreshPendingUsersTask _refreshPendingUsersTask;
        private readonly IMyNaatiRefreshAllUsersTask _refreshAllUsersTask;
        private readonly IMyNaatiRefreshCookieTask _refreshCookieTask;
        private readonly IMyNaatiRefreshSystemCacheTask _refreshSystemCacheTask;

        public PrivateApiController(IMembershipProviderService membershipProviderService, IMyNaatiRefreshPendingUsersTask refreshPendingUsersTask, IMyNaatiRefreshCookieTask refreshCookieTask, IMyNaatiRefreshSystemCacheTask refreshSystemCacheTask, IMyNaatiRefreshAllUsersTask refreshAllUsersTask)
        {
            _membershipProviderService = membershipProviderService;
            _refreshPendingUsersTask = refreshPendingUsersTask;
            _refreshCookieTask = refreshCookieTask;
            _refreshSystemCacheTask = refreshSystemCacheTask;
            _refreshAllUsersTask = refreshAllUsersTask;
        }

        [HttpPost]
        [Route(MyNaatiIntegrationSettings.DeleteUser)]
        public HttpResponseMessage DeleteUser(dynamic request)
        {
            return this.CreateResponse(() => _membershipProviderService.DeleteUser(request.userName.ToString()));
        }

        [HttpPost]
        [Route(MyNaatiIntegrationSettings.RenameUser)]
        public HttpResponseMessage RenameUser(dynamic request)
        {
            return this.CreateResponse(() => _membershipProviderService.RenameUser(request.oldUserName.ToString(), request.newUserName.ToString()));
        }

        [HttpPost]
        [Route(MyNaatiIntegrationSettings.UnlockUser)]
        public HttpResponseMessage UnlockUser(dynamic request)
        {
            var userName = request.userName.ToString();
            return this.CreateResponse(() => _membershipProviderService.UnlockUser(userName));
        }

        [HttpPost]
        [Route(MyNaatiIntegrationSettings.GetUser)]
        public HttpResponseMessage GetUser(dynamic request)
        {
            var userName = request.userName.ToString();
            return this.CreateResponse(() => _membershipProviderService.GetUser(userName, false));
        }
        [HttpPost]
        [MyNaatiPrivateApiAuthentication(EndpointCaller.MyNaati)]
        [Route(MyNaatiIntegrationSettings.RefreshUserCache)]
        public HttpResponseMessage RefreshUserCache(RefreshUsersRequest request)
        {
            _refreshPendingUsersTask.RefreshLocalUserCache(request.Users);
            return this.CreateResponse(() => true);
        }
        [HttpPost]
        [MyNaatiPrivateApiAuthentication(EndpointCaller.MyNaati)]
        [Route(MyNaatiIntegrationSettings.RefreshAllUsersCache)]
        public HttpResponseMessage RefreshAllUsersCache(dynamic request)
        {
            _refreshAllUsersTask.RefreshAllUsersLocalCache();
            return this.CreateResponse(() => true );
        }

        [HttpPost]
        [Route(MyNaatiIntegrationSettings.RefreshAllInvalidCookies)]
        [MyNaatiPrivateApiAuthentication(EndpointCaller.MyNaati)]
        public HttpResponseMessage RefreshAllInvalidCookies(dynamic request)
        {
            _refreshCookieTask.RefreshAllInvalidLocalCookies();
            return this.CreateResponse(() => true);
        }

        [HttpPost]
        [Route(MyNaatiIntegrationSettings.RefreshSystemCache)]
        [MyNaatiPrivateApiAuthentication(EndpointCaller.MyNaati)]
        public HttpResponseMessage RefreshSystemCache(dynamic request)
        {
            _refreshSystemCacheTask.RefreshLocalSystemCache();
            return this.CreateResponse(() => true);
        }

        [HttpPost]
        [Route(MyNaatiIntegrationSettings.ExecuteRefreshPendingUsersTask)]
        [MyNaatiPrivateApiAuthentication(EndpointCaller.MyNaati)]
        public HttpResponseMessage ExecuteRefreshPendingUserTask(dynamic request)
        {
            Startup.RefreshPendingUsersCache(null, null);
            return this.CreateResponse(() => true);
        }
    }
}