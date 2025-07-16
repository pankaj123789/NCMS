using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Bl.BackgroundOperation;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundOperation;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Bl.Security;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundOperations;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl
{
    public class NcmsBackgroundOperationService : BaseBackgroundOperationService<NcmsBackgoundOperationTypeName>
    {
        private readonly ISecretsCacheQueryService _secretsProvider;
        private readonly INotificationQueryService _notificationQueryService;

        public NcmsBackgroundOperationService(IUtilityQueryService utilityQueryService,
            IEmailMessageQueryService emailMessageQueryService,
            IBackgroundTaskLogger backgroundTaskLogger,
            ISecretsCacheQueryService secretsProvider,
            INotificationQueryService notificationQueryService) : base(utilityQueryService, emailMessageQueryService, backgroundTaskLogger)
        {
            _secretsProvider = secretsProvider;
            _notificationQueryService = notificationQueryService;
        }

        protected override string BackgroundServiceName => nameof(NcmsBackgroundOperationService);

        protected override IReadOnlyDictionary<NcmsBackgoundOperationTypeName, Func<IBackgroundOperation>> GetTaskFactory()
        {
            return new Dictionary<NcmsBackgoundOperationTypeName, Func<IBackgroundOperation>>
            {
                { NcmsBackgoundOperationTypeName.DownloadAllTestMaterials, CreateTask<IDownloadAllTestMaterialsOperation>}
            };
        }

        protected override IBackgroundOperation CreateTask<T>()
        {
            return ServiceLocator.Resolve<T>();
        }

        protected override void SetPrincipal()
        {
            Thread.CurrentPrincipal = new NcmsPrincipal(_secretsProvider.Get(SecuritySettings.NcmsDefaultIdentityKey));
        }

        protected override string GetSystemValue(string systemValueKey)
        {
            return ServiceLocator.Resolve<ISystemService>().GetSystemValue(systemValueKey);
        }
    }
}
