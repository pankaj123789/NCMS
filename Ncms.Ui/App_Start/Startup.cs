using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Wiise;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Ncms.Bl.Security;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using Ncms.Ui;
using Ncms.Ui.Security;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Ncms.Ui
{
    public class Startup
    {
        
        public void Configuration(IAppBuilder app)
        {
            //LoggingHelper.LogInfo("MapSignalR...");

            //app.MapSignalR();
            GlobalHost.HubPipeline.RequireAuthentication();

            LoggingHelper.LogInfo("Loading cache...");
            LoadCache();

            LoggingHelper.LogInfo("Configuring hangfire...");
            ConfigureHangFire(app);
            LoggingHelper.LogInfo("Configuring Authentication builder...");
            MicrosoftAuthenticationBuilder.Configure(app);
            LoggingHelper.LogInfo("Configuring paypal...");
            ConfigurePayPal();
            ConfigureWiise();


            LoggingHelper.LogInfo("Startup Configuration finished.");
        }

        private void LoadCache()
        {
            ServiceLocator.Resolve<INcmsRefreshSystemCacheTask>().RefreshLocalSystemCache();
            ServiceLocator.Resolve<INcmsRefreshCookieTask>().RefreshAllInvalidLocalCookies();
            ServiceLocator.Resolve<INcmsRefreshAllUserCacheTask>().RefreshAllUsersLocalCache();
            ServiceLocator.Resolve<INcmsUserRefreshCacheQueryService>().RefreshAllCache();
        }

        private void ConfigurePayPal()
        {
            var secretsProvider = DependencyResolver.Current.GetService<ISecretsCacheQueryService>();

            var PayPalMode = ConfigurationManager.AppSettings["PayPalMode"];
            var PayPalConnectionTimeout = ConfigurationManager.AppSettings["PayPalConnectionTimeout"];
            var PayPalRequestRetries = ConfigurationManager.AppSettings["PayPalRequestRetries"];
            var PayPalClientId = secretsProvider.Get(SecuritySettings.PayPalClientId); 
            var PayPalClientSecret = secretsProvider.Get(SecuritySettings.PayPalClientSecret);
            LoggingHelper.LogInfo($"PayPal NCMS Configure - Mode {PayPalMode}");
            PayPalConfigurationHelper.ConfigurePayPal(PayPalMode, PayPalConnectionTimeout, PayPalRequestRetries, PayPalClientId, PayPalClientSecret);
        }

        private void ConfigureWiise()
        {
            var secretsProvider = DependencyResolver.Current.GetService<ISecretsCacheQueryService>();
            var WiiseBaseUrl = ConfigurationManager.AppSettings["WiiseBaseUrl"];
            var WiiseResource = ConfigurationManager.AppSettings["WiiseResource"];
            var WiiseAuthRedirectUri = ConfigurationManager.AppSettings["WiiseAuthRedirectUri"];
            var WiiseAuthClientId = secretsProvider.Get(SecuritySettings.WiiseAuthClientId);
            var WiiseClientSecret = secretsProvider.Get(SecuritySettings.WiiseClientSecret);
            if (string.IsNullOrEmpty(WiiseBaseUrl))
            {
                LoggingHelper.LogError($"Wiise NCMS Configure - BaseUrl Failed to read values{WiiseBaseUrl}");
                LoggingHelper.LogError($"Wiise NCMS Configure - WiiseResource Failed to read values{WiiseResource}");
                LoggingHelper.LogError($"Wiise NCMS Configure - WiiseAuthRedirectUri Failed to read values{WiiseAuthRedirectUri}");
                LoggingHelper.LogError($"Wiise NCMS Configure - WiiseAuthClientId Failed to read values{WiiseAuthClientId}");
                LoggingHelper.LogError($"Wiise NCMS Configure - WiiseClientSecret Failed to read values{WiiseClientSecret}");
            }
            else
            {
                LoggingHelper.LogInfo($"Wiise NCMS Configure - BaseUrl {WiiseBaseUrl}");
                var wiiseAccountingApi = DependencyResolver.Current.GetService<IWiiseAccountingApi>();
                wiiseAccountingApi.ConfigureWiise(WiiseBaseUrl, WiiseResource, WiiseAuthRedirectUri, WiiseAuthClientId, WiiseClientSecret);
            }

        }

        /// <summary>
        /// HangFire.Console : Copyright (c) 2016 Alexey Skalozub
        /// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
        /// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
        /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        /// </summary>
        public static void ConfigureHangFire(IAppBuilder app)
        {
            LoggingHelper.LogInfo("Configuring Hangfire");

            try
            {
                var userName = ServiceLocator.Resolve<ISecretsCacheQueryService>().Get(SecuritySettings.NcmsDefaultIdentityKey);

                //Todo: fix this
                var principal = new NcmsPrincipal(userName);

                var secretsProvider = DependencyResolver.Current.GetService<ISecretsCacheQueryService>();
                var lifeCycleService = DependencyResolver.Current.GetService<ILifecycleService>();
                var serverName = lifeCycleService.GetSystemIp();
                var connectionString = secretsProvider.Get("ConnectionString");
                GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);
                GlobalConfiguration.Configuration.UseConsole();

                app.UseHangfireDashboard("/DashBoard", new DashboardOptions()
                {
                    Authorization = new[] { new NcmsHangfireAuthorizationFilter() },
                    DashboardTitle = Naati.Resources.Shared.NcmsDashBoard,
                    IsReadOnlyFunc = x => false,
                });

                var serverOptions = new BackgroundJobServerOptions
                {
                    ServerName = serverName,
                };
                app.UseHangfireServer(serverOptions);

                var cronApplications = GetSystemValue("ApplicationsCheckingInterval");
                var cronWiiseOperations = GetSystemValue("WiiseOperationsCheckingInterval");
                var cronCandidateBriefs = GetSystemValue("SendCandidateBriefsCheckingInterval");
                var cronIssueTestResults = GetSystemValue("IssueTestResultsAndCredentialsCheckingInterval");
                var cronProcessPendingEmails = GetSystemValue("ProcessPendingEmailsInterval");
                var cronDeleteTemporaryFiles = GetSystemValue("DeleteTemporaryFilesInterval");
                var cronDeletePodDataHistory = GetSystemValue("DeletePodDataHistoryInterval");
                var cronSyncReportLogs = GetSystemValue("ProcessSyncReportLogsInterval");
                var refreshCookieCache = GetSystemValue("RefreshNcmsCookieCacheInterval");
                var sendRecertificationReminder = GetSystemValue("SendRecertificationReminderInterval");
                var sendTestSessionReminder = GetSystemValue("SendTestSessionReminderInterval");
                var sendTestSessionAvailabilityNotice = GetSystemValue("SendTestSessionAvailabilityNoticeInterval");
                var processNcmsRefund = GetSystemValue("ProcessNcmsRefundInterval");
                var refreshNotifications = GetSystemValue("RefreshNcmsNotificationsInterval");
                var deleteBankDetails = GetSystemValue("DeleteNcmsBankDetailsInterval");
                var sendTestSittingsWithoutMaterialReminder = GetSystemValue("TestSittingsWithoutMaterialReminderInterval");
                var processFileDeletesPastExpiryDate = GetSystemValue("ProcessFileDeletesPastExpiryDateInterval");
                var processFileDeletesHardDelete = GetSystemValue("ProcessFileDeletesHardDeleteInterval");

                string desiredTimeZone = ConfigurationManager.AppSettings["DesiredTimeZone"];

                //this is to expose the config value to Hangfire because it doesnt have an HttpContext
                LoggingHelper.LogInfo("Configuring testSittingsWithoutMaterialReminderEmailAddresses");
                var testSittingsWithoutMaterialReminderEmailAddresses = ConfigurationManager.AppSettings["TestSittingsWithoutMaterialReminderEmailAddresses"];
                if (testSittingsWithoutMaterialReminderEmailAddresses != null)
                {
                    SetSystemValue("TestSittingsWithoutMaterialReminderEmailAddresses", testSittingsWithoutMaterialReminderEmailAddresses);
                }
                else
                {
                    LoggingHelper.LogInfo($"Configuring testSittingsWithoutMaterialReminderEmailAddresses failed");
                }

                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(desiredTimeZone);
                RecurringJob.AddOrUpdate(() => ProcessingPendingAccountingOperations(null), cronWiiseOperations, timeZone);
                RecurringJob.AddOrUpdate(() => ProcessPendingApplications(null), cronApplications, timeZone);
                RecurringJob.AddOrUpdate(() => SendCandidateBriefs(null), cronCandidateBriefs, timeZone);
                RecurringJob.AddOrUpdate(() => IssueTestResultsAndCredentials(null), cronIssueTestResults, timeZone);
                RecurringJob.AddOrUpdate(() => ProcessPendingEmailsOperations(null), cronProcessPendingEmails, timeZone);
                RecurringJob.AddOrUpdate(() => ProcessSyncReportLogs(null), cronSyncReportLogs, timeZone);
                RecurringJob.AddOrUpdate(() => DeleteTemporaryFiles(null), cronDeleteTemporaryFiles, timeZone);
                RecurringJob.AddOrUpdate(() => DeletePodDataHistory(null), cronDeletePodDataHistory, timeZone);
                RecurringJob.AddOrUpdate(() => RefreshCookieCache(null), refreshCookieCache, timeZone);
                RecurringJob.AddOrUpdate(() => SendRecertificationReminder(null), sendRecertificationReminder, timeZone);
                RecurringJob.AddOrUpdate(() => SendTestSessionReminder(null), sendTestSessionReminder, timeZone);
                RecurringJob.AddOrUpdate(() => SendTestSessionAvailabilityNotice(null), sendTestSessionAvailabilityNotice, timeZone);
                RecurringJob.AddOrUpdate(() => ProcessNcmsRefund(null), processNcmsRefund, timeZone);
                RecurringJob.AddOrUpdate(() => RefreshNotifications(null), refreshNotifications, timeZone);
                RecurringJob.AddOrUpdate(() => DeleteBankDetails(null), deleteBankDetails, timeZone);
                //RecurringJob.AddOrUpdate(() => UtilityBackgroundJob(null), utilityBackgroundJob, timeZone);
                //RecurringJob.AddOrUpdate(() => DownloadTestAssets(null), downloadTestAssets, timeZone);
                //RecurringJob.AddOrUpdate(() => CreateTelevicUsers(null), createTelevicUsers, timeZone);
                RecurringJob.AddOrUpdate(() => SendTestSittingsWithoutMaterialReminder(null), sendTestSittingsWithoutMaterialReminder, timeZone);
                RecurringJob.AddOrUpdate(() => ProcessFileDeletesPastExpiryDate(null), processFileDeletesPastExpiryDate, timeZone);
                RecurringJob.AddOrUpdate(() => ProcessFileDeletesHardDelete(null), processFileDeletesHardDelete, timeZone);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex);
                throw;
            }
        }
        public static void SetContext()
        {
            var defaultUrl = ConfigurationManager.AppSettings["DefaultAppLocalUrl"];
            HttpContext.Current = new HttpContext(new HttpRequest(string.Empty, defaultUrl, string.Empty), new HttpResponse(new StringWriter()));
        }

        public static void ExecuteTask(NcmsJobTypeName jobTypeName, PerformContext context, bool multiServer, bool allowDisable = true, IDictionary<string, string> parameters = null)
        {
            SetContext();
            ServiceLocator.Resolve<IBackgroundTaskService<NcmsJobTypeName>>().ExecuteTask(jobTypeName, context, multiServer, allowDisable, parameters);
        }

        [JobDisplayName("Process Emails")]
        public static void ProcessPendingEmailsOperations(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.ProcessPendingEmails, context, false);
        }
        [JobDisplayName("Sync Report Logs")]
        public static void ProcessSyncReportLogs(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.SyncNcmsReportLogs, context, false);
        }

        [JobDisplayName("Refresh Cookie Cache")]
        public static void RefreshCookieCache(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.RefreshNcmsCookieCache, context, false);
        }

        [JobDisplayName("Send Recertification Reminders")]
        public static void SendRecertificationReminder(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.SendRecertificationReminder, context, false);
        }

        [JobDisplayName("Send Test Session Reminders")]
        public static void SendTestSessionReminder(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.SendTestSessionReminder, context, false);
        }

        [JobDisplayName("Send Test Session Availability Notices")]
        public static void SendTestSessionAvailabilityNotice(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.SendTestSessionAvailabilityNotice, context, false);
        }

        [JobDisplayName("Process Reports")]
        public static void ProcessExecuteNcmsReports(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.ExecuteNcmsReports, context, false);
        }

        [JobDisplayName("Process invoices")]
        public static void ProcessingPendingAccountingOperations(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.ProcessingPendingAccountingOperations, context, false);
        }

        [JobDisplayName("Process Applications")]
        public static void ProcessPendingApplications(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.ProcessingPendingApplications, context, false);
        }

        [JobDisplayName("Send Candidate Briefs")]
        public static void SendCandidateBriefs(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.SendCandidateBriefs, context, false);
        }

        [JobDisplayName("Issue Test Results and Credentials")]
        public static void IssueTestResultsAndCredentials(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.IssueTestResultsAndCredentials, context, false);
        }

        [JobDisplayName("Delete Temporary Files")]
        public static void DeleteTemporaryFiles(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.DeleteTemporaryFiles, context, false);
        }

        [JobDisplayName("Delete System Data History")]
        public static void DeletePodDataHistory(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.DeletePodDataHistory, context, false);
        }

        [JobDisplayName("Process refund requests")]
        public static void ProcessNcmsRefund(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.ProcessNcmsRefund, context, false);
        }

        [JobDisplayName("Refresh Notifications")]
        public static void RefreshNotifications(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.RefreshNcmsNotifications, context, false);
        }

        [JobDisplayName("Delete Bank Details")]
        public static void DeleteBankDetails(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.DeleteNcmsBankDetails, context, false);
        }

        //[JobDisplayName("Download Test Assets")]
        //public static void DownloadTestAssets(PerformContext context)
        //{
        //    ExecuteTask(NcmsJobTypeName.DownloadNcmsTestAssets, context, false);
        //}

        //[JobDisplayName("Create Televic Users")]
        //public static void CreateTelevicUsers(PerformContext context)
        //{
        //    ExecuteTask(NcmsJobTypeName.CreateNcmsTelevicUsers, context, false);
        //}

        [JobDisplayName("Utility Background Job")]
        public static void UtilityBackgroundJob(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.UtilityBackgroundJob, context, false);
        }

        
        [JobDisplayName("Send TestSittings Without Material Reminder")]
        public static void SendTestSittingsWithoutMaterialReminder(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.SendTestSittingsWithoutMaterialReminder, context, false);
        }

        [JobDisplayName("Process File Deletes Past Expiry Date")]
        public static void ProcessFileDeletesPastExpiryDate(PerformContext context)
        {
            var userName = ServiceLocator.Resolve<ISecretsCacheQueryService>().Get(SecuritySettings.NcmsDefaultIdentityKey);

            ExecuteTask(NcmsJobTypeName.ProcessFileDeletesPastExpiryDate, context, false, parameters: new Dictionary<string, string>() { { "UserName", userName } });
        }

        [JobDisplayName("Process File Deletes Hard Delete")]
        public static void ProcessFileDeletesHardDelete(PerformContext context)
        {
            var userName = ServiceLocator.Resolve<ISecretsCacheQueryService>().Get(SecuritySettings.NcmsDefaultIdentityKey);

            ExecuteTask(NcmsJobTypeName.ProcessFileDeletesHardDelete, context, false, parameters: new Dictionary<string, string>() { { "UserName", userName } });
        }
        public static void ClearCache()
        {
            Enqueue(() => ClearCache(null));
        } 
        
        public static void RefreshUsersCache()
        {
            Enqueue(() => RefreshUsersCache(null));
        }

        public static void SendRecertificationReminders()
        {
            Enqueue(() => SendRecertificationReminder(null));
        }

        public static void SendTestSessionReminders()
        {
            Enqueue(() => SendTestSessionReminder(null));
        }

        public static void SendTestSessionAvailabilityNotices()
        {
            Enqueue(() => SendTestSessionAvailabilityNotice(null));
        }

        public static void RefreshCookies()
        {
            Enqueue(() => RefreshCookieCache(null));
        }

        public static void DeleteTemporaryFiles()
        {
            Enqueue(() => DeleteTemporaryFiles(null));
        }
        public static void DeletePodDataHistory()
        {
            Enqueue(() => DeletePodDataHistory(null));
        }

        public static void SyncNcmsReportLogs()
        {
            Enqueue(() => ProcessSyncReportLogs(null));
        }

        public static void ExecuteNcmsReports()
        {
            Enqueue(() => ProcessExecuteNcmsReports(null));
        }

        public static void ExecuteProcessRefund()
        {
            Enqueue(() => ProcessNcmsRefund(null));
        }

        public static void ExecuteRefreshNotifications()
        {
            Enqueue(() => RefreshNotifications(null));
        }

        public static void ExecuteDeleteBankDetails()
        {
            Enqueue(() => DeleteBankDetails(null));
        }

        public static void ExecuteSendTestMaterialReminders()
        {
            Enqueue(() => SendTestSittingsWithoutMaterialReminder(null));
        }

        public static void ExecuteProcessFileDeletesPastExpiryDate()
        {
            Enqueue(() => ProcessFileDeletesPastExpiryDate(null));
        }

        //public static void ExecuteDownloadTestAssets()
        //{
        //    Enqueue(() => DownloadTestAssets(null));
        //}

        //public static void ExecuteCreateTelevicUsers()
        //{
        //    Enqueue(() => CreateTelevicUsers(null));
        //}

        private static void Enqueue(Expression<Action> methodCall)
        {
            BackgroundJob.Enqueue(methodCall);
        }

        public static void ForceProcessApplications()
        {
            Enqueue(() => ProcessingPendingAccountingOperations(null));
            Thread.Sleep(15000); // Delay to wait the first job to finish
            Enqueue(() => ProcessPendingApplications(null));
            Thread.Sleep(10000);
        }

        public static void ForceSendCandidateBriefs()
        {
            Enqueue(() => SendCandidateBriefs(null));
        }

        public static void ForceIssueTestResultsAndCredentials()
        {
            Enqueue(() => IssueTestResultsAndCredentials(null));
        }

        public static void ForceSendPendingEmails()
        {
            Enqueue(() => ProcessPendingEmailsOperations(null));
        }

        [JobDisplayName("Clear System Cache")]
        public static void ClearCache(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.RefresNcmsSystemCache, context, false);
        }

        [JobDisplayName("Refresh All User Cache")]
        public static void RefreshUsersCache(PerformContext context)
        {
            ExecuteTask(NcmsJobTypeName.RefreshNcmsAllUsersCache, context, false);
        }

        [JobDisplayName("Refresh Pending Users Cache")]
        public static void RefreshPendingUsersCache(PerformContext context, Dictionary<string, string> parameters)
        {
            ExecuteTask(NcmsJobTypeName.RefreshNcmsPendingUsers, context, true, allowDisable:false, parameters: parameters);
        }

        public static void RefreshUserCache(string userName, string invalidCookie, DateTime expiryDate)
        {
            var taskService = ServiceLocator.Resolve<INcmsRefreshPendingUsersTask>();
            var jobDelaySeconds = taskService.RegisterUserCacheRefresh(userName, invalidCookie, expiryDate);
            if (jobDelaySeconds > 0)
            {
                var lifeCycleService = DependencyResolver.Current.GetService<ILifecycleService>();
                var serverName = lifeCycleService.GetSystemIp();
               
                var parameters = new Dictionary<string, string>() { { BackgroundTasksParameters.ServerName, serverName } };
                Schedule(() => RefreshPendingUsersCache(null, parameters), TimeSpan.FromSeconds(jobDelaySeconds));
            }
        }

        private static void Schedule(Expression<Action> methodCall, TimeSpan timeSpan)
        {
            BackgroundJob.Schedule(methodCall, timeSpan);
        }

        private static string GetSystemValue(string systemValueKey)
        {
            SetContext();
            var value = ServiceLocator.Resolve<ISystemService>().GetSystemValue(systemValueKey);
            if (string.IsNullOrWhiteSpace(value))
            {
                LoggingHelper.LogWarning($"System value {systemValueKey} does not have values ");
            }

            return value;
        }

        private static void SetSystemValue(string systemValueKey, string systemValue)
        {
            SetContext();
            ServiceLocator.Resolve<ISystemService>().SetSystemValue(systemValueKey, systemValue);
        }
    }
}
