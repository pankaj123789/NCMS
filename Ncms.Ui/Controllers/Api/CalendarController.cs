using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/calendar")]
    public class CalendarController : BaseApiController
    {
        private readonly IApplicationService _applicationService;

        public CalendarController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        [Route("events")]
        public HttpResponseMessage Events([FromUri]CredentialTestSearchRequestModel request)
        {
            return this.CreateResponse(() => _applicationService.GetAllCredentialTests(request));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        [Route("testlocation")]
        public HttpResponseMessage TestLocation()
        {
            return this.CreateResponse(() => _applicationService.GetLookupType(LookupType.TestLocation.ToString()));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Venue)]
        [Route("venue")]
        public HttpResponseMessage Venue([FromUri]IEnumerable<int> testLocation)
        {
            return this.CreateResponse(() => _applicationService.GetVenue(testLocation));
        }
    }
}
