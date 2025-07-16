using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Ui.App_Start.Security;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/emailTemplate")]
    public class EmailTemplateController : BaseApiController
    {
        private readonly IEmailTemplateService _messageService;


        public EmailTemplateController(IEmailTemplateService messageService)
        {
            _messageService = messageService;
        }
        
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.EmailTemplate)]
		[HttpGet]
		[Route("search")]
		public HttpResponseMessage Search([FromUri]SearchRequest request)
		{
            return this.CreateResponse(() => _messageService.GetAllEmailTemplates(request));
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.EmailTemplate)]
        public HttpResponseMessage Get(int id)
        {
            return this.CreateResponse(() => _messageService.Get(id));
        }

        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.EmailTemplate)]
        [EncodeScriptOnly]
        [HttpPost]
        [Route("SaveEmailTemplate")]
        public HttpResponseMessage Save(ServiceEmailTemplateModel model)
        {
            return this.CreateResponse(() => _messageService.Save(model));
        }
    }
}
