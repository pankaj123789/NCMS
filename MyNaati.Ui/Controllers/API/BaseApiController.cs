using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;

namespace MyNaati.Ui.Controllers.API
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BaseApiController : ApiController
    {
        protected HttpResponseMessage GetErrors()
        {
            var errors = new List<string>();
            foreach (var state in ModelState)
            {
                errors.AddRange(state.Value.Errors.Select(error => error.ErrorMessage));
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, errors);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage ResponseMessage(Func<HttpResponseMessage> action)
        {
            try
            {
                return action.Invoke();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        protected string GetRemoteIpAddress()
        {
            var remoteIpAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(remoteIpAddress))
            {
                remoteIpAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return remoteIpAddress;
        }
    }
}
