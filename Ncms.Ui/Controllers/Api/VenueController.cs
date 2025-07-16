using System;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using Ncms.Contracts.Models.User;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;
namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/venue")]
    public class VenueController : BaseApiController
    {
        private readonly IVenueService _venueService;



        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        //venue-search
        [HttpGet]
        [Route("venueSearch")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Venue)]
        public HttpResponseMessage VenueSearch()
        {
            return this.CreateResponse(() => _venueService.VenueSearch());
        }
    }
}