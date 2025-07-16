using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Test;
using Ncms.Ui.Security;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/test")]
    public class TestController : BaseApiController
    {
        private readonly ITestService _testService;
        private readonly INoteService _noteService;

        public TestController(ITestService test, INoteService noteService)
        {
            _testService = test;
            _noteService = noteService;
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSitting)]
        public HttpResponseMessage Get([FromUri] TestSearchRequest request)
        {
            return this.CreateSearchResponse(() => _testService.List(request));
        }


        [HttpPost]
        [Route("examiner")]
        [NcmsAuthorize(SecurityVerbName.Create, SecurityNounName.Examiner)]
        public HttpResponseMessage Examiner([FromBody]AddExaminerRequestModel request)
        {
            return this.CreateResponse(() => _testService.AddExaminers(request));
        }

        [HttpPost]
        [Route("examiner/update")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Examiner)]
        public HttpResponseMessage UpdateExaminers([FromBody]UpdateExaminersRequestModel request)
        {
            return this.CreateResponse(() => _testService.UpdateExaminers(request));
        }

        [HttpGet]
        [Route("{id}/summary")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSitting)]
        public HttpResponseMessage GetSummary(int id)
        {
            return this.CreateResponse(() => _testService.GetTestSummary(id));
        }

        [HttpGet]
        //not implemented
        [Route("documents/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSitting)]
        public HttpResponseMessage GetDocuments(int id)
        {
            return this.CreateResponse(() => _testService.GetTestDocuments(id));
        }

        [HttpGet]
        //not implemented
        [Route("assets/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSitting)]
        public HttpResponseMessage GetAssets(int id)
        {
            return this.CreateResponse(() => _testService.GetTestAssets(id));
        }

        [HttpGet]
        //not implemented
        [Route("paidreview/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSitting)]
        public HttpResponseMessage GetPaidReview(int id)
        {
            return this.CreateResponse(() => _testService.GetTestPaidReview(id));
        }

        [Route("notes/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSitting)]
        public HttpResponseMessage GetNotes(int id)
        {
            return this.CreateResponse(() => _noteService.ListTestNotes(id).OrderByDescending(x => x.CreatedDate));
        }

        [HttpPost]
        [Route("notes")]
        [NcmsAuthorize(new[] { SecurityVerbName.Create, SecurityVerbName.Update }, SecurityNounName.Notes)]
        public HttpResponseMessage Post(TestNoteModel model)
        {
            model.UserId = CurrentUser.Id;
            return this.CreateResponse(() => _noteService.CreateTestNote(model));
        }

        [HttpDelete]
        [Route("notes/{id}/{testSittingId}")]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Notes)]
        public HttpResponseMessage Delete(int id, int testSittingId)
        {
            return this.CreateResponse(() => _noteService.DeleteTestNote(testSittingId, id));
        }

        [HttpPost]
        [Route("rubricmarks")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.RubricResult)]
        public HttpResponseMessage PostRubricMarks(TestRubricModel model)
        {
            return this.CreateResponse(() => _testService.SaveExaminerRubricMarking(model));
        }

        [HttpGet]
        [Route("rubricmarks/{jobExaminerId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.RubricResult)]
        public HttpResponseMessage GetRubricMarkings(int jobExaminerId)
        {
            return this.CreateResponse(() => _testService.GetExaminerRubricMarking(jobExaminerId));
        }

        [HttpGet]
        [Route("rubricfinal/{testResultId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.RubricResult)]
        public HttpResponseMessage GetRubricFinal(int testResultId)
        {
            return this.CreateResponse(() => _testService.GetTestResultRubricMarking(testResultId));
        }

        [HttpPost]
        [Route("rubricfinal")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.RubricResult)]
        public HttpResponseMessage PostRubricFinal(TestRubricModel model)
        {
            return this.CreateResponse(() => _testService.SaveTestResultRubricMarking(model));
        }

        [HttpGet]
        [Route("computeRubricResults/{jobExaminerId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.RubricResult)]
        public HttpResponseMessage ComputeRubricResults(int jobExaminerId)
        {
            return this.CreateResponse(() => _testService.ComputeRubricResults(jobExaminerId));
        }

        [HttpGet]
        [Route("computeFinalRubric/{testResultId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.RubricResult)]
        public HttpResponseMessage ComputeFinalRubric(int testResultId)
        {
            return this.CreateResponse(() => _testService.ComputeFinalRubric(testResultId));
        }

        [HttpGet]
        [Route("feedback/{testAttendanceId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSitting)]
        public HttpResponseMessage Feedback(int? testAttendanceId)
        {
            return this.CreateResponse(() => _testService.Feedback(testAttendanceId));
        }
    }
}
