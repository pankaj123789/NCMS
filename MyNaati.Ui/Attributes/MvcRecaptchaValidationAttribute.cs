using System.Configuration;
using System.Web.Mvc;
using System.Web.WebPages;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Security;

namespace MyNaati.Ui.Attributes
{
    public class MvcRecaptchaValidationAttribute : ActionFilterAttribute
    {
        private readonly string _errorMessage;
        private readonly string[] _modelErrorKeys;

        public MvcRecaptchaValidationAttribute(string errorMessage = "Recaptcha validation failed.", params string[] modelErrorKeys)
        {
            _errorMessage = errorMessage;
            _modelErrorKeys = modelErrorKeys;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var validator = ServiceLocator.Resolve<IRecaptchaValidatorHelper>();
            var captchaResponse = filterContext.HttpContext.Request.Form["g-recaptcha-response"];
            var isValid = validator.IsValidRecaptcha(captchaResponse);
            if (!isValid)
            {
                RedirectErrorResult(filterContext);
            }

            base.OnActionExecuting(filterContext);
        }

        private void RedirectErrorResult(ActionExecutingContext filterContext)
        {
            foreach (var errorKey in _modelErrorKeys)
            {
                filterContext.Controller.ViewData.ModelState.AddModelError(errorKey, _errorMessage);
            }
        }
    }
}