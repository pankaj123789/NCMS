using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using Ncms.Ui.Extensions;
using Ncms.Ui.Models;

namespace Ncms.Ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IUserService _userService;

        public HomeController(ISecurityService securityService, IUserService userService)
        {
            _securityService = securityService;
            _userService = userService;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("~/login")]
        [NoCache]
        public ActionResult LoginGet()
        {
            FormsAuthentication.SignOut();
            return View("Login");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("~/login")]
        public ActionResult LoginPost(LoginModel model)
        {
            try
            {
                FormsAuthentication.SignOut();
                var username = model.Username;

                if (_securityService.AuthenticateNonWindowsUser(ref username, model.Password)
                    || username.Contains("\\") && _securityService.AuthenticateWindowsUser(ref username, model.Password))
                {
                    SetAuthCookie(username);
                    return Redirect("/#?redir=" + Server.UrlEncode(Request.QueryString["ReturnUrl"]));
                }
                LogLogin(username, false);
                ModelState.AddModelError("Login", "Invalid username or password, or inactive account");
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex);
            }

            return View("Login", model);
        }

        [AllowAnonymous]
        public ActionResult Signin()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = $"/loginchallenge?ReturnUrl={Server.UrlEncode(Request.QueryString["ReturnUrl"])}" },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
                return Content(null);
            }

            return LoginChallenge();
        }

        /// <summary>
        /// Login with Windows credentials. A 401 challenge is issued whenever we come here (see global.asax.cs)
        /// </summary>
        [Authorize]
        [Route("~/loginchallenge")]
        public ActionResult LoginChallenge()
        {
            try
            {
                var user = System.Threading.Thread.CurrentPrincipal;
                if (user.Identity.IsAuthenticated)
                {
                    var userName = String.Empty;
                    if (user.Identity is WindowsIdentity)
                    {
                        userName = user.Identity.Name;
                    }
                    else if (User.Identity is System.Security.Claims.ClaimsIdentity)
                    {
                        var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;
                        userName = userClaims?.FindFirst("preferred_username")?.Value;
                    }

                    if (!String.IsNullOrWhiteSpace(userName) && _securityService.AuthenticateWindowsUser(userName))
                    {
                        SetAuthCookie(userName);
                        return Redirect("/#?redir=" + Server.UrlEncode(Request.QueryString["ReturnUrl"]));
                    }

                    // sometimes the user is already logged in but they click the login button in another browser tab
                    if (user.Identity is FormsIdentity)
                    {
                        return Redirect("/#?redir=" + Server.UrlEncode(Request.QueryString["ReturnUrl"]));
                    }
                }

                LogLogin(user.Identity.Name, false);
                ModelState.AddModelError("Login", $"User {user.Identity.Name} could not be authenticated");
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex);
            }

            return View("Login");
        }

        [AllowAnonymous]
        [Route("~/logout")]
        public ActionResult LogoutGet()
        {
            InvalidateCookie();
            FormsAuthentication.SignOut();
            Session?.Abandon();
            return RedirectToAction("LoginGet", "Home");
        }

        private void InvalidateCookie()
        {
            var cookie = Request.Cookies["NCMS"]?.Value;

            if (cookie != null)
            {
                var formsTicket = FormsAuthentication.Decrypt(cookie);
                if (formsTicket != null && !formsTicket.Expired)
                {
                    var user = System.Threading.Thread.CurrentPrincipal?.Identity?.Name;

                    Startup.RefreshUserCache(user, cookie, formsTicket.Expiration);
                }
            }
        }

        /// <summary>
        /// Create and set a cookie containing FormsAuthenticationTicket. This means the user is logged in. 
        /// You must not call this without first verifiying the user credentials.
        /// </summary>
        private void SetAuthCookie(string username)
        {
            string dbUserName = null;
            if (!string.IsNullOrWhiteSpace(username))
            {
                dbUserName = _userService.GetUser(username).Name;
            }
            var cookie = FormsAuthentication.GetAuthCookie(dbUserName, false);
            cookie.Secure = FormsAuthentication.RequireSSL;
            HttpContext.User = new GenericPrincipal(new FormsIdentity(FormsAuthentication.Decrypt(cookie.Value)), null);
            FormsAuthentication.SetAuthCookie(HttpContext.User.Identity.Name, false);
            LogLogin(username, true);
        }

        private void LogLogin(string username, bool successful)
        {
            var forwardedFor = GetForwardedFor();
            var template = successful
                ? "User {UserName} signing into NCMS from {IpAddress}"
                : "Refused NCMS login to {UserName} from {IpAddress}";

            if (forwardedFor == null)
            {
                LoggingHelper.LogInfo(template, username, Request.UserHostAddress);
            }
            else
            {
                template += " (forwarded for {ForwardedFor})";
                LoggingHelper.LogInfo(template, username, Request.UserHostAddress, forwardedFor);
            }
        }

        private string GetForwardedFor()
        {
            var forwardedFor = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            return !String.IsNullOrEmpty(forwardedFor) ? forwardedFor.Split(',')[0] : null;
        }

        [AllowAnonymous]
        public ActionResult TestApiNcms()
        {
            LoggingHelper.LogWarning("TestAPI NCMS CALLED");

            return Json(new List<int>(), JsonRequestBehavior.AllowGet);
        }
    }
}
