using System.ServiceModel;
using System.Web.Mvc;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace MyNaati.Ui.Common
{
    public class HandleCustomErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {

            if (((context.Exception as FaultException)?.Reason.ToString() ?? string.Empty) == Constants.INVALID_USER)
            {
                context.Result = new HttpUnauthorizedResult();
                context.ExceptionHandled = true;
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.StatusCode = 500;
                context.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
            base.OnException(context);

            if (!context.ExceptionHandled)
                return;

            LoggingHelper.LogException(context.Exception);
        }
    }
}
