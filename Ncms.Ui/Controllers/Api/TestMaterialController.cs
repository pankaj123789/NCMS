using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Bl;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.TestMaterial;
using Ncms.Contracts.Models.User;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;
using IApplicationService = Ncms.Contracts.IApplicationService;
using ITestMaterialService = Ncms.Contracts.ITestMaterialService;
using SearchRequest = Ncms.Contracts.SearchRequest;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/testMaterial")]
    public class TestMaterialController : BaseApiController
    {
        private readonly ITestMaterialService _testMaterialService;
        private readonly IUserService _user;
        private readonly IApplicationWizardLogicService _applicationWizardLogicService;
        private readonly IApplicationService _applicationService;

        public TestMaterialController(ITestMaterialService testMaterialService, IUserService user,
            IApplicationWizardLogicService applicationWizardLogicService, IApplicationService applicationService)
        {
            _testMaterialService = testMaterialService;
            _user = user;
            _applicationWizardLogicService = applicationWizardLogicService;
            _applicationService = applicationService;
        }


        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestMaterial)]
        [Route("createOrUpdateTestMaterial")]
        public HttpResponseMessage CreateOrUpdateTestMaterial(TestMaterialRequest model)
        {
            return this.CreateResponse(() => _testMaterialService.CreateOrUpdateTestMaterial(model));
        }

        [Route("request")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetRequest([FromUri] string request)
        {
            throw new NotSupportedException("This Implementation was deleted");
            //return this.CreateSearchResponse(() => _testMaterialService.List(request));
        }

        [HttpGet]
        [Route("searchTestMaterials")]
        [NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.TestMaterial)]
        public HttpResponseMessage TestMaterials([FromUri]SearchRequest request)
        {
            return this.CreateResponse(() => _testMaterialService.SearchTestMaterials(request));
        }

        [HttpGet]
        [Route("getTestMaterials/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetTestMaterials(int id)
        {
            return this.CreateResponse(() => _testMaterialService.GetTestMaterials(id));
        }

        [HttpPost]
        [Route("getTestMaterialsByAttendees")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetTestMaterialsByAttendees([FromBody] AssignTestMaterialRequestModel request)
        {
            var idList = request.TestSittingIds.ToList();
            var idsInt = idList.Select(int.Parse).ToList();

            return this.CreateResponse(() => _testMaterialService.GetTestMaterialsByAttendees(idsInt, request.ShowAllMaterials));
        }



        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        [Route("fromTestTask")]
        public HttpResponseMessage GetTestMaterialsFromTestTask([FromUri] int testComponentId, [FromUri] int? skillId, [FromUri] bool? includeSystemValueSkillTypes)
        {
            return this.CreateResponse(() => _testMaterialService.GetTestMaterialsFromTestTask(testComponentId, skillId, includeSystemValueSkillTypes));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        [Route("getIncludeSystemValueSkillNames")]
        public HttpResponseMessage GetIncludeSystemValueSkillNames()
        {
            
            return this.CreateResponse(() => _testMaterialService.GetIncludeSystemValueSkillNames());
        }


        [HttpPost]
        [Route("getExistingTestMaterialIdsByAttendees")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetExistingTestMaterialIdsByAttendees([FromBody] AssignTestMaterialRequestModel request)
        {
            var idList = request.TestSittingIds.ToList();
            var idsInt = idList.Select(int.Parse).ToList();

            var response = _testMaterialService.GetExistingTestMaterialIdsByAttendees(idsInt);

            return this.CreateResponse(() => response);
        }

        [HttpGet]
        [Route("getTestTaskPendingToAssign/{testSessionId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetTestTasksPendingToAssign(int testSessionId)
        {
            return this.CreateResponse(() => _testMaterialService.GetTestTasksPendingToAssign(testSessionId).Data);
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage Get([FromUri] TestMaterialSearchRequestModel request)
        {

            var response = _testMaterialService.GetTestMaterialsByAttendee(request.AttendanceId);

            return this.CreateResponse(() => response);
        }

        [HttpGet]
        [Route("documents/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage DocumentsGet([FromUri] int id)
        {
            return this.CreateResponse(() => _testMaterialService.GetTestMaterialAttachment(id));
        }

        [HttpGet]
        [Route("taskTypes")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetTaskTypes([FromUri]IEnumerable<int> credentialTypeIds)
        {
            return this.CreateResponse(() =>
            {
                var response = _applicationService.GetTestTask(credentialTypeIds).Data.ToList();

                response.ForEach(x =>
                    {
                        if (!x.Active)
                        {
                            x.DisplayName = string.Format(Naati.Resources.Shared.InactiveDisplayName, x.DisplayName);
                        }
                    });
                return response;

            });
        }
        [HttpGet]
        [Route("documentTypes")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Document)]
        public HttpResponseMessage DocumentTypes()
        {
            return this.CreateResponse(() => _testMaterialService.GetDocumentTypesForTestMaterialType());
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestMaterial)]
        [Route("documents")]
        public HttpResponseMessage DocumentsPost(TestMaterialAttachmentRequest request)
        {
            var user = _user.Get() ?? new UserModel();
            request.UploadedByUserId = user.Id;
            request.FileType = (StoredFileType)Enum.Parse(typeof(StoredFileType), request.Type);
            request.UpdateStoredFileId = request.StoredFileId;
            return this.CreateResponse(() => _testMaterialService.CreateOrUpdateTestMaterialAttachment(request));
        }


        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Assign, SecurityNounName.TestMaterial)]
        [Route("assignMaterial")]
        public HttpResponseMessage AssignMaterialPost([FromBody] AssignTestMaterialModel model)
        {
            return this.CreateResponse(() => _testMaterialService.AssignMaterial(model));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.TestMaterial)]
        [Route("removeTestMaterials")]
        public HttpResponseMessage RemoveTestMaterialsPost([FromBody] AssignTestMaterialModel model)
        {
            return this.CreateResponse(() => _testMaterialService.RemoveTestMaterials(model));
        }

        [HttpGet]
        [Route("bulkDownloadTestMaterial/{testSessionId}/{includeExaminer}")]
        [NcmsAuthorize(SecurityVerbName.Download, SecurityNounName.TestMaterial)]
        public HttpResponseMessage BulkDownloadTestMaterial(int testSessionId, bool includeExaminer = false)
        {
            return this.CreateResponse(() => _testMaterialService.BulkDownloadTestMaterial(testSessionId, includeExaminer));
        }

        //public static void GenerateZipAndNotifyCaller(int testSessionId, bool includeExaminer, string userName)
        //{
        //    Startup.SetContext();
        //    var testMaterialService = ServiceLocator.Resolve<ITestMaterialService>();
        //    try
        //    {
        //        var response = testMaterialService.BulkDownloadTestMaterial(testSessionId, includeExaminer);
        //        MessageHub.Success(userName, String.Format(Naati.Resources.TestMaterial.YourMaterialsAreReady, response.Data));
        //    }
        //    catch (UserFriendlySamException ex)
        //    {
        //        MessageHub.Error(userName, String.Format(Naati.Resources.TestMaterial.CouldNotToProcessMaterials, ex.Message));
        //    }
        //}

        [HttpPost]
        [Route("documents/upload")]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.TestMaterial)]
        public async Task<HttpResponseMessage> CreateOrUpdateTestMaterialAttachment()
        {
            var user = _user.Get() ?? new UserModel();
            return await this.ProcessMultipartFileData((fileName, fileData, provider) =>
            {
                var id = Convert.ToInt32(provider.FormData["id"]);
                var testMaterialId = Convert.ToInt32(provider.FormData["testMaterialId"]);
                var type = provider.FormData["type"];
                var title = provider.FormData["title"];
                var storedFileId = Convert.ToInt32(provider.FormData["storedFileId"]);
                var availableForExaminers = Convert.ToBoolean(provider.FormData["availableForExaminers"]);
                var mergeDocument = Convert.ToBoolean(provider.FormData["mergeDocument"]);
                //var deleted = bool.Parse(provider.FormData["deleted"]);

                var request = new TestMaterialAttachmentRequest
                {
                    Title = title,
                    FileType = (StoredFileType)Enum.Parse(typeof(StoredFileType), type),
                    FilePath = fileData.LocalFileName,
                    UploadedByUserId = user.Id,
                    StoragePath = $@"{type}\{testMaterialId}\{fileName}",
                    UpdateStoredFileId = storedFileId,
                    AvailableForExaminers = availableForExaminers,
                    MergeDocument = mergeDocument,
                    Id = id,
                    //Deleted = deleted,
                    TestMaterialId = testMaterialId,
                };
                var response = _testMaterialService.CreateOrUpdateTestMaterialAttachment(request);
                return response.StoredFileId;
            });
        }

        [HttpDelete]
        [Route("documents/{id}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestMaterial)]
        public HttpResponseMessage DocumentsDelete([FromUri] int id)
        {
            return this.CreateResponse(() => _testMaterialService.DeleteAttachment(id));
        }

        [HttpDelete]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.TestMaterial)]
        public HttpResponseMessage Delete(int id)
        {
            return this.CreateResponse(() => _testMaterialService.DeleteMaterialById(id));
        }


        [HttpPost]
        [Route("getSteps")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetSteps(TestMaterialBulkAssignmentRequest request)
        {
            return this.CreateResponse(() => _applicationWizardLogicService.GetTestMaterialWizardSteps(request));
        }

        [HttpPost]
        [Route("getSupplementaryTests")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetSupplemenntaryTests(TestMaterialBulkAssignmentRequest request)
        {

            return this.CreateResponse(() => _testMaterialService.GetSupplementaryTests(request));
        }

        [HttpPost]
        [Route("getTestSpecifications")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetTestSpecifications(TestMaterialBulkAssignmentRequest request)
        {
            return this.CreateResponse(() => _testMaterialService.GetTestSpecificationDetails(request));
        }

        [HttpPost]
        [Route("testSpecifications")]
        [NcmsAuthorize(new[] { SecurityVerbName.Update, SecurityVerbName.Assign }, SecurityNounName.TestMaterial)]
        public HttpResponseMessage ValidateTestSpecification(TestMaterialBulkAssignmentRequest request)
        {
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _testMaterialService.ValidateTestSpecification(request).Data.ToList();
                if (validationErrors.Any())
                {
                    return this.CreateResponse(() => new { InvalidFields = validationErrors });
                }
                return sucessResponse;
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("getTestSpecificationSkills")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetTestSpecificationSkills(TestMaterialBulkAssignmentRequest request)
        {
            return this.CreateResponse(() => _testMaterialService.GetTestSpecificationSkills(request));
        }

        [HttpPost]
        [Route("testSpecificationSkills")]
        [NcmsAuthorize(new[] { SecurityVerbName.Update, SecurityVerbName.Assign }, SecurityNounName.TestMaterial)]
        public HttpResponseMessage ValidateTestSpecificationSkills(TestMaterialBulkAssignmentRequest request)
        {
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _testMaterialService.ValidateSkill(request).Data.ToList();
                if (validationErrors.Any())
                {
                    return this.CreateResponse(() => new { InvalidFields = validationErrors });
                }
                return sucessResponse;
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }

        }

        [HttpPost]
        [Route("getTestSpecificationMaterials")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetTestMaterials(PagedTestMaterialBulkAssignmentRequest request)
        {
            return this.CreateResponse(() => _testMaterialService.GetTestMaterials(request));
        }

        [HttpPost]
        [Route("getTestSpecificationTasks")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetTestTasks(TestMaterialBulkAssignmentRequest request)
        {
            return this.CreateResponse(() => _testMaterialService.GetTestTasks(request));
        }

        [HttpPost]
        [Route("testSpecificationMaterials")]
        [NcmsAuthorize(SecurityVerbName.Validate, SecurityNounName.TestMaterial)]
        public HttpResponseMessage ValidateTestSpecificationMaterials(TestMaterialBulkAssignmentRequest request)
        {
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _testMaterialService.ValidateTestMaterials(request).Data.ToList(); ;
                if (validationErrors.Any())
                {
                    return this.CreateResponse(() => new { InvalidFields = validationErrors });
                }
                return sucessResponse;
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }



        [HttpPost]
        [Route("getTestMaterialSummary")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetTestMaterialSummay(TestMaterialBulkAssignmentRequest request)
        {
            return this.CreateResponse(() => _testMaterialService.GetTestMaterialsSummary(request));
        }


        [HttpPost]
        [Route("getExaminersAndRolePlayers")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetExaminersAndRolePlayers(TestMaterialBulkAssignmentRequest request)
        {
            return this.CreateResponse(() => _testMaterialService.GetExaminersAndRolePlayers(request));
        }

        [HttpPost]
        [Route("testMaterialSummary")]
        [NcmsAuthorize(SecurityVerbName.Validate, SecurityNounName.TestMaterial)]
        public HttpResponseMessage ValidateTestMaterialSummary(TestMaterialBulkAssignmentRequest request)
        {
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _testMaterialService.ValidateTestMaterialSummary(request).Data.ToList(); ;
                if (validationErrors.Any())
                {
                    return this.CreateResponse(() => new { InvalidFields = validationErrors });
                }
                return sucessResponse;
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("wizard")]
        [NcmsAuthorize(new[] { SecurityVerbName.Update, SecurityVerbName.Assign }, SecurityNounName.TestMaterial)]
        public HttpResponseMessage PostWizard(dynamic request)
        {
            try
            {
                var wizardModel = new TestMaterialAssignmentBulkModel { Data = request };
                return this.CreateResponse(() => _applicationService.PerformTestMaterialAssignementBulkAction(wizardModel));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpGet]
        [Route("downloadFromNotification")]
        [NcmsAuthorize(SecurityVerbName.Download, SecurityNounName.TestMaterial)]
        public HttpResponseMessage GetSasTokenFromNotification(int notificationId)
        {
            return this.CreateResponse(() => _testMaterialService.GetTestMaterialUriFromBackgroundOperation(notificationId));
        }
    }
}
