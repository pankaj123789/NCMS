using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.UserSearch;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    public class UserSearchController : BaseApiController
    {
        private readonly IUserSearchService _userSearchService;

        public UserSearchController(IUserSearchService userSearchService)
        {
            _userSearchService = userSearchService;
        }

        //Test Material Request
        //Test main page
        [NcmsAuthorize(SecurityVerbName.Read, new SecurityNounName[] { SecurityNounName.General})]
        public HttpResponseMessage Get([FromUri]UserSearchListRequest request)
        {
            return this.CreateResponse(() => _userSearchService.List(request));
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.General)]
        public HttpResponseMessage Post([FromBody]UserSearchModel request)
        {
            return this.CreateResponse(() => _userSearchService.Save(request));
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.General)]
        public HttpResponseMessage Delete([FromUri]int id)
        {
            return this.CreateResponse(() => _userSearchService.Delete(id));
        }
    }
}
