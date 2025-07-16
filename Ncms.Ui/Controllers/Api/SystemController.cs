using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;
using Newtonsoft.Json;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/system")]
    public class SystemController : BaseApiController
    {
        private readonly ISystemService _systemService;
        private readonly ISecretsCacheQueryService _secretsProvider;

        public SystemController(ISystemService systemService, ISecretsCacheQueryService secretsProvider)
        {
            _systemService = systemService;
            _secretsProvider = secretsProvider;
        }

        [HttpGet]
        [Route("basicinfo")]
        public HttpResponseMessage BasicInfo()
        {
            var keys = new List<string> { "Environment", "Name", "Version" };

#if DEBUG
            keys.Add("Database name");
            keys.Add("Debug");
#endif

            var data = SystemInfo();
            return this.CreateResponse(() => data.Where(k => keys.Contains(k.Key)).ToDictionary(k => k.Key, k => k.Value));
        }

        [HttpGet]
        [Route("info")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage Info()
        {
            var data = SystemInfo();
            return this.CreateResponse(() => data);
        }

        private Dictionary<string, string> SystemInfo()
        {
            var data = new Dictionary<string, string>();
            var configDetails = _systemService.GetConfigDetails();
            var version = GetAssemblyFileVersion();

            data["Name"] = Naati.Resources.Shared.AppName;
            data["Version"] = version;

            var buildNum = version.Split('.').Last();
            if (buildNum.Length > 1)
            {
                // make sure the year is the same as the year in FunctionLibrary.ps1
                data["Build date"] = new DateTime(2022, 1, 1).AddDays(int.Parse(buildNum.Substring(0, 3))).ToShortDateString();
            }

#if DEBUG
            data["Debug"] = "1";
#endif
            data["Database name"] = configDetails.DatabaseName;
            var env = _systemService.GetEnvironmentName();
            data["Environment"] = String.IsNullOrEmpty(env) ? "Production" : env;
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                {
                    data["PrincipalContext"] = JsonConvert.SerializeObject(context);
                }
            }
            catch { }

            try
            {
                var cookie = HttpContext.Current.Request.Cookies["NCMS"]?.Value;
                if (cookie != null)
                {
                    data["Ticket"] = JsonConvert.SerializeObject(FormsAuthentication.Decrypt(cookie));
                }
            }
            catch { }

            return data;
        }

        [HttpGet]
        [Route("value/{valuekey}")]

        //This permission should not be changed. If a specific value key is required and roles doesn''t have this permission then
        // a new method to get just specific key should be created.
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.System)]
        public HttpResponseMessage SystemValue(string valuekey)
        {
            return this.CreateResponse(() => _systemService.GetSystemValue(valuekey));
        }

        [HttpGet]
        [Route("domainname")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.System)]
        public HttpResponseMessage GetDomainName()
        {
            return this.CreateResponse(() => _secretsProvider.Get(SecuritySettings.DefaultUserDomain));
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetVersionInfo()
        {
            return this.CreateResponse(() => GetAssemblyFileVersion());
        }

        private static string GetAssemblyFileVersion()
        {
            var attribute = Assembly.GetAssembly(typeof(HomeController))
                .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)
                .Cast<AssemblyFileVersionAttribute>()
                .FirstOrDefault();

            return attribute != null ? attribute.Version : string.Empty;
        }

        [HttpGet]
        [Route("AadAuthUrl")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.System)]
        public HttpResponseMessage AadAuthUrl()
        {
            var tenant = _secretsProvider.Get("AadAuthTenant");
            var clientId = _secretsProvider.Get("AadAuthClientId");
            var redirectUri = ConfigurationManager.AppSettings["AadAuthRedirectUri"];
            var scope = ConfigurationManager.AppSettings["AadAuthScope"];
            var url = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize?client_id={clientId}&response_type=code&redirect_uri={redirectUri}&response_mode=query&scope={scope}";
            return this.CreateResponse(() => url);
        }

        [HttpGet]
        [Route("WiiseAuthUrl")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.System)]
        public HttpResponseMessage WiiseAuthUrl()
        {
            var tenant = _secretsProvider.Get("WiiseAuthTenant");
            var clientId = _secretsProvider.Get("WiiseAuthClientId");
            var redirectUri = ConfigurationManager.AppSettings["WiiseAuthRedirectUri"];
            var scope = ConfigurationManager.AppSettings["WiiseAuthScope"];
            var url = $"https://login.windows.net/{tenant}/oauth2/authorize?prompt=login&resource=https://api.businesscentral.dynamics.com&response_type=code&state=&client_id={clientId}&scope=&redirect_uri={redirectUri}";
            LoggingHelper.LogInfo($"Wiise Auth begin step 1 {url}");
            var response = this.CreateResponse(() => url);
            LoggingHelper.LogInfo("Wiise Auth before step 1");
            return response;
        }


        [HttpGet]
        [Route("AadResponse")]
        public IHttpActionResult AadResponse(string code)
        {
            if (!String.IsNullOrWhiteSpace(code))
            {
                _systemService.ObtainGraphAccessToken(code);
            }
            var uri = new Uri(Url.Content(ConfigurationManager.AppSettings["AadResponseRedirect"]));
            return Redirect(uri);
        }

        [HttpGet]
        [Route("WiiseResponse")]
        public IHttpActionResult WiiseResponse(string code)
        {
            if (!String.IsNullOrWhiteSpace(code))
            {
                _systemService.ObtainWiiseAccessToken(code);
            }
            var uri = new Uri(Url.Content(ConfigurationManager.AppSettings["WiiseResponseRedirect"]));
            return Redirect(uri);
        }

        [HttpGet]
        [Route("throwFrontEnd")]
        public HttpResponseMessage ThrowFrontEndException()
        {
            return this.CreateResponse(() => _systemService.ThrowFrontEndException());
        }

        [HttpGet]
        [Route("throwBackEnd")]
        public HttpResponseMessage ThrowBackEndException()
        {
            return this.CreateResponse(() => _systemService.ThrowBackEndException());
        }

        [HttpGet]
        [Route("throwUserFriendly")]
        public HttpResponseMessage ThrowUserFriendlyException()
        {
            return this.CreateResponse(() => _systemService.ThrowUserFriendlyException());
        }

        [HttpGet]
        [Route("throwUnhandled")]
        public HttpResponseMessage ThrowUnhandledException()
        {
            throw new Exception($"Unhandled exception thrown for testing from {GetType().Name}.");
        }

        [HttpPost]
        [Route("ClearCache")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage ClearCache()
        {
            return this.CreateResponse(() =>
            {
                Startup.ClearCache();
            });
        }

        [HttpPost]
        [Route("refreshUsersCache")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage RefreshUsersCache()
        {
            return this.CreateResponse(() =>
            {
                Startup.RefreshUsersCache();
            });
        }

        [HttpPost]
        [Route("refreshCookieCache")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage RefreshCookieCache()
        {
            return this.CreateResponse(() =>
            {
                Startup.RefreshCookies();
            });
        }

        [HttpPost]
        [Route("processRefund")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage ProcessRefund()
        {
            return this.CreateResponse(() =>
            {
                Startup.ExecuteProcessRefund();
            });
        }

        [HttpPost]
        [Route("refreshNotifications")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage RefreshNotifications()
        {
            return this.CreateResponse(() =>
            {
                Startup.ExecuteRefreshNotifications();
            });
        }

        [HttpPost]
        [Route("deleteBankDetails")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage DeleteBankDetails()
        {
            return this.CreateResponse(() =>
            {
                Startup.ExecuteDeleteBankDetails();
            });
        }

        //[HttpPost]
        //[Route("downloadTestAssets")]
        //[NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        //public HttpResponseMessage DownloadTestAssets()
        //{
        //    return this.CreateResponse(() =>
        //    {
        //        Startup.ExecuteDownloadTestAssets();
        //    });
        //}

        //[HttpPost]
        //[Route("createTelevicUsers")]
        //[NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        //public HttpResponseMessage CreateTelevicUsers()
        //{
        //    return this.CreateResponse(() =>
        //    {
        //        Startup.ExecuteCreateTelevicUsers();
        //    });
        //}


        [HttpPost]
        [Route("DeleteTemporaryFiles")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage DeleteTemporaryFiles()
        {
            return this.CreateResponse(() =>
            {
                Startup.DeleteTemporaryFiles();
            });
        }

        [HttpPost]
        [Route("DeletePodDataHistory")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage DeletePodDataHistory()
        {
            return this.CreateResponse(() =>
            {
                Startup.DeletePodDataHistory();
            });
        }

        [HttpPost]
        [Route("SyncNcmsReportLogs")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage SyncNcmsReportLogs()
        {
            return this.CreateResponse(() =>
            {
                Startup.SyncNcmsReportLogs();
            });
        }

        [HttpPost]
        [Route("ExecuteNcmsReports")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage ExecuteNcmsReports()
        {
            return this.CreateResponse(() =>
            {
                Startup.ExecuteNcmsReports();
            });
        }

        [HttpPost]
        [Route("ProcessPendingApplications")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage ProcessPendingApplications()
        {
            return this.CreateResponse(() =>
            {
                Startup.ForceProcessApplications();

            });
        }

        [HttpPost]
        [Route("SendCandidateBriefs")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage ForceSendCandidateBriefs()
        {
            return this.CreateResponse(() =>
            {
                Startup.ForceSendCandidateBriefs();
            });
        }

        [HttpPost]
        [Route("issueTestResultsAndCredentials")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage ForceIssueTestResultsAndCredentials()
        {
            return this.CreateResponse(() =>
            {
                Startup.ForceIssueTestResultsAndCredentials();
            });
        }

        [HttpPost]
        [Route("SendPendingEmails")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage ForceSendPendingEmails()
        {
            return this.CreateResponse(() =>
            {
                Startup.ForceSendPendingEmails();
            });
        }

        [HttpPost]
        [Route("sendRecertificationReminders")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage SendRecertificationReminders()
        {
            return this.CreateResponse(() =>
            {
                Startup.SendRecertificationReminders();
            });
        }

        [HttpPost]
        [Route("sendTestSessionReminders")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage SendTestSessionReminders()
        {
            return this.CreateResponse(() =>
            {
                Startup.SendTestSessionReminders();
            });
        }

        [HttpPost]
        [Route("sendTestSessionAvailabilityNotices")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage SendTestSessionAvailabilityNotices()
        {
            return this.CreateResponse(() =>
            {
                Startup.SendTestSessionAvailabilityNotices();
            });
        }

        [HttpPost]
        [Route("sendTestMaterialReminders")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage SendTestMaterialReminders()
        {
            return this.CreateResponse(() =>
            {
                Startup.ExecuteSendTestMaterialReminders();
            });
        }

        [HttpPost]
        [Route("processFileDeletesPastExpiryDate")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage ProcessFileDeletesPastExpiryDate()
        {
            return this.CreateResponse(() =>
            {
                Startup.ExecuteProcessFileDeletesPastExpiryDate();
            });
        }

        //motd
        [HttpGet]
        [Route("getMotdValues")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage GetMotdValues()
        {
            var data = GetMotdValuesFromSystemValues();
            return this.CreateResponse(() => data);
        }

        [HttpPost]
        [Route("postMotdValues")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage PostMotdValues(Dictionary<string,string> motdValues)
        {
            var result = new
            {
                InvalidFields = new List<object>()
            };
            _systemService.SetSystemValue("ShowMessageOfTheDay", motdValues["ShowMessageOfTheDay"]);
            _systemService.SetSystemValue("MyNaatiAvailable", motdValues["MyNaatiAvailable"]);
            _systemService.SetSystemValue("MessageOfTheDay", motdValues["MessageOfTheDay"]);
            return this.CreateResponse(() => result);
        }

        [HttpPost]
        [Route("postUpdatedSystemValues")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage PostUpdatedSystemValues(SystemValueRequest[] systemValuePairs)
        {
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _systemService.ValidateSystemValues(systemValuePairs).Data.ToList();
                if (validationErrors.Any())
                {
                    return this.CreateResponse(() => new { InvalidFields = validationErrors });
                }

                foreach (var systemValuePair in systemValuePairs)
                {
                    // if not bool proceed as normal
                    if (systemValuePair.DataType != "bool")
                    {
                        _systemService.SetSystemValue(systemValuePair.Key, systemValuePair.Value);
                        continue;
                    }

                    // if bool, check if true and change value to 1
                    if (systemValuePair.Value == "true")
                    {
                        systemValuePair.Value = "1";
                        _systemService.SetSystemValue(systemValuePair.Key, systemValuePair.Value);
                        continue;
                    }

                    // if false change value to 0
                    systemValuePair.Value = "0";
                    _systemService.SetSystemValue(systemValuePair.Key, systemValuePair.Value);
                    continue;
                }

                return sucessResponse;
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpGet]
        [Route("getfiledeletereport")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.System)]
        public HttpResponseMessage GetFileDeleteReport()
        {
            var data = _systemService.GetFileDeleteReport();
            return this.CreateResponse(() => data);
        }


        private Dictionary<string, string> GetMotdValuesFromSystemValues()
        {
            var motdValues = new Dictionary<string, string>
            {
                { "MessageOfTheDay", _systemService.GetSystemValue("MessageOfTheDay") },
                { "MyNaatiAvailable", _systemService.GetSystemValue("MyNaatiAvailable") },
                { "ShowMessageOfTheDay", _systemService.GetSystemValue("ShowMessageOfTheDay") }
            };

            return motdValues;
        }

        public class MotdValues
        {
            public bool MyNaatiAvailable { get; set; }
            public bool ShowMessageOfTheDay { get; set; }
            public string MessageOfTheDay { get; set; }
        }
    }
}
