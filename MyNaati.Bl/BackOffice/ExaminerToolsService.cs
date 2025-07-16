using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using Aspose.Words;
using Aspose.Words.Replacing;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.BackOffice;
using DeleteAttachmentRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.DeleteAttachmentRequest;
using DeleteAttachmentResponse = MyNaati.Contracts.BackOffice.DeleteAttachmentResponse;
using DeleteMaterialRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.DeleteMaterialRequest;
using DeleteMaterialRequestRoundAttachmentRequest = MyNaati.Contracts.BackOffice.DeleteMaterialRequestRoundAttachmentRequest;
using DeleteMaterialResponse = MyNaati.Contracts.BackOffice.DeleteMaterialResponse;
using DeleteUnavailabilityRequest = MyNaati.Contracts.BackOffice.DeleteUnavailabilityRequest;
using DeleteUnavailabilityResponse = MyNaati.Contracts.BackOffice.DeleteUnavailabilityResponse;
using DocumentTypeContract = MyNaati.Contracts.BackOffice.DocumentTypeContract;
using ExaminerUnavailableContract = MyNaati.Contracts.BackOffice.ExaminerUnavailableContract;
using GetDocumentResponse = MyNaati.Contracts.BackOffice.GetDocumentResponse;
using GetDocumentTypesResponse = MyNaati.Contracts.BackOffice.GetDocumentTypesResponse;
using GetMaterialFileRequest = MyNaati.Contracts.BackOffice.GetMaterialFileRequest;
using GetMaterialFileResponse = MyNaati.Contracts.BackOffice.GetMaterialFileResponse;
using GetMaterialRequestRoundAttachmentRequest = MyNaati.Contracts.BackOffice.GetMaterialRequestRoundAttachmentRequest;
using GetMaterialRequestRoundAttachmentsRequest = MyNaati.Contracts.BackOffice.GetMaterialRequestRoundAttachmentsRequest;
using GetMaterialRequestRoundLinkResponse = MyNaati.Contracts.BackOffice.GetMaterialRequestRoundLinkResponse;
using GetMaterialRequestsRequest = MyNaati.Contracts.BackOffice.GetMaterialRequestsRequest;
using GetRolePlayerSettingsRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetRolePlayerSettingsRequest;
using GetRolePlayerSettingsResponse = MyNaati.Contracts.BackOffice.GetRolePlayerSettingsResponse;
using GetRolePlaySessionRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetRolePlaySessionRequest;
using GetRolePlaySessionResponse = MyNaati.Contracts.BackOffice.GetRolePlaySessionResponse;
using GetTestAssetsFileRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetTestAssetsFileRequest;
using GetTestAssetsFileResponse = MyNaati.Contracts.BackOffice.GetTestAssetsFileResponse;
using GetTestAttendanceDocumentRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetTestAttendanceDocumentRequest;
using GetTestAttendanceDocumentResponse = MyNaati.Contracts.BackOffice.GetTestAttendanceDocumentResponse;
using GetTestDetailsRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetTestDetailsRequest;
using GetTestDetailsResponse = MyNaati.Contracts.BackOffice.GetTestDetailsResponse;
using GetTestMaterialCreationPaymentsResponse = MyNaati.Contracts.BackOffice.GetTestMaterialCreationPaymentsResponse;
using GetTestMaterialRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetTestMaterialRequest;
using GetTestMaterialsFileRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetTestMaterialsFileRequest;
using GetTestMaterialsFileResponse = MyNaati.Contracts.BackOffice.GetTestMaterialsFileResponse;
using GetTestsMaterialRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetTestsMaterialRequest;
using GetTestsMaterialResponse = MyNaati.Contracts.BackOffice.GetTestsMaterialResponse;
using GetTestsRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetTestsRequest;
using GetTestsResponse = MyNaati.Contracts.BackOffice.GetTestsResponse;
using License = Aspose.Words.License;
using ListUnavailabilityRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.ListUnavailabilityRequest;
using ListUnavailabilityResponse = MyNaati.Contracts.BackOffice.ListUnavailabilityResponse;
using MaterialContract = MyNaati.Contracts.BackOffice.MaterialContract;
using MaterialRequestRoundAttachmentDto = MyNaati.Contracts.BackOffice.MaterialRequestRoundAttachmentDto;
using RolePlayerSettingsRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.RolePlayerSettingsRequest;
using RolePlaySessionContract = MyNaati.Contracts.BackOffice.RolePlaySessionContract;
using SaveAttachmentRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.SaveAttachmentRequest;
using SaveAttachmentResponse = MyNaati.Contracts.BackOffice.SaveAttachmentResponse;
using SaveExaminerMarkingResponse = MyNaati.Contracts.BackOffice.SaveExaminerMarkingResponse;
using SaveMaterialRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.SaveMaterialRequest;
using SaveMaterialRequestRoundLinkRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.SaveMaterialRequestRoundLinkRequest;
using SaveMaterialRequestRoundLinkResponse = MyNaati.Contracts.BackOffice.SaveMaterialRequestRoundLinkResponse;
using SaveMaterialResponse = MyNaati.Contracts.BackOffice.SaveMaterialResponse;
using SaveUnavailabilityRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.SaveUnavailabilityRequest;
using SaveUnavailabilityResponse = MyNaati.Contracts.BackOffice.SaveUnavailabilityResponse;
using SubmitMaterialRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.SubmitMaterialRequest;
using SubmitMaterialResponse = MyNaati.Contracts.BackOffice.SubmitMaterialResponse;
using SubmitTestRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.SubmitTestRequest;
using SubmitTestResponse = MyNaati.Contracts.BackOffice.SubmitTestResponse;
using TestAttendanceDocumentContract = MyNaati.Contracts.BackOffice.TestAttendanceDocumentContract;
using TestContract = MyNaati.Contracts.BackOffice.TestContract;
using TestMaterialContract = MyNaati.Contracts.BackOffice.TestMaterialContract;
using UpdateMaterialRequestMembersRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.UpdateMaterialRequestMembersRequest;
using UpdateMaterialRequestMembersResponse = MyNaati.Contracts.BackOffice.UpdateMaterialRequestMembersResponse;


namespace MyNaati.Bl.BackOffice
{
    public class ExaminerToolsService : MyNaati.Contracts.BackOffice.IExaminerToolsService
    {
        private readonly F1Solutions.Naati.Common.Contracts.Dal.QueryServices.IExaminerToolsService mExaminerToolsService;
        private readonly IPayrollQueryService mPayrollService;
        private readonly IExaminerRepository mExaminerRepository;
        private readonly IApplicationQueryService mApplicationQueryService;
        private readonly ITestResultQueryService mTestResultQueryService;
        private readonly ITokenReplacementService mTokenReplacementService;
        private readonly ITestSessionQueryService mTestSessionQueryService;
        private readonly ITestMaterialQueryService mTestMaterialQueryService;
        private readonly IMaterialRequestQueryService mMaterialRequestQueryService;
        private readonly IFileStorageService mFileStorageService;
        private readonly IPanelQueryService mPanelService;
        private readonly INoteQueryService mNoteService;
        private readonly IUserQueryService mUserService;
        private readonly ISecretsCacheQueryService mSecretsProvider;
        private readonly IAutoMapperHelper _autoMapperHelper;

        private static int MaxRubrickCommentsLength = 2000;
        private static int MaxFeedbackLength = 2000;

        public ExaminerToolsService(F1Solutions.Naati.Common.Contracts.Dal.QueryServices.IExaminerToolsService examinerToolsService,
                                    IExaminerRepository examinerRepository,
                                    IPayrollQueryService payrollService,
                                    IApplicationQueryService applicationQueryService,
                                    ITestResultQueryService testResultQueryService,
                                    ITokenReplacementService tokenReplacementService,
                                    ITestSessionQueryService testSessionQueryService,
                                    ITestMaterialQueryService testMaterialQueryService,
                                    IMaterialRequestQueryService materialRequestQueryService,
                                    IFileStorageService fileStorageService,
                                    IPanelQueryService panelService,
                                    INoteQueryService noteQueryService,
                                    IUserQueryService userQueryService,
                                    ISecretsCacheQueryService secretsProvider,
                                    IAutoMapperHelper autoMapperHelper)
        {
            mExaminerToolsService = examinerToolsService;
            mExaminerRepository = examinerRepository;
            mPayrollService = payrollService;
            mApplicationQueryService = applicationQueryService;
            mTestResultQueryService = testResultQueryService;
            mTokenReplacementService = tokenReplacementService;
            mTestSessionQueryService = testSessionQueryService;
            mMaterialRequestQueryService = materialRequestQueryService;
            mFileStorageService = fileStorageService;
            mPanelService = panelService;
            mNoteService = noteQueryService;
            mUserService = userQueryService;
            mTestMaterialQueryService = testMaterialQueryService;
            mSecretsProvider = secretsProvider;
            _autoMapperHelper = autoMapperHelper;
        }

        public GetTestsResponse GetTests(MyNaati.Contracts.BackOffice.GetTestsRequest request)
        {
            var response = mExaminerToolsService.GetTests(new GetTestsRequest
            {
                UserId = request.UserId,
                RoleCategories = request.RoleCategoryIds.Select(x => (PanelRoleCategoryName)x).ToArray(),
                AsChair = request.AsChair,
                NAATINumber = request.NAATINumber,
                DateAllocatedFrom = request.DateAllocatedFrom,
                DateAllocatedTo = request.DateAllocatedTo,
                PanelId = request.PanelId,
                DateDueFrom = request.DateDueFrom,
                DateDueTo = request.DateDueTo,
                TestStatus = request.TestStatus,
            });

            return new GetTestsResponse
            {
                Tests = response.Tests.Select(_autoMapperHelper.Mapper.Map<TestContract>).ToArray()
            };
        }

        public GetTestsMaterialResponse GetTestsMaterial(MyNaati.Contracts.BackOffice.GetTestsMaterialRequest request)
        {
            var response = mExaminerToolsService.GetTestsMaterial(new GetTestsMaterialRequest
            {
                UserId = request.UserId,
                RoleCategories = request.RoleCategoryIds.Select(x => (PanelRoleCategoryName)x).ToArray(),
                PrimaryContact = true
            });

            return new GetTestsMaterialResponse
            {
                Tests = response.Tests.Select(_autoMapperHelper.Mapper.Map<TestMaterialContract>).ToArray()
            };
        }

        public TestMaterialContract GetTestMaterial(MyNaati.Contracts.BackOffice.GetTestMaterialRequest request)
        {
            var response = mExaminerToolsService.GetTestMaterial(new GetTestMaterialRequest
            {
                NAATINumber = request.NAATINumber,
                TestMaterialId = request.TestMaterialId,
            });

            return _autoMapperHelper.Mapper.Map<TestMaterialContract>(response);
        }

        public GetTestDetailsResponse GetTestDetails(MyNaati.Contracts.BackOffice.GetTestDetailsRequest request)
        {
            var response = mExaminerToolsService.GetTestDetails(_autoMapperHelper.Mapper.Map<GetTestDetailsRequest>(request));

            return new GetTestDetailsResponse
            {
                OverAllPassMark = response.OverAllPassMark.OverAllPassMark,
                TestMarkingTypeId = response.TestMarkingTypeId,
                Components = response.Components.Select(x =>
                {
                    var result = _autoMapperHelper.Mapper.Map<TestComponentContract>(x);
                    result.ReadOnly = x.MarkingResultTypeId == (int)MarkingResultTypeName.FromOriginal;
                    return result;
                }).ToArray(),
                Attachments = response.Attachments.Select(_autoMapperHelper.Mapper.Map<TestAttendanceDocumentContract>).ToArray(),
                Feedback = response.Feedback,
            };
        }

        public int? GetTestSittingIdByTestResult(int testResultId)
        {
            var response = mExaminerToolsService.GetTestSittingIdByTestResult(testResultId);
            return response;
        }


        public SubmitTestResponse SubmitTest(MyNaati.Contracts.BackOffice.SubmitTestRequest request)
        {
            var serviceRequest = new SubmitTestRequest
            {
                UserId = request.UserId,
                TestResultID = request.TestResultID,
                Components = new List<StandardTestComponentContract>(),
                Comments = request.Comments,
                Feedback = request.Feedback,
                ReasonsForPoorPerformance = request.ReasonsForPoorPerformance,
                PrimaryReasonForFailure = request.PrimaryReasonForFailure,
            };

            foreach (var c in request.Components)
            {
                serviceRequest.Components.Add(_autoMapperHelper.Mapper.Map<StandardTestComponentContract>(c));
            }

            var response = mExaminerToolsService.SubmitTest(serviceRequest);

            return _autoMapperHelper.Mapper.Map<SubmitTestResponse>(response);
        }

        public bool SaveExaminerPapersRecievedDateRequested(SaveExaminerPapersRecievedDateRequest request)
        {
            return mExaminerRepository.SaveExaminerPapersRecievedDateRequested(request);
        }

        public ListUnavailabilityResponse ListUnavailability(MyNaati.Contracts.BackOffice.ListUnavailabilityRequest request)
        {
            var response = mExaminerToolsService.ListUnavailability(new ListUnavailabilityRequest { NAATINumber = request.NAATINumber });

            return new ListUnavailabilityResponse
            {
                Periods = response.Periods.Select(_autoMapperHelper.Mapper.Map<ExaminerUnavailableContract>).ToArray()
            };
        }

        public SaveUnavailabilityResponse SaveUnavailability(MyNaati.Contracts.BackOffice.SaveUnavailabilityRequest request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<SaveUnavailabilityRequest>(request);
            mExaminerToolsService.SaveUnavailability(serviceRequest);
            return new SaveUnavailabilityResponse();
        }

        public void SaveRolePlayerSettings(MyNaati.Contracts.BackOffice.RolePlayerSettingsRequest request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<RolePlayerSettingsRequest>(request);
            mExaminerToolsService.SaveRolePlayerSettings(serviceRequest);
        }

        public GetRolePlayerSettingsResponse GetRolePlayerSettings(MyNaati.Contracts.BackOffice.GetRolePlayerSettingsRequest request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<GetRolePlayerSettingsRequest>(request);
            var serviceResponse = mExaminerToolsService.GetRolePlayerSettings(serviceRequest);
            return new GetRolePlayerSettingsResponse()
            {
                Settings = _autoMapperHelper.Mapper.Map<Contracts.BackOffice.RolePlayerSettingsDto>(serviceResponse.Settings)
            };
        }

        public GetRolePlaySessionResponse GetRolePlaySession(MyNaati.Contracts.BackOffice.GetRolePlaySessionRequest request)
        {

            var languages = mApplicationQueryService.GetLookupType(LookupType.Languages.ToString())
                .Results.ToDictionary(x => x.Id, v => v);

            var serviceRequest = _autoMapperHelper.Mapper.Map<GetRolePlaySessionRequest>(request);
            var dtos = mTestSessionQueryService.GetSessionRolePlayers(serviceRequest).Data;

            var models = dtos.Select(x =>
            {
                var r = _autoMapperHelper.Mapper.Map<RolePlaySessionContract>(x);
                r.CanAccept = r.RolePlayerStatusId == (int)RolePlayerStatusTypeName.Pending;
                r.CanReject = r.RolePlayerStatusId == (int)RolePlayerStatusTypeName.Pending;
                r.AcceptActionId = (int)SystemActionTypeName.RolePlayerAcceptTestSessionFromMyNaati;
                r.RejectActionId = (int)SystemActionTypeName.RolePlayerRejectTestSessionFromMyNaati;
                r.Details.ForEach(y => y.LanguageName = languages[y.LanguageId].DisplayName);
                return r;

            }).ToList();

            var response = new GetRolePlaySessionResponse()
            {
                Sessions = models
            };

            return response;

        }

        public bool IsValidExaminerForAvailability(int examinerUnavailableId, int naatinumber)
        {
            var response = mExaminerToolsService.IsValidExaminerForAvailability(examinerUnavailableId, naatinumber);
            return response;
        }

        public DeleteUnavailabilityResponse DeleteUnavailability(DeleteUnavailabilityRequest request)
        {
            mExaminerToolsService.DeleteUnavailability(new F1Solutions.Naati.Common.Contracts.Dal.Request.DeleteUnavailabilityRequest
            {
                Id = request.Id
            });

            return new DeleteUnavailabilityResponse();
        }

        public SaveMaterialResponse SaveMaterial(MyNaati.Contracts.BackOffice.SaveMaterialRequest request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<SaveMaterialRequest>(request);
            var response = mExaminerToolsService.SaveMaterial(serviceRequest);

            return new SaveMaterialResponse
            {
                StoredFileId = response.StoredFileId,
                TestMaterialId = response.TestMaterialId,
            };
        }

        public GetMaterialFileResponse GetMaterialFile(GetMaterialFileRequest request)
        {
            var response = mExaminerToolsService.GetMaterialFile(new F1Solutions.Naati.Common.Contracts.Dal.Request.GetMaterialFileRequest
            {
                MaterialId = request.MaterialId,
                TempFileStorePath = request.TempFileStorePath
            });

            return _autoMapperHelper.Mapper.Map<GetMaterialFileResponse>(response);
        }

        public DeleteMaterialResponse DeleteMaterial(MyNaati.Contracts.BackOffice.DeleteMaterialRequest request)
        {
            var response = mExaminerToolsService.DeleteMaterial(_autoMapperHelper.Mapper.Map<DeleteMaterialRequest>(request));
            return _autoMapperHelper.Mapper.Map<DeleteMaterialResponse>(response);
        }

        public SubmitMaterialResponse SubmitMaterial(MyNaati.Contracts.BackOffice.SubmitMaterialRequest request)
        {
            var response = mExaminerToolsService.SubmitMaterial(_autoMapperHelper.Mapper.Map<SubmitMaterialRequest>(request));
            return _autoMapperHelper.Mapper.Map<SubmitMaterialResponse>(response);
        }

        public SaveAttachmentResponse SaveAttachment(MyNaati.Contracts.BackOffice.SaveAttachmentRequest request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<SaveAttachmentRequest>(request);
            var response = mExaminerToolsService.SaveAttachment(serviceRequest);
            return _autoMapperHelper.Mapper.Map<SaveAttachmentResponse>(response);
        }

        public DeleteAttachmentResponse DeleteAttachment(MyNaati.Contracts.BackOffice.DeleteAttachmentRequest request)
        {
            var response = mExaminerToolsService.DeleteAttachment(_autoMapperHelper.Mapper.Map<DeleteAttachmentRequest>(request));
            return _autoMapperHelper.Mapper.Map<DeleteAttachmentResponse>(response);
        }

        public bool IsValidAttendeeDeleteAttachment(int testAttendanceDocumentId, int naatiNumber)
        {
            var response = mExaminerToolsService.IsValidAttendeeDeleteAttachment(testAttendanceDocumentId, naatiNumber);
            return response;
        }

        public GetTestAssetsFileResponse GetTestAssetsFile(MyNaati.Contracts.BackOffice.GetTestAssetsFileRequest request)
        {
            var response = mExaminerToolsService.GetTestAssetsFile(new GetTestAssetsFileRequest
            {
                TestSittingId = request.TestSittingId,
                TempFileStorePath = request.TempFileStorePath
            });

            return _autoMapperHelper.Mapper.Map<GetTestAssetsFileResponse>(response);
        }

        public string[] TestMaterialReplaceTokens(GetAttendeesTestSpecificationTestMaterialResponse request, string tempFileStorePath)
        {
            var filePaths = new List<string>();

            var testMaterialTokens = new DocumentTokenReplacement
            {
                AttendanceId = request.AttandanceId.ToString(),
                TestTessionId = request.TestSessionId.ToString()
            };

            var documentAdditionalTokenValue = mExaminerToolsService.GetDocumentAdditionalTokens(request.AttandanceId);

            testMaterialTokens.ApplicationReference = documentAdditionalTokenValue.ApplicationReference;
            testMaterialTokens.CredentialRequestType = documentAdditionalTokenValue.CredentialRequestType;
            testMaterialTokens.Skill = documentAdditionalTokenValue.Skill;
            testMaterialTokens.VenueName = documentAdditionalTokenValue.VenueName;
            testMaterialTokens.TestSessionName = documentAdditionalTokenValue.TestSessionName;
            testMaterialTokens.StrTestTessionDate = documentAdditionalTokenValue.TestTessionDate;
            testMaterialTokens.StrTestTessionTime = documentAdditionalTokenValue.TestTessionTime;

            var license = new License();
            license.SetLicense("Aspose.Words.lic");

            foreach (var item in request.AttendeeTestSpecificationTestMaterialList)
            {
                var testSpecificationFileList = item.AttendeeTestSpecification.StoredFileList;

                if (testSpecificationFileList.Any())
                {
                    foreach (var testSpecificationFile in testSpecificationFileList)
                    {
                        string filePath;
                        var fileExtension = testSpecificationFile.FilePath.Split('.').Last();
                        if (string.Equals("docx", fileExtension) || string.Equals("doc", fileExtension))
                        {
                            var srcTestSpecificationDoc = new Document(testSpecificationFile.FilePath);
                            srcTestSpecificationDoc.FirstSection.PageSetup.SectionStart = SectionStart.NewPage;
                            srcTestSpecificationDoc.FirstSection.PageSetup.RestartPageNumbering = false;

                            testMaterialTokens.TestSpecificationId = "SPEC" + item.AttendeeTestSpecification.Id;
                            var tokensReplaced = ReplaceTokens(testMaterialTokens, srcTestSpecificationDoc.Range.Text,
                                (token, value) =>
                                {
                                    ReplaceTokenInDocument(srcTestSpecificationDoc, token, value);
                                });

                            filePath = MakeUnique(tempFileStorePath + "\\" + item.AttendeeTestSpecification.Id + " - " + testSpecificationFile.Title + ".docx");
                            srcTestSpecificationDoc.Save(filePath);
                            filePaths.Add(filePath);

                            System.IO.File.Delete(testSpecificationFile.FilePath);
                        }
                        else
                        {
                            filePath = MakeUnique(tempFileStorePath + "\\" + item.AttendeeTestSpecification.Id + " - " + testSpecificationFile.Title + "." + fileExtension);

                            System.IO.File.Copy(testSpecificationFile.FilePath, filePath);
                            filePaths.Add(filePath);
                            System.IO.File.Delete(testSpecificationFile.FilePath);
                        }
                    }
                }

                var attendeeTestMaterialList = item.AttendeeTestMaterialList;
                foreach (var attendeeTestMaterial in attendeeTestMaterialList)
                {
                    var testMaterialFileList = attendeeTestMaterial.StoredFileList;

                    if (testMaterialFileList.Any())
                    {
                        foreach (var testMaterialFile in testMaterialFileList)
                        {
                            string filePath;
                            var fileExtension = testMaterialFile.FilePath.Split('.').Last();
                            if (string.Equals("docx", fileExtension) || string.Equals("doc", fileExtension))
                            {
                                var srcTestMaterialFileDoc = new Document(testMaterialFile.FilePath);
                                srcTestMaterialFileDoc.FirstSection.PageSetup.SectionStart = SectionStart.NewPage;
                                srcTestMaterialFileDoc.FirstSection.PageSetup.RestartPageNumbering = false;

                                var taskTypeTokenValue = attendeeTestMaterial.TaskTypeLabel + attendeeTestMaterial.Label;
                                var testMaterialIdValue = "#" + testMaterialFile.TestMaterialId;
                                var taskLabelToken = GetTokenNameFor(TokenReplacementField.TaskLabel);
                                var testMaterialIdToken = GetTokenNameFor(TokenReplacementField.TestMaterialId);
                                var extraTokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                {
                                    { testMaterialIdToken, testMaterialIdValue },
                                    { taskLabelToken, taskTypeTokenValue }
                                };

                                var taskLabeltokenReplaced = ReplaceTokenArray(extraTokens, srcTestMaterialFileDoc.Range.Text,
                                    (token, value) =>
                                    {
                                        ReplaceTokenInDocument(srcTestMaterialFileDoc, token, value);
                                    });

                                filePath = MakeUnique(tempFileStorePath + "\\" + attendeeTestMaterial.TaskTypeLabel + attendeeTestMaterial.Label + " - #" + attendeeTestMaterial.Id + " - " + testMaterialFile.Title + ".docx");
                                srcTestMaterialFileDoc.Save(filePath);
                                filePaths.Add(filePath);

                                System.IO.File.Delete(testMaterialFile.FilePath);
                            }
                            else
                            {
                                filePath = MakeUnique(tempFileStorePath + "\\" + attendeeTestMaterial.TaskTypeLabel + attendeeTestMaterial.Label + " - #" + attendeeTestMaterial.Id + " - " + testMaterialFile.Title + "." + fileExtension);
                                System.IO.File.Copy(testMaterialFile.FilePath, filePath);
                                filePaths.Add(filePath);
                                System.IO.File.Delete(testMaterialFile.FilePath);
                            }

                        }
                    }
                }
            }

            return filePaths.ToArray();
        }

        public string MakeUnique(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string fileExt = Path.GetExtension(path);

            for (int i = 1; ; ++i)
            {
                if (!File.Exists(path))
                    return path;

                path = Path.Combine(dir, fileName + " " + i + fileExt);
            }
        }

        private bool ReplaceTokens(DocumentTokenReplacement request, string text, Action<string, string> replacementAction)
        {
            var applicationReference = GetTokenNameFor(TokenReplacementField.ApplicationReference);
            var credentialRequestType = GetTokenNameFor(TokenReplacementField.CredentialRequestType);
            var skill = GetTokenNameFor(TokenReplacementField.Skill);
            var venueName = GetTokenNameFor(TokenReplacementField.VenueName);

            var testSessionId = GetTokenNameFor(TokenReplacementField.TestSessionId);
            var testSessionDate = GetTokenNameFor(TokenReplacementField.TestSessionDate);
            var testSessionStartTime = GetTokenNameFor(TokenReplacementField.TestSessionStartTime);
            var testAttendanceId = GetTokenNameFor(TokenReplacementField.TestAttendanceId);

            var testSessionName = GetTokenNameFor(TokenReplacementField.TestSessionName);
            var testSpecificationId = GetTokenNameFor(TokenReplacementField.TestSpecificationId);

            var extraTokens =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { applicationReference, request.ApplicationReference },
                    { credentialRequestType, request.CredentialRequestType },
                    { skill, request.Skill },
                    { venueName, request.VenueName },

                    { testAttendanceId, request.AttendanceId },
                    { testSessionId, request.TestTessionId },
                    { testSessionDate, request.StrTestTessionDate },
                    { testSessionStartTime, request.StrTestTessionTime },
                    { testSessionName, request.TestSessionName },
                    { testSpecificationId, request.TestSpecificationId }
                };

            IEnumerable<string> errors;
            mTokenReplacementService.ReplaceTemplateFieldValues(text, replacementAction, extraTokens, true, out errors);

            if (errors.Any())
            {
                return false;
            }

            return true;
        }

        private bool ReplaceTaskLabelToken(string taskLabel, string text, Action<string, string> replacementAction)
        {
            var taskLabelToken = GetTokenNameFor(TokenReplacementField.TaskLabel);

            var extraTokens =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { taskLabelToken, taskLabel },
                };

            IEnumerable<string> errors;
            mTokenReplacementService.ReplaceTemplateFieldValues(text, replacementAction, extraTokens, true, out errors);

            if (errors.Any())
            {
                return false;
            }

            return true;
        }

        private bool ReplaceTokenArray(Dictionary<string, string> extraTokens, string text, Action<string, string> replacementAction)
        {
            mTokenReplacementService.ReplaceTemplateFieldValues(text, replacementAction, extraTokens, true, out var errors);
            return !errors.Any();
        }

        private void ReplaceTokenInDocument(Document document, string token, string value)
        {
            document.Range.Replace(token, SanitiseTokenValue(value) ?? string.Empty, new FindReplaceOptions());
        }
        private string SanitiseTokenValue(string tokenValue)
        {
            if (!String.IsNullOrEmpty(tokenValue))
            {
                // replace CRLF with something Aspose.Words understands, and remove excess whitespace and trailing commas
                var lines = tokenValue.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                return String.Join("&l", lines.Select(x => x.Trim().Trim(',')));
            }
            return tokenValue;
        }

        private string GetTokenNameFor(TokenReplacementField token)
        {
            return typeof(TokenReplacementField)
                .GetMember(token.ToString())
                .First().GetCustomAttribute<DisplayAttribute>()
                .Name;
        }

        public GetAttendeesTestSpecificationTestMaterialResponse GetTestMaterialsByAttendaceId(int attendanceId)
        {
            var response = mExaminerToolsService.GetTestMaterialsByAttendaceId(attendanceId);
            return _autoMapperHelper.Mapper.Map<GetAttendeesTestSpecificationTestMaterialResponse>(response);
        }

        public GetAttendeesTestSpecificationTestMaterialResponse GetFileStoreTestSpecificationTestMaterialList(GetFileStoreTestSpecificationTestMaterialRequest request)
        {
            var response = mExaminerToolsService.GetFileStoreTestSpecificationTestMaterialList(request);
            return response;
        }

        public GetTestMaterialsFileResponse GetTestMaterialsFile(MyNaati.Contracts.BackOffice.GetTestMaterialsFileRequest request)
        {
            var response = mExaminerToolsService.GetTestMaterialsFile(new GetTestMaterialsFileRequest
            {
                TestMaterialId = request.TestMaterialId,
                NAATINumber = request.NAATINumber,
                TempFileStorePath = request.TempFileStorePath
            });

            return _autoMapperHelper.Mapper.Map<GetTestMaterialsFileResponse>(response);
        }

        public GetTestAttendanceDocumentResponse GetTestAttendanceDocument(MyNaati.Contracts.BackOffice.GetTestAttendanceDocumentRequest request)
        {
            var response = mExaminerToolsService.GetTestAttendanceDocument(new GetTestAttendanceDocumentRequest
            {
                TestAttendanceDocumentId = request.TestAttendanceDocumentId,
                TempFileStorePath = request.TempFileStorePath
            });
           
            return _autoMapperHelper.Mapper.Map<GetTestAttendanceDocumentResponse>(response);
        }

        public GetDocumentTypesResponse GetDocumentTypes()
        {
            var response = mExaminerToolsService.GetDocumentTypes();
            return new GetDocumentTypesResponse
            {
                DocumentTypes = response.DocumentTypes.Select(_autoMapperHelper.Mapper.Map<DocumentTypeContract>).ToArray()
            };
        }

        public GetPayrollHistoryResponse GetPayrollHistory(GetPayrollHistoryRequest request)
        {
            var response = mPayrollService.GetMarkingsForPayroll(new GetMarkingsForPayrollRequest
            {
                ExaminerNaatiNumber = request.ExaminerNaatiNumber,
                PayrollStatuses = new[]
                {
                    PayrollStatusName.Received,
                    PayrollStatusName.Ready,
                    PayrollStatusName.InProgress,
                    PayrollStatusName.Complete
                }
            });

            var markings = response.Markings
                .Select(_autoMapperHelper.Mapper.Map<MarkingPayrollItemContract>)
                .ToList();

            foreach (var marking in markings)
            {
                marking.InvoiceTotal =
                    markings
                    .Where(x => x.AccountingReference == marking.AccountingReference)
                    .Sum(x => x.ExaminerCost);
            }

            return new GetPayrollHistoryResponse
            {
                MarkingPayrollItems = markings
            };
        }

        public TestRubricMarkingContract GetExaminerRubricMarking(int naatiNumber, int testResultId)
        {
            var rubricDto = mTestResultQueryService.GetExaminerMarkingResult(naatiNumber, testResultId);
            
            var rubricModel = _autoMapperHelper.Mapper.Map<TestRubricMarkingContract>(rubricDto);
            return rubricModel;
        }

        private string ValidateMarks(IEnumerable<TestMarkingComponentContract> testcomponents, int maxLength, DateTime? submitedDate, string feedback)
        {
            var hasAttemptedTasks = false;
            foreach (var testComponent in testcomponents)
            {
                if (testComponent.WasAttempted)
                {
                    hasAttemptedTasks = true;
                }
                if (testComponent.Competencies.SelectMany(x => x.Assessments).Any(a => (a.Comment ?? string.Empty).Length > maxLength))
                {
                    return $"Comments can not be longer than {maxLength} characters";
                };
            }

            if (!hasAttemptedTasks && submitedDate.HasValue)
            {
                return "At least one task must be marked as attempted before the test can be submitted";
            }

            // Feedback has a character limit of 2000 characters
            if(feedback?.Length > MaxFeedbackLength)
            {
                return $"Feedback can not be longer than {MaxFeedbackLength} characters";
            }

            return null;
        }
        public SaveExaminerMarkingResponse SaveExaminerRubricMarking(TestRubricMarkingContract model)
        {
            var response = new SaveExaminerMarkingResponse
            {
                ErrorMessage = ValidateMarks(model.TestComponents, MaxRubrickCommentsLength, model.SubmittedDate, model.Feedback)
            };

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                return response;
            }
            
            var markingDto = _autoMapperHelper.Mapper.Map<JobExaminerMarkingDto>(model);
            markingDto.ReceivedDate = markingDto.SubmittedDate;
            var request = new SaveExaminerMarkingRequest
            {
                JobExaminerMarking = markingDto,
                ClearNotAttempted = markingDto.SubmittedDate != null,
                IsExaminerRequest = true,
                MaxCommentsLength = MaxRubrickCommentsLength
            };

            var rubricDto = mTestResultQueryService.GetExaminerMarkingResult(model.NaatiNumber, model.TestResultId);
            foreach (var testComponent in request.JobExaminerMarking.TestComponents)
            {
                foreach (var compentency in testComponent.RubricMarkingCompentencies)
                {
                    foreach (var criterion in compentency.RubricMarkingAssessmentCriteria)
                    {
                        var bands = rubricDto.TestComponents.First(x => x.TestComponentId == testComponent.TestComponentId)
                            .RubricMarkingCompentencies.First(c => c.CompetencyId == compentency.CompetencyId)
                            .RubricMarkingAssessmentCriteria
                            .First(cr => cr.AssessmentCriterionId == criterion.AssessmentCriterionId)
                            .Bands;

                        if (criterion.SelectedBandId.HasValue && bands.All(x => x.BandId != criterion.SelectedBandId.GetValueOrDefault()))
                        {
                            throw new Exception($"Band {criterion.SelectedBandId} not found");
                        }
                    }
                }
            }
            var result = mTestResultQueryService.SaveJobExaminerMarkingResultWithNaatiNumber(request);
            response.ErrorMessage = result.ErrorMessage;
            return response;
        }

        public void SubmitExaminerRubricMarking(TestRubricMarkingContract model)
        {
            var markingDto = _autoMapperHelper.Mapper.Map<JobExaminerMarkingDto>(model);
            var request = new SaveExaminerMarkingRequest
            {
                JobExaminerMarking = markingDto,
                ClearNotAttempted = true,
                IsExaminerRequest = true,
                MaxCommentsLength = MaxRubrickCommentsLength
            };
            mTestResultQueryService.SaveJobExaminerMarkingResultWithNaatiNumber(request);
        }

        public MaterialRequestSearchResponse GetMaterialRequests(GetMaterialRequestsRequest request)
        {
            var filters = new List<TestMaterialRequestSearchCriteria>();

            filters.Add(new TestMaterialRequestSearchCriteria
            {
                Filter = TestMaterialRequestFilterType.MemberNaatiNumberIntList,
                Values = new[] { request.NaatiNumber.ToString() }
            });

            if (request.CredentialTypeId?.Any() ?? false)
            {
                filters.Add(new TestMaterialRequestSearchCriteria
                {
                    Filter = TestMaterialRequestFilterType.CredentialTypeIntList,
                    Values = request.CredentialTypeId.Select(x => x.ToString())
                });
            }
            if (request.DueDateFrom.HasValue)
            {
                filters.Add(new TestMaterialRequestSearchCriteria
                {
                    Filter = TestMaterialRequestFilterType.DueDateFromString,
                    Values = new[] { request.DueDateFrom.Value.ToFilterString() }
                });
            }
            if (request.DueDateTo.HasValue)
            {
                filters.Add(new TestMaterialRequestSearchCriteria
                {
                    Filter = TestMaterialRequestFilterType.DueDateToString,
                    Values = new[] { request.DueDateTo.Value.ToFilterString() }
                });
            }

            if (request.Overdue.HasValue)
            {
                filters.Add(new TestMaterialRequestSearchCriteria
                {
                    Filter = TestMaterialRequestFilterType.OverdueBoolean,
                    Values = new[] { request.Overdue.Value.ToString() }
                });
            }

            if (request.RoundStatusId?.Any() ?? false)
            {
                filters.Add(new TestMaterialRequestSearchCriteria
                {
                    Filter = TestMaterialRequestFilterType.RoundStatusIntList,
                    Values = request.RoundStatusId.Select(x => x.ToString())
                });
            }

            if (request.TestMaterialId?.Any() ?? false)
            {
                filters.Add(new TestMaterialRequestSearchCriteria
                {
                    Filter = TestMaterialRequestFilterType.OutputTestMaterialIdIntList,
                    Values = request.TestMaterialId.Select(x => x.ToString())
                });
            }

            var searchRequest = new TestMaterialRequestSearchRequest
            {
                Filters = filters,
                Take = request.Take,
                Skip = request.Skip
            };

            var response = mMaterialRequestQueryService.SearchTestMaterialRequests(searchRequest);
            var availableRows = mMaterialRequestQueryService.CountTestMaterialRequests(searchRequest).Data;
            return new MaterialRequestSearchResponse
            {
                MaterialRequests = response.Results.Select(x => MapMaterialRequestSearch(x, request.NaatiNumber)),
                TotalAvailableRows = availableRows
            };
        }

        private MaterialRequestSearchResultContract MapMaterialRequestSearch(TestMaterialRequestSearchDto dto, int naatiNumber)
        {
            var isCoordinator = dto.CoordinatorNaatiNumber == naatiNumber;
            return new MaterialRequestSearchResultContract()
            {
                MaterialRequestId = dto.Id,
                Title = dto.RequestTitle,
                TaskTypeName = dto.TestTaskType,
                CredentialTypeExternalName = dto.CredentialType,
                TestMaterialId = dto.OutputMaterialId,
                DueDate = dto.DueDate,
                RequestStatusName = ((MaterialRequestStatusTypeName)dto.RequestStatusTypeId).ToString().ToSentence(),
                RequestTypeName = dto.RequestType,
                RoundNumber = dto.Round,
                RoundId = dto.LatestRoundId,
                RoundStatusId = dto.RoundStatusTypeId,
                SkillDisplayName = dto.Language,
                RoundStatusName = ((MaterialRequestRoundStatusTypeName)dto.RoundStatusTypeId).ToString().ToSentence(),
                IsCoordinator = isCoordinator,
                IsEditable = isCoordinator && (dto.RoundStatusTypeId == (int)MaterialRequestRoundStatusTypeName.SentForDevelopment)
            };
        }

        public GetMaterialRequestResponse GetMaterialRequest(int materialRequestId, int naatiNumber, bool write)
        {
            if (!CanAccessMaterialRequest(naatiNumber, materialRequestId, write))
            {
                throw new Exception($"User not authorised to read material request id {materialRequestId}");
            }
            var documentTypes = mFileStorageService.ListDocumentTypes(new ListDocumentTypesRequest
            {
                Category = DocumentTypeCategoryTypeName.MaterialRequestSubmission
            });

            var response = mMaterialRequestQueryService.GetMaterialRequest(materialRequestId);

            return new GetMaterialRequestResponse
            {
                MaterialRequest = MapMaterialRequest(response),
                MaterialRequestTaskTypes = mApplicationQueryService.GetLookupType("MaterialRequestTaskType").Results,
                DocumentTypes = documentTypes.Types.Select(t => new KeyValuePair<int, string>(t.Id, t.DisplayName))
            };
        }

        private TestMaterialRequestContract MapMaterialRequest(MaterialRequestInfoDto material)
        {
            var contract = new TestMaterialRequestContract();

            contract.MaterialRequestId = material.MaterialRequestId;
            contract.TestMaterialId = material.OutputTestMaterial.Id;
            contract.Title = material.OutputTestMaterial.Title;
            contract.CredentialTypeExternalName = material.OutputTestMaterial.CredentialType;
            contract.SkillDisplayName = material.OutputTestMaterial.SkillName ?? material.OutputTestMaterial.Language;
            contract.TaskTypeName = material.OutputTestMaterial.TestComponentTypeName;
            contract.RequestTypeName = material.RequestTypeDisplayName;
            contract.RequestStatusName = material.StatusTypeDisplayName;
            contract.RoundId = material.LastRound.MaterialRoundId;
            contract.Round = $"Round {material.LastRound.RoundNumber}";
            contract.RoundStatusId = material.LastRound.StatusTypeId;
            contract.RoundStatusName = material.LastRound.StatusTypeDisplayName;
            contract.RoundRequestedDate = material.LastRound.RequestedDate;
            contract.DueDate = material.LastRound.DueDate;
            contract.Members = material.Members?.Select(m => _autoMapperHelper.Mapper.Map<MaterialRequestPanelMembershipContract>(m)).ToArray();
            contract.ProductSpecificationCostPerUnit = material.ProductSpecificationCostPerUnit;
            contract.PanelId = material.PanelId;
            contract.CredentialTypeId = material.OutputTestMaterial.CredentialTypeId;
            contract.RoundNumber = material.LastRound.RoundNumber;
            contract.MaxBillableHours = material.MaxBillableHours;
            contract.TestMaterialDomainId = material.TestMaterialDomainId;

            return contract;
        }
        
        public SaveMaterialRequestRoundAttachmentResponse SaveMaterialRequestRoundAttachment(SaveMaterialRequestRoundAttachmentRequest request)
        {
            if (!CanAccessRound(request.NAATINumber, request.MaterialRequestRoundId,true, MaterialRequestRoundStatusTypeName.SentForDevelopment))
            {
                throw new Exception($"User not authorised to save round attachment on round {request.MaterialRequestRoundId}");
            }

            var response = mMaterialRequestQueryService.CreateOrReplaceAttachment(new CreateOrReplaceMaterialRequestRoundAttachmentRequest
            {
                NAATINumber = request.NAATINumber,
                FilePath = request.FilePath,
                Title = request.Title,
                MaterialRequestRoundId = request.MaterialRequestRoundId,
                StoragePath = $"MaterialRequestRound/{request.MaterialRequestRoundId}/{request.Title}",
                Type = (StoredFileType)request.Type,
                UploadedDateTime = DateTime.Now,
                NcmsAvailable = false,
                EportalDownload = true
            });

            return new SaveMaterialRequestRoundAttachmentResponse
            {
                StoredFileId = response.StoredFileId
            };
        }

        public GetMaterialRequestRoundAttachmentsResponse GetMaterialRequestRoundAttachments(GetMaterialRequestRoundAttachmentsRequest request)
        {
            if (!CanAccessRound(request.NAATINumber, request.MaterialRequestRoundId, false))
            {
                return new GetMaterialRequestRoundAttachmentsResponse
                {
                    Attachments = new List<MaterialRequestRoundAttachmentDto>()
                };
            }
            var serviceRequest = _autoMapperHelper.Mapper.Map<F1Solutions.Naati.Common.Contracts.Dal.Request.GetMaterialRequestRoundAttachmentsRequest>(request);
            var response = mMaterialRequestQueryService.GetAttachments(serviceRequest);
            return _autoMapperHelper.Mapper.Map<GetMaterialRequestRoundAttachmentsResponse>(response);
        }

        public void DeleteMaterialRequestRoundAttachment(DeleteMaterialRequestRoundAttachmentRequest request)
        {
            if (!CanAccessRound(request.NaatiNumber, request.MaterialRequestRoundId,true, MaterialRequestRoundStatusTypeName.SentForDevelopment))
            {
                throw new Exception($"User not authorised to remove round attachment id {request.MaterialRequestRoundAttachmentId}");
            }

            var existingDocumentsRequest = new F1Solutions.Naati.Common.Contracts.Dal.Request.GetMaterialRequestRoundAttachmentsRequest
            {
                MaterialRequestRoundId = request.MaterialRequestRoundId,
                ExaminerAvailable = true,
                NAATINumber = request.NaatiNumber,

            };

            var document = mMaterialRequestQueryService.GetAttachments(existingDocumentsRequest).Attachments.FirstOrDefault(x => x.MaterialRequestRoundAttachmentId == request.MaterialRequestRoundAttachmentId);

            if (document == null)
            {
                return;
            }

            if (!document.IsOwner)
            {
                throw new Exception($"User is not onwer of round attachment id {request.MaterialRequestRoundAttachmentId}.");
            }

            var serviceRequest = new F1Solutions.Naati.Common.Contracts.Dal.Request.DeleteMaterialRequestRoundAttachmentRequest
            {
                MaterialRequestRoundAttachmentId = request.MaterialRequestRoundAttachmentId
            };

            mMaterialRequestQueryService.DeleteAttachment(serviceRequest);
        }

        public GetMaterialRequestRoundAttachmentResponse GetMaterialRequestRoundAttachment(GetMaterialRequestRoundAttachmentRequest request)
        {
            if (!CanAccessRound(request.NaatiNumber, request.MaterialRequestRoundId, false))
            {
                throw new Exception($"User not authorised to remove round attachment id {request.MaterialRequestRoundAttachmentId}");
            }

            var response = mMaterialRequestQueryService.GetAttachment(new F1Solutions.Naati.Common.Contracts.Dal.Request.GetMaterialRequestRoundAttachmentRequest
            {
                MaterialRequestRoundAttachmentId = request.MaterialRequestRoundAttachmentId,
                TempFileStorePath = request.TempFileStorePath
            });

            return _autoMapperHelper.Mapper.Map<GetMaterialRequestRoundAttachmentResponse>(response);
        }

        public GetPanelMembershipLookUpResponse GetPanelMembershipLookUp(GetPanelMemberLookupRequestModel request)
        {
            var getRequest = new GetPanelMemberLookupRequest { PanelIds = request.PanelIds, ActiveMembersOnly = request.ActiveMembersOnly, CredentialTypeId = request.CredentialTypeId };
            var response = mPanelService.GetPanelMembershipLookUp(getRequest);
            
            var models = response.Results.Select(_autoMapperHelper.Mapper.Map<PanelMembershipLookupModel>).ToList();
            return new GetPanelMembershipLookUpResponse
            {
                Members = models
            };
        }

        public UpdateMaterialRequestMembersResponse UpdateMaterialRequestMembers(MyNaati.Contracts.BackOffice.UpdateMaterialRequestMembersRequest request)
        {
            if (!CanAccessRound(request.NaatiNumber, request.RoundId, true, MaterialRequestRoundStatusTypeName.SentForDevelopment, 1))
            {
                throw new Exception($"User isn not authorised to save members on round{request.RoundId}");
            }
            var serviceRequest = _autoMapperHelper.Mapper.Map<UpdateMaterialRequestMembersRequest>(request);
            mMaterialRequestQueryService.UpdateMaterialRequestMembers(serviceRequest);
            return new UpdateMaterialRequestMembersResponse();
        }

        private bool CanAccessMaterialRequest(int naatiNumber, int materialRequestId, bool write)
        {
            var search = new TestMaterialRequestSearchRequest()
            {
                Filters = new[]
                {
                    new TestMaterialRequestSearchCriteria()
                    {
                        Filter = write ? TestMaterialRequestFilterType.CoordinatorNaatiNumberIntList :TestMaterialRequestFilterType.MemberNaatiNumberIntList,
                        Values = new[] {naatiNumber.ToString()}
                    },
                    new TestMaterialRequestSearchCriteria()
                    {
                        Filter = TestMaterialRequestFilterType.MaterialRequestIdIntList,
                        Values = new[] {materialRequestId.ToString()}
                    },
                },
                Take = 1
            };
            var result = mMaterialRequestQueryService.SearchTestMaterialRequests(search);
            return result.Results.Any();
        }

        private bool CanAccessRound(int naatiNumber, int materialRequestRoundId, bool write, MaterialRequestRoundStatusTypeName? roundStatus = null, int? roundNumber = null)
        {
            var filters = new List<TestMaterialRequestSearchCriteria>
            {
                new TestMaterialRequestSearchCriteria()
                {
                    Filter =write ? TestMaterialRequestFilterType.CoordinatorNaatiNumberIntList :TestMaterialRequestFilterType.MemberNaatiNumberIntList,
                    Values = new[] {naatiNumber.ToString()}
                },
                new TestMaterialRequestSearchCriteria()
                {
                    Filter = TestMaterialRequestFilterType.LatestRoundIdIntList,
                    Values = new[] {materialRequestRoundId.ToString()}
                },
            };

            if (roundStatus.HasValue)
            {
                filters.Add(
                    new TestMaterialRequestSearchCriteria()
                    {
                        Filter = TestMaterialRequestFilterType.RoundStatusIntList,
                        Values = new[] { ((int)roundStatus).ToString() }
                    });
            }
            if (roundNumber.HasValue)
            {
                filters.Add(
                    new TestMaterialRequestSearchCriteria()
                    {
                        Filter = TestMaterialRequestFilterType.RoundNumberIntList,
                        Values = new[] { ((int)roundNumber).ToString() }
                    });
            }
            var search = new TestMaterialRequestSearchRequest()
            {
                Filters = filters,
                Take = 1
            };
            var result = mMaterialRequestQueryService.SearchTestMaterialRequests(search);
            return result.Results.Any();
        }

        public ListMaterialRequestPublicNotesResponse ListMaterialRequestPublicNotes(int materialRequestId)
        {
            var notesRequest = new GetNotesRequest
            {
                NoteType = NoteType.MaterialRequestPublic,
                EntityId = materialRequestId
            };

            var notesResponse = mNoteService.GetNotes(notesRequest);
            var notes = notesResponse?.Notes.Select(_autoMapperHelper.Mapper.Map<NoteModel>).OrderByDescending(x => x.CreatedDate).ToArray() ?? new NoteModel[0];

            return new ListMaterialRequestPublicNotesResponse
            {
                Notes = notes
            };
        }

        public void CreateMaterialRequestPublicNote(int materialRequestId, string note)
        {
            var defaultIdentity = mSecretsProvider.Get(SecuritySettings.MyNaatiDefaultIdentityKey);
            var user = mUserService.GetUser(defaultIdentity);

            var noteRequest = new CreateNoteRequest
            {
                NoteType = NoteType.MaterialRequestPublic,
                EntityId = materialRequestId,
                UserId = user.Id,
                Note = note,
                Highlight = true,
                ReadOnly = false
            };

            mNoteService.CreateOrUpdateNote(noteRequest);
        }

        public IEnumerable<LookupContract> GetMaterialRequestCredentialTypes(int naatiNumber)
        {
            return mMaterialRequestQueryService.GetAvailableMaterialRequestCredentialTypes(naatiNumber)?.Results?.Select(r => _autoMapperHelper.Mapper.Map<LookupContract>(r));
        }

        public IEnumerable<LookupContract> GetRoundStatusTypes()
        {
            return mApplicationQueryService.GetLookupType("MaterialRequestRoundStatus")?.Results?.OrderBy(r => r.Id)?.Select(r =>
            {
                var item = _autoMapperHelper.Mapper.Map<LookupContract>(r);
                item.DisplayName = 
                    item.Id == (int)MaterialRequestRoundStatusTypeName.SentForDevelopment ? "Awaiting Development" :
                    item.Id == (int)MaterialRequestRoundStatusTypeName.AwaitingApproval ? "Awaiting NAATI's Approval" :
                    item.DisplayName;
                return item;
            });
        }

        public GetMaterialRequestRoundLinkResponse GetMaterialRequestRoundLink(int materialRequestRoundId, int naatiNumber)
        {
            if (!CanAccessRound(naatiNumber, materialRequestRoundId, false))
            {
                return new GetMaterialRequestRoundLinkResponse
                {
                    Results = new List<MaterialRequestLinkContract>()
                };
            }

            var response = mMaterialRequestQueryService.GetMaterialRequestRoundLink(new GetMaterialRequestRoundLinkRequest
            {
                MaterialRequestRoundId = materialRequestRoundId,
            });

            var results = response.Results.Select(
                m => new MaterialRequestLinkContract()
                {
                    Id = m.Id,
                    Link = m.Link,
                    MaterialRequestRoundId = materialRequestRoundId,
                    IsOwner = m.PersonNaatiNumber.GetValueOrDefault() == naatiNumber
                });

            return new GetMaterialRequestRoundLinkResponse
            {
                Results = results
            };
        }

        public SaveMaterialRequestRoundLinkResponse SaveMaterialRequestRoundLink(MyNaati.Contracts.BackOffice.SaveMaterialRequestRoundLinkRequest request)
        {
            if (!CanAccessRound(request.NaatiNumber, request.MaterialRequestRoundId, true, MaterialRequestRoundStatusTypeName.SentForDevelopment))
            {
                throw new Exception($"User not authorised to delete link on round {request.MaterialRequestRoundId}");
            }
            mMaterialRequestQueryService.SaveMaterialRequestRoundLink(new SaveMaterialRequestRoundLinkRequest
            {
                MaterialRequestRoundId = request.MaterialRequestRoundId,
                NaatiNumber = request.NaatiNumber,
                Link = request.Link
            });

            return new SaveMaterialRequestRoundLinkResponse();
        }

        public DeleteMaterialRequestLinkResponse DeleteMaterialRequestLink(int materialRequestRoundLinkId, int naatiNumber, int materialRoundId)
        {
            if (!CanAccessRound(naatiNumber, materialRoundId, true,MaterialRequestRoundStatusTypeName.SentForDevelopment))
            {
                throw new Exception($"User not authorised to delete link on round {materialRoundId}");
            }

            var existingLink = mMaterialRequestQueryService.GetMaterialRequestRoundLink(
                    new GetMaterialRequestRoundLinkRequest() { MaterialRequestRoundId = materialRoundId })
                .Results.FirstOrDefault(x => x.Id == materialRequestRoundLinkId);

            if (existingLink == null)
            {
                throw new Exception(
                    $"Document link {materialRequestRoundLinkId} does not exist in round {materialRoundId}");
            }

            mMaterialRequestQueryService.DeleteMaterialRequestLink(new DeleteMaterialRequestLinkRequest
            {
                MaterialRequestRoundLinkId = materialRequestRoundLinkId,
            });

            return new DeleteMaterialRequestLinkResponse();
        }

        public IEnumerable<LookupContract> GetTestMaterialDomains(int credentialTypeId)
        {
            var response = mTestMaterialQueryService.GetTestMaterialDomains(credentialTypeId);
            return response.Results.Select(l => _autoMapperHelper.Mapper.Map<LookupContract>(l));
        }

        public GetTestMaterialCreationPaymentsResponse GettestMaterialCreationPayments(GetTestMaterialCreationPaymentsRequest getTestMaterialCreationPaymentsRequest)
        {
            var response = mTestMaterialQueryService.GetTestMaterialCreationPayments(getTestMaterialCreationPaymentsRequest);

            var payments = response.Payments
                .Select(_autoMapperHelper.Mapper.Map<TestMaterialCreationPaymentContract>)
                .ToList();

            return new GetTestMaterialCreationPaymentsResponse
            {
                Payments = payments
            };
        }
    }
}
