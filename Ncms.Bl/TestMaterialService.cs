using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Aspose.Words;
using Aspose.Words.Replacing;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Contracts.Security;
using Hangfire;
using Ncms.Contracts;
using Ncms.Contracts.Models;

using Ncms.Contracts.Models.Common;
using Ncms.Contracts.Models.TestMaterial;
using GetExaminersAndRolePlayersResponse = Ncms.Contracts.GetExaminersAndRolePlayersResponse;
using SearchRequest = Ncms.Contracts.SearchRequest;

namespace Ncms.Bl
{
    public class TestMaterialService : ITestMaterialService
    {
        private readonly ITestMaterialQueryService _testMaterialQueryService;
        private readonly IBackgroundOperationScheduler _backgroundOperationScheduler;
        private readonly INotificationQueryService _notificationQueryService;
        private readonly IUserService _userService;
        private readonly ISharedAccessSignature _sasAccessSignature;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public TestMaterialService(ITestMaterialQueryService testMaterialQueryService,
            IBackgroundOperationScheduler backgroundOperationScheduler,
            INotificationQueryService notificationQueryService, IUserService userService, ISharedAccessSignature sasAccessSignature, IAutoMapperHelper autoMapperHelper)
        {
            _testMaterialQueryService = testMaterialQueryService;
            _backgroundOperationScheduler = backgroundOperationScheduler;
            _notificationQueryService = notificationQueryService;
            _userService = userService;
            _sasAccessSignature = sasAccessSignature;
            _autoMapperHelper = autoMapperHelper;
        }

        public GenericResponse<IEnumerable<TestMaterialSearchModel>> SearchTestMaterials(SearchRequest request)
        {
            var filters = request.Filter.ToFilterList<TestMaterialSearchCriteria, TestMaterialFilterType>();
            var getRequest = _autoMapperHelper.Mapper.Map<TestMaterialSearchRequest>(request);
            getRequest.Filter = filters;

            var results =
                _testMaterialQueryService.SearchTestMaterials(getRequest)
                    .Results
                    .ToList();

            var testMaterials = results.Select(_autoMapperHelper.Mapper.Map<TestMaterialSearchModel>);

            return new GenericResponse<IEnumerable<TestMaterialSearchModel>> { Data = testMaterials };
        }

        private TestMaterialResponse ToTestMaterialResponseModel(TestMaterialDto model)
        {
            var availability = !model.TestSpecificationActive ? "(Unavailable)" : string.Empty;

            return new TestMaterialResponse
            {
                Id = model.Id,
                LanguageId = model.LanguageId,
                CredentialTypeId = model.CredentialTypeId,
                TaskType = $"{model.TestComponentTypeName} - SPEC{model.TestSpecificationId} {availability}",
                TestComponentTypeId = model.TestComponentTypeId,
                SkillId = model.SkillId,
                Available = model.Available,
                CredentialType = model.CredentialType,
                HasFile = model.HasFile,
                Language = model.Language,
                Title = model.Title,
                Notes = model.Notes,
                TestSittingTestMaterialId = model.TestSittingTestMaterialId,
                TestSittingId = model.TestSittingId,
                SourceTestMaterialId = model.SourceTestMaterialId,

                TestSpecificationId = model.TestSpecificationId,
                TestSpecificationActive = model.TestSpecificationActive,
                TestMaterialDomainId = model.TestMaterialDomainId,
                IsTestMaterialTypeSource = model.TestMaterialTypeId == (int)TestMaterialTypeName.Source
            };
        }

        public IEnumerable<TestMaterialResponse> GetTestMaterialsByAttendees(IList<int> ids, bool showAll = false)
        {
            var testMaterialResponse = _testMaterialQueryService.GetTestMaterialsByAttendees(ids, showAll).Results.Select(ToTestMaterialResponseModel);
            return testMaterialResponse;
        }

        public IEnumerable<TestMaterialResponse> GetTestMaterialsByAttendee(int id)
        {
            var testMaterialResponse = _testMaterialQueryService.GetTestMaterialsByAttendee(id).Results.Select(ToTestMaterialResponseModel);
            return testMaterialResponse;
        }

        public IList<int> GetExistingTestMaterialIdsByAttendees(IList<int> ids)
        {
            var existingTestMaterialIdsResponse = _testMaterialQueryService.GetExistingTestMaterialIdsByAttendees(ids);
            return existingTestMaterialIdsResponse;
        }

        public TestMaterialResponse GetTestMaterials(int id)
        {
            return ToTestMaterialResponseModel(_testMaterialQueryService.GetTestMaterials(id));
        }
        public GenericResponse<IEnumerable<TestMaterialAttachMentResponse>> GetTestMaterialAttachment(int testMaterialId)
        {
            var attachment = _testMaterialQueryService.GetTestMaterialAttachment(testMaterialId).Select(ToTestMaterialAttachMentResponse);
            return new GenericResponse<IEnumerable<TestMaterialAttachMentResponse>>(attachment);
        }

        private TestMaterialAttachMentResponse ToTestMaterialAttachMentResponse(TestMaterialAttachmentDto attachment)
        {
            return new TestMaterialAttachMentResponse()
            {
                Title = attachment.Title,
                FileType = Path.GetExtension(attachment.StoredFile.FileName),
                FileName = attachment.StoredFile.FileName,
                StoredFileId = attachment.StoredFile.Id,
                FileSize = attachment.StoredFile.FileSize,
                DocumentType = attachment.StoredFile.DocumentType.DisplayName,
                DocumentTypeId = attachment.StoredFile.DocumentType.Id,
                UploadedDateTime = attachment.UploadedDateTime,
                UploadedByName = attachment.UploadedBy,
                MaterialId = attachment.Id,
                EportalDownload = attachment.EportalDownload,
                MergeDocument = attachment.MergeDocument,
                SoftDeleteDate = attachment.StoredFile.StoredFileStatusType != 1 ? attachment.StoredFile.StoredFileStatusChangedDate : (DateTime?)null
            };
        }

        public void DeleteAttachment(int storedFileId)
        {
            _testMaterialQueryService.DeleteAttachment(storedFileId);
        }

        public GenericResponse<IEnumerable<PersonTestTask>> GetTestTasksPendingToAssign(int testSessionId)
        {
            var result = _testMaterialQueryService.GetTestTasksPendingToAssign(testSessionId).Select(_autoMapperHelper.Mapper.Map<PersonTestTask>).ToList();
            return new GenericResponse<IEnumerable<PersonTestTask>>(result);
        }

        public void DeleteMaterialById(int id)
        {
            _testMaterialQueryService.DeleteMaterialById(id);
        }

        public void AssignMaterial(AssignTestMaterialModel model)
        {
            var request = new AssignTestMaterialRequest
            {
                TestSittingIds = model.TestSittingIds.ToList().Distinct(),
                TestComponentIds = model.TestComponentIds
            };
            _testMaterialQueryService.AssignMaterial(request);
        }

        public void RemoveTestMaterials(AssignTestMaterialModel model)
        {
            var request = new AssignTestMaterialRequest
            {
                TestSittingIds = model.TestSittingIds.ToList().Distinct()
            };
            _testMaterialQueryService.RemoveTestMaterials(request);
        }

        public BusinessServiceResponse BulkDownloadTestMaterial(int testSessionId, bool includeExaminer)
        {
            var parameters = new Dictionary<string, string>()
            {
                { BackgroundOperationParameters.TestSessionId, testSessionId.ToString() },
                { BackgroundOperationParameters.IncludeExaminers, includeExaminer.ToString() },
            };
            return _backgroundOperationScheduler.Enqueue(NcmsBackgoundOperationTypeName.DownloadAllTestMaterials, parameters);
        }

        public GenericResponse<IEnumerable<string>> GetDocumentTypesForTestMaterialType()
        {
            var response = _testMaterialQueryService.GetDocumentTypesForTestMaterialType();
            return new GenericResponse<IEnumerable<string>>(response.Results);
        }

        public CreateOrUpdateResponse CreateOrUpdateTestMaterial(TestMaterialRequest model)
        {
            if (model.CredentialTypeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CredentialTypeId));
            }
            if (model.TestComponentTypeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.TestComponentTypeId));
            }
            if (model.TestMaterialDomainId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.TestMaterialDomainId));
            }

            return _testMaterialQueryService.CreateOrUpdateTestMaterial(model);
        }

        public AttachmentResponse CreateOrUpdateTestMaterialAttachment(TestMaterialAttachmentRequest model)
        {
            if (model.TestMaterialId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.TestMaterialId));
            }

            return _testMaterialQueryService.CreateOrUpdateTestMaterialAttachment(model);
        }

        public GenericResponse<IEnumerable<TestMaterialResponse>> GetTestMaterialsFromTestTask(int testComponentId, int? skillId, bool? includeSystemValueSkillTypes)
        {
            var response = _testMaterialQueryService.GetTestMaterialsFromTestTask(new GetTestMaterialsFromTestTaskRequest { TestComponentId = testComponentId, SkillId = skillId, IncludeSystemValueSkillTypes = includeSystemValueSkillTypes });
            return new GenericResponse<IEnumerable<TestMaterialResponse>>(response.Results.Select(ToTestMaterialResponseModel));
        }

        public GetTestSpecificationResponse GetTestSpecificationDetails(TestMaterialBulkAssignmentRequest request)
        {
            var respponse =
                _testMaterialQueryService.GetTestSpecificationDetails(
                    new TestSpecificationDetailsRequest { TestSessionIds = request.TestSessionIds });

            return new GetTestSpecificationResponse
            {
                Results = respponse.Select(_autoMapperHelper.Mapper.Map<TestSpecificationDetailsModel>).ToList()
            };
        }

        private GenericResponse<IEnumerable<object>> Validate(Func<bool> validation, string propertyName, string message)
        {
            var errors = new List<object>();

            if (!validation())
            {
                errors.Add(new
                {
                    FieldName = propertyName,
                    Message = message
                });
            }

            return new GenericResponse<IEnumerable<object>>(errors);
        }
        public GenericResponse<IEnumerable<object>> ValidateTestSpecification(TestMaterialBulkAssignmentRequest request)
        {
            return Validate(() => request.TestSpecificationId > 0, nameof(request.TestSpecificationId),
                Naati.Resources.TestMaterial.SelectTestSpecificationMessage);
        }

        public TestSpecificationSkillsResponse GetTestSpecificationSkills(TestMaterialBulkAssignmentRequest request)
        {
            var respponse =
                _testMaterialQueryService.GetTestSpecificationSkills(new TestSpecificationSkillsRequest { TestSessionIds = request.TestSessionIds, TestSpecificationId = request.TestSpecificationId });

            return new TestSpecificationSkillsResponse
            {
                Results = respponse.Select(_autoMapperHelper.Mapper.Map<SpecificationSkillModel>).ToList()
            };
        }

        public GenericResponse<IEnumerable<object>> ValidateSkill(TestMaterialBulkAssignmentRequest request)
        {
            return Validate(() => request.SkillId > 0, nameof(request.SkillId),
                Naati.Resources.TestMaterial.SelectTestSpecificationSkillMessage);
        }

        public GetTestMaterialsResponse GetTestMaterials(PagedTestMaterialBulkAssignmentRequest request)
        {
            var respponse =
                _testMaterialQueryService.GetTestSpecificationMaterials(new TestSpecificationMaterialRequest
                {
                    TestSpecificationId = request.TestSpecificationId,
                    SkillId = request.SkillId,
                    TestSessionIds = request.TestSessionIds,
                    Take = request.Take == 0 ? 20 : request.Take,
                    Skip = request.Skip,
                    IncludeSkillTypes = request.ShowIncludedValue
                });

            return new GetTestMaterialsResponse
            {
                Results = respponse.Select(_autoMapperHelper.Mapper.Map<TestMaterialModel>).ToList()
            };
        }

        public TestTaskResponse GetTestTasks(TestMaterialBulkAssignmentRequest request)
        {
            var respponse =
                _testMaterialQueryService.GetTestSpecificationTasks(new TestTaskRequest { TestSpecificationId = request.TestSpecificationId });

            return new TestTaskResponse
            {
                Results = respponse.Select(_autoMapperHelper.Mapper.Map<TestTaskModel>).ToList()
            };
        }

        public GenericResponse<IEnumerable<object>> ValidateTestMaterials(TestMaterialBulkAssignmentRequest request)
        {
            return Validate(() => (request.TestMaterialAssignments?.Count() ?? 0) > 0, nameof(request.TestMaterialAssignments),
                Naati.Resources.TestMaterial.AssignMaterialMessage);
        }

        public TestMaterialSummaryResponse GetTestMaterialsSummary(TestMaterialBulkAssignmentRequest request)
        {
            var respponse =
                _testMaterialQueryService.GetTestMaterialsSummary(new TestMaterialsSummaryRequest
                {
                    TestSessionIds = request.TestSessionIds,
                    TestMaterialAssignments = request.TestMaterialAssignments.Select(_autoMapperHelper.Mapper.Map<TestMaterialAssignmentDto>),
                    SkillId = request.SkillId,
                    TestSpecificationId = request.TestSpecificationId
                });

            var alreadySatPersonIds = new HashSet<int>();
            respponse.ApplicantsAlreadySat.ForEach(x => alreadySatPersonIds.Add(x.PersonId)); // Create hashset to improve search


            var result = new TestMaterialSummaryResponse
            {
                TotalNewApplicantsNotSat =
                    respponse.NewMaterialApplicantsIds.Count(x => !alreadySatPersonIds.Contains(x.PersonId)),
                TotalNewApplicantsSat =
                    respponse.NewMaterialApplicantsIds.Count(x => alreadySatPersonIds.Contains(x.PersonId)),
                TotalApplicantsToOverrideNotSat =
                    respponse.ApplicantIdsToOverride.Count(x => !alreadySatPersonIds.Contains(x.PersonId)),
                TotalApplicantsToOverrideSat =
                    respponse.ApplicantIdsToOverride.Count(x => alreadySatPersonIds.Contains(x.PersonId)),
                ApplicantsAlreadySat = respponse.ApplicantsAlreadySat.Select(_autoMapperHelper.Mapper.Map<TestMaterialApplicantModel>)
            };
            return result;
        }

        public GetExaminersAndRolePlayersResponse GetExaminersAndRolePlayers(TestMaterialBulkAssignmentRequest request)
        {
            var response =
                _testMaterialQueryService.GetExaminersAndRolePlayers(new TestMaterialsSummaryRequest
                {
                    TestSessionIds = request.TestSessionIds,
                    SkillId = request.SkillId,
                    TestSpecificationId = request.TestSpecificationId
                });

            return new GetExaminersAndRolePlayersResponse
            {
                Results = response.Results.Select(_autoMapperHelper.Mapper.Map<TestMaterialExaminerModel>).ToList()
            };
        }

        public GenericResponse<IEnumerable<object>> ValidateTestMaterialSummary(TestMaterialBulkAssignmentRequest request)
        {
            return Validate(() => (request.TestSessionIds?.Count() ?? 0) > 0 &&
                                  (request.TestMaterialAssignments?.Count() ?? 0) > 0, nameof(request.TestMaterialAssignments),
                Naati.Resources.TestMaterial.AssignTestSittingMessage);

        }

        public GenericResponse<string> GetTestMaterialUriFromBackgroundOperation(int notificationId)
        {
            var response = new GenericResponse<string>();
            var result = _notificationQueryService.GetNotificationById(notificationId);
            var download = result.Data as NotificationDto<NotificationDownloadTestMaterialParameterDto>;
            if (download == null)
            {
                response.Errors.Add(Naati.Resources.TestMaterial.TestMaterialNotAvailable);
                LoggingHelper.LogWarning($"Notification {notificationId} to download test material was not found");
                return response;
            }

            if (download.NotificationTypeId != (int)NotificationTypeName.DownloadTestMaterial)
            {
                response.Errors.Add(Naati.Resources.TestMaterial.TestMaterialNotAvailable);
                LoggingHelper.LogError($"Notification {notificationId} to download test material was not found is not valid. Current notification type: {download.NotificationTypeId}, Expected:{(int)NotificationTypeName.DownloadTestMaterial}");
                return response;
            }

            var userName = _userService.Get().Name.ToUpper();
            if (userName != download.ToUserName.ToUpper())
            {
                response.Errors.Add(Naati.Resources.TestMaterial.TestMaterialNotAvailable);
                LoggingHelper.LogError($"User is not allowed to download a test material for notification {notificationId}");
                return response;
            }

            var filePath = download.Data.Path;
            if (!File.Exists(filePath))
            {
                response.Errors.Add(Naati.Resources.TestMaterial.TestMaterialNotAvailable);
                LoggingHelper.LogError($"Test material File {filePath} for notification {notificationId} was not found");
            }

            var sasToken = _sasAccessSignature.GetUrlForFile(filePath);

            return sasToken;
        }

        public GetSupplemeantaryTestResponse GetSupplementaryTests(TestMaterialBulkAssignmentRequest request)
        {
            var respponse =
                _testMaterialQueryService.GetSupplementaryTestApplicants(new SupplementaryTestRequest
                {
                    TestSessionIds = request.TestSessionIds
                });

            var result = respponse.Select(_autoMapperHelper.Mapper.Map<SupplementarytTestApplicantModel>);
            return new GetSupplemeantaryTestResponse { Results = result };
        }

        public TestTaskResponse GetTestSpecificationComponents(int testSepecificationId)
        {
            var respponse =
                _testMaterialQueryService.GetTestSpecificationTasks(new TestTaskRequest { TestSpecificationId = testSepecificationId });

            return new TestTaskResponse
            {
                Results = respponse.Select(_autoMapperHelper.Mapper.Map<TestTaskModel>).ToList()
            };
        }

        public GenericResponse<IEnumerable<ApplicationBriefModel>> GetPendingCandidateBriefsToSend(PendingBriefRequest request)
        {
            var result = _testMaterialQueryService.GetPendingCandidateBriefsToSend(request);

            var data = result.Select(_autoMapperHelper.Mapper.Map<ApplicationBriefModel>).ToList();

            return new GenericResponse<IEnumerable<ApplicationBriefModel>>(data);

        }

        public GenericResponse<List<string>> GetIncludeSystemValueSkillNames()
        {
            return _testMaterialQueryService.GetIncludeSystemValueSkillNames();
        }
    }
}