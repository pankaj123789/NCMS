using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Contracts.BackOffice.NcmsIntegration;
using MyNaati.Contracts.BackOffice.Panel;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Helpers;
using MyNaati.Ui.Models;
using MyNaati.Ui.Security;
using MyNaati.Ui.UI;
using MyNaati.Ui.ViewModels.Home;
using SendSecurityCodeRequest = MyNaati.Contracts.BackOffice.Panel.SendSecurityCodeRequest;
using ValidateExaminerSecurityCodeRequest = MyNaati.Contracts.BackOffice.Panel.ValidateExaminerSecurityCodeRequest;

namespace MyNaati.Ui.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ISystemService mSystemService;
        private readonly IPanelMembershipService mPanelService;
        private readonly INcmsIntegrationService _ncmsIntegrationService;
        private readonly ILookupProvider mLookupProvider;


        public HomeController(ISystemService systemService, IPanelMembershipService panelService, INcmsIntegrationService ncmsIntegrationService, ILookupProvider lookupProvider)
        {
            mSystemService = systemService;
            mPanelService = panelService;
            mLookupProvider = lookupProvider;
            _ncmsIntegrationService = ncmsIntegrationService;
        }

        public static IEnumerable<MenuLinkModel> Menus(IPrincipal user)
        {
            var menuHelper = ServiceLocator.Resolve<IMenuHelper>();
            return menuHelper.Menus(user);
        }

        public ActionResult Menus()
        {
            return Json(Menus(User), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Welcome();
            }
            mLookupProvider.SystemValues.RefreshAllCache();
            var model = new LogOnModel()
            {
                MessageOfTheDay = mLookupProvider.SystemValues.MessageOfTheDay,
                MyNaatiAvailable = mLookupProvider.SystemValues.MyNaatiAvailable == "true",
                ShowMessageOfTheDay = mLookupProvider.SystemValues.ShowMessageOfTheDay == "true",
            };


            return View(model);
        }

        public ActionResult About()
        {
            var model = new AboutModel();
            Assembly mvcAssembly = typeof(HomeController).Assembly;
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(mvcAssembly.Location);
            model.Version = fvi.FileVersion;

            return View(model);
        }

        [Authorize]
        public ActionResult Welcome()
        {
            var model = new HomeModel();

            return View("Welcome", model);
        }

        [Authorize]
        [AuthorizeRoles(SystemRoles.EXAMINER)]
        [HttpGet]
        public ActionResult ValidateExaminer()
        {
            var bypassValidation = false;
            var bypassSetting = ConfigurationManager.AppSettings["BypassExaminerAuthentication"];

            if (bool.TryParse(bypassSetting, out bypassValidation) && bypassValidation)
            {
                Roles.AddUserToRole(User.Identity.Name, SystemRoles.AUTHENTICATED_EXAMINER);
                return RedirectToAction("Index");
            }

            mPanelService.SendSecurityCode(new SendSecurityCodeRequest { NAATINumber = User.NaatiNumber() });
            return View();
        }

        [Authorize]
        [AuthorizeRoles(SystemRoles.EXAMINER)]
        [HttpPost]
        public ActionResult ValidateExaminer(ValidateExaminerModel model)
        {
            if (model.Action == "resend")
            {
                return RedirectToAction("ValidateExaminer");
            }

            if (model.Action == "continue")
            {
                Roles.AddUserToRole(User.Identity.Name, SystemRoles.UNAUTHENTICATED_EXAMINER);
                return RedirectToAction("Welcome");
            }

            if (!ModelState.IsValid)
                return View();

            if (model.Action != "validate")
            {
                return View();
            }

            var request = new ValidateExaminerSecurityCodeRequest
            {
                SecurityCode = model.SecurityCode,
                NAATINumber = User.NaatiNumber()
            };

            if (!mPanelService.ValidateExaminerSecurityCode(request).Valid)
            {
                var message = @"
                    Sorry, the security code you entered is incorrect. 
                    Click Resend security code to generate security code, 
                    or, Continue to log into the ePortal
                    without access to the Examiner Tools.";

                ModelState.AddModelError("SecurityCode", message);
                return View();
            }

            Roles.AddUserToRole(User.Identity.Name, SystemRoles.AUTHENTICATED_EXAMINER);
            return RedirectToAction("Index");
        }

        public ActionResult Help()
        {
            return View("HelpWindow");
        }

        public ActionResult HelpContent()
        {
            return View("HelpContent");
        }

        public ActionResult HelpMenu()
        {
            return View("HelpMenu");
        }

        public ActionResult ComingSoon()
        {
            return View();
        }

        public ActionResult LearnMore()
        {
            return PartialView("LearnMore");
        }

        [AuthorizeRoles(SystemRoles.ADMINISTRATORS)]
        public ActionResult Diagnostics()
        {
            DiagnosticResponse data = mSystemService.RunDiagnostics(new DiagnosticRequest());
            var model = new DiagnosticsModel()
            {
                ErrorMessages = data.Errors,
                IsFullyFunctional = data.Errors.Any() == false
            };
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult TestConnection()
        {
            Stopwatch stopWatch = null;
            if (DebugMode)
            {
                LoggingHelper.LogWarning("MyNaati TestConnection Started)");
                stopWatch = Stopwatch.StartNew();
            }

            var result = _ncmsIntegrationService.TestConnection();

            if (stopWatch != null)
            {
                stopWatch.Stop();
                LoggingHelper.LogWarning("MyNAaati TestConnection finished in ms:{duration}, endpoint: {endpoint} ", stopWatch.Elapsed.TotalMilliseconds, "TestConnection");
            }
            
            if (result)
            {
                return new EmptyResult();
            }
            
            throw new Exception("Test connection failed. ");
        }
 
        public ActionResult OnlineDirectory()
        {
            var onlineDirectoryUrl = ConfigurationManager.AppSettings["OnlineDirectoryUrl"];
            return Redirect(onlineDirectoryUrl);
        }

        //public ActionResult OnlineHelp()
        //{
        //    var onlineDirectoryUrl = ConfigurationManager.AppSettings["OnlineHelpUrl"];
        //    return Redirect(onlineDirectoryUrl);
        //}

    }
}