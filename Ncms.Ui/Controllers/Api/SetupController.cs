using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.System;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;
using SearchRequest = Ncms.Contracts.SearchRequest;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/setup")]
    public class SetupController : BaseApiController
    {
        private readonly ISystemService _systemService;
        private readonly IAccountingService _financeService;
        private readonly ISecretsCacheQueryService _secretsProvider;

        public SetupController(ISystemService systemService, IAccountingService financeService, ISecretsCacheQueryService secretsProvider)
        {
            _systemService = systemService;
            _financeService = financeService;
            _secretsProvider = secretsProvider;
        }



        [HttpGet]
        [Route("language")]
        [NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.Language)]
        public HttpResponseMessage Language([FromUri]SearchRequest request)
        {
            return this.CreateResponse(() => _systemService.LanguageSearch(request));
        }

        [HttpGet]
        [Route("language/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Language)]
        public HttpResponseMessage Language(int id)
        {
            return this.CreateResponse(() => _systemService.GetLanguage(id));
        }

        [HttpPost]
        [Route("language")]
        [NcmsAuthorize(new[] { SecurityVerbName.Create, SecurityVerbName.Update }, new[] { SecurityNounName.Language, SecurityNounName.Language })]
        public HttpResponseMessage Language(LanguageRequest language)
        {
            return this.CreateResponse(() => _systemService.SaveLanguage(language));
        }

        [HttpGet]
        [Route("venue")]
        [NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.Venue)]
        public HttpResponseMessage Venue([FromUri]SearchRequest request)
        {
            return this.CreateResponse(() => _systemService.VenueSearch(request));
        }

        [HttpGet]
        [Route("venue/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Venue)]
        public HttpResponseMessage Venue(int id)
        {
            return this.CreateResponse(() => _systemService.GetVenue(id));
        }

        [HttpPost]
        [Route("venue")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Venue)]
        public HttpResponseMessage CreateVenue(VenueRequest venue)
        {
            return this.CreateResponse(() => _systemService.SaveVenue(venue));
        }

        [HttpPost]
        [Route("saveapiadmin")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.ApiAdministrator)]
        public HttpResponseMessage SaveApiAdmin(ApiAdminRequest request)
        {
            return this.CreateResponse(() => _systemService.SaveApiAdmin(request));
        }

        [HttpGet]
        [Route("apiadmin")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.ApiAdministrator)]
        public HttpResponseMessage ApiAdmin([FromUri] SearchRequest request)
        {
            return this.CreateResponse(() => _systemService.GetApiAdmin(0));
        }

        [HttpGet]
        [Route("apiadmin/{id}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.ApiAdministrator)]
        public HttpResponseMessage ApiAdmin(int id)
        {
            return this.CreateResponse(() => _systemService.GetApiAdmin(id));
        }

        [HttpGet]
        [Route("skill")]
        [NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.Skill)]
        public HttpResponseMessage Skill([FromUri]SearchRequest request)
        {
            return this.CreateResponse(() => _systemService.SkillSearch(request));
        }

        [HttpGet]
        [Route("skill/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Skill)]
        public HttpResponseMessage Skill(int id)
        {
            return this.CreateResponse(() => _systemService.GetSkill(id));
        }

        [HttpPost]
        [Route("skill")]
        [NcmsAuthorize(new[] { SecurityVerbName.Create, SecurityVerbName.Update }, new[] { SecurityNounName.Skill, SecurityNounName.Skill })]
        public HttpResponseMessage Skill(SkillRequest skill)
        {
            return this.CreateResponse(() => _systemService.SaveSkill(skill));
        }

        [HttpGet]
        [Route("newguid")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Skill)]
        public HttpResponseMessage NewGuid()
        {
            return this.CreateResponse(() => Guid.NewGuid().ToString().Replace("-", string.Empty).ToUpper());
        }

        [HttpGet]
        [Route("getapipermissionoptions")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Skill)]
        public HttpResponseMessage GetApiPermissionOptions()
        {
            return this.CreateResponse(() => _systemService.GetApiPermissionOptions());
        }
    }
}
