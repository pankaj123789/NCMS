using System;
using System.Collections.Generic;
using System.Threading;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Bl.Security;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl
{
    public class NcmsBackgroundTaskService : BaseBackgroundService<NcmsJobTypeName, IBackgroundTask>
    {
        private readonly ISecretsCacheQueryService _secretsProvider;

        public NcmsBackgroundTaskService(
            IUtilityQueryService utilityQueryService,
            IEmailMessageQueryService emailMessageQueryService,
            ISecretsCacheQueryService secretsProvider,
            IBackgroundTaskLogger backgroundTaskLogger) : base(utilityQueryService, emailMessageQueryService, backgroundTaskLogger)
        {
            _secretsProvider = secretsProvider;
        }
        protected override string BackgroundServiceName => "NCMS";

        protected override IReadOnlyDictionary<NcmsJobTypeName, Func<IBackgroundTask>> GetTaskFactory()
        {
            return new Dictionary<NcmsJobTypeName, Func<IBackgroundTask>>
            {
                { NcmsJobTypeName.ProcessingPendingAccountingOperations, CreateTask<IAccountingOperationBackgroundTask> },
                { NcmsJobTypeName.ProcessingPendingApplications, CreateTask<IApplicationProcessBackgroundTask> },
                { NcmsJobTypeName.RefresNcmsSystemCache, CreateTask<INcmsRefreshSystemCacheTask> },
                { NcmsJobTypeName.SendCandidateBriefs, CreateTask<ICandidateBriefBackgroundTask> },
                { NcmsJobTypeName.IssueTestResultsAndCredentials, CreateTask<IIssueResultsAndCredentialsBackgroundTask> },
                { NcmsJobTypeName.ExecuteNcmsReports, CreateTask<IProcessNcmsReportsBackgroundTask> },
                { NcmsJobTypeName.SyncNcmsReportLogs, CreateTask<INcmsReportsLogSyncBackgroundTask> },
                { NcmsJobTypeName.ProcessPendingEmails, CreateTask<IEmailSendBackgroundTask> },
                { NcmsJobTypeName.DeleteTemporaryFiles, CreateTask<ITemporaryFileDeletionBackgroundTask> },
                { NcmsJobTypeName.DeletePodDataHistory, CreateTask<IDeleteOldSystemDataBackgroundTask> },
                { NcmsJobTypeName.RefreshNcmsCookieCache, CreateTask<INcmsRefreshCookieTask> },
                { NcmsJobTypeName.RefreshNcmsAllUsersCache, CreateTask<INcmsRefreshAllUserCacheTask> },
                { NcmsJobTypeName.RefreshNcmsPendingUsers, CreateTask<INcmsRefreshPendingUsersTask> },
                { NcmsJobTypeName.SendRecertificationReminder, CreateTask<IApplicationSendRecertificationReminderTask> },
                { NcmsJobTypeName.SendTestSessionReminder, CreateTask<IApplicationSendTestSessionReminderTask> },
                { NcmsJobTypeName.SendTestSessionAvailabilityNotice, CreateTask<IApplicationSendTestSessionAvailabilityNoticeTask> },
                { NcmsJobTypeName.ProcessNcmsRefund, CreateTask<IApplicationProcessRefundBackgroundTask> },
                { NcmsJobTypeName.RefreshNcmsNotifications, CreateTask<INcmsRefreshNotificationsTask> },
                { NcmsJobTypeName.DeleteNcmsBankDetails, CreateTask<INcmsDeleteBankDetailsTask> },
                //{ NcmsJobTypeName.DownloadNcmsTestAssets, CreateTask<IApplicationDownloadTestAssetsTask> },
                //{ NcmsJobTypeName.CreateNcmsTelevicUsers, CreateTask<ICreateTelevicUsersTask> },
                { NcmsJobTypeName.UtilityBackgroundJob, CreateTask<IUtilityBackgroundTask> },
                { NcmsJobTypeName.SendTestSittingsWithoutMaterialReminder, CreateTask<ISendTestSittingsWithoutMaterialReminderTask> },
                { NcmsJobTypeName.ProcessFileDeletesPastExpiryDate, CreateTask<IProcessFileDeletesPastExpiryDateTask> },
                { NcmsJobTypeName.ProcessFileDeletesHardDelete, CreateTask<IProcessFileDeletesHardDeleteTask> },
            };
        }

        protected override IBackgroundTask CreateTask<T>()
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