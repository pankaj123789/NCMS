using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/rubricconfiguration")]
    public class RubricConfigurationController : BaseApiController
    {
        private readonly ITestSpecificationService _testSpecificationService;

        public RubricConfigurationController(ITestSpecificationService testSpecificationService)
        {
            _testSpecificationService = testSpecificationService;
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.RubricResult)]
        public HttpResponseMessage Get(int id)
        {
            return this.CreateResponse(() => _testSpecificationService.GetRubricConfiguration(id));
        }

        [HttpGet]
        [Route("questionpassrule/{testSpecificationId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSpecification)]
        public HttpResponseMessage GetQuestionPassRule(int testSpecificationId)
        {
            return this.CreateResponse(() => _testSpecificationService.GetQuestionPassRules(testSpecificationId));
        }

        [HttpPost]
        [Route("questionpassrule/{testSpecificationId}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSpecification)]
        public HttpResponseMessage PostQuestionPassRule(int testSpecificationId, IEnumerable<RubricQuestionPassRuleModel> request)
        {
            return this.CreateResponse(() => _testSpecificationService.SaveQuestionPassRules(testSpecificationId, request));
        }

        [HttpGet]
        [Route("testbandrule/{testSpecificationId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSpecification)]
        public HttpResponseMessage GetTestBandRule(int testSpecificationId)
        {
            return this.CreateResponse(() => _testSpecificationService.GetTestBandRules(testSpecificationId));
        }

        [HttpPost]
        [Route("testbandrule/{testSpecificationId}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSpecification)]
        public HttpResponseMessage PostTestBandRule(int testSpecificationId, IEnumerable<RubricTestBandRuleModel> request)
        {
            return this.CreateResponse(() => _testSpecificationService.SaveTestBandRules(testSpecificationId, request));
        }

        [HttpGet]
        [Route("testquestionrule/{testSpecificationId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSpecification)]
        public HttpResponseMessage GetTestQuestionRule(int testSpecificationId)
        {
            return this.CreateResponse(() => _testSpecificationService.GetTestQuestionRules(testSpecificationId));
        }

        [HttpPost]
        [Route("testquestionrule/{testSpecificationId}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSpecification)]
        public HttpResponseMessage PosttTestQuestionRule(int testSpecificationId, IEnumerable<RubricTestQuestionRuleModel> request)
        {
            return this.CreateResponse(() => _testSpecificationService.SaveTestQuestionRules(testSpecificationId, request));
        }

        [HttpGet]
        [Route("markingband/{rubricMarkingBandId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.RubricResult)]
        public HttpResponseMessage GetMarkingBand(int rubricMarkingBandId)
        {
            return this.CreateResponse(() => _testSpecificationService.GetMarkingBand(rubricMarkingBandId));
        }

        [HttpPost]
        [EncodeScriptOnly]
        [Route("markingband")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSpecification)]
        public HttpResponseMessage UpdateMarkingBand(RubricMarkingBandModel rubricMarkingBandModel)
        {
            return this.CreateResponse(() => _testSpecificationService.UpdateMarkingBand(rubricMarkingBandModel));
        }
    }
}
