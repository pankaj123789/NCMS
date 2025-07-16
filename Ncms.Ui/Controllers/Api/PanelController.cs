using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Panel;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/panel")]
    public class PanelController : BaseApiController
    {
        private readonly IPanelService _panelService;

        public PanelController(IPanelService panelService)
        {
            _panelService = panelService;
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Panel)]
        public HttpResponseMessage Get([FromUri]string request)
        {
            return this.CreateSearchResponse(() => _panelService.List(request));
        }

        [HttpPost]
        [NcmsAuthorize(new []{ SecurityVerbName.Update, SecurityVerbName.Create }, SecurityNounName.Panel)]
        public HttpResponseMessage Post(PanelModel model)
        {
            return this.CreateResponse(() => _panelService.CreateOrUpdate(model));
        }

        [HttpDelete]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Panel)]
        public HttpResponseMessage Delete(int id)
        {
            return this.CreateResponse(() => _panelService.Delete(id));
        }

        [HttpGet]
        [Route("PanelTypes")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Panel)]
        public HttpResponseMessage PanelTypes()
        {
            return this.CreateResponse(() => _panelService.GetPanelTypes());
        }

        [HttpPost]
        [Route("GetPanelMembershipLookup")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Panel)]
        public HttpResponseMessage GetPanelMembershipLookup(GetPanelMemberLookupRequestModel request)
        {
            return this.CreateResponse(() => _panelService.GetPanelMembershipLookUp(request));
        }
    }
}
