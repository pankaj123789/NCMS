using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Examiner;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;
using Newtonsoft.Json;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/examiner")]
    public class ExaminerController : BaseApiController
    {
        private readonly IExaminerService _examinerService;

        public ExaminerController(IExaminerService examinerService)
        {
            _examinerService = examinerService;
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Examiner)]
        public HttpResponseMessage Get(string request)
        {
            var examinersRequest = JsonConvert.DeserializeObject<GetJobExaminersRequestModel>(request);

            return  this.CreateResponse(() => _examinerService.GetTestExaminers(examinersRequest));
               
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Examiner)]
        [Route("{language1Id}/{language2Id}/{credentialTypeId}/{testAttendanceId}/byLanguageAndCredentialType")]
        public HttpResponseMessage GetByLanguageAndCredentialType(int language1Id, int language2Id, int credentialTypeId, int testAttendanceId)
        {
            var examinersRequest =
                new GetExaminersByLanguageRequestModel {Language1Id = language1Id, Language2Id = language2Id , CredentialTypeId = credentialTypeId, TestAttendanceId = testAttendanceId };
            return this.CreateResponse(() => _examinerService.GetActiveExaminersByLanguageAndCredentialType(examinersRequest));

        }

        [HttpGet]
        [Route("{jobExaminerId}/marks/{testResultId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.ExaminerMarks)]
        public HttpResponseMessage Marks([FromUri]int jobExaminerId, [FromUri]int testResultId)
        {
            return this.CreateResponse(() => _examinerService.GetMarks(new GetExaminerMarksRequestModel { JobExaminerId = jobExaminerId, TestResultId = testResultId }));
        }

        [HttpPost]
        [Route("{jobExaminerId}/marks/{testResultId}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.ExaminerMarks)]
        public HttpResponseMessage MarksPost([FromUri]int jobExaminerId, [FromUri]int testResultId, [FromBody]SaveExaminerMarksRequestModel request)
        {
            request.JobExaminerId = jobExaminerId;
            request.TestResultId = testResultId;
            return this.CreateResponse(() => _examinerService.SaveMarks(request));
        }

        [HttpPost]
        [Route("countMarks")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestResult)]
        public HttpResponseMessage MarksPost([FromBody]UpdateCountMarksRequestModel request)
        {
            return this.CreateResponse(() => _examinerService.UpdateCountMarks(request));
        }

        [HttpDelete]
        [Route("examiner/{jobExaminerId}")]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Examiner)]
        public HttpResponseMessage Examiner(int jobExaminerId)
        {
            return this.CreateResponse(() => _examinerService.RemoveJobExaminer(jobExaminerId));
        }

        [HttpGet]
        [Route("examiner/{jobExaminerId}/{includeMarks}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Examiner)]
        public HttpResponseMessage GetExaminer(int jobExaminerId, bool includeMarks)
        {
            return this.CreateResponse(() => _examinerService.GetJobExaminer(jobExaminerId, includeMarks));
        }
    }
}
