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
    [RoutePrefix("api/panelMembership")]
    public class PanelMembershipController : BaseApiController
    {
        private readonly IPanelService _panelService;
        private readonly ISystemService _systemService;

        public PanelMembershipController(IPanelService panelService, ISystemService systemService)
        {
            _panelService = panelService;
            _systemService = systemService;
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.PanelMember)]
        public HttpResponseMessage Get([FromUri]string request)
        {
            return this.CreateResponse(() => _panelService.ListMembership(request));
        }

        [HttpPost]
        [NcmsAuthorize(new []{ SecurityVerbName.Create, SecurityVerbName.Update }, new[] { SecurityNounName.PanelMember, SecurityNounName.PanelMember })]
        public HttpResponseMessage Post(PanelMembershipModel model)
        {
            return this.CreateResponse(() => _panelService.AddOrUpdateMember(model));
        }

        [HttpPost]
        [Route("Reappoint")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.PanelMember)]
        public HttpResponseMessage Reappoint(ReappointMembersModel reappointMembersModel)
        {
            return this.CreateResponse(() => _panelService.ReappointMembers(reappointMembersModel));
        }

        [HttpGet]
        [Route("{panelMembershipId}/unavailability")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.PanelMember)]
        public HttpResponseMessage Unavailability([FromUri]int panelMembershipId)
        {
            return this.CreateResponse(() => _panelService.GetUnavailability(panelMembershipId));
        }

        [HttpGet]
        [Route("{panelMembershipId}/markingrequests")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.PanelMember)]
        public HttpResponseMessage MarkingRequests([FromUri]int panelMembershipId)
        {
            return this.CreateResponse(() => _panelService.GetMarkingRequests(panelMembershipId));
        }

        [HttpGet]
        [Route("{panelMembershipId}/materialrequests")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Panel)]
        public HttpResponseMessage MaterialRequests([FromUri]int panelMembershipId)
        {
            return this.CreateResponse(() => _panelService.GetMaterialRequests(panelMembershipId));
        }

        [HttpGet]
        [Route("hasPersonEmailAddress/{personId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.PanelMember)]
        public HttpResponseMessage HasPersonEmailAddress([FromUri] int personId)
        {
            return this.CreateResponse(() => _panelService.HasPersonEmailAddress(personId));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.PanelMember)]
        [Route("HasOverlappingMembership")]
        public HttpResponseMessage HasOverlappingMembership(OverlappingRequestModel request)
        {
            return this.CreateResponse(() => _panelService.HasOverlappingMembership(request.Items));
        }

        [HttpGet]
        [Route("hasRolePlayerRatingLocation/{personId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.PanelMember)]
        public HttpResponseMessage HasRolePlayerRatingLocation([FromUri] int personId)
        {
            return this.CreateResponse(() => _panelService.HasRolePlayerRatingLocation(personId));
        }

        [HttpDelete]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.PanelMember)]
        public HttpResponseMessage Delete(int id)
        {
            return this.CreateResponse(() => _panelService.RemoveMember(id));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.PanelMember)]
        [Route("AvailableCredentialTypes/{panelId}/{membershipId}")]
        public HttpResponseMessage GetAvailableCredentialTypes(int panelId, int membershipId)
        {
            return this.CreateResponse(() => _panelService.GetAvailableMembershipCredentialTypes(panelId, membershipId));
        }

        [HttpGet]
        [Route("value/ExaminerRoles")]

        //This permission should not be changed. If a specific value key is required and roles doesn''t have this permission then
        // a new method to get just specific key should be created.
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.PanelMember)]
        public HttpResponseMessage GetExaminerRoles()
        {
            return this.CreateResponse(() => _systemService.GetSystemValue("ExaminerRoles"));
        }
    }
}
