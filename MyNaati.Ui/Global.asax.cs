using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using AutoMapper;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Global.Common.SystemLifecycle;
using F1Solutions.Naati.Common.Bl.AutoMappingProfiles;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.Nhibernate.Mappings;
using F1Solutions.Naati.Common.Dal.NHibernate.SharpArchitecture;
using MyNaati.Ui.Common;
using MyNaati.Ui.Helpers;
using Newtonsoft.Json.Serialization;
using WebApiThrottle;
using MyNaati.Ui.Security;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Dal.AutoMappingProfiles;
using MyNaati.Bl.AutoMappingProfiles;
using MyNaati.Ui.AutoMappingProfiles;
using System.Web.Http.Tracing;

namespace MyNaati.Ui
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private WebSessionStorage mWebSessionStorage;
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleCustomErrorAttribute());
            filters.Add(new HtmlEncodeAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
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

        public MvcApplication()
        {
            this.mWebSessionStorage = new WebSessionStorage(this);
        }

        public const string HIBERNATE_CONNECTION_STRING = "connection.connection_string";
        public const string WEB_CONFIG_CONNECTION_STRING = "CONNECTION_STRING";

        protected void Application_Start()
        {
            string loggingDiagnosticsFileName = ConfigurationManager.AppSettings["logging:DiagnosticsLogFile"];
            var machineName = System.Net.Dns.GetHostEntry("").HostName;
            var loggingDiagnosticsFile = Path.Combine(ConfigurationManager.AppSettings["applicationData"], machineName, loggingDiagnosticsFileName);
            var bufferFileBase = Path.Combine(ConfigurationManager.AppSettings["applicationData"], machineName, ConfigurationManager.AppSettings["logging:SeqLogBufferFileBase"]);
            var logFile = Path.Combine(ConfigurationManager.AppSettings["applicationData"], machineName, ConfigurationManager.AppSettings["logging:LogFile"]);

            CheckOrCreateFile(loggingDiagnosticsFile);
            CheckOrCreateFile(bufferFileBase);
            CheckOrCreateFile(logFile);

            LoggingHelper.ConfigureLogging("NAATI", "MyNAATI",
                bufferFileBase, logFile,
                String.IsNullOrEmpty(loggingDiagnosticsFileName) ? null : loggingDiagnosticsFile, GetDatabase());
            LoggingHelper.LogInfo("Validating System Time Zone...");
            SystemLifecycleHelper.ValidateSystemTimeZone();
            LoggingHelper.LogInfo("Configuring Nhibernate...");
            NHibernateInitializer.Instance().InitializeNHibernateOnce(InitializeNHibernateConfiguration);
            LoggingHelper.LogInfo("AddProfiles...");
            AddProfiles();
            LoggingHelper.LogInfo("Logging system starting date...");
            LogSystemStartedDate();
            LoggingHelper.LogInfo("Setting SecurityProtocol...");
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            LoggingHelper.LogInfo("RegisterAllAreas...");
            AreaRegistration.RegisterAllAreas();
            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            LoggingHelper.LogInfo("RegisterGlobalFilters...");
            RegisterGlobalFilters(GlobalFilters.Filters);

            LoggingHelper.LogInfo("Registering WebApiConfig...");
            GlobalConfiguration.Configure(WebApiConfig.Register);
            LoggingHelper.LogInfo("RegisterRoutes...");
            RegisterRoutes(RouteTable.Routes);
            LoggingHelper.LogInfo("PreApplicationStartCode.Start...");
            System.Web.Optimization.PreApplicationStartCode.Start();

            LoggingHelper.LogInfo("Setting DefaultBinder...");
            ModelBinders.Binders.DefaultBinder = new DefaultModelBinderWithHtmlValidation();

            SystemLifecycleHelper.UpdateLifecycleStatus(SystemLifecycleStatus.Running);
            LoggingHelper.LogInfo("Register HtmlEncodeAttribute...");
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // if a valid FormAuthenticationTicket cookie exists, set the current principal from it
            var cookie = Request.Cookies["MYNAATI"]?.Value;

            if (cookie != null)
            {
                var formsTicket = FormsAuthentication.Decrypt(cookie);
                if (formsTicket != null && !formsTicket.Expired)
                {
                    if (!ServiceLocator.Resolve<ICookieQueryService>().IsInvalidCookie(cookie))
                    {
                        System.Web.HttpContext.Current.User = new GenericPrincipal(new FormsIdentity(formsTicket), null);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    }
                }
            }
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            if (String.IsNullOrEmpty(exception.Source))
            {
                exception.Source = "Application_Error";
            }
            LoggingHelper.LogExceptionWithHttpContext(exception, Context);
        }

        private static void LogSystemStartedDate()
        {
            var utilityQueryService = DependencyResolver.Current.GetService<IUtilityQueryService>();
            utilityQueryService.LogSystemStartDate();
        }

        protected void Application_BeginRequest()
        {
            NHibernateInitializer.Instance().InitializeNHibernateOnce(InitializeNHibernateConfiguration);
        }

        protected void InitializeNHibernateConfiguration()
        {
            NHibernate.Cfg.Configuration config = NHibernateSession.Init(this.mWebSessionStorage, new[] { Server.MapPath("~/bin/F1Solutions.Naati.Common.Dal.Domain.dll") },
                new AutoPersistenceModelGenerator().Generate(), Server.MapPath("~/NHibernate.config"),
                GetAdditionalNHibernateProperties(), string.Empty, GetConnectionString());
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

        private Dictionary<string, string> GetAdditionalNHibernateProperties()
        {
            var properties = new Dictionary<string, string>();

            // Set connection string based on value set in web.config
            properties[HIBERNATE_CONNECTION_STRING] = GetConnectionString();
            return properties;
        }

        private void AddProfiles()
        {
            //Mapper.Reset();
            //Mapper.AllowNullDestinationValues = true;
            //var profiles = typeof(ApplicationForAccreditationWizardProfile).Assembly.GetExportedTypes()
            var assemblies = new[]
            {
                typeof(ApplicationForAccreditationWizardProfile).Assembly,
                typeof(AccountingProfile).Assembly,
                typeof(CertificationPeriodProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };

            var autoMapper = ServiceLocator.Resolve<IAutoMapperHelper>();
            autoMapper.Configure(assemblies);
            //    .Where(t => t.IsSubclassOf(typeof(Profile)))
            //    .Select(ServiceLocator.GetService);

            //foreach (Profile profile in profiles)
            //{
            //    Mapper.AddProfile(profile);
            //}

            //profiles = typeof(ProductSpecificationProfile).Assembly.GetExportedTypes()
            //    .Where(t => t.IsSubclassOf(typeof(Profile)))
            //    .Select(ServiceLocator.GetService);


            //foreach (Profile profile in profiles)
            //{
            //    Mapper.AddProfile(profile);
            //}
        }

        //public void Configure(IApplicationBuilder app)
        //{
        //    app.UseStaticFiles();

        //    // Enable middleware to serve generated Swagger as a JSON endpoint.
        //    app.;

        //    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
        //    // specifying the Swagger JSON endpoint.
        //    app.UseSwaggerUI(c =>
        //    {
        //        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        //    });di

        //    app.UseRouting();
        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapControllers();
        //    });
        //}

        protected void Application_OnEnd(object sender, EventArgs e)
        {
            LoggingHelper.LogWarning($"MYNAATI Application Ending");
        }
        public static class WebApiConfig
        {
            public static void Register(HttpConfiguration config)
            {
                config.Services.Add(typeof(IExceptionLogger), new HttpUnhandledExceptionLogger { DefaultSource = "WebAPI" });
                Configure(config);

                config.MapHttpAttributeRoutes();

                config.Routes.MapHttpRoute(
                    name: "PublicApi",
                    routeTemplate: "api/{version}/{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                GlobalConfiguration.Configuration.EnsureInitialized();
            }

            private static void Configure(HttpConfiguration config)
            {
                config.Formatters.Remove(config.Formatters.XmlFormatter);
                var json = config.Formatters.JsonFormatter;
                json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                ConfigureThrottling(config);
            }

            private static void ConfigureThrottling(HttpConfiguration config)
            {

                //https://medium.com/@josiahmahachi/secure-asp-net-apis-using-x-api-key-api-keys-62d63b2b9fb0
                var rateLimit = Convert.ToInt32(ConfigurationManager.AppSettings["CertificationApiLimit"]);

                //trace provider
                var traceWriter = new MyNaatiThrottlingTracer();

                var throttlingHandler = new MyNaatiThrottlingHandler(
                   policy: new ThrottlePolicy()
                   {
                       IpThrottling = true,
                       ClientThrottling = true,
                       EndpointThrottling = true,
                       EndpointRules = new Dictionary<string, RateLimits>
                       {
                            //Fine tune throttling per specific API here
                            {
                                "api/1.0/Public/Certifications",
                                new RateLimits
                                {
                                    PerSecond = rateLimit
                                }
                            }
                       }

                   },
                   policyRepository: new PolicyCacheRepository(),
                   repository: new CacheRepository(),
                   ipAddressParser: new MyNaatiThrottlingCustomAddressParser(),
                   logger: new TracingThrottleLogger(traceWriter)
                   );

                throttlingHandler.QuotaExceededMessage =
                        "Cannot verify the details of this practitioner as we are experiencing a high volume of calls to the API.  Please try again in a few minutes.";

                config.MessageHandlers.Add(throttlingHandler);
            }
        }
    }
}
