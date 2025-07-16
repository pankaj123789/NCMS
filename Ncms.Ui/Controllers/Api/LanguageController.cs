using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/language")]
    public class LanguageController : BaseApiController
    {
        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        //Test main page
        [NcmsAuthorize(SecurityVerbName.Read, new SecurityNounName[] { SecurityNounName.Language, SecurityNounName.TestSession })]
        public HttpResponseMessage Get([FromUri]string request)
        {
            return this.CreateResponse(() => _languageService.List(request));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Language)]
        public HttpResponseMessage English()
        {
            return this.CreateResponse(() => _languageService.English());
        }
    }
}
