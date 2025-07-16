using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Security;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace MyNaati.Ui.Attributes
{
    public class HttpRecaptchaValidationAttribute : ActionFilterAttribute
    {
        private readonly string _errorMessage;

        public HttpRecaptchaValidationAttribute(string errorMessage = "Invalid recaptcha response.")
        {
            _errorMessage = errorMessage;
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var validator = ServiceLocator.Resolve<IRecaptchaValidatorHelper>();
            var captchaResponse = actionExecutedContext.Request.GetQueryNameValuePairs().FirstOrDefault(v => v.Key == "g-recaptcha-response").Value;
            var isValid = validator.IsValidRecaptcha(captchaResponse);
            if (!isValid)
            {
                RedirectErrorResult(actionExecutedContext);
            }
            return Task.FromResult(0);
        }

        private void RedirectErrorResult(HttpActionExecutedContext context)
        {
            context.Exception = new InvalidOperationException(_errorMessage);
        }
    }
}