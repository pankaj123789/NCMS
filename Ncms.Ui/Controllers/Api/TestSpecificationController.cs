using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.User;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/testspecification")]
    public class TestSpecificationController : BaseApiController
    {
        private readonly ITestSpecificationService _testSpecificationService;
        private readonly ITestSpecificationSpreadsheetService _testSpecificationSpreadsheetService;
        private readonly IUserService _userService;

        public TestSpecificationController(ITestSpecificationService testSpecificationService, ITestSpecificationSpreadsheetService testSpecificationSpreadsheetService, IUserService userService)
        {
            _testSpecificationService = testSpecificationService;
            _testSpecificationSpreadsheetService = testSpecificationSpreadsheetService;
            _userService = userService;
        }

        public HttpResponseMessage Get(int? id = null)
        {
            return this.CreateSearchResponse(() => _testSpecificationService.Get(id ?? 0));
        }

        [Route("testComponents/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSpecification)]
        public HttpResponseMessage GetTestComponents(int id)
        {
            return this.CreateSearchResponse(() => _testSpecificationService.GetTestComponentsBySpecificationId(id));
        }

        [HttpGet]
        [Route("canUpload/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSpecification)]
        public HttpResponseMessage CanUpload(int id)
        {
            return this.CreateResponse(() => _testSpecificationService.CanUpload(id));
        }

        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSpecification)]
        public HttpResponseMessage Post([FromBody]TestSpecificationModel model)
        {
            return this.CreateResponse(() => _testSpecificationService.UpdateTestSpecification(model));
        }

        [HttpGet]
        [Route("documentTypes")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Document)]
        public HttpResponseMessage DocumentTypes()
        {
            return this.CreateSearchResponse(() => _testSpecificationService.GetDocumentTypesForTestSpecificationType());
        }

        [HttpGet]
        [Route("documents/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSpecification)]
        public HttpResponseMessage DocumentsGet([FromUri]int id)
        {
            return this.CreateSearchResponse(() => _testSpecificationService.GetAttachments(id));
        }

        [Route("documents/upload")]
        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.Document)]
        public async Task<HttpResponseMessage> Upload()
        {
            return await this.ProcessMultipartFileData((fileName, fileData, provider) =>
            {
                var testSpecificationId = Convert.ToInt32(provider.FormData["testSpecificationId"]);
                var id = Convert.ToInt32(provider.FormData["id"]);
                var type = (StoredFileType)Enum.Parse(typeof(StoredFileType), provider.FormData["type"]);
                var title = provider.FormData["title"];
                var storedFileId = Convert.ToInt32(provider.FormData["storedFileId"]);
                var mergeDocument = Convert.ToBoolean(provider.FormData["mergeDocument"]);
                var availableForExaminers = Convert.ToBoolean(provider.FormData["availableForExaminers"]);

                var user = _userService.Get() ?? new UserModel();
                var request = new TestSpecificationAttachmentModel
                {
                    Id = id,
                    FileName = fileName,
                    Type = type,
                    FilePath = fileData.LocalFileName,
                    UploadedByUserId = user.Id,
                    StoragePath = $@"{type}\{testSpecificationId}\{fileName}",
                    StoredFileId = storedFileId,
                    TestSpecificationId = testSpecificationId,
                    TestSpecificationAttachmentId = id,
                    Title = title,
                    MergeDocument = mergeDocument,
                    EportalDownload = availableForExaminers
                };

                return _testSpecificationService.CreateOrReplaceAttachment(request);
            });
        }

        [HttpPost]
        [Route("addTestSpecification")]
        [NcmsAuthorize(SecurityVerbName.Create, SecurityNounName.TestSpecification)]
        public HttpResponseMessage AddTestSpecification(AddTestSpecificationRequest model)
        {
            return this.CreateResponse(() => _testSpecificationService.AddTestSpecification(model));
        }

        [HttpPost]
        [Route("documents")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSpecification)]
        public HttpResponseMessage DocumentsPost(TestSpecificationAttachmentModel request)
        {
            var user = _userService.Get() ?? new UserModel();
            request.UploadedByUserId = user.Id;
            request.UpdateStoredFileId = request.StoredFileId;
            request.TestSpecificationAttachmentId = request.Id ?? 0;
            return this.CreateResponse(() => _testSpecificationService.CreateOrReplaceAttachment(request));
        }

        [HttpDelete]
        [Route("documents/{id}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSpecification)]
        public HttpResponseMessage DocumentsDelete([FromUri]int id)
        {
            return this.CreateResponse(() => _testSpecificationService.DeleteAttachment(id));
        }

        [Route("testSpecifications")]
        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.TestSpecification)]
        public async Task<HttpResponseMessage> TestSpecificationUpload()
        {
            var filePath = string.Empty;
            var testSpecificationName = string.Empty; ;
            var verifyOnly = true;
            var response = await this.ProcessMultipartFileData((Func<string, MultipartFileData, MultipartFormDataStreamProvider, int>)((fileName, fileData, provider) =>
            {
                foreach (var dataContent in provider.Contents)
                {
                    var name = dataContent.Headers.ContentDisposition.Name;
                    if(name.Equals("\"id\""))
                    {
                        var value = dataContent.ReadAsStringAsync().Result;
                        var testSpecificationId = Convert.ToInt16(value);
                        var result = _testSpecificationSpreadsheetService.GetTestSpecificationDescription(testSpecificationId);
                        if (!result.Success)
                        {
                            return 500;
                        }
                        testSpecificationName = result.Data;
                    }
                    if (name.Equals("\"verifyOnly\""))
                    {
                        var value = dataContent.ReadAsStringAsync().Result;
                        verifyOnly = Convert.ToBoolean(value);
                    }
                }
                filePath = fileData.LocalFileName;
                File.Copy(filePath, $"{filePath}.xlsx");
                //not sure if it matters what is returned here
                return 0;
            }));
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return response;
            }
            return this.CreateResponse(() => _testSpecificationSpreadsheetService.UploadTestSpecifications($"{filePath}.xlsx", testSpecificationName, verifyOnly));
        }

        [HttpGet]
        [Route("testSpecifications")]
        [NcmsAuthorize(SecurityVerbName.Download, SecurityNounName.TestSpecification)]
        public HttpResponseMessage DownloadTestSpecifications(string testSpecificationName)
        {
            return this.CreateResponse(() => _testSpecificationSpreadsheetService.GetTestSpecifications(testSpecificationName));
        }
    }
}
