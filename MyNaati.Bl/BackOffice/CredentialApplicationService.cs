using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.ExtensionHelpers;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using MyNaati.Bl.Properties;
using MyNaati.Contracts.BackOffice;
using CredentialLookupTypeResponse = MyNaati.Contracts.BackOffice.CredentialLookupTypeResponse;
using DocumentTypesResponse = MyNaati.Contracts.BackOffice.DocumentTypesResponse;
using GetCredentialAttachmentFileRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetCredentialAttachmentFileRequest;
using GetCredentialAttachmentFileResponse = MyNaati.Contracts.BackOffice.GetCredentialAttachmentFileResponse;
using GetCredentialAttachmentsByIdResponse = MyNaati.Contracts.BackOffice.GetCredentialAttachmentsByIdResponse;
using GetEndorsedQualificationForApplicationFormResponse = MyNaati.Contracts.BackOffice.GetEndorsedQualificationForApplicationFormResponse;
using GetTestDetailsRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetTestDetailsRequest;
using GetTestResultsResponse = MyNaati.Contracts.BackOffice.GetTestResultsResponse;
using IExaminerToolsService = F1Solutions.Naati.Common.Contracts.Dal.QueryServices.IExaminerToolsService;
using ISystemValueService = F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValues.ISystemValueService;

namespace MyNaati.Bl.BackOffice
{
    public class CredentialApplicationService : ICredentialApplicationService
    {

        public const string SurnameNotStated = "[Not Stated]";
        private readonly IApplicationQueryService mApplicationQueryService;
        private readonly IPersonQueryService mPersonQueryService;
        private readonly ITestQueryService mTestQueryService;
        private readonly ITestSessionQueryService mTestSessionQueryService;
        private readonly IApplicationFormsService mApplicationFormsService;
        private readonly IApplicationBusinessLogicService mApplicationBusinessLogicService;
        private readonly ISystemQueryService mSystemQueryService;
        private readonly ITestResultQueryService mTestResultQueryService;
        private readonly IExaminerToolsService mExaminerToolsService;
        private readonly IDisplayBillsCacheQueryService _displayBillsCacheQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly ISystemValueService _systemValueService;

        private const string SupplementaryTest = " (Supplementary Test)";

        public CredentialApplicationService(
            IApplicationQueryService applicationQueryService,
            IPersonQueryService personQueryService, ITestQueryService testQueryService,
            ITestSessionQueryService testSessionQueryService,
            IApplicationFormsService applicationFormsService,
            IApplicationBusinessLogicService applicationBusinessLogicService,
            ITestResultQueryService testResultQueryService,
            IExaminerToolsService examinerToolsService,
            ISystemQueryService systemQueryService,
            IDisplayBillsCacheQueryService displayBillsCacheQueryService,
            IAutoMapperHelper autoMapperHelper,
            ISystemValueService systemValueService)
        {
            mApplicationQueryService = applicationQueryService;
            mPersonQueryService = personQueryService;
            mTestQueryService = testQueryService;
            mTestSessionQueryService = testSessionQueryService;
            mApplicationFormsService = applicationFormsService;
            mApplicationBusinessLogicService = applicationBusinessLogicService;
            mTestResultQueryService = testResultQueryService;
            mExaminerToolsService = examinerToolsService;
            mSystemQueryService = systemQueryService;
            _displayBillsCacheQueryService = displayBillsCacheQueryService;
            _autoMapperHelper = autoMapperHelper;
            _systemValueService = systemValueService;
        }

        public SkillLookupResponse GetSkillsForApplicationForCredentialType(GetSkillsForApplicationForCredentialTypeRequest request)
        {
            var response = mApplicationQueryService.GetSkillsForCredentialType(new GetSkillsForCredentialTypeRequest { CredentialTypeIds = request.CredentialTypeIds, ApplicationId = request.ApplicationId }).Results;
            var skillsInProgress = GetInProgressCredentialRequests(request.NAATINumber).Results.ToLookup(x => x.SkillId, y => y);
            var activeCredentialSkills = GetActiveAndFutureCredentials(request.NAATINumber).Credentials.ToLookup(x => x.SkillId, y => y);
            response = response.Where(x => !skillsInProgress[x.Id].Any() && !activeCredentialSkills[x.Id].Any());
            return new SkillLookupResponse { Results = response.OrderBy(x => x.DisplayName).Select(_autoMapperHelper.Mapper.Map<SkillLookupContract>) };
        }

        public SaveCredentialRequestResponse SaveCredentialRequest(CredentialRequestRequestContract request)
        {
            var applicationFormHelper = mApplicationFormsService;
            var credentialRequest = applicationFormHelper.SaveCredentialRequest(request);

            return new SaveCredentialRequestResponse { CredentialRequestId = credentialRequest.Id, CredentialRequestPathTypeId = credentialRequest.CredentialRequestPathTypeId };
        }

        public GetApplicationDetailsResponse GetApplicationDetails(int applicationId)
        {
            var applicationFormHelper = mApplicationFormsService;
            return applicationFormHelper.GetApplicationDetails(applicationId);
        }

        public void DeleteCredentialRequest(DeleteCredentialRequestContract request)
        {
            var applicationFormHelper = mApplicationFormsService;
            applicationFormHelper.DeleteCredentialRequest(request);
        }

        public CreateOrReplaceAttachmentResponse CreateOrReplaceAttachment(CreateOrReplaceAttachmentContract request)
        {
            var user = mApplicationQueryService.GetUser(new GetUserRequest { UserName = request.UserName });
            request.UploadedByUserId = user.UserId.GetValueOrDefault();
            request.StoragePath = $@"{(StoredFileType)request.Type}\{request.CredentialApplicationId}\{request.FileName}";
            var attachmentRequest = _autoMapperHelper.Mapper.Map<CreateOrReplaceApplicationAttachmentRequest>(request);
            var response = mApplicationQueryService.CreateOrReplaceAttachment(attachmentRequest);
            return new CreateOrReplaceAttachmentResponse { StoredFileId = response.StoredFileId };
        }

        public void DeleteAttachment(int attachmentId)
        {
            mApplicationQueryService.DeleteAttachment(
                new DeleteApplicationAttachmentRequest { StoredFileId = attachmentId });
        }

        public GetAvailableTestSessionsResponse GetAllAvailableTestSessionsAndRejectableTestSessions(int credentialRequestId)
        {
            var serviceResponse = mApplicationQueryService.GetAllAvailableTestSessionsAndRejectableTestSession(credentialRequestId);
            
            var availableTestSessionDtos = serviceResponse.AvailableTestSessions;
            var allocatedTestSessionDto = serviceResponse.AllocatedTestSession;

            //make sure the allocated test session is not in the available sessions
            if (allocatedTestSessionDto != null && availableTestSessionDtos!= null && availableTestSessionDtos.Any())
            {
                availableTestSessionDtos = availableTestSessionDtos.Where(x => x.TestSessionId != allocatedTestSessionDto.TestSessionId).ToList();
            }

            return new GetAvailableTestSessionsResponse
            {
                AvailableTestSessions = availableTestSessionDtos == null ? Enumerable.Empty<AvailableTestSessionContract>() : availableTestSessionDtos.Select(_autoMapperHelper.Mapper.Map<AvailableTestSessionContract>),
                AllocatedTestSession = allocatedTestSessionDto == null ? null :  _autoMapperHelper.Mapper.Map<AvailableTestSessionContract>(allocatedTestSessionDto)
            };
        }

        public bool CheckCredentialRequestBelongsToCurrentUser(int? naatiNumber, int credentialRequestId)
        {
            var statusesToShow = new[]
            {
                CredentialRequestStatusTypeName.TestSessionAccepted,
                CredentialRequestStatusTypeName.AwaitingTestPayment,
                CredentialRequestStatusTypeName.ProcessingTestInvoice,
                CredentialRequestStatusTypeName.TestAccepted,
                CredentialRequestStatusTypeName.EligibleForTesting,
            };

            var filters = new List<ApplicationSearchCriteria>
            {
                new ApplicationSearchCriteria
                {
                    Filter = ApplicationFilterType.NaatiNumberIntList,
                    Values = new[] {naatiNumber.ToString()}
                },
                new ApplicationSearchCriteria
                {
                    Filter = ApplicationFilterType.CredentialRequestStatusIntList,
                    Values =  statusesToShow.Select(x=> ((int)x).ToString()).ToList()
                },
                new ApplicationSearchCriteria
                {
                    Filter = ApplicationFilterType.CredentialRequestIntList,
                    Values =  new []{ credentialRequestId.ToString()}
                },
            };

            var getRequest = new GetApplicationSearchRequest
            {
                Filters = filters
            };
            var applications = mApplicationQueryService.SearchApplication(getRequest);

            return applications.Results.Any();
        }

        public bool HasAvailableTests(int? naatiNumber)
        {
            var statusesToShow = new[]
            {
                CredentialRequestStatusTypeName.TestSessionAccepted,
                CredentialRequestStatusTypeName.AwaitingTestPayment,
                CredentialRequestStatusTypeName.ProcessingTestInvoice,
                CredentialRequestStatusTypeName.TestAccepted,
                CredentialRequestStatusTypeName.EligibleForTesting,
            };

            var filters = new List<ApplicationSearchCriteria>
            {
                new ApplicationSearchCriteria
                {
                    Filter = ApplicationFilterType.NaatiNumberIntList,
                    Values = new[] {naatiNumber.ToString()}
                },
                new ApplicationSearchCriteria
                {
                    Filter = ApplicationFilterType.CredentialRequestStatusIntList,
                    Values =  statusesToShow.Select(x=> ((int)x).ToString()).ToList()
                },
            };

            var getRequest = new GetApplicationSearchRequest
            {
                Filters = filters,
                Take = 1
            };
            var applications = mApplicationQueryService.SearchApplication(getRequest);

            return applications.Results.Any();
        }

        public bool HasCredentialsByNaatiNumber(int? naatiNumber)
        {
            var filters = new List<ApplicationSearchCriteria>
            {
                new ApplicationSearchCriteria
                {
                    Filter = ApplicationFilterType.NaatiNumberIntList,
                    Values = new[] {naatiNumber.ToString()}
                },
                new ApplicationSearchCriteria
                {
                    Filter = ApplicationFilterType.CredentialRequestStatusIntList,
                    Values = new [] {((int) CredentialRequestStatusTypeName.CertificationIssued).ToString()}
                }
            };

            var getRequest = new GetApplicationSearchRequest
            {
                Filters = filters,Take = 1
            };
            var applications = mApplicationQueryService.SearchApplication(getRequest);

            return applications.Results.Any();  
        }

        public GenericResponse<bool> GetIsPractitionerFromNaatiNumber(int naatiNumber)
        {
            var isPractitionerResponse = mApplicationQueryService.GetIsPractitionerFromNaatiNumber(naatiNumber);

            if (!isPractitionerResponse.Success)
            {
                return new GenericResponse<bool>(isPractitionerResponse.Data)
                {
                    Success = false,
                    Errors = isPractitionerResponse.Errors
                };
            }

            return isPractitionerResponse.Data;
        }

        public bool IsDisplayBills(int naatiNumber)
        {
            return _displayBillsCacheQueryService.IsDisplayBills(naatiNumber);
        }

        public GetSittableCredentialRequestResponse GetPayableRequestsByNaatiNumber(int? naatiNumber)
        {
            var statusesToShow = new[]
            {
                CredentialRequestStatusTypeName.AwaitingTestPayment,
            };

            var excludedStatuses = Enum.GetValues(typeof(CredentialRequestStatusTypeName))
                .Cast<CredentialRequestStatusTypeName>()
                .Where(e => !statusesToShow.Contains(e)).ToList();

            var serviceResponse = mApplicationQueryService.GetBasicCredentialRequestsByNaatiNumber(naatiNumber.GetValueOrDefault(), excludedStatuses);
            var credentialTests = serviceResponse.Results.Select(MapSittableRequest).ToList();

            var allCredentials = new GetSittableCredentialRequestResponse
            {
                Results = credentialTests
            };

            return allCredentials;
        }

        public GetSittableCredentialRequestResponse GetSittableRequestsByNaatiNumberAndApplicationId(int naatiNumber,int applicationId)
        {
            var statusesToShow = new[]
            {
                CredentialRequestStatusTypeName.RequestEntered
            };

            var excludedStatuses = Enum.GetValues(typeof(CredentialRequestStatusTypeName))
                .Cast<CredentialRequestStatusTypeName>()
                .Where(e => !statusesToShow.Contains(e)).ToList();

            var serviceResponse = mApplicationQueryService.GetBasicCredentialRequestsByApplicationId(applicationId, excludedStatuses);
            var credentialTests = serviceResponse.Results.Select(MapSittableRequest).ToList();

            var allCredentials = new GetSittableCredentialRequestResponse
            {
                Results = credentialTests
            };

            return allCredentials;
        }

       public GetSittableCredentialRequestResponse GetSittableRequestsByNaatiNumber(int? naatiNumber)
        {
            var statusesToShow = new[]
            {
                CredentialRequestStatusTypeName.TestSessionAccepted,
                CredentialRequestStatusTypeName.AwaitingTestPayment,
                CredentialRequestStatusTypeName.ProcessingTestInvoice,
                CredentialRequestStatusTypeName.TestAccepted,
                CredentialRequestStatusTypeName.EligibleForTesting
            };

            var excludedStatuses = Enum.GetValues(typeof(CredentialRequestStatusTypeName))
                .Cast<CredentialRequestStatusTypeName>()
                .Where(e => !statusesToShow.Contains(e)).ToList();
            
            var serviceResponse = mApplicationQueryService.GetBasicCredentialRequestsByNaatiNumber(naatiNumber.GetValueOrDefault(), excludedStatuses);
            var credentialTests = serviceResponse.Results.Select(MapSittableRequest).ToList();
            
            var allCredentials = new GetSittableCredentialRequestResponse
            {
                Results = credentialTests
            };

            return allCredentials;
        }

        public GetTestResultsResponse GetTestResults(int naatiNumber)
        {
            var response = mTestQueryService.GetTestResults(new GetTestResultsRequest { NaatiNumber = naatiNumber });

            var paidTestReviewAvailableDays = Convert.ToInt32(mSystemQueryService.GetSystemValue(new GetSystemValueRequest { ValueKey = "PaidTestReviewAvailableDays" }).Value);
            var supplementaryTestAvailableDays = Convert.ToInt32(mSystemQueryService.GetSystemValue(new GetSystemValueRequest { ValueKey = "SupplementaryTestAvailableDays" }).Value);

            var lastCredentialTestIds = response.Results.GroupBy(x => x.CredentialRequestId)
                .Select(y => y.OrderByDescending(z => z.TestSittingId).First().TestSittingId).ToList();

            var inProgressCredentialRequests = GetInProgressCredentialRequests(naatiNumber);

            var testResults = response.Results.Select(x => MapTestResultContract(x, lastCredentialTestIds.Any(y => y == x.TestSittingId), paidTestReviewAvailableDays, supplementaryTestAvailableDays, inProgressCredentialRequests));

            var results = new GetTestResultsResponse
            {
                Results = testResults
            };

            return results;
        }

        private SittableCredentialRequestContract MapSittableRequest(CredentialRequestBasicDto credentailRequestBasicDto)
        {
            var allocateSesssionStatuses = new[]
            {
                (int)CredentialRequestStatusTypeName.EligibleForTesting,
                (int)CredentialRequestStatusTypeName.TestAccepted,
                (int)CredentialRequestStatusTypeName.TestSessionAccepted,
            };
            var refundEligibleStatuses = new[]
            {
                (int)CredentialRequestStatusTypeName.TestAccepted
            };
            var credentialTestContract = new SittableCredentialRequestContract
            {
                TestSessionId = credentailRequestBasicDto.TestSessionId,
                TestDate = credentailRequestBasicDto.TestDate,
                ApplicationTypeDisplayName = credentailRequestBasicDto.ApplicationTypeDisplayName + (credentailRequestBasicDto.Supplementary ? SupplementaryTest : ""), 
                CredentialTypeDisplayName = credentailRequestBasicDto.CredentialTypeExternalName,
                SkillDisplayName = credentailRequestBasicDto.SkillDisplayName,
                VenueName = credentailRequestBasicDto.VenueName,
                Status = GetStatusDisplayName(credentailRequestBasicDto.StatusTypeId, credentailRequestBasicDto.Status),
                CredentialRequestStatusId = credentailRequestBasicDto.StatusTypeId,
                CredentialRequestId = credentailRequestBasicDto.Id,
                CanOpenDetails = credentailRequestBasicDto.TestSessionId.HasValue,
                CanSelectTestSession = credentailRequestBasicDto.HasAvailableTestSessions && allocateSesssionStatuses.Any(x=> x == credentailRequestBasicDto.StatusTypeId),
                CredentialApplicationId = credentailRequestBasicDto.CredentialApplicationId,
                CanRequestRefund = refundEligibleStatuses.Any(x => x == credentailRequestBasicDto.StatusTypeId)
            };

            return credentialTestContract;
        }

        private CredentialContract MapCredentialDetails(CredentialDto credential)
        {
            var credentialContract = new CredentialContract
            {
                Id = credential.Id,
                StartDate = credential.StartDate,
                ExpiryDate = GetCredentialExpiryDate(credential),
                CredentialTypeInternalName = credential.CredentialTypeInternalName,
                ShowInOnlineDirectory = credential.ShowInOnlineDirectory,
                CredentialTypeExternalName = credential.CredentialTypeExternalName,
                CredentialCategoryName = credential.CredentialCategoryName,
                SkillDisplayName = credential.SkillDisplayName,
                SkillId = credential.SkillId,
                Status = credential.Status,
                Certification = credential.Certification
            };

            return credentialContract;
        }


        private DateTime? GetCredentialExpiryDate(CredentialDto credential)
        {
          
            if (credential.CertificationPeriod != null)
            {
                DateTime? expiryDate = credential.CertificationPeriod.EndDate;

                if (credential.TerminationDate.HasValue && credential.TerminationDate < expiryDate)
                {
                    expiryDate = credential.TerminationDate;
                }

                return expiryDate;

            }

            return credential.ExpiryDate;
        }
        private TestResultContract MapTestResultContract(TestSittingResultDto result, bool isLastCredentialRequestTest, int paidTestReviewAvailableDays, int supplementaryTestAvailableDays, GetCredentialRequestsResponse inProgressCredentialRequests)
        {
            var overallResult = result.OverallResult;
            if (isLastCredentialRequestTest)
            {
                var statusId = result.CredentialRequestStatusTypeId;

                if (statusId == (int)CredentialRequestStatusTypeName.TestSat)
                {
                    overallResult = "In Progress";
                }

                if (statusId == (int)CredentialRequestStatusTypeName.UnderPaidTestReview
                    || statusId == (int)CredentialRequestStatusTypeName.ProcessingPaidReviewInvoice
                    || statusId == (int)CredentialRequestStatusTypeName.AwaitingPaidReviewPayment)
                {
                    overallResult = "Review In Progress";
                }
            }

            var eligibleForSupplementary = result.EligibleForASupplementaryTest &&
                                           result.CredentialRequestStatusTypeId == (int)CredentialRequestStatusTypeName.TestFailed &&
                                           result.ResultDate.HasValue;
            if (result.ResultDate.HasValue)
            {
                var eligibleForAPaidTestReviewClosingDate = result.ResultDate.Value.AddDays(paidTestReviewAvailableDays);
                result.EligibleForAPaidTestReview = result.EligibleForAPaidTestReview && DateTime.Now <= eligibleForAPaidTestReviewClosingDate;
                eligibleForSupplementary = eligibleForSupplementary && DateTime.Now < result.ResultDate.Value.AddDays(supplementaryTestAvailableDays);
            }

            var applicationInprogessWithSameCredential = inProgressCredentialRequests.Results != null && inProgressCredentialRequests.Results.Any(x => x.LevelId == result.CredentialTypeId && x.SkillId == result.SkillId);
            result.EligibleForAPaidTestReview = result.EligibleForAPaidTestReview && !applicationInprogessWithSameCredential;
            if (result.MinStandardMarkForPaidReview.HasValue && result.LastTestResultId.HasValue)
            {
                var marks = mExaminerToolsService.GetTestDetails(new GetTestDetailsRequest
                {
                    TestResultId = result.LastTestResultId.Value,
                    UseOriginalResultMark = false
                });

                result.EligibleForAPaidTestReview = result.EligibleForAPaidTestReview && marks.Components.Sum(x => x.Mark.GetValueOrDefault()) >= result.MinStandardMarkForPaidReview.Value;
            }
            var credentialTestContract = new TestResultContract
            {
                TestSittingId = result.TestSittingId,
                EligibleForAPaidTestReview = result.EligibleForAPaidTestReview,
                EligibleForASupplementaryTest = eligibleForSupplementary,
                TestDate = result.TestDate,
                CredentialTypeDisplayName = result.CredentialTypeDisplayName,
                SkillDisplayName = result.SkillDisplayName,
                VenueName = result.VenueName,
                OverallResult = overallResult,
                State = result.State,
                TestLocationName = result.TestLocationName,
                Supplementary = result.Supplementary
            };

            return credentialTestContract;
        }

        /// <summary>
        /// unused
        /// </summary>
        /// <param name="credentialRequestDto"></param>
        /// <returns></returns>
        private bool GetCanSelectTestSession(CredentialRequestDto credentialRequestDto)
        {
            return mApplicationQueryService.HasAvailableTestSessions(credentialRequestDto.Id);
        }
        private CredentialRequestTestSessionDto GetLastTestSessionDtoFrom(CredentialRequestDto credentialRequest)
        {
            return credentialRequest.TestSessions.OrderByDescending(x => x.CredentialTestSessionId).FirstOrDefault();
        }

        private string GetStatusDisplayName(int credentialRequestStausId, string status)
        {
            switch (credentialRequestStausId)
            {
                case (int)CredentialRequestStatusTypeName.TestAccepted:
                case (int)CredentialRequestStatusTypeName.EligibleForTesting:
                    return Resources.ToBeScheduled;

                case (int)CredentialRequestStatusTypeName.AwaitingTestPayment:
                    return Resources.AwaitingPayment;

                case (int)CredentialRequestStatusTypeName.ProcessingTestInvoice:
                    return Resources.ProcessingInvoice;

                case (int)CredentialRequestStatusTypeName.TestSessionAccepted:
                    return Resources.TestConfirmed;

                default:
                    return status;
            }
        }
        public GetManageTestSessionSamResponse GetManageTestSession(int naatiNumber, int credentialRequestId)
        {
            var credentialRequestResponse = mApplicationQueryService.GetCredentialRequestForUser(naatiNumber, credentialRequestId);

            var manageTestSessionContract = new ManageTestSessionContract();

            if (credentialRequestResponse.CredentialRequest != null)
            {
                var application = mApplicationQueryService.GetApplicationByCredentialRequestId(credentialRequestId);
                var credentialTestSession = GetLastTestSessionDtoFrom(credentialRequestResponse.CredentialRequest);
                TestSessionDto testSession = null;
                VenueDto venue = null;
                if (credentialTestSession != null)
                {
                    testSession = mTestSessionQueryService.GetTestSessionById(credentialTestSession.TestSessionId).Result;
                    venue = mTestQueryService.GetVenueById(testSession.VenueId).Result;
                }

                manageTestSessionContract = MapToManageTestSessionContract(testSession, credentialRequestResponse.CredentialRequest, credentialTestSession, application, venue);

            }

            return new GetManageTestSessionSamResponse { Result = manageTestSessionContract };
        }

        private ManageTestSessionContract MapToManageTestSessionContract(TestSessionDto testSession, CredentialRequestDto credentialRequestDto, CredentialRequestTestSessionDto credentialTestSession, GetApplicationResponse application, VenueDto venue)
        {

            var canRejectTestDate = testSession.TestDate >= DateTime.Now.AddDays(credentialRequestDto.CredentialType.TestSessionBookingRejectHours / 24);
            var manageTestSessionContract = new ManageTestSessionContract
            {
                TestSessionId = testSession.TestSessionId,
                TestSessionCredentialRequestId = credentialTestSession.CredentialTestSessionId,
                CustomerNo = application.Result.NaatiNumber.ToString(),
                Application = credentialRequestDto.ApplicationTypeDisplayName +
                              (credentialRequestDto.Supplementary ? SupplementaryTest : ""),
                TestDate = testSession.TestDate,
                Duration = testSession.Duration ?? 0,
                ArrivalTime = testSession.ArrivalTime ?? 0,
                CredentialType = credentialRequestDto.CredentialType.ExternalName,
                VenueName = venue.Name,
                VenueAddress = venue.Address,
                VenueCoordinates = venue.Coordinates,
                Skill = credentialRequestDto.Skill.DisplayName,
                Status = GetStatusDisplayName(credentialRequestDto.StatusTypeId, credentialRequestDto.Status),
                Notes = testSession.PublicNotes ?? string.Empty,
                CredentialRequestId = credentialRequestDto.Id,
                CredentialApplicationId = application.Result.ApplicationId,

                CanRejectTest = credentialRequestDto.StatusTypeId == (int)CredentialRequestStatusTypeName.TestSessionAccepted,
                CanChangeRejectTestDate = canRejectTestDate && credentialRequestDto.StatusTypeId == (int)CredentialRequestStatusTypeName.TestSessionAccepted,
            };


            return manageTestSessionContract;
        }

        public AttachmentsResponse GetAttachments(int applicationId)
        {
            var response =
                mApplicationQueryService.GetAttachments(
                    new GetApplicationAttachmentsRequest { ApplicationId = applicationId });

            var attachments = response.Attachments.Select(x => new AttachmentContract
            {
                Id = x.StoredFileId,
                Name = x.FileName,
                Size = x.FileSize,
                Type = x.DocumentType,
                TypeId = (int)x.Type
            });

            return new AttachmentsResponse { Results = attachments };
        }

        public DocumentTypesResponse GetDocumentTypes(GetDocumentTypesRequestContract request)
        {
            var applicationFormHelper = mApplicationFormsService;
            var documentTypes = applicationFormHelper.GetDocumentTypes(request);
            return new DocumentTypesResponse { Results = documentTypes };
        }

        public LookupResponse GetLocations(int applicationFormId)
        {
            var response = mApplicationQueryService.GetLocations(applicationFormId);
            return new LookupResponse { Results = response.Results.OrderBy(x => x.DisplayName).Select(_autoMapperHelper.Mapper.Map<LookupContract>) };
        }

        public void UpdatePersonPhoto(UpdatePhotoRequestContract request)
        {
            var updatePhotoRequest =_autoMapperHelper.Mapper.Map<UpdatePhotoDto>(request);
            mPersonQueryService.UpdatePhoto(updatePhotoRequest);
            File.Delete(request.FilePath);
        }

        public PersonImageResponse GetPersonImage(GetImageRequestContract request)
        {
            var getPersonRequest = new GetPersonPhotoByNaatiNumber
            {
                NaatiNumber = request.NaatiNumber,
                PractitionerNumber = request.PractitionerId,
                TempFolderPath = request.TempFolderPath

            };

            GetPersonPhotoResponse response = null;

            try
            {
                response = mPersonQueryService.GetPersonPhoto(getPersonRequest);
            }
            catch (WebServiceException e)
            {
                LoggingHelper.LogException(e);
            }

            return new PersonImageResponse
            {
                FilePath = response?.PersonPhotoFilePath
            };
        }

        public bool PersonHasPhoto(int naatiNumber)
        {
            var getPersonDetailsRequest = new GetPersonDetailsRequest
            {
                NaatiNumber = naatiNumber
            };

            var personDetailsResponse = mPersonQueryService.GetPersonDetailsBasic(getPersonDetailsRequest);

            return personDetailsResponse.PersonDetails.HasPhoto;
        }

        public GetPersonDetailsBasicResponse GetPersonDetailsBasic(int naatiNumber)
        {
            var getPersonDetailsRequest = new GetPersonDetailsRequest
            {
                NaatiNumber = naatiNumber
            };

            var personDetailsResponse = mPersonQueryService.GetPersonDetailsBasic(getPersonDetailsRequest);
            personDetailsResponse.PersonDetails.FamilyName =
                personDetailsResponse.PersonDetails.FamilyName.Equals(SurnameNotStated,
                    StringComparison.CurrentCultureIgnoreCase)
                    ? string.Empty
                    : personDetailsResponse.PersonDetails.FamilyName;

            return personDetailsResponse;
        }

        public FormLookupResponse GetPublicApplicationForms(bool isPractitioner, bool isRecertification, bool isAutenticated)
        {
            var request = new GetFormRequest
            {
                Public = true,
                Practitioner = isPractitioner,
                Private = isAutenticated,
                Recertification = isRecertification,
                NonPractitioner = !isPractitioner

            };
            var response = mApplicationQueryService.GetCredentialApplicationForms(request);
            var forms = response.Results.Select(_autoMapperHelper.Mapper.Map<FormLookupContract>).ToArray();


            return new FormLookupResponse { Results = forms };
        }

        public int GetNaatiNumberByApplicationId(int applicationId)
        {
            var naatiNumber = mApplicationQueryService.GetNaatiNumberByApplicationId(applicationId);
            return naatiNumber;
        }

        public ApplicationFormQuestionContract ReplaceQuestionFormTokens(ReplaceFormTokenRequest request)
        {
            return mApplicationFormsService.ReplaceQuestionFormTokens(request);
        }

        public WorkPracticeStatusResponse GetRecertficationtWorkPracticeStatus(WorkPracticeStatusRequest request)
        {
            return mApplicationFormsService.GetRecertficationtWorkPracticeStatus(request);
        }

        public RecertificationOptionsResponse GetRecertificationOptions(int naatiNumber)
        {
            var options = mApplicationFormsService.GetRecertificationOptions(naatiNumber);
            return new RecertificationOptionsResponse
            {
                Options = options
            };
        }

        public PdPointsStatusResponse GetRecertficationPdPointsStatus(PdPointsStatusRequest request)
        {
            return mApplicationFormsService.GetRecertficationPdPointsStatus(request);
        }

        public LookupResponse GetActiveApplicationForms()
        {
            var response = mApplicationQueryService.GetLookupType(LookupType.ActiveApplicationForms.ToString());

            return new LookupResponse { Results = response.Results.Select(_autoMapperHelper.Mapper.Map<LookupContract>) };
        }

        public LookupResponse GetPrivateApplicationForms()
        {
            var response = mApplicationQueryService.GetLookupType(LookupType.PrivateApplicationForms.ToString());

            return new LookupResponse { Results = response.Results.Select(_autoMapperHelper.Mapper.Map<LookupContract>) };
        }

        public LookupResponse GetPractionerApplicationForms()
        {
            var response = mApplicationQueryService.GetLookupType(LookupType.PractitionerApplicationForms.ToString());

            return new LookupResponse { Results = response.Results.Select(_autoMapperHelper.Mapper.Map<LookupContract>) };
        }

        public LookupResponse GetRecertificationApplicationForms()
        {
            var response = mApplicationQueryService.GetLookupType(LookupType.RecertificationApplicationForms.ToString());

            return new LookupResponse { Results = response.Results.Select(_autoMapperHelper.Mapper.Map<LookupContract>) };
        }

        public GetApplicationFormResponse GetCredentialApplicationForm(int applicationFormId)
        {

            var response = mApplicationQueryService.GetCredentialApplicationForm(applicationFormId);

            return new GetApplicationFormResponse { Result = _autoMapperHelper.Mapper.Map<ApplicationFormContract>(response.Result) };
        }

        public string GetInvoiceNumberByApplicationId(int applicationId)
        {
            var response = mApplicationQueryService.GetInvoiceNumberByApplicationId(applicationId);
            return response;
        }

        public int? GetFeeQuestionId()
        {
            var feeQuestionId = mApplicationQueryService.GetFeesQuestionId();
            if (feeQuestionId == null)
                throw new Exception("Fees Question Id was not found.");
            return mApplicationQueryService.GetFeesQuestionId();
        }

        public GetApplicationFormSectionsResponse GetCredentialApplicationFormSections(GetFormSectionRequestContract request)
        {
            var sections = new List<ApplicationFormSectionContract>();
            var response = GetCredentialApplicationForm(request.ApplicationFormId);
            var hasPermissions = response.Result != null &&
                                 !response.Result.Inactive &&
                                 (response.Result.FormUserTypeId == (int)CredentialApplicationFormUserTypeName.AnonymousUser ||
                                  (response.Result.FormUserTypeId == (int)CredentialApplicationFormUserTypeName.LoggedInUser && request.NaatiNumber > 0) ||
                                  (response.Result.FormUserTypeId == (int)CredentialApplicationFormUserTypeName.PractitionerUser && request.IsPractitioner) ||
                                  (response.Result.FormUserTypeId == (int)CredentialApplicationFormUserTypeName.RecertificationUser && request.IsRecertificationUser) ||
                                  (response.Result.FormUserTypeId == (int)CredentialApplicationFormUserTypeName.NonPractitionerUser && !request.IsPractitioner)
                                  );
            if (hasPermissions)
            {
                var applicationFormHelper = mApplicationFormsService;
                sections = applicationFormHelper.GetSections(request).ToList();
            }

            return new GetApplicationFormSectionsResponse
            {
                Results = sections
            };

        }


        private IList<ApplicationFormSectionContract> GetPaidReviewSections()
        {
            var termsAndConditions = mSystemQueryService
                .GetSystemValue(new GetSystemValueRequest() { ValueKey = "PaidReviewTermsAndConditions" })
                .Value;
            var sections = new List<ApplicationFormSectionContract>
            {
                new ApplicationFormSectionContract()
                {
                    Id = 1,
                    Name = "Test Details",
                    Questions = new List<ApplicationFormQuestionContract>
                    {
                        new ApplicationFormQuestionContract()
                        {
                            Id = 1,
                            AnswerTypeId = (int)CredentialApplicationFormAnswerTypeName.TestDetails,
                            Description = "Please note that you cannot apply for a new test until your review result has been finalised.",
                            AnswerOptions = Enumerable.Empty<AnswerOptionContract>(),
                            QuestionLogics = Enumerable.Empty<QuestionLogicContract>()
                        },

                    }
                },
                new ApplicationFormSectionContract()
                {
                    Id = 2,
                    Name = "Payment",
                    Questions = new List<ApplicationFormQuestionContract>
                    {
                        new ApplicationFormQuestionContract()
                        {
                            Id = 2,
                            AnswerTypeId =  (int)CredentialApplicationFormAnswerTypeName.PaymentControl,
                            AnswerOptions = Enumerable.Empty<AnswerOptionContract>(),
                            QuestionLogics = Enumerable.Empty<QuestionLogicContract>()
                        }
                    }
                },
                new ApplicationFormSectionContract()
                {
                    Id = 3,
                    Name = "Terms and Conditions",
                    Questions = new List<ApplicationFormQuestionContract>
                    {
                        new ApplicationFormQuestionContract()
                        {
                            Id = 3,
                            Text = $"Do you agree to the following <a href=\"{termsAndConditions}\" target=\"_blank\">Terms and Conditions</a>?",
                            AnswerTypeId =(int)CredentialApplicationFormAnswerTypeName.RadioOptions,
                            AnswerOptions = new []
                            {
                                new AnswerOptionContract
                                {
                                    Id = 1,
                                    Option = "Yes",
                                    Description = "Click <b>Finish</b> to complete this application.",
                                    Documents = Enumerable.Empty<AnswerDocumentContract>()
                                },
                                new AnswerOptionContract
                                {
                                    Id = 2,
                                    Option = "No",
                                    Description = "Click <b>Finish</b> to cancel this application.",
                                    Documents = Enumerable.Empty<AnswerDocumentContract>()
                                },
                            },
                            QuestionLogics = Enumerable.Empty<QuestionLogicContract>()
                        }
                    }
                }
            };

            return sections;
        }


        private IList<ApplicationFormSectionContract> GetSelectTestSessionSections(
            bool paymentRequired, 
            string refundPolicy)
        {
            var termsAndConditions = mSystemQueryService
               .GetSystemValue(new GetSystemValueRequest() { ValueKey = "SelectTestTermsAndConditionsUrl" })
               .Value;

            var sections = new List<ApplicationFormSectionContract>
            {
                new ApplicationFormSectionContract()
                {
                    Id = 1,
                    Name = "Test Session",
                    Questions = new List<ApplicationFormQuestionContract>
                    {
                        new ApplicationFormQuestionContract()
                        {
                            Id = 1,
                            AnswerTypeId = (int) CredentialApplicationFormAnswerTypeName.TestDetails,
                            AnswerOptions = Enumerable.Empty<AnswerOptionContract>(),
                            QuestionLogics = Enumerable.Empty<QuestionLogicContract>()
                        },

                    }
                }
            };

            if (paymentRequired)
            {
                sections.Add(new ApplicationFormSectionContract()
                {
                    Id = 2,
                    Name = "Payment",
                    Questions = new List<ApplicationFormQuestionContract>
                    {
                        new ApplicationFormQuestionContract()
                        {
                            Id = 2,
                            AnswerTypeId =  (int)CredentialApplicationFormAnswerTypeName.PaymentControl,
                            AnswerOptions = Enumerable.Empty<AnswerOptionContract>(),
                            QuestionLogics = Enumerable.Empty<QuestionLogicContract>()
                        }
                    }
                });
            }

            sections.Add(new ApplicationFormSectionContract()
            {
                Id = 3,
                Name = "Terms and Conditions",
                Questions = new List<ApplicationFormQuestionContract>
                {
                    new ApplicationFormQuestionContract()
                    {
                        Id = 3,
                        Text = $"Do you agree to the <a href=\"{termsAndConditions}\" target=\"_blank\">Terms and Conditions</a>?",
                        AnswerTypeId = (int) CredentialApplicationFormAnswerTypeName.RadioOptions,
                        AnswerOptions = new[]
                        {
                            new AnswerOptionContract
                            {
                                Id = 1,
                                Option = "Yes",
                                Description = refundPolicy +
                                "</br>" + 
                                "Click <b>Finish</b> to select this test session.",
                                Documents = Enumerable.Empty<AnswerDocumentContract>()
                            },
                            new AnswerOptionContract
                            {
                                Id = 2,
                                Option = "No",
                                Description = "Click <b>Finish</b> to cancel your selection.",
                                Documents = Enumerable.Empty<AnswerDocumentContract>()
                            },
                        },
                        QuestionLogics = Enumerable.Empty<QuestionLogicContract>()
                    }
                }
            });


            return sections;
        }


        private IList<ApplicationFormSectionContract> GetSupplementaryTestSections()
        {
            var termsAndConditions = mSystemQueryService
                .GetSystemValue(new GetSystemValueRequest() { ValueKey = "SupplementaryTestTermsAndConditions" })
                .Value;
            var sections = new List<ApplicationFormSectionContract>
            {
                new ApplicationFormSectionContract()
                {
                    Id = 1,
                    Name = "Test Details",
                    Questions = new List<ApplicationFormQuestionContract>
                    {
                        new ApplicationFormQuestionContract()
                        {
                            Id = 1,
                            AnswerTypeId = (int)CredentialApplicationFormAnswerTypeName.TestDetails,
                            Description = "These are the details of the original test.",
                            AnswerOptions = Enumerable.Empty<AnswerOptionContract>(),
                            QuestionLogics = Enumerable.Empty<QuestionLogicContract>()
                        },

                    }
                },
                new ApplicationFormSectionContract
                {
                    Id = 2,
                    Name = "Tasks",
                    Questions = new List<ApplicationFormQuestionContract>
                    {
                        new ApplicationFormQuestionContract
                        {
                            Id = 2,
                            AnswerTypeId = (int)CredentialApplicationFormAnswerTypeName.SupplementaryTestTasks
                        }
                    }
                },
                new ApplicationFormSectionContract()
                {
                    Id = 3,
                    Name = "Payment",
                    Questions = new List<ApplicationFormQuestionContract>
                    {
                        new ApplicationFormQuestionContract()
                        {
                            Id = 3,
                            AnswerTypeId =  (int)CredentialApplicationFormAnswerTypeName.PaymentControl,
                            AnswerOptions = Enumerable.Empty<AnswerOptionContract>(),
                            QuestionLogics = Enumerable.Empty<QuestionLogicContract>()
                        }
                    }
                },
                new ApplicationFormSectionContract()
                {
                    Id = 4,
                    Name = "Terms and Conditions",
                    Questions = new List<ApplicationFormQuestionContract>
                    {
                        new ApplicationFormQuestionContract()
                        {
                            Id = 4,
                            Text = $"Do you agree to the following <a href=\"{termsAndConditions}\" target=\"_blank\">Terms and Conditions</a>?",
                            AnswerTypeId =(int)CredentialApplicationFormAnswerTypeName.RadioOptions,
                            AnswerOptions = new []
                            {
                                new AnswerOptionContract
                                {
                                    Id = 1,
                                    Option = "Yes",
                                    Description = "Click <b>Finish</b> to complete this application.",
                                    Documents = Enumerable.Empty<AnswerDocumentContract>()
                                },
                                new AnswerOptionContract
                                {
                                    Id = 2,
                                    Option = "No",
                                    Description = "Click <b>Finish</b> to cancel this application.",
                                    Documents = Enumerable.Empty<AnswerDocumentContract>()
                                },
                            },
                            QuestionLogics = Enumerable.Empty<QuestionLogicContract>()
                        }
                    }
                }
            };
            return sections;
        }

        private TestFeeContract GetTestFeeDetails(int naatiNumber, int testSittingId, FeeTypeName feeType)
        {
            var test = GetTestDetails(naatiNumber, testSittingId);
            if (test == null || (feeType == FeeTypeName.PaidTestReview && !test.ElilgibleForPaidReview)
                || (feeType == FeeTypeName.SupplementaryTest && !test.ElilgibleForSupplementary))
            {
                return null;
            }

            var summary = mTestQueryService.GetTestSummary(new GetTestSummaryRequest { TestAttendanceId = testSittingId }).Result;

            var application = mApplicationQueryService.GetApplication(summary.ApplicationId).Result;

            return new TestFeeContract
            {
                TestAttendancId = test.AttendanceId,
                ApplicationId = summary.ApplicationId,
                ApplicationReference = summary.ApplicationReference,
                TestSessionId = test.TestSessionId,
                CredentialRequestId = test.CredentialRequestId,
                TestDate = summary.TestDate,
                TestLocationName = summary.TestLocation,
                CredentialTypeDisplayName = summary.CredentialTypeExternalName,
                SkillDisplayName = summary.Skill,
                State = summary.State,
                NaatiNumber = test.NaatiNumber,
                ApplicantPrimaryEmail = application.ApplicantPrimaryEmail,
                ApplicantGivenName = application.ApplicantGivenName,
                ApplicantFamilyName = application.ApplicantFamilyName,
                Sponsor = new SponsorContract
                {
                    OrganisationName = application.SponsorInstitutionName,
                    Contact = application.SponsorInstitutionContactPersonName,
                    Trusted = application.TrustedInstitutionPayer.GetValueOrDefault()
                },
                Fee = GetFee(application.ApplicationTypeId, summary.CredentialTypeId, feeType, application.HasAddressInAustralia)
            };
        }

        public TestFeeContract GetPaidReviewTestDetails(int naatiNumber, int testSittingId)
        {
            return GetTestFeeDetails(naatiNumber, testSittingId, FeeTypeName.PaidTestReview);
        }

        public TestFeeContract GetSupplementaryTestDetails(int naatiNumber, int testSittingId)
        {
            return GetTestFeeDetails(naatiNumber, testSittingId, FeeTypeName.SupplementaryTest);
        }

        public TestFeeContract GetSelectTestSessionTestDetails(int testSessionId, int applicationId, int credentialRequestId)
        {
            var test = GetTestSessionDetails(testSessionId);

            if (test == null)
            {
                return null;
            }
            var credentialRequest = mApplicationQueryService.GetCredentialRequest(credentialRequestId).CredentialRequest;
            var application = mApplicationQueryService.GetApplication(applicationId).Result;

            return new TestFeeContract
            {
                ApplicationReference = application.ApplicationReference,
                ApplicationId = applicationId,
                TestSessionId = test.TestSessionId,
                CredentialRequestId = credentialRequestId,
                TestDate = test.TestDate.GetValueOrDefault(),
                TestLocationName = test.TestLocationName,
                State = test.TestLocationStateName,
                CredentialTypeDisplayName = test.CredentialTypeExternalName,
                SkillDisplayName = credentialRequest.Skill.DisplayName,
                ApplicantPrimaryEmail = application.ApplicantPrimaryEmail,
                ApplicantGivenName = application.ApplicantGivenName,
                ApplicantFamilyName = application.ApplicantFamilyName,
                Sponsor = new SponsorContract
                {
                    OrganisationName = application.SponsorInstitutionName,
                    Contact = application.SponsorInstitutionContactPersonName,
                    Trusted = application.TrustedInstitutionPayer.GetValueOrDefault()
                },
                Fee = GetFee(application.ApplicationTypeId, test.CredentialTypeId, FeeTypeName.Test, application.HasAddressInAustralia)
            };
        }

        public SupplementaryTestTaskResponse GetSupplementaryTestTasks(int naatiNumber, int testSittingId)
        {
            var testSummary =
                mTestQueryService.GetTestSummary(new GetTestSummaryRequest() { TestAttendanceId = testSittingId });
            if (testSummary?.Result.NaatiNumber != naatiNumber)
            {
                return null;
            }
            var testResultId = testSummary.Result.LastReviewTestResultId ??
                              testSummary.Result.ResultId.GetValueOrDefault();

            IEnumerable<SupplementaryTestTaskContract> tasks = null;
            if (testSummary.Result.MarkingSchemaTypeId == (int)MarkingSchemaTypeName.Standard)
            {
                var marksResponse = mExaminerToolsService.GetTestDetails(new GetTestDetailsRequest
                {
                    TestResultId = testResultId,
                    UseOriginalResultMark = false
                });

                tasks = marksResponse.Components
                    .Where(x => x.MarkingResultTypeId == (int)MarkingResultTypeName.Original)
                    .Select(y => new SupplementaryTestTaskContract { Name = y.Name })
                    .ToList();
            }
            else
            {
                var rubricResults = mTestResultQueryService.GetTestResultMarkingResult(new TestMarkingResultRequest { TestResultId = testResultId });
                tasks = rubricResults.TestComponents.Where(x => x.MarkingResultTypeId == (int)MarkingResultTypeName.Original).Select(y => new SupplementaryTestTaskContract { Name = y.Name }).ToList();
            }

            return new SupplementaryTestTaskResponse
            {
                Tasks = tasks
            };

        }

        public GetCredentialsResponse GetActiveAndFutureCredentials(int naatiNumber)
        {
            var activeCredentials = mPersonQueryService.GetPersonCredentials(naatiNumber)
                .Data.Where(x => x.StatusId == (int)CredentialStatusTypeName.Active ||
                                 x.StatusId == (int)CredentialStatusTypeName.Future);


            var mappedCredentials = activeCredentials.Select(_autoMapperHelper.Mapper.Map<CredentialContract>).ToList();

            return new GetCredentialsResponse
            {
                Credentials = mappedCredentials
            };
        }

        public GetCredentialRequestsResponse GetInProgressCredentialRequests(int naatiNumber)
        {
            var activeApplicationFilter = new[]
            {
                new ApplicationSearchCriteria { Filter = ApplicationFilterType.NaatiNumberIntList, Values =  new []{naatiNumber.ToString()}},
                new ApplicationSearchCriteria { Filter = ApplicationFilterType.ActiveApplicationBoolean, Values =  new []{ true.ToString()}},
            };

            var activeApplicationSearchRequest = new GetApplicationSearchRequest
            {
                Filters = activeApplicationFilter,
                Skip = 0,
                Take = 20,
            };

            var activeApplicationsResponse = mApplicationQueryService.GetApplicationsWithCredentialRequests(activeApplicationSearchRequest);

            var credentialrequests = activeApplicationsResponse.CredentialRequestResults.Where(
                x => !((CredentialRequestStatusTypeName)x.Item2.StatusTypeId).IsFinalisedStatus()).Select(r =>
            {
                var credentialApplication = r.Item1;
                var credentialRequest = _autoMapperHelper.Mapper.Map<CredentialRequestContract>(r.Item2);
                credentialRequest.ApplicationReference = credentialApplication.ApplicationReference;
                return credentialRequest;

            }).ToList();

            return new GetCredentialRequestsResponse
            {
                Results = credentialrequests
            };
        }

        public CredentialTypeResponse GetCredentialTypeByTestSittingId(int testSittingId)
        {
            var response = mApplicationQueryService.GetCredentialTypeByTestSittingId(testSittingId);
            return response;
        }

        private FeeContract GetFee(int applicationTypeId, int credentialTypeId, FeeTypeName feeType, bool isInAustralia)
        {
            var productSpecifiation = mApplicationBusinessLogicService.GetProductSpecificationFee(applicationTypeId, credentialTypeId, feeType);

            if (productSpecifiation == null)
            {
                return new FeeContract();
            }
            var total = 0d;
            var exGst = 0d;
            var gst = 0d;

            if (!productSpecifiation.GstApplies || !isInAustralia)
            {
                exGst = Convert.ToDouble(productSpecifiation.CostPerUnit);
                gst = 0;
                total = exGst + gst;
            }
            else
            {
                total = Convert.ToDouble(productSpecifiation.CostPerUnit);
                exGst = total / 1.1;
                gst = total - exGst;
            }

            var variables = _systemValueService.GetAll();
            var payPalSurcharge = double.Parse(variables.First(r => r.Key.Equals("PayPalSurcharge")).Value);

            return new FeeContract
            {
                Code = productSpecifiation.Code,
                Description = productSpecifiation.Description,
                ExGST = exGst,
                GST = gst,
                Total = total,
                PayPalSurcharge = payPalSurcharge
            };
        }

        private TestSessionSearchResultDto GetTestSessionDetails(int testSessionId)
        {
            var response = mTestSessionQueryService.Search(new GetTestSessionSearchRequest
            {
                Filters = new[]
                {
                    new TestSessionSearchCriteria
                    {
                        Filter = TestSessionFilterType.TestSessionIntList,
                        Values = new[] {testSessionId.ToString()}
                    }
                }
            });

            return response.TestSessions.SingleOrDefault();
        }

        private TestSearchResultDto GetTestDetails(int naatiNumber, int testSittingId)
        {
            var response = mTestQueryService.Search(new GetTestSearchRequest
            {
                Filters = new[]
                {
                    new TestSearchCriteria
                    {
                        Filter = TestFilterType.NaatiNumberIntList,
                        Values = new[] {naatiNumber.ToString()}
                    },
                    new TestSearchCriteria
                    {
                        Filter = TestFilterType.AttendanceIdIntList,
                        Values = new[] {testSittingId.ToString()}
                    }
                }
            });

            return response.Tests.SingleOrDefault();
        }
        public GetApplicationFormSectionsResponse GetPaidReviewSections(int naatiNumber, int testSittingId)
        {
            var test = GetTestDetails(naatiNumber, testSittingId);
            if (test == null || !test.ElilgibleForPaidReview)
            {
                return new GetApplicationFormSectionsResponse
                {
                    Results = new List<ApplicationFormSectionContract>()
                };
            }

            var sections = GetPaidReviewSections();
            return new GetApplicationFormSectionsResponse { Results = sections };
        }
        public GetApplicationFormSectionsResponse GetSelectTestSessionSections(int naatiNumber, int testSessionId, int credentialRequestId, int credentialApplicationId)
        {
            var test = GetTestSessionDetails(testSessionId);
            if (test == null)
            {
                return new GetApplicationFormSectionsResponse
                {
                    Results = new List<ApplicationFormSectionContract>()
                };
            }

            var credentialRequest = mApplicationQueryService.GetCredentialRequest(credentialRequestId).CredentialRequest;
            var application = mApplicationQueryService.GetApplication(credentialApplicationId).Result;
            var productSpecifiation = mApplicationBusinessLogicService.GetProductSpecificationFee(application.ApplicationTypeId, credentialRequest.CredentialTypeId, FeeTypeName.Test);

            var refundPolicy = mApplicationQueryService.GetCredentialApplicationRefundPolicy(credentialRequest.CredentialTypeId, application.ApplicationTypeId, productSpecifiation.Id);

            var paymentRequired =
                credentialRequest.StatusTypeId != (int)CredentialRequestStatusTypeName.TestAccepted &&
                credentialRequest.StatusTypeId != (int)CredentialRequestStatusTypeName.TestSessionAccepted;

            var sections = GetSelectTestSessionSections(paymentRequired, refundPolicy.Description);
            return new GetApplicationFormSectionsResponse { Results = sections };
        }

        public GetApplicationFormSectionsResponse GetSupplementaryTestSections(int naatiNumber, int testSittingId)
        {
            var test = GetTestDetails(naatiNumber, testSittingId);
            if (test == null || !test.ElilgibleForSupplementary)
            {
                return new GetApplicationFormSectionsResponse
                {
                    Results = new List<ApplicationFormSectionContract>()
                };
            }

            var sections = GetSupplementaryTestSections();
            return new GetApplicationFormSectionsResponse { Results = sections };
        }

        private int? GetPersonTokenByApplicationId(int applicationId)
        {
            return mPersonQueryService.GetPersonToken(new GetTokenRequest { Type = TokenRequestType.ApplicationId, Value = applicationId })?.Token;
        }

        public bool ValidateApplicationToken(int applicationId, int token)
        {
            var serverToken = GetPersonTokenByApplicationId(applicationId);

            return serverToken == token;
        }

        public GetApplicationDetailsResponse SaveApplicationForm(SaveApplicationFormRequestContract request)
        {
            var applicationFormHelper = mApplicationFormsService;
            return applicationFormHelper.SaveApplicationForm(request);
        }


        public VerifyPersonResponse VerifyPersonUsingCustomerNumber(VerifyPersonRequestContract request,
            List<PersonSearchDto> peopleSearchByNaatiNumber,
            List<PersonSearchDto> peopleSearchByDetails)
        {
            if (!peopleSearchByNaatiNumber.Any())
            {
                return new VerifyPersonResponse { Message = string.Format(Resources.VerificationFailed, "CN1") };
            }

            if (peopleSearchByNaatiNumber.Count > 1)
            {
                return new VerifyPersonResponse { Message = Resources.MultipleCustomersForNaatiNumberFound };
            }

            var foundPerson = peopleSearchByDetails.FirstOrDefault(p => p.PersonId == peopleSearchByNaatiNumber.First().PersonId
            && p.PrimaryEmail?.ToUpper() == request.Email.ToUpper());
            if (foundPerson == null)
            {
                return new VerifyPersonResponse { Message = string.Format(Resources.VerificationFailed, "CN2") };
            }

            return new VerifyPersonResponse { NaatiNumber = foundPerson.NaatiNumber, Message = string.Empty };
        }

        private int? GetPersonToken(int naatiNumber)
        {
            return mPersonQueryService.GetPersonToken(new GetTokenRequest { Value = naatiNumber, Type = TokenRequestType.NaatiNumber })?.Token;
        }

        public VerifyPersonResponse VerifyPersonUsingDetails(VerifyPersonRequestContract request,
            string formattedFamilyName,
            List<PersonSearchDto> peopleSearchByEmail, List<PersonSearchDto> peopleSearchByDetails)
        {


            if (!peopleSearchByDetails.Any() && !peopleSearchByEmail.Any())
            {
                return CreateNewPerson(request, formattedFamilyName);
            }

            if (!peopleSearchByDetails.Any() && peopleSearchByEmail.Any())
            {
                return new VerifyPersonResponse { Message = string.Format(Resources.VerificationFailed, "M2") };
            }

            if (peopleSearchByDetails.Count == 1 && !peopleSearchByEmail.Any())
            {
                return new VerifyPersonResponse { Message = string.Format(Resources.VerificationFailed, "M3") };
            }

            if (peopleSearchByDetails.Count == 1 && peopleSearchByEmail.Count == 1 && peopleSearchByEmail.First().PersonId == peopleSearchByDetails.First().PersonId)
            {
                return new VerifyPersonResponse { NaatiNumber = peopleSearchByDetails.First().NaatiNumber, Message = string.Empty };
            }

            if (peopleSearchByDetails.Count == 1 && peopleSearchByEmail.Count > 1)
            {
                return new VerifyPersonResponse { Message = string.Format(Resources.VerificationFailed, "M5") };
            }

            var personByDetailsIds = peopleSearchByDetails.Select(x => x.PersonId).ToArray();
            if (peopleSearchByDetails.Count > 1 && (!peopleSearchByEmail.Any() || !peopleSearchByEmail.Any(p => personByDetailsIds.Contains(p.PersonId))))
            {
                return new VerifyPersonResponse { Message = string.Format(Resources.VerificationFailed, "M6") };
            }

            if (peopleSearchByDetails.Count > 1 && peopleSearchByEmail.Count == 1 && peopleSearchByDetails.Any(p => p.PersonId == peopleSearchByEmail.First().PersonId))
            {
                return new VerifyPersonResponse { NaatiNumber = peopleSearchByEmail.First().NaatiNumber, Message = string.Empty };
            }

            if (peopleSearchByDetails.Count > 1 && peopleSearchByEmail.Count > 1)
            {
                return new VerifyPersonResponse { Message = string.Format(Resources.VerificationFailed, "M8") };
            }

            if (peopleSearchByDetails.Count == 1 && peopleSearchByEmail.Count == 1 && peopleSearchByEmail.First().PersonId != peopleSearchByDetails.First().PersonId)
            {
                return new VerifyPersonResponse { Message = string.Format(Resources.VerificationFailed, "M9") };
            }

            var generalErrorCode = $"General person verification Error details:{peopleSearchByDetails.Count}, emails: {peopleSearchByEmail.Count}";
            return new VerifyPersonResponse { Message = string.Format(Resources.VerificationFailed, generalErrorCode) };
        }

        public bool HasSubmittedApplications(int naatiNumber)
        {
            var hasSubmittedApplications = mApplicationQueryService.HasSubmittedApplications(naatiNumber);
            return hasSubmittedApplications;
        }

        public VerifyPersonResponse UpdateOrVerifyPersonDetails(VerifyPersonRequestContract request, bool isLoggedIn)
        {
            if (!isLoggedIn)
            {
                return VerifyPerson(request);
            }

            var personDetails = GetPersonDetailsBasic(request.NaatiNumber.GetValueOrDefault()).PersonDetails;

            var surName = string.IsNullOrWhiteSpace(request.FamilyName) ? SurnameNotStated : request.FamilyName;

            var nameModified = personDetails.TitleId != request.Title ||
                               personDetails.GivenName?.ToUpper() != request.FirstName?.ToUpper() ||
                               personDetails.OtherNames?.ToUpper() != request.MiddleNames?.ToUpper() ||
                               personDetails.FamilyName?.ToUpper() != surName?.ToUpper();

            var detailsModified = personDetails.CountryOfBirthId != request.CountryOfBirth ||
                                  personDetails.Gender != request.Gender ||
                                  personDetails.DateOfBirth != request.DateOfBirth;

            if (nameModified || detailsModified)
            {
                var hasSubmittedApplications = HasSubmittedApplications(request.NaatiNumber.GetValueOrDefault());

                if (!hasSubmittedApplications)
                {
                    if (detailsModified)
                    {
                        mPersonQueryService.UpdatePersonDetails(new UpdatePersonDetailsRequest()
                        {
                            BirthCountryId = request.CountryOfBirth,
                            BirthDate = request.DateOfBirth,
                            Gender = request.Gender,
                            Deceased = personDetails.Deceased,
                            DoNotSendCorrespondence = personDetails.DoNotSendCorrespondence,
                            EntityTypeId = personDetails.EntityTypeId,
                            NaatiNumberDisplay = request.NaatiNumber.ToString(),
                            PersonId = personDetails.PersonId
                        });
                    }

                    if (nameModified)
                    {
                        mPersonQueryService.AddName(new AddNameRequest()
                        {
                            NaatiNumber = request.NaatiNumber.GetValueOrDefault(),
                            Name = new PersonNameDto()
                            {
                                AlternativeGivenName = string.Empty,
                                AlternativeSurname = string.Empty,
                                EffectiveDate = DateTime.Now,
                                GivenName = request.FirstName,
                                OtherNames = request.MiddleNames,
                                Surname = string.IsNullOrWhiteSpace(request.FamilyName) ? SurnameNotStated : request.FamilyName,
                                TitleId = request.Title > 0 ? request.Title : (int?)null
                            }
                        });
                    }
                }
            }

            return new VerifyPersonResponse
            {
                IsNewPerson = false,
                Message = string.Empty,
                NaatiNumber = request.NaatiNumber.GetValueOrDefault(),
                Token = GetPersonToken(request.NaatiNumber.GetValueOrDefault()).GetValueOrDefault()
            };
        }

        public VerifyPersonResponse VerifyPerson(VerifyPersonRequestContract request)
        {
            var result = VerifyPersonDetails(request);
            if (result.NaatiNumber > 0)
            {
                result.Token = GetPersonToken(result.NaatiNumber).GetValueOrDefault();
            }

            return result;
        }

        public bool ValidatePersonToken(int naatiNumber, int token)
        {
            var serverToken = GetPersonToken(naatiNumber);

            return serverToken == token;
        }


        private VerifyPersonResponse VerifyPersonDetails(VerifyPersonRequestContract request)
        {
            var formattedFamilyName = string.IsNullOrWhiteSpace(request.FamilyName) ? SurnameNotStated : request.FamilyName;

            var personDetailsFilter = new List<PersonSearchCriteria>()
            {
                new PersonSearchCriteria { Filter = PersonFilterType.GivenNamesString, Values = new []{request.FirstName?.Trim()}},
                new PersonSearchCriteria { Filter = PersonFilterType.FamilyNameString, Values = new []{formattedFamilyName.Replace("[", "[[]").Trim()}},
                new PersonSearchCriteria { Filter = PersonFilterType.DateOfBirthFromString, Values = new []{ $"{request.DateOfBirth.Date:yyyy-MM-dd}"}},
                new PersonSearchCriteria { Filter = PersonFilterType.DateOfBirthToString, Values = new []{ $"{request.DateOfBirth.Date:yyyy-MM-dd}"}},

            };

            if (request.NaatiNumber.HasValue)
            {
                personDetailsFilter.Add(new PersonSearchCriteria { Filter = PersonFilterType.NaatiNumberIntList, Values = new[] { request.NaatiNumber.ToString() } });
            }

            var peopleSearchByDetails = GetPeople(personDetailsFilter);
            if (request.NaatiNumber.HasValue)
            {
                var naatiNumberFilter = new[] { new PersonSearchCriteria { Filter = PersonFilterType.NaatiNumberIntList, Values = new[] { request.NaatiNumber.ToString() } } };
                var peopleSearchByNaatiNumber = GetPeople(naatiNumberFilter);
                return VerifyPersonUsingCustomerNumber(request, peopleSearchByNaatiNumber, peopleSearchByDetails);
            }

            var peopleSearchByEmail = GetPersonDetailsByEmail(request.Email);
            return VerifyPersonUsingDetails(request, formattedFamilyName, peopleSearchByEmail, peopleSearchByDetails);
        }


        public List<PersonSearchDto> GetPersonDetailsByEmail(string email)
        {
            var emailFilter = new List<PersonSearchCriteria>
            {
                new PersonSearchCriteria { Filter = PersonFilterType.PrimaryEmailString, Values = new []{ email }}
            };
            var peopleSearchByEmail = GetPeople(emailFilter);

            return peopleSearchByEmail;
        }


        public IEnumerable<int> GetPersonNaatiNumber(string email)
        {
            return GetPersonDetailsByEmail(email).Select(x => x.NaatiNumber);
        }

        private VerifyPersonResponse CreateNewPerson(VerifyPersonRequestContract request, string formattedFamilyName)
        {
            var createPersonRequest = new CreatePersonRequest
            {
                DateOfBirth = request.DateOfBirth,
                PrimaryEmail = request.Email,
                EnteredDate = DateTime.Now,
                Gender = null,
                GivenName = request.FirstName,
                OtherNames = request.MiddleNames ?? string.Empty,
                SurName = formattedFamilyName,
                BirthCountryId = null,
                Title = request.Title,
                AllowAutoRecertification = true
            };

            var createPersonResponse = mPersonQueryService.CreatePerson(createPersonRequest);
            if (!string.IsNullOrWhiteSpace(createPersonResponse.ErrorMessage))
            {
                return new VerifyPersonResponse { Message = createPersonResponse.ErrorMessage };
            }

            return new VerifyPersonResponse { Message = string.Empty, NaatiNumber = createPersonResponse.NaatiNumber, IsNewPerson = true };
        }

        private List<PersonSearchDto> GetPeople(IEnumerable<PersonSearchCriteria> filter)
        {
            var searchRequest = new GetPersonSearchRequest
            {
                Filters = filter,
                Skip = 0,
                Take = 20,
            };

            return mPersonQueryService.SearchPerson(searchRequest).Results.ToList();
        }

        public GetCredentialApplicationResponse GetExistingDraftApplication(ExitingApplicationRequestContract request)
        {
            var applicationForm = GetCredentialApplicationForm(request.ApplicationFormId);
            var applicationTypeId = applicationForm.Result.CredentialApplicationTypeId;

            var canCreateApplication = mApplicationQueryService.CanCreateNewApplication(request.NaatiNumber, applicationTypeId);
            if (!canCreateApplication)
            {
                return new GetCredentialApplicationResponse
                {
                    CredentialApplication = null,
                    Message = Resources.ActiveApplicationFound
                };
            }
          
            var draftApplications = mApplicationQueryService.GetDraftApplications(request.NaatiNumber, applicationTypeId);
            if (draftApplications.Count() > 1)
            {
                return new GetCredentialApplicationResponse
                {
                    CredentialApplication = null,
                    Message = Resources.MoreThatOneDraftApplicationFound
                };
            }

            var draftApplication = draftApplications.FirstOrDefault();

            var draftContract = _autoMapperHelper.Mapper.Map<CredentialApplicationContract>(draftApplication);

            if (draftContract != null)
            {
                var sections = GetCredentialApplicationFormSections(new GetFormSectionRequestContract
                {
                    ApplicationId = draftContract.Id,
                    ApplicationFormId = request.ApplicationFormId,
                    ExternalUrls = request.ExternalUrls
                });

                draftContract.Sections = sections.Results;
            }

            return new GetCredentialApplicationResponse
            {
                CredentialApplication = draftContract,
                Message = string.Empty
            };

        }

        public CreateCredentialApplicationResponse CreateCredentialApplication(CreateApplicationRequest request)
        {
            return mApplicationFormsService.CreateCredentialApplication(request);
        }

        public PersonDetailsResponse GetPersonDetails(int naatiNumber)
        {
            return mApplicationFormsService.GetPersonDetails(naatiNumber);
        }

        public LookupResponse GetLanguagesForApplicationForm(int applicationFormId, int applicationId, int naatiNumber)
        {
            var response = mApplicationFormsService.GetLanguagesForApplicationForm(applicationFormId, applicationId).Results;
            var skillsInProgress = GetInProgressCredentialRequests(naatiNumber).Results.ToLookup(x => x.SkillId, y => y);
            var activeCredentialSkills = GetActiveAndFutureCredentials(naatiNumber).Credentials.ToLookup(x => x.SkillId, y => y);
            response = response.Where(x => !skillsInProgress[x.SkillId].Any() && !activeCredentialSkills[x.SkillId].Any()).ToList();
            return new LookupResponse() { Results = response };
        }

        public GetEndorsedQualificationForApplicationFormResponse GetEndorsedQualificationForApplicationForm(int applicationFormId, int applicationId, int naatiNumber)
        {
            return mApplicationFormsService.GetEndorsedQualificationForApplicationForm(applicationFormId, applicationId);
        }

        public SkillLookupResponse GetLanguagesForCredentialTypes(List<int> credentialTypes)
        {
            return mApplicationFormsService.GetLanguagesForCredentialTypes(credentialTypes);
        }

        public GetCredentialRequestsResponse GetCredentialRequests(int applicationId)
        {
            var response =
                mApplicationQueryService.GetCredentialRequests(applicationId,
                    new[] { CredentialRequestStatusTypeName.Deleted });

            var requests = response.Results;

            return new GetCredentialRequestsResponse
            {
                Results = requests.Select(_autoMapperHelper.Mapper.Map<CredentialRequestContract>)
            };
        }

        public GetAllCredentialsResponse GetAllCredentialsByNaatiNumber(int naatiNumber)
        {

            var credentials = mPersonQueryService.GetPersonCredentials(naatiNumber).Data.Select(MapCredentialDetails);
       
            var response = new GetAllCredentialsResponse
            {
                Results = credentials
            };

            return response;
        }

        public GetCredentialAttachmentsByIdResponse GetCredentialAttachmentsById(int credentialId, int naatiNumber)
        {
            var getRequest = new GetCredentialAttachmentsByIdRequest
            {
                CredentialId = credentialId,
                NaatiNumber = naatiNumber
            };

            var serviceResponse = mApplicationQueryService.GetCredentialAttachmentsById(getRequest);

            var getCredentialAttachmentsByIdResponse = new GetCredentialAttachmentsByIdResponse
            {
                Results = serviceResponse.Attachments.Select(_autoMapperHelper.Mapper.Map<CredentialAttachmentContract>)
            };

            return getCredentialAttachmentsByIdResponse;
        }

        public GetCredentialAttachmentFileResponse GetCredentialAttachmentFileByCredentialAttachmentId(MyNaati.Contracts.BackOffice.GetCredentialAttachmentFileRequest getrequest)
        {
            var samRequest = new GetCredentialAttachmentFileRequest
            {
                StoredFileId = getrequest.StoredFileId,
                TempFileStorePath = getrequest.TempFileStorePath
            };

            var serviceResponse = mApplicationQueryService.GetCredentialAttachmentFileByCredentialAttachmentId(samRequest);

            if (!ValidatePersonToken(getrequest.NaatiNumber, serviceResponse.PersonHash.GetValueOrDefault()))
            {
                throw new Exception($"Error getting document owner StorageId: {getrequest.StoredFileId}, NaatiNumber: {getrequest.NaatiNumber}");
            }

            return _autoMapperHelper.Mapper.Map<GetCredentialAttachmentFileResponse>(serviceResponse);
        }

        public GetTestSessionSkillAvailabilityResponse GetAvailableTestSessions(GetAvailableTestSessionRequest request)
        {
            var credentialRequests = mApplicationQueryService.GetBasicCredentialRequestsByApplicationId(request.ApplicationId, new[] { CredentialRequestStatusTypeName.Deleted });

            var sessionContracts = new List<TestSessionContract>();
            foreach (var credentialRequest in credentialRequests.Results)
            {
                var credentialRequestSessions = mTestSessionQueryService.GetAvailableTestSessionAfterBacklogAssignment(credentialRequest.Id)
                    .AvailableTestSessions.Select(x => new TestSessionContract
                    {
                        AvailableSeats = x.AvailableSeats,
                        CredentialTypeName = credentialRequest.CredentialTypeExternalName,
                        Duration = x.TestSessionDuration,
                        SkillName = credentialRequest.SkillDisplayName,
                        TestDateTime = x.TestDateTime,
                        VenueName = x.VenueName,
                        TestLocation = x.TestLocation,
                        IsPreferedLocation = x.IsPreferedLocation
                    });

                sessionContracts.AddRange(credentialRequestSessions);
            }

            return new GetTestSessionSkillAvailabilityResponse
            {
                Results = sessionContracts.OrderBy(x=> x.IsPreferedLocation).ThenBy(y=> y.TestDateTime)
            };
        }

        public void UpdateCredential(CredentialContract credential)
        {
            var serverRequest = _autoMapperHelper.Mapper.Map<CredentialDto>(credential);

            mApplicationQueryService.UpdateCredential(serverRequest);
        }

        public bool IsValidCredentialByNaatiNumber(int credentialId, int naatiNumber)
        {
            var response = mApplicationQueryService.IsValidCredentialByNaatiNumber(credentialId, naatiNumber);
            return response;
        }

        private CredentialDetailRequestContract MapCredentialRequest(CredentialRequestDto credentialRequest)
        {
            return new CredentialDetailRequestContract
            {
                Id = credentialRequest.Id,
                Certification = credentialRequest.Certification,
                Category = credentialRequest.Category,
                CategoryId = credentialRequest.CategoryId,
                CredentialName = credentialRequest.ExternalCredentialName,
                Direction = credentialRequest.Direction,
                Status = credentialRequest.Status,
                StatusTypeId = credentialRequest.StatusTypeId,
                SkillId = credentialRequest.SkillId,
                StatusChangeDate = credentialRequest.StatusChangeDate,
                ModifiedBy = credentialRequest.ModifiedBy,
                StatusChangeUserId = credentialRequest.StatusChangeUserId,
                CredentialTypeId = credentialRequest.CredentialTypeId,
                Credentials = credentialRequest.Credentials.Select(_autoMapperHelper.Mapper.Map<CredentialContract>).ToList()
            };
        }

        public LookupResponse GetCredentialCategoriesForApplication(int applicationId, int applicationFormId)
        {
            var response = mApplicationQueryService.GetCredentialCategoriesForApplicationType(applicationId).Results;

            var credentialTypeCategoryFilter =
                mApplicationQueryService.GetCredentialApplicationFormCredentialTypes(applicationFormId).Results.GroupBy(x => x.CategoryId).ToDictionary(x => x.Key, y => y);

            if (credentialTypeCategoryFilter.Any())
            {
                response = response.Where(x => credentialTypeCategoryFilter.ContainsKey(x.Id));
            }

            return new LookupResponse { Results = response.OrderBy(x => x.DisplayName).Select(_autoMapperHelper.Mapper.Map<LookupContract>) };
        }

        public CredentialLookupTypeResponse GetAvailableCredentials(int applicationId, int naatiNumber, int applicationFormId)
        {
            var results = mApplicationQueryService.GetAvailableCredentials(applicationId, naatiNumber).Results.ToArray();

            var credentialTypeFilter =
                mApplicationQueryService.GetCredentialApplicationFormCredentialTypes(applicationFormId).Results.ToDictionary(x => x.Id, y => y);

            if (credentialTypeFilter.Any())
            {
                foreach (var result in results)
                {
                    result.Children = result.Children.Where(x => credentialTypeFilter.ContainsKey(x.Id));
                }
            }

            return new CredentialLookupTypeResponse
            {
                Results = results.Select(_autoMapperHelper.Mapper.Map<CredentialLookupTypeContract>)
            };
        }

        public SkillLookupResponse GetAdditionalSkills(GetSkillsForApplicationForCredentialTypeRequest request)
        {
            var response = mApplicationQueryService.GetAdditionalSkills(new GetCredentialTypeSkillsRequest
            {
                CredentialTypeIds = request.CredentialTypeIds,
                NAATINumber = request.NAATINumber,
                CredentialRequestPathTypeId = request.CredentialRequestPathTypeId,
                ApplicationId = request.ApplicationId
            }).Results;
           
            var skillsInProgress = GetInProgressCredentialRequests(request.NAATINumber).Results.ToLookup(x => x.SkillId, y => y);
            var activeCredentialSkills = GetActiveAndFutureCredentials(request.NAATINumber).Credentials.ToLookup(x => x.SkillId, y => y);
            response = response.Where(x => !skillsInProgress[x.Id].Any() && !activeCredentialSkills[x.Id].Any()).ToList();
            return new SkillLookupResponse { Results = response.OrderBy(x => x.DisplayName).Select(_autoMapperHelper.Mapper.Map<SkillLookupContract>) };
        }

        public LookupResponse GetCredentialTypesForApplicationForm(int applicationFormId, int applicationId, int? categoryId)
        {
            var response = mApplicationQueryService.GetCredentialTypesForApplication(applicationId, categoryId).Results;

            var credentialTypeFilter =
                mApplicationQueryService.GetCredentialApplicationFormCredentialTypes(applicationFormId).Results.ToDictionary(x => x.Id, y => y);

            if (credentialTypeFilter.Any())
            {
                response = response.Where(x => credentialTypeFilter.ContainsKey(x.Id));
            }
            return new LookupResponse { Results = response.Select(_autoMapperHelper.Mapper.Map<LookupContract>) };
        }

        public GetApplicationTypeFeesResponse GetApplicationTypeFees(int applicationFormId)
        {
            var applicationForm = GetCredentialApplicationForm(applicationFormId);
            var applicationTypeId = applicationForm.Result.CredentialApplicationTypeId;
            return mApplicationQueryService.GetApplicationTypeFees(applicationTypeId, FeeTypeName.Application);
        }

        public CredentialApplicationRefundPolicyResponse GetCredentialApplicationRefundPolicy(int credentialTypeId, int applicationTypeId, int productSpecificationId)
        {
            var refundPolicy = mApplicationQueryService.GetCredentialApplicationRefundPolicy(credentialTypeId, applicationTypeId, productSpecificationId);
            if (refundPolicy != null)
            {
                return new CredentialApplicationRefundPolicyResponse
                {
                    Name = refundPolicy.Name,
                    Description = refundPolicy.Description
                };
            }
            return null; 
        }

        public bool IsDisplayInvoices(int naatiNumber)
        {
            //TFS 170751 - SHould we be calculating whether there are any to display?
            return true;
        }

        public GenericResponse<bool> DoesCredentialBelongToUser(int storedFileId, int currentUserNaatiNumber)
        {
            var result = mApplicationQueryService.DoesCredentialBelongToUser(storedFileId, currentUserNaatiNumber);
            if (!result.Success)
            {
                return new GenericResponse<bool>()
                {
                    Errors = result.Errors,
                    Success = result.Success
                };
            }
            return result.Data;
        }
    }
}
