using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using MyNaati.Contracts.BackgroundTask;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.BackgroundTasks
{
    public class MyNaatiBackgroundTaskService : BaseBackgroundService<MyNaatiJobTypeName, IBackgroundTask>
    {
        private readonly ISecretsCacheQueryService _secretsProvider;

        public MyNaatiBackgroundTaskService(ISecretsCacheQueryService secretsProvider, IUtilityQueryService utilityQueryService, IEmailMessageQueryService emailMessageQueryService, IBackgroundTaskLogger backgroundTaskLogger) : base(utilityQueryService, emailMessageQueryService, backgroundTaskLogger)
        {
            _secretsProvider = secretsProvider;
        }

        protected override string BackgroundServiceName => "MyNaati";

        protected override IReadOnlyDictionary<MyNaatiJobTypeName, Func<IBackgroundTask>> GetTaskFactory()
        {
            return new Dictionary<MyNaatiJobTypeName, Func<IBackgroundTask>>
            {
                { MyNaatiJobTypeName.MyNaatiRefreshAllUsersCache, CreateTask<IMyNaatiRefreshAllUsersTask> },
                { MyNaatiJobTypeName.MyNaatiRefreshCookieCache, CreateTask<IMyNaatiRefreshCookieTask> },
                { MyNaatiJobTypeName.MyNaatiRefreshSystemCache, CreateTask<IMyNaatiRefreshSystemCacheTask> },
                { MyNaatiJobTypeName.MyNaatiRefreshPendingUsersCache, CreateTask<IMyNaatiRefreshPendingUsersTask> },
            };
        }

        protected override IBackgroundTask CreateTask<T>()
        {
            return ServiceLocator.Resolve<T>();
        }

        protected override void SetPrincipal()
        {
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(_secretsProvider.Get(SecuritySettings.MyNaatiDefaultIdentityKey)), null);
        }

        protected override string GetSystemValue(string systemValueKey)
        {
            var service = ServiceLocator.Resolve<IConfigurationService>();
            var value = service.GetSystemValue(systemValueKey);
            return value;
        }
    }
}
