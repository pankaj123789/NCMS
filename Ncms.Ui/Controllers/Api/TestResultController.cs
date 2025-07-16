using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Job;
using Ncms.Contracts.Models.TestResult;
using Ncms.Ui.Extensions;
using Ncms.Ui.Helpers;
using Ncms.Ui.Security;
using F1Solutions.Global.Common.Logging;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/testresult")]
    public class TestResultController : BaseApiController
    {
        private readonly ITestResultService _testResult;

        public TestResultController(ITestResultService testResultService)
        {
            _testResult = testResultService;
        }
            
        [Route("specification/{id}")]
        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestResult)]
        public HttpResponseMessage Specification([FromUri]int id)
        {
            return this.CreateResponse(() => _testResult.Specification(id, false));
        }

        [Route("testResult/{testResultId}")]
        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestResult)]
        public HttpResponseMessage GetTestResults([FromUri]int testResultId)
        {
            return this.CreateResponse(() => _testResult.GetTestResult(testResultId));
        }

        [Route("calculatetestcomponentresult")]
        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestResult)]
        public HttpResponseMessage CalculateTestComponentResult([FromBody]List<Dictionary<string, object>> dataSet)
        {
            return this.CreateResponse(() => _testResult.CalculateTestComponentResult(dataSet));
        }

        [HttpGet]
        [Route("marks/{testResultId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestResult)]
        public HttpResponseMessage Marks([FromUri]int testResultId)
        {
            return this.CreateResponse(() => _testResult.GetMarks(new GetMarksRequestModel { TestResultId = testResultId }));
        }

        [HttpPost]
        [Route("marks/{testResultId}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestResult)]
        public HttpResponseMessage MarksPost([FromUri]int testResultId, [FromBody]SaveMarksRequestModel request)
        {
            request.TestResultId = testResultId;
            return this.CreateResponse(() => _testResult.SaveMarks(request));
        }

        [HttpPost]
        [Route("update")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestResult)]
        public HttpResponseMessage UpdateTestResult([FromBody]TestResultModel testresult)
        {
           return this.CreateResponse(() => _testResult.UpdateTestResult(testresult));
        }

        [HttpPost]
        [Route("validate")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestResult)]
        public HttpResponseMessage ValidateTestResult([FromBody]TestResultModel testresult)
        {
           return this.CreateResponse(() => _testResult.ValidateTestResult(testresult));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Document)]
        [Route("documents/{testSittingId}")]
        public HttpResponseMessage GetDocuments(int testSittingId)
        {
            return this.CreateResponse(() => _testResult.GetAssetDocuments(testSittingId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Document)]
        [Route("examinerDocuments/{testSittingId}")]
        public HttpResponseMessage GetExaminerDocuments(int testSittingId)
        {
            return this.CreateResponse(() => _testResult.GetExaminerDocuments(testSittingId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Document)]
        [Route("testDocuments/{testSittingId}")]
        public HttpResponseMessage GetTestDocuments(int testSittingId)
        {
            return this.CreateResponse(() => _testResult.GetTestDocuments(testSittingId));
        }

        [HttpDelete]
        [Route("document/{testSittingDocumentId}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestResult)]
        public HttpResponseMessage DeleteDocument(int testSittingDocumentId)
        {
            return this.CreateResponse(() => _testResult.DeleteDocument(testSittingDocumentId));
        }

        [HttpGet]
        [Route("downloaddocuments/{testSittingId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestResult)]
        public HttpResponseMessage BulkDownloadDocuments(int testSittingId)
        {
            return this.FileStreamResponse(() =>
            {
                return _testResult.GetDocumentsAsZip(testSittingId);
            });
        }

        [HttpGet]
        [Route("downloaddocument/{testSittingDocumentId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestResult)]
        public HttpResponseMessage DownloadDocument(int testSittingDocumentId)
        {
            return this.FileStreamResponse(() => _testResult.GetDocument(testSittingDocumentId));
        }

        [HttpPost]
        [Route("document")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSitting, SecurityNounName.TestResult, SecurityNounName.TestMaterial)]
        public HttpResponseMessage PostDocument(CreateOrReplaceTestSittingDocumentModel request)
        {
            return this.CreateResponse(() =>
            {
                _testResult.CreateOrUpdateDocument(request);
            });
        }

        [HttpPost]
        [Route("updateduedate")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestResult)]
        public HttpResponseMessage UpdateDueDate(UpdateDueDateRequestModel request)
        {
            return this.CreateResponse(() => _testResult.UpdateDueDate(request));
        }

        
        [HttpPost]
        [Route("document/upload")]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.TestSitting, SecurityNounName.TestResult, SecurityNounName.TestMaterial, SecurityNounName.Document)]
        public async Task<HttpResponseMessage> Upload()
        {
            return await this.ProcessMultipartFileData((fileName, fileData, provider) =>
            {
                var testAttendanceId = Convert.ToInt32(provider.FormData["testSittingId"]);
                var id = (int?)Convert.ToInt32(provider.FormData["id"]);
                if (id == 0)
                {
                    id = null;
                }

                var type = provider.FormData["type"];
                var title = provider.FormData["title"];
                var storedFileId = Convert.ToInt32(provider.FormData["storedFileId"]);
                var eportalDownload = bool.Parse(provider.FormData["eportalDownload"]);

                LoggingHelper.LogInfo($"Upload Document - title:{title} type:{type} fileName:{fileName} eportalDownload:{eportalDownload} testAttendanceId:{testAttendanceId} LocalFileName:{fileData.LocalFileName}");

                var request = new CreateOrReplaceTestSittingDocumentModel
                {
                    Id = id,
                    Title = title,
                    Type = type,
                    File = fileName,
                    EportalDownload = eportalDownload,
                    TestSittingId = testAttendanceId,
                    StoredFileId = storedFileId,
                    FilePath = fileData.LocalFileName,
                };

                var response = _testResult.CreateOrUpdateDocument(request);

                title = title.Replace("[UniqueID]", response.ToString());
                title = title.Replace("[DocumentType]", Regex.Replace(type, "(\\B[A-Z])", " $1"));

                return response;
            });
        }

        [HttpPost]
        [Route("automationReady")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestResult)]
        public HttpResponseMessage UpdateAutomaticIssuingExaminer(AutomaticIssuingExaminerRequestModel request)
        {
            return this.CreateResponse(() => _testResult.UpdateAutomaticIssuingExaminer(request.TestSittingId, request.AutomaticIssuingExaminer));
        }

        [HttpGet]
        [Route("{testSittingId}/getAutomaticIssuingExaminer")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestResult)]
        public HttpResponseMessage GetAutomaticIssuingExaminer(int testSittingId)
        {
            return this.CreateResponse(() => _testResult.GetAutomaticIssuingExaminer(testSittingId));
        }
        
        [HttpGet]
        [Route("{testSittingId}/getTestSpecificationAutomaticIssuing")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestResult)]
        public HttpResponseMessage GetTestSpecificationAutomaticIssuingByTestSittingId(int testSittingId)
        {
            return this.CreateResponse(() => _testResult.GetTestSpecificationAutomaticIssuingByTestSittingId(testSittingId));
        }
    }
}
