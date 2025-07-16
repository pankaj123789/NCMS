using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/email")]
    public class EmailController : BaseApiController
    {
        private readonly IEmailMessageService _emailMessageService;

        public EmailController(IEmailMessageService emailMessageService)
        {
            _emailMessageService = emailMessageService;
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Email)]
        public HttpResponseMessage GetEmailMessages([FromUri] SearchRequest request)
        {
            return this.CreateResponse(() => _emailMessageService.GetEmailMessageQueue(request));
        }

        [HttpPost]
        [Route("sendEmail")]
        [NcmsAuthorize(SecurityVerbName.Send, SecurityNounName.Email)]
        public HttpResponseMessage SendEmailMessage(dynamic request)
        {
            try
            {
                var emailMessageId = (int)request.emailMessageId;
                return this.CreateResponse(() =>
                {
                    var response = _emailMessageService.SendEmailMessage(emailMessageId);
                    response.Success = !response.Warnings.Any() && !response.Errors.Any();
                    response.Errors.AddRange(response.Warnings);
                    return response;
                });
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }
    }
}
