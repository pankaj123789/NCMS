using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;

namespace Ncms.Contracts.BackgroundTask
{
    public interface IAccountingOperationBackgroundTask : IBackgroundTask { }
    public interface IApplicationProcessBackgroundTask : IBackgroundTask { }
    public interface ICandidateBriefBackgroundTask : IBackgroundTask { }
    public interface IEmailSendBackgroundTask : IBackgroundTask { }
    public interface INcmsDeleteBankDetailsTask : IBackgroundTask { }
    //public interface ICreateTelevicUsersTask : IBackgroundTask { }
    public interface IIssueResultsAndCredentialsBackgroundTask : IBackgroundTask { }
    public interface IApplicationSendRecertificationReminderTask : IBackgroundTask { }
    public interface IApplicationSendTestSessionReminderTask : IBackgroundTask { }
    public interface IApplicationSendTestSessionAvailabilityNoticeTask : IBackgroundTask { }
    public interface IApplicationProcessRefundBackgroundTask : IBackgroundTask { }
    //public interface IApplicationDownloadTestAssetsTask : IBackgroundTask { }
    /// <summary>
    /// Send out reminders if Test Material has not been allocated within 2 weeks of session start
    /// </summary>
    public interface ISendTestSittingsWithoutMaterialReminderTask : IBackgroundTask { }

    public interface INcmsRefreshNotificationsTask : IBackgroundTask
    {
        void RefreshLocalUserNotifications(IEnumerable<string> userNames);
    }

    public interface INcmsRefreshSystemCacheTask : IBackgroundTask
    {
        void RefreshLocalSystemCache();
    }

    public interface INcmsRefreshPendingUsersTask : IBackgroundTask
    {
        int RegisterUserCacheRefresh(string userName, string invalidCookie, DateTime cookieExpiryDate);
        int RegisterHubNotification(string userName, int notificationId);
        void RefreshLocalUsers(IEnumerable<NcmsUserRefreshDto> users);
    }

    public interface INcmsRefreshAllUserCacheTask : IBackgroundTask
    {
        void RefreshAllUsersLocalCache();
       
    }

    public interface INcmsRefreshCookieTask : IBackgroundTask
    {
        void RefreshAllInvalidLocalCookies();
    }

  
    public interface INcmsReportsLogSyncBackgroundTask : IBackgroundTask { }
    public interface IProcessNcmsReportsBackgroundTask : IBackgroundTask { }
    public interface ITemporaryFileDeletionBackgroundTask : IBackgroundTask { }
    public interface IDeleteOldSystemDataBackgroundTask : IBackgroundTask { }
    public interface IProcessFileDeletesPastExpiryDateTask : IBackgroundTask { }
    public interface IProcessFileDeletesHardDeleteTask : IBackgroundTask { }


    public interface IUtilityBackgroundTask : IBackgroundTask { }

    public enum NcmsJobTypeName
    {
        ProcessingPendingAccountingOperations,
        ProcessingPendingApplications,
        RefresNcmsSystemCache,
        SendCandidateBriefs,
        IssueTestResultsAndCredentials,
        ProcessPendingEmails,
        ExecuteNcmsReports,
        SyncNcmsReportLogs,
        DeleteTemporaryFiles,
        DeletePodDataHistory,
        RefreshNcmsAllUsersCache,
        RefreshNcmsCookieCache,
        RefreshNcmsPendingUsers,
        SendRecertificationReminder,
        SendTestSessionReminder,
        SendTestSessionAvailabilityNotice,
        ProcessNcmsRefund,
        RefreshNcmsNotifications,
        DownloadNcmsTestAssets,
        CreateNcmsTelevicUsers,
        DeleteNcmsBankDetails,
        UtilityBackgroundJob,
        SendTestSittingsWithoutMaterialReminder,
        ProcessFileDeletesPastExpiryDate,
        ProcessFileDeletesHardDelete
    }

    public static class BackgroundTasksParameters
    {
        public const string ServerName = "ServerName";
    }


}
