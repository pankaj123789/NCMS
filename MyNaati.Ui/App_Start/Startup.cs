using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Wiise;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Hangfire.SqlServer;
using Microsoft.Owin;
using MyNaati.Bl.Portal;
using MyNaati.Contracts.BackgroundTask;
using MyNaati.Ui.Security;
using Owin;

[assembly: OwinStartup(typeof(MyNaati.Ui.Startup))]
namespace MyNaati.Ui
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LoggingHelper.LogInfo("Loading cache...");
            LoadCache();
            LoggingHelper.LogInfo("Configure hangfire...");
            ConfigureHangFire(app);
            LoggingHelper.LogInfo("Configuring paypal...");
            ConfigurePayPal();
            ConfigureWiise();
        }

        private void LoadCache()
        {
            ServiceLocator.Resolve<IMyNaatiRefreshSystemCacheTask>().RefreshLocalSystemCache();
            ServiceLocator.Resolve<IMyNaatiRefreshCookieTask>().RefreshAllInvalidLocalCookies();
            ServiceLocator.Resolve<IMyNaatiRefreshAllUsersTask>().RefreshAllUsersLocalCache();
            ServiceLocator.Resolve<IMyNaatiUserRefreshCacheQueryService>().RefreshAllCache();
        }
        /// <summary>
        /// HangFire.Console : Copyright (c) 2016 Alexey Skalozub
        /// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
        /// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
        /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        /// </summary>
        public static void ConfigureHangFire(IAppBuilder app)
        {
            LoggingHelper.LogInfo("Configuring hangfire...");

            try
            {
                var secretsProvider = DependencyResolver.Current.GetService<ISecretsCacheQueryService>();
                var lifeCycleService = DependencyResolver.Current.GetService<ILifecycleService>();
                var serverName = lifeCycleService.GetSystemIp();
                var connectionString = secretsProvider.Get("ConnectionString");
                var sqlServerOptions = new SqlServerStorageOptions
                {
                    SchemaName = "MyNaatiHangFire"
                };
                GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString, sqlServerOptions);
                GlobalConfiguration.Configuration.UseConsole();

                app.UseHangfireDashboard("/DashBoard", new DashboardOptions()
                {
                    IsReadOnlyFunc = x => false,
                    DashboardTitle = "MyNAATI Dashboard",
                    Authorization = new[] { new MyNaatiHangfireAuthorizationFilter() },
                });

                var serverOptions = new BackgroundJobServerOptions
                {
                    ServerName = serverName,
                };
                app.UseHangfireServer(serverOptions);

                var refreshSystemCacheInterval = GetSystemValue("MyNaatiRefreshSystemCacheInterval");
                var refreshCookieCache = GetSystemValue("MyNaatiRefreshCookieCacheInterval");
                var refreshUsersCache = GetSystemValue("MyNaatiRefreshAllUsersCacheInterval");

                string desiredTimeZone = ConfigurationManager.AppSettings["DesiredTimeZone"];

                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(desiredTimeZone);
                RecurringJob.AddOrUpdate(() => RefreshSystemCache(null), refreshSystemCacheInterval, timeZone);
                RecurringJob.AddOrUpdate(() => RefreshCookieCache(null), refreshCookieCache, timeZone);
                RecurringJob.AddOrUpdate(() => RefreshUsersCache(null), refreshUsersCache, timeZone);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex);
                throw;
            }
        }

        private void ConfigurePayPal()
        {
            var secretsProvider = DependencyResolver.Current.GetService<ISecretsCacheQueryService>();

            var PayPalMode = ConfigurationManager.AppSettings["PayPalMode"];
            var PayPalConnectionTimeout = ConfigurationManager.AppSettings["PayPalConnectionTimeout"];
            var PayPalRequestRetries = ConfigurationManager.AppSettings["PayPalRequestRetries"];
            var PayPalClientId = secretsProvider.Get("PayPalClientId");
            var PayPalClientSecret = secretsProvider.Get("PayPalClientSecret");
            LoggingHelper.LogInfo($"PayPal MyNaati Configure - Mode {PayPalMode}");
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

        [JobDisplayName("Refresh MyNaati System Cache")]
        public static void RefreshSystemCache(PerformContext context)
        {
            ExecuteTask(MyNaatiJobTypeName.MyNaatiRefreshSystemCache, context, false);
        }

        [JobDisplayName("Refresh Invalid Cookies Cache")]
        public static void RefreshCookieCache(PerformContext context)
        {
            ExecuteTask(MyNaatiJobTypeName.MyNaatiRefreshCookieCache, context, false);
        }

        public static void RefreshUserCache(int naatiNumber, string invalidCookie, DateTime expiryDate)
        {
            var taskService = ServiceLocator.Resolve<IMyNaatiRefreshPendingUsersTask>();
            var jobDelaySeconds = taskService.RegisterUserCacheRefresh(naatiNumber, invalidCookie, expiryDate);
            if (jobDelaySeconds >0)
            {
                var lifeCycleService = DependencyResolver.Current.GetService<ILifecycleService>();
                var serverName = lifeCycleService.GetSystemIp();
                
                var parameters = new Dictionary<string, string>() { { BackgroundTasksParameters.ServerName, serverName } };
                Schedule(() => RefreshPendingUsersCache( null, parameters), TimeSpan.FromSeconds(jobDelaySeconds));
            }
        }

        private static void Schedule(Expression<Action> methodCall, TimeSpan timeSpan)
        {
            BackgroundJob.Schedule(methodCall, timeSpan);
        }

        [JobDisplayName("Refresh Pending Users Cache")]
        public static void RefreshPendingUsersCache(PerformContext context, IDictionary<string, string> parameters)
        {
            ExecuteTask(MyNaatiJobTypeName.MyNaatiRefreshPendingUsersCache, context, true, false, parameters);
        }

        [JobDisplayName("Refresh All Users Cache")]
        public static void RefreshUsersCache(PerformContext context)
        {
            ExecuteTask(MyNaatiJobTypeName.MyNaatiRefreshAllUsersCache, context, false);
        }

        public static void ExecuteTask(MyNaatiJobTypeName jobTypeName, PerformContext context, bool multiServer, bool allowDisable = true, IDictionary<string, string> parameters = null)
        {
            SetContext();
            ServiceLocator.Resolve<IBackgroundTaskService<MyNaatiJobTypeName>>().ExecuteTask(jobTypeName, context, multiServer, allowDisable, parameters);
        }

        public static void SetContext()
        {
            var defaultUrl = ConfigurationManager.AppSettings["DefaultAppLocalUrl"];
            HttpContext.Current = new HttpContext(new HttpRequest(string.Empty, defaultUrl, string.Empty), new HttpResponse(new StringWriter()));
        }
        private static string GetSystemValue(string systemValueKey)
        {
            SetContext();
            var value = ServiceLocator.Resolve<ConfigurationService>().GetSystemValue(systemValueKey);
            if (string.IsNullOrWhiteSpace(value))
            {
                LoggingHelper.LogWarning($"System value {systemValueKey} does not have values ");
            }

            return value;
        }
    }
}
