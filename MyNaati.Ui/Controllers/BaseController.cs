using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Bl.Credentials;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Bl.Credentials;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;
using MyNaati.Bl.BackOffice;
using MyNaati.Contracts.BackOffice;
using MyNaati.Ui.Security;
using MyNaati.Ui.UI;
using MyNaati.Ui.ViewModels.Account;
using Newtonsoft.Json;

namespace MyNaati.Ui.Controllers
{
    public class BaseController : Controller
    {
        private static bool? _debugMode;

        protected static bool DebugMode => (_debugMode ?? (_debugMode = Convert.ToBoolean(ConfigurationManager.AppSettings["DebugMode"]))).Value;
        public int CurrentUserNaatiNumber {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    //  Allow for admin user, not just Customer number user names.
                    return User.NaatiNumber();
                }

                return 0;
            }
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is IllegalInputCharacterException)
            {
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" )
                {
                    filterContext.ExceptionHandled = true;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    filterContext.Result =
                        Content(JsonConvert.SerializeObject(new List<string> { filterContext.Exception.Message }));
                    return;
                }

                var result = ViewEngines.Engines.FindView(ControllerContext, filterContext.RouteData.Values["action"].ToString(), null);
                if (result.View != null)
                {
                    filterContext.ExceptionHandled = true;
                    filterContext.Result = new ViewResult
                    {
                        ViewName = filterContext.RouteData.Values["action"].ToString(),
                        TempData = filterContext.Controller.TempData,
                        ViewData = filterContext.Controller.ViewData
                    };
                }
                else
                {
                    throw filterContext.Exception;
                }
            }
            if (filterContext.Exception is System.ServiceModel.FaultException && "INVALID USER".Equals(filterContext.Exception.Message))
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = new RedirectResult(Url.Action("Logoff", "Account"));
            }
            base.OnException(filterContext);
        }
        protected void AddValidationMessagesToModelState(IEnumerable<ValidationResult> messages, ModelStateDictionary modelState)
        {
            foreach (var error in messages)
                modelState.AddModelError(error.PropertyName ?? string.Empty, error.Message);
        }

        protected void AddValidationMessagesToModelState(ModelStateDictionary modelState, params ValidationResult[] messages)
        {
            foreach (var error in messages)
                modelState.AddModelError(error.PropertyName ?? string.Empty, error.Message);
        }

        protected JsonResult Json<T, U>(SearchResults<T> results, Func<T, U> conversion)
        {
            var result = new
            {
                results.PageNumber,
                results.TotalResultsCount,
                Results = results.Results.Select(conversion),
                TotalPageCount = results.TotalPageCount
            };
            return Json(result);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.User.IsInRole(SystemRoles.TEMPORARYPASSWORDUSERS))
            {
                ForceExecuteAction(filterContext, "Account", "ChangePassword");
                return;
            }

            if (filterContext.Result == null && HttpContext.User.IsInRole(SystemRoles.EXAMINER) && !(HttpContext.User.IsInRole(SystemRoles.AUTHENTICATED_EXAMINER) || HttpContext.User.IsInRole(SystemRoles.UNAUTHENTICATED_EXAMINER)))
            {
                ForceExecuteAction(filterContext, "Home", "ValidateExaminer");
                return;
            }
 
            if (filterContext.Result == null && filterContext.RouteData.Values.Any(v =>
        string.Equals(v.Value?.ToString(), "examinertools", StringComparison.OrdinalIgnoreCase)) && !HttpContext.User.IsInRole(SystemRoles.EXAMINER) && !HttpContext.User.IsInRole(SystemRoles.AUTHENTICATED_EXAMINER) && !HttpContext.User.IsRolePlayer())
            {
                ForceExecuteAction(filterContext, "Home", "Index");
                return;
            }

            if (filterContext.Result == null)
                base.OnActionExecuting(filterContext);
        }

        protected MfaAndAccessCodeModel GetMfaConfigurationAndEmailCodeActiveStatus(int naatiNumber)
        {
            var credentialQrCodeService = ServiceLocator.Resolve<ICredentialQrCodeService>();
            var mfaConfiguredAndActiveResponse = credentialQrCodeService.GetMFAConfiguredAndValid(naatiNumber);

            return new MfaAndAccessCodeModel
            {
                MfaConfigured = mfaConfiguredAndActiveResponse.Data.MfaConfigured,
                MfaActive = mfaConfiguredAndActiveResponse.Data.MfaActive,
                Email = mfaConfiguredAndActiveResponse.Data.Email,
            };
        }

        private void ForceExecuteAction(ActionExecutingContext filterContext, string controllerName, string actionName)
        {
            var currentActionName = filterContext.ActionDescriptor.ActionName;
            if (filterContext.Controller.ToString().IndexOf("AccountController") != -1 && currentActionName == "LogOff")
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            else if (filterContext.Controller.ToString().IndexOf(controllerName + "Controller") != -1 && currentActionName == actionName)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            filterContext.Result = new RedirectResult(Url.Action(actionName, controllerName));
        }

        private IEnumerable<string> GetErrors(HttpResponseMessage response)
        {
            var data = response.Content.ReadAsStringAsync().Result;
            var errors = new List<string>();
            try
            {
                errors.AddRange(JsonConvert.DeserializeObject<IEnumerable<string>>(data));
            }
            catch
            {
                errors.Add(data);
            }
            return errors;
        }
    }
}