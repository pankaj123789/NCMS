using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Global.Common.SystemLifecycle;
using F1Solutions.Naati.Common.Bl.AutoMappingProfiles;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValues;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.Nhibernate.Mappings;
using F1Solutions.Naati.Common.Dal.NHibernate.SharpArchitecture;
using Microsoft.Win32;
using Ncms.Bl;
using Ncms.Bl.Security;
using Ncms.Contracts;
using Ncms.Ui.App_Start.Security;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Dal.AutoMappingProfiles;
using Ncms.Bl.AutoMappingProfiles;
using Microsoft.AspNet.Identity;
using F1Solutions.Global.Common;

namespace Ncms.Ui
{
    public class WebApiApplication : HttpApplication
    {

        private WebSessionStorage mWebSessionStorage;



        public WebApiApplication()
        {
            mWebSessionStorage = new WebSessionStorage(this);
        }
        public const string HIBERNATE_CONNECTION_STRING = "connection.connection_string";
        public const string WEB_CONFIG_CONNECTION_STRING = "CONNECTION_STRING";
        protected void Application_Start()
        {

            //this.BeginRequest += OnBeginRequest;

            string loggingDiagnosticsFileName = ConfigurationManager.AppSettings["logging:DiagnosticsLogFile"];

            //Todo iMPROVE THIS
            var machineName = System.Net.Dns.GetHostEntry("").HostName;
            var loggingDiagnosticsFile = Path.Combine(ConfigurationManager.AppSettings["applicationData"], machineName, loggingDiagnosticsFileName);
            var bufferFileBase = Path.Combine(ConfigurationManager.AppSettings["applicationData"], machineName, ConfigurationManager.AppSettings["logging:SeqLogBufferFileBase"]);

            var logFile = Path.Combine(ConfigurationManager.AppSettings["applicationData"], machineName, ConfigurationManager.AppSettings["logging:LogFile"]);

            CheckOrCreateFile(loggingDiagnosticsFile);
            CheckOrCreateFile(bufferFileBase);
            CheckOrCreateFile(logFile);

            LoggingHelper.ConfigureLogging("NAATI", "NCMS",
                bufferFileBase, logFile,
                String.IsNullOrEmpty(loggingDiagnosticsFileName) ? null : loggingDiagnosticsFile, GetDatabase());
            LoggingHelper.LogInfo("Validating System Time Zone...");
            SystemLifecycleHelper.ValidateSystemTimeZone();
            LoggingHelper.LogInfo("InitializeNHibernateOnce...");
            NHibernateInitializer.Instance().InitializeNHibernateOnce(InitializeNHibernateConfiguration);
            LoggingHelper.LogInfo("AddProfiles...");
            AddProfiles();
            LoggingHelper.LogInfo("Log System started date...");
            LogSystemStartedDate();
            LoggingHelper.LogInfo("SecurityProtocol...");
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            LoggingHelper.LogInfo("RegisterAllAreas...");
            AreaRegistration.RegisterAllAreas();
            LoggingHelper.LogInfo("Configure WebApiConfig...");
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Filters.Add(new HtmlEncodeAttribute());
            LoggingHelper.LogInfo("RegisterGlobalFilters...");
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            LoggingHelper.LogInfo("RegisterRoutes...");
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            LoggingHelper.LogInfo("RegisterBundles...");
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            LoggingHelper.LogInfo("Setting ContractResolver...");
            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CustomContractResolver(ServiceLocator.Resolve<ILookupTypeConverterHelper>);

            LoggingHelper.LogInfo("InitialiseApplication...");
            InitialiseApplication();
        }

        private bool IsSignalrRequest(string path)
        {
            return path.IndexOf("/signalr/", StringComparison.OrdinalIgnoreCase) > -1;
        }

        protected void Application_PreSendRequestHeaders()
        {
            var httpContext = Context;
            if (IsSignalrRequest(httpContext.Request.Path))
            {
                // Remove auth cooke to avoid sliding expiration renew
                httpContext.Response.Cookies.Remove(DefaultAuthenticationTypes.ApplicationCookie);
            }
        }

        private static void LogSystemStartedDate()
        {
            var principal = new NcmsPrincipal(ServiceLocator.Resolve<ISecretsCacheQueryService>().Get(SecuritySettings.NcmsDefaultIdentityKey));
            Thread.CurrentPrincipal = principal;
            System.Threading.Thread.CurrentPrincipal = principal;
            var utilityQueryService = DependencyResolver.Current.GetService<IUtilityQueryService>();
            utilityQueryService.LogSystemStartDate();
        }

        private static string GetConnectionString()
        {
            var secretsProvider = DependencyResolver.Current.GetService<ISecretsCacheQueryService>();
            return secretsProvider.Get("ConnectionString");
        }

        private static string GetDatabase()
        {
            var connectionString = GetConnectionString();
            if (!String.IsNullOrEmpty(connectionString))
            {
                var parts = connectionString.Split(new[] { "Catalog=" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    return parts[1].Substring(0, parts[1].IndexOf(';'));
                }
            }
            return null;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            Application_BeginRequest();
        }

        private void CheckOrCreateFile(string filePath)
        {
            var directoryName = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }
        }
        private void InitialiseApplication()
        {
            try
            {
                RunDatabaseMigrations();
                SystemLifecycleHelper.UpdateLifecycleStatus(SystemLifecycleStatus.Running);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex);
                SystemLifecycleHelper.UpdateLifecycleStatus(SystemLifecycleStatus.Failed);
                throw;
            }
        }

        private void RunDatabaseMigrations()
        {
            LoggingHelper.LogInfo("Getting dbVersion..");
            var previousDbVersion = GetDbVersion();

            LoggingHelper.LogInfo("Checking migrations...");
            var databaseMigrationService = DependencyResolver.Current.GetService<IDatabaseMigrationService>();

            var retries = 30;
            bool migrated;
            bool error;
            var timeBetweenRetries = 10000;
            do
            {
                LoggingHelper.LogInfo($"Attempting to update db.. Pending retries: {retries}");
                var result = databaseMigrationService.MigrateDb();
                migrated = result.migrated;
                error = result.error;
                retries--;
                if (!migrated && !error && (retries >= 0))
                {
                    LoggingHelper.LogInfo($"Database migration token is being used.. Sleeping for {timeBetweenRetries} milliseconds");
                    Thread.Sleep(timeBetweenRetries);
                }
            }
            while (!migrated && !error && (retries >= 0));

            if (!migrated || error)
            {
                throw new Exception($"Impossible to migrate database.");
            }

            LoggingHelper.LogInfo("Getting new dbVersion...");

            var newDbVersion = GetDbVersion();
            if (newDbVersion != previousDbVersion)
            {
                LoggingHelper.LogInfo("Updating Keys...");
                UpdateApiKeys();
            }
        }

        private void UpdateApiKeys()
        {
            var principal = new NcmsPrincipal(ServiceLocator.Resolve<ISecretsCacheQueryService>().Get(SecuritySettings.NcmsDefaultIdentityKey));
            Thread.CurrentPrincipal = principal;
            System.Threading.Thread.CurrentPrincipal = principal;
            var systemValueService = DependencyResolver.Current.GetService<ISystemService>();
            UpdateKey(systemValueService, SecuritySettings.MyNaatiPrivateKeyValue);
            UpdateKey(systemValueService, SecuritySettings.MyNaatiPublicKeyValue);
            UpdateKey(systemValueService, SecuritySettings.NcmsPrivateKeyValue);
            UpdateKey(systemValueService, SecuritySettings.NcmsPublicKeyValue);
        }

        private void UpdateKey(ISystemService service, string systemValueKey)
        {
            var newKey = HmacCalculatorHelper.GenerateProtectedKey();
            service.SetSystemValue(systemValueKey, newKey);
        }

        private string GetDbVersion()
        {
            var service = ServiceLocator.Resolve<ISystemService>();
            var value = service.GetSystemValue("BuildVersion", true);
            return value;
        }
        protected void Application_BeginRequest()
        {
            //            LoggingHelper.LogWarning("Application_BeginRequest");
            //   NHibernateInitializer.Instance().InitializeNHibernateOnce(InitializeNHibernateConfiguration);
        }
     


        protected void InitializeNHibernateConfiguration()
        {
            NHibernateSession.Init(mWebSessionStorage, new[] { Server.MapPath("~/bin/F1Solutions.Naati.Common.Dal.Domain.dll") },
                new AutoPersistenceModelGenerator().Generate(), Server.MapPath("~/NHibernate.config"),
                GetAdditionalNHibernateProperties(), string.Empty, GetConnectionString());
        }

        private Dictionary<string, string> GetAdditionalNHibernateProperties()
        {
            var properties = new Dictionary<string, string>();

            // Set connection string based on value set in web.config
            properties[HIBERNATE_CONNECTION_STRING] = GetConnectionString();
            return properties;
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            if (String.IsNullOrEmpty(exception.Source))
            {
                exception.Source = "Application_Error";
            }
            LoggingHelper.LogException(exception);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // if a valid FormAuthenticationTicket cookie exists, set the current principal from it
            var cookie = HttpContext.Current.Request.Cookies["NCMS"]?.Value;

            if (cookie != null)
            {
                var formsTicket = FormsAuthentication.Decrypt(cookie);
                if (formsTicket != null && !formsTicket.Expired)
                {
                    if (!ServiceLocator.Resolve<ICookieQueryService>().IsInvalidCookie(cookie))
                    {
                        HttpContext.Current.User = new GenericPrincipal(new FormsIdentity(formsTicket), null);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    }
                }
            }
        }

        protected void Application_OnEnd(object sender, EventArgs e)
        {
            LoggingHelper.LogWarning($"NCMS Application Ending");
        }
       

        protected void Application_EndRequest()
        {
            if (Response.IsRequestBeingRedirected)
            {
                // with Forms Authentication, all pages are redirected to the login when there is no auth ticket cookie. if we're trying
                // to get to the /home/loginchallenge action, we need to suppress the redirect and allow the page to load; we also need
                // to change the response to a 401 to trigger a challenge
                if (Request.Url.AbsolutePath.ToLower().Contains("challenge") && Response.RedirectLocation.ToLower().Contains("login"))
                {
                    Response.StatusCode = 401;
                    Response.SuppressFormsAuthenticationRedirect = true;
                }
            }
          //  LoggingHelper.LogWarning("Application_EndRequest");
        }

        private void AddProfiles()
        {
            var assemblies = new[]
            {
                typeof(AccountingProfile).Assembly,
                typeof(CertificationPeriodProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };

            var autoMapper = ServiceLocator.Resolve<IAutoMapperHelper>();
            autoMapper.Configure(assemblies);}


    }
}
