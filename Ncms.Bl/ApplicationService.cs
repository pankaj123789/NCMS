using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.Extensions;
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
using Ncms.Bl.ApplicationActions;
using Ncms.Bl.Export;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Common;
using Ncms.Contracts.Models.CredentialRequest;
using Ncms.Contracts.Models.Institution;
using Ncms.Contracts.Models.Person;
using Newtonsoft.Json;
using CredentialDto = F1Solutions.Naati.Common.Contracts.Dal.DTO.CredentialDto;
using IApplicationService = Ncms.Contracts.IApplicationService;
using IPersonService = Ncms.Contracts.IPersonService;
using SearchRequest = Ncms.Contracts.SearchRequest;

namespace Ncms.Bl
{
    // this class requires an interface for whatever methods use the signPdf method
    public class ApplicationService : IApplicationService
    {
        private readonly IUserService _userService;
        private readonly ITokenReplacementService _tokenReplacementService;
        private readonly ISystemService _systemService;
        private readonly IEmailMessageService _emailMessageService;
        private readonly IApplicationQueryService _applicationQueryService;
        private readonly IApplicationWizardLogicService _wizardLogicService;
        private readonly IPersonQueryService _personQueryService;
        private readonly IInstitutionService _institutionService;
        private readonly IFinanceService _financeService;
        private readonly ITestMaterialQueryService _testMaterialQueryService;
        private readonly IApplicationBusinessLogicService _applicationBusinessLogicService;
        private readonly IPersonService _personService;
        private readonly IExaminerQueryService _examinerQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly ICredentialPrerequisiteService _credentialPrerequisiteService;
        private readonly ICredentialPrerequisiteDalService _prerequisiteApplicationDalService;

        public ApplicationService(IUserService userService, ITokenReplacementService tokenReplacementService,
            ISystemService systemService, IEmailMessageService emailMessageService, IApplicationQueryService applicationQueryService,
            IApplicationWizardLogicService wizardLogicService, IPersonQueryService personQueryService,
            IInstitutionService institutionService, IFinanceService financeService, ITestMaterialQueryService testMaterialQueryService,
            IApplicationBusinessLogicService applicationBusinessLogicService, IPersonService personService, IExaminerQueryService examinerQueryService, IAutoMapperHelper autoMapperHelper,
            ICredentialPrerequisiteService credentialPrerequisiteService, ICredentialPrerequisiteDalService prerequisiteApplicationDalService)
        {
            _userService = userService;
            _tokenReplacementService = tokenReplacementService;
            _systemService = systemService;
            _emailMessageService = emailMessageService;
            _applicationQueryService = applicationQueryService;
            _wizardLogicService = wizardLogicService;
            _personQueryService = personQueryService;
            _institutionService = institutionService;
            _financeService = financeService;
            _testMaterialQueryService = testMaterialQueryService;
            _applicationBusinessLogicService = applicationBusinessLogicService;
            _personService = personService;
            _credentialPrerequisiteService = credentialPrerequisiteService;
            _examinerQueryService = examinerQueryService;
            _autoMapperHelper = autoMapperHelper;
            _prerequisiteApplicationDalService = prerequisiteApplicationDalService;
        }

        public GetApplicationDetailsResponse GetApplicationDetailsByApplicationId(GetApplicationDetailsRequest request)
        {
            GetApplicationDetailsResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetApplicationDetails(request);
            return serviceReponse;
        }

        public GenericResponse<IEnumerable<ApplicationSearchResultModel>> Search(SearchRequest request)
        {
            var filters = request.Filter.ToFilterList<ApplicationSearchCriteria, ApplicationFilterType>();
            var getRequest = _autoMapperHelper.Mapper.Map<GetApplicationSearchRequest>(request);
            getRequest.Filters = filters;
            return Search(getRequest);
        }

        public CredentialModel CreateOrUpdateCredential(CreateOrUpdateCredentialModel request)
        {
            var req = new CreateOrUpdateCredentialRequest
            {
                CredentialId = request.CredentialId,
                CredentialRequestId = request.CredentialRequestId,
                StartDate = request.StartDate,
                ExpiryDate = request.ExpiryDate,
                TerminationDate = request.TerminationDate,
                ShowInOnlineDirectory = request.ShowInOnlineDirectory,
                CertificationPeriod = request.CertificationPeriod
            };

            var response = _applicationQueryService.CreateOrUpdateCredential(req);
            return _autoMapperHelper.Mapper.Map<CredentialModel>(response.Data);
        }
        private GenericResponse<IEnumerable<ApplicationSearchResultModel>> Search(GetApplicationSearchRequest request)
        {
            ApplicationSearchResultResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.SearchApplication(request);

            var models = serviceReponse.Results.Select(_autoMapperHelper.Mapper.Map<ApplicationSearchResultModel>).ToList();
            var response = new GenericResponse<IEnumerable<ApplicationSearchResultModel>>(models);

            if (request.Take.HasValue && models.Count == request.Take.Value)
            {
                response.Warnings.Add($"Search result were limited to {request.Take.Value} records.");
            }

            if (request.Skip.HasValue)
            {
                response.Warnings.Add($"First {request.Skip.Value} records were skipped.");
            }

            return response;
        }


        public string GetInvoiceNumber(int applicationId)
        {
            return _applicationQueryService.GetInvoiceNumberByApplicationId(applicationId);
        }

        public DowngradedCredentialRequestModel GetDowngradedCredentialRequest(int credentialRequestId)
        {
            var response = _applicationQueryService.GetDowngradedCredentialRequest(credentialRequestId);

            var result = _autoMapperHelper.Mapper.Map<DowngradedCredentialRequestModel>(response);

            var existingCredentials = _personQueryService.GetPersonCredentials(result.NaatiNumber)
                .Data;

            var alreadyHasCredential = existingCredentials.Any(x => x.SkillId == result.SkillId
                                         && x.CredentialTypeId == result
                                             .CredentailTypeId
                                         && x.CategoryId == result.CategorId
                                         && x.StatusId != (int)CredentialStatusTypeName.Terminated
                                         && x.StatusId != (int)CredentialStatusTypeName.Expired);

            result.HasCredential = alreadyHasCredential;
            return result;
        }


        public FileModel ExportApplicationsExcel(ApplicationExport request)
        {
            var getRequest = _autoMapperHelper.Mapper.Map<GetApplicationSearchRequest>(request);
            getRequest.Filters = request.Filter.ToFilterList<ApplicationSearchCriteria, ApplicationFilterType>();
            ApplicationsWithCredentialRequestsResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetApplicationsWithCredentialRequests(getRequest);

            request.DeserializedFilter = JsonConvert.DeserializeObject<ApplicationRequest>(request.Filter);

            var applications = serviceReponse.ApplicationResults.Select(x => new ApplicationModel
            {
                ContactNumber = x.PrimaryContactNumber,
                NaatiNumber = x.NaatiNumber,
                Name = x.Name,
                Owner = x.ApplicationOwner,
                Reference = x.ApplicationReference,
                Status = x.ApplicationStatus,
                StatusDate = x.StatusChangeDate,
                Type = x.ApplicationType
            });

            var credentialRequests = serviceReponse.CredentialRequestResults.Select(x => new Tuple<ApplicationModel, CredentialRequestModel>(new ApplicationModel
            {
                ContactNumber = x.Item1.PrimaryContactNumber,
                NaatiNumber = x.Item1.NaatiNumber,
                Name = x.Item1.Name,
                Owner = x.Item1.ApplicationOwner,
                Reference = x.Item1.ApplicationReference,
                Status = x.Item1.ApplicationStatus,
                StatusDate = x.Item1.StatusChangeDate,
                Type = x.Item1.ApplicationType
            },
            new CredentialRequestModel
            {
                Category = x.Item2.Category,
                CredentialName = x.Item2.CredentialName,
                Credentials = x.Item2.Credentials.Select(b => new CredentialModel
                {
                    ExpiryDate = b.ExpiryDate,
                    Id = b.Id,
                    ShowInOnlineDirectory = b.ShowInOnlineDirectory,
                    StartDate = b.StartDate,
                    TerminationDate = b.TerminationDate,
                    Status = b.Status.ToString(),
                    StatusId = b.StatusId,
                    StoredFileIds = b.StoredFileIds

                }).ToList(),
                TestSessions = x.Item2.TestSessions.Select(b => new CredentialRequestTestSessionModel
                {
                    TestSessionId = b.TestSessionId,
                    CredentialTestSessionId = b.CredentialTestSessionId,
                    Name = b.Name,
                    Rejected = b.Rejected,
                    TestDate = b.TestDate,
                    Supplementary = b.Supplementary,
                    TestSpecificationId = b.TestSpecificationId,
                    MarkingSchemaTypeId = b.MarkingSchemaTypeId,
                    EligibleForConcededPass = b.EligibleForConcededPass,
                    EligibleForSupplementary = b.EligibleForSupplementary,
                    AutomaticIssuing = b.AutomaticIssuing,
                    MaxScoreDifference = b.MaxScoreDifference
                }).ToList(),
                CredentialType = new CredentialTypeModel
                {
                    Certification = x.Item2.CredentialType.Certification,
                    ExternalName = x.Item2.CredentialType.ExternalName,
                    Id = x.Item2.CredentialType.Id,
                    InternalName = x.Item2.CredentialType.InternalName,
                    Simultaneous = x.Item2.CredentialType.Simultaneous,
                    DisplayOrder = x.Item2.CredentialType.DisplayOrder,
                    ActiveTestSpecificationId = x.Item2.CredentialType.ActiveTestSpecificationId,
                    MarkingSchemaTypeId = x.Item2.CredentialType.MarkingSchemaTypeId,
                    AutomaticIssuing = x.Item2.CredentialType.AutomaticIssuing,
                    MaxScoreDifference = x.Item2.CredentialType.MaxScoreDifference,
                    TestSessionBookingAvailabilityWeeks = x.Item2.CredentialType.TestSessionBookingAvailabilityWeeks,
                    TestSessionBookingClosedWeeks = x.Item2.CredentialType.TestSessionBookingClosedWeeks,
                    TestSessionBookingRejectHours = x.Item2.CredentialType.TestSessionBookingRejectHours,
                    AllowAvailabilityNotice = x.Item2.CredentialType.AllowAvailabilityNotice,
                    SkillType = x.Item2.CredentialType.SkillType
                },
                CredentialTypeId = x.Item2.CredentialTypeId,
                Supplementary = x.Item2.Supplementary,
                CredentialRequestPathTypeId = x.Item2.CredentialRequestPathTypeId,
                Direction = x.Item2.Direction,
                Fields = x.Item2.Fields.Select(b => new CredentialRequestFieldModel
                {
                    DataTypeId = b.DataTypeId,
                    DefaultValue = b.DefaultValue,
                    Description = b.Description,
                    FieldDataId = b.Id,
                    FieldTypeId = b.FieldTypeId,
                    Id = b.Id,
                    Mandatory = b.Mandatory,
                    Name = b.Name,
                    PerCredentialRequest = b.PerCredentialRequest,
                    Value = b.Value
                }).ToList(),
                Id = x.Item2.Id,
                ModifiedBy = x.Item2.ModifiedBy,
                SkillId = x.Item2.SkillId,
                ApplicationTypeDisplayName = x.Item2.ApplicationTypeDisplayName,
                Status = x.Item2.Status,
                StatusChangeDate = x.Item2.StatusChangeDate,
                StatusChangeUserId = x.Item2.StatusChangeUserId,
                StatusTypeId = x.Item2.StatusTypeId,
                ConcededFromCredentialRequestId = x.Item2.ConcededFromCredentialRequestId
            })).ToList();

            var summary = new[]
            {
                new ApplicationSummaryModel
                {
                    SummaryItem = "Credential Type:",
                    ApplicationCount = "",
                    CredentialRequestCount = ""
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Recognised Practising Translator",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.RecognisedPractisingTranslator).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.RecognisedPractisingTranslator).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Certified Translator",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedTranslator).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedTranslator).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Certified Advanced Translator",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedAdvancedTranslator).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedAdvancedTranslator).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Certified Advanced Translator LOTE to LOTE",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedAdvancedTranslatorLOTEtoLOTE).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedAdvancedTranslatorLOTEtoLOTE).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Recognised Practising Interpreter",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.RecognisedPractisingInterpreter).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.RecognisedPractisingInterpreter).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Certified Provisional Interpreter",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedProvisionalInterpreter).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedProvisionalInterpreter).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Certified Interpreter",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedInterpreter).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedInterpreter).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Certified Specialist Interpreter",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedSpecialistInterpreter).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedSpecialistInterpreter).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Certified Conference Interpreter",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedConferenceInterpreter).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedConferenceInterpreter).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Certified Conference Interpreter LOTE to LOTE",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedConferenceInterpreterLOTEtoLOTE).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedConferenceInterpreterLOTEtoLOTE).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Recognised Practising Deaf Interpreter",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.RecognisedPractisingDeafInterpreter).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.RecognisedPractisingDeafInterpreter).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Certified Provisional Deaf Interpreter",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedProvisionalDeafInterpreter).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.CertifiedProvisionalDeafInterpreter).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "CCL",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.CCL).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.CCL).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "CLAS",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.CLAS).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.CLAS).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Ethics",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.Ethics).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.Ethics).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Intercultural",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.CredentialType.Id == (int)CredentialType.Intercultural).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialType.Intercultural).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Application Type:",
                    ApplicationCount = "",
                    CredentialRequestCount = ""
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Transition Application",
                    ApplicationCount = applications.Count(x => x.Type == "Transition").ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item1.Type == "Transition").ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Credential Requests Status:",
                    ApplicationCount = "",
                    CredentialRequestCount = ""
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Draft",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.StatusTypeId == (int)CredentialRequestStatusType.Draft).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialRequestStatusType.Draft).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Application Rejected",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.StatusTypeId == (int)CredentialRequestStatusType.Rejected).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialRequestStatusType.Rejected).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Request Entered",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.StatusTypeId == (int)CredentialRequestStatusType.RequestEntered).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialRequestStatusType.RequestEntered).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Ready for Assessment",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.StatusTypeId == (int)CredentialRequestStatusType.ReadyForAssessment).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialRequestStatusType.ReadyForAssessment).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Being Assessment",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.StatusTypeId == (int)CredentialRequestStatusType.BeingAssessed).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialRequestStatusType.BeingAssessed).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Pending",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.StatusTypeId == (int)CredentialRequestStatusType.Pending).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialRequestStatusType.Pending).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Assessment Failed",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.StatusTypeId == (int)CredentialRequestStatusType.AssessmentFailed).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialRequestStatusType.AssessmentFailed).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Assessment Paid Review",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.StatusTypeId == (int)CredentialRequestStatusType.AssessmentPaidReview).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialRequestStatusType.AssessmentPaidReview).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Assessment Complete",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.StatusTypeId == (int)CredentialRequestStatusType.AssessmentComplete).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialRequestStatusType.AssessmentComplete).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "In Progress",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.StatusTypeId == (int)CredentialRequestStatusType.EligibleForTesting).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialRequestStatusType.EligibleForTesting).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Test Failed",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.StatusTypeId == (int)CredentialRequestStatusType.TestFailed).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialRequestStatusType.TestFailed).ToString()
                },
                new ApplicationSummaryModel
                {
                    SummaryItem = "Certification Issued",
                    ApplicationCount = credentialRequests.Where(x => x.Item2.StatusTypeId == (int)CredentialRequestStatusType.CertificationIssued).Select(x => x.Item1).Distinct().Count().ToString(),
                    CredentialRequestCount = credentialRequests.Count(x => x.Item2.CredentialType.Id == (int)CredentialRequestStatusType.CertificationIssued).ToString()
                }
            };

            LookupTypeResponse lookupReponse = null;
            if (request.DeserializedFilter.ApplicationStatusIntList != null && request.DeserializedFilter.ApplicationStatusIntList.Any())
            {
                lookupReponse = _applicationQueryService.GetLookupType("CredentialApplicationStatusType");
                request.DeserializedFilter.ApplicationStatusIntList =
                    request.DeserializedFilter.ApplicationStatusIntList.Select(x => lookupReponse.Results.First(a => a.Id == int.Parse(x)).DisplayName).ToArray();
            }

            if (request.DeserializedFilter.CredentialRequestTypeIntList != null && request.DeserializedFilter.CredentialRequestTypeIntList.Any())
            {
                lookupReponse = _applicationQueryService.GetLookupType("CredentialType");
                request.DeserializedFilter.CredentialRequestTypeIntList = request.DeserializedFilter.CredentialRequestTypeIntList
                    .Select(x => lookupReponse.Results.First(a => a.Id == int.Parse(x)).DisplayName).ToArray();
            }

            if (request.DeserializedFilter.CredentialRequestStatusIntList != null && request.DeserializedFilter.CredentialRequestStatusIntList.Any())
            {
                lookupReponse = _applicationQueryService.GetLookupType("CredentialRequestStatusType");
                request.DeserializedFilter.CredentialRequestStatusIntList = request.DeserializedFilter.CredentialRequestStatusIntList
                    .Select(x => lookupReponse.Results.First(a => a.Id == int.Parse(x)).DisplayName).ToArray();
            }

            if (request.DeserializedFilter.ApplicationTypeIntList != null && request.DeserializedFilter.ApplicationTypeIntList.Any())
            {
                lookupReponse = _applicationQueryService.GetLookupType("CredentialApplicationType");
                request.DeserializedFilter.ApplicationTypeIntList = request.DeserializedFilter.ApplicationTypeIntList
                    .Select(x => lookupReponse.Results.First(a => a.Id == int.Parse(x)).DisplayName).ToArray();
            }

            if (request.DeserializedFilter.ApplicationOwnerIntList != null && request.DeserializedFilter.ApplicationOwnerIntList.Any())
            {
                lookupReponse = _applicationQueryService.GetLookupType("ApplicationOwner");
                request.DeserializedFilter.ApplicationOwnerIntList = request.DeserializedFilter.ApplicationOwnerIntList
                    .Select(x => lookupReponse.Results.First(a => a.Id == int.Parse(x)).DisplayName).ToArray();
            }

            var exporter = new ApplicationExporter(request.DeserializedFilter, applications, credentialRequests, summary);

            var fileName = $"Application-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";

            var returnFile = new FileModel
            {
                FileData = exporter.Export(FileType.Xlsx),
                FileName = fileName + ".xlsx",
                FileType = FileType.Xlsx
            };

            return returnFile;
        }

        public GenericResponse<IEnumerable<CredentialRequestModel>> GetAllCredentialRequests(GetApplicationSearchRequest request)
        {
            var applications = Search(request);
            List<CredentialRequestModel> credentialRequests = new List<CredentialRequestModel>();
            foreach (var application in applications.Data)
            {
                var serviceReponse = _applicationQueryService.GetCredentialRequests(application.Id, new List<CredentialRequestStatusTypeName>());

                credentialRequests = credentialRequests.Concat(serviceReponse.Results.Select(MapCredentialRequest)).ToList();
            }

            return credentialRequests;
        }

        public GenericResponse<IEnumerable<CredentialRequestTestRequestModel>> GetAllCredentialTests(CredentialTestSearchRequestModel request)
        {
            var response = _applicationQueryService.GetAllCredentialTests(_autoMapperHelper.Mapper.Map<GetAllCredentialTestsRequest>(request));
            return new GenericResponse<IEnumerable<CredentialRequestTestRequestModel>>(response.Results.Select(s => _autoMapperHelper.Mapper.Map<CredentialRequestTestRequestModel>(s)));
        }

        public GenericResponse<IEnumerable<CredentialApplicationInfoModel>> ApplicationsForCredential(int credentialId)
        {
            ApplicationSearchResultResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetApplicationsForCredential(credentialId);

            var models = serviceReponse.Results.Select(x => new CredentialApplicationInfoModel()
            {
                ApplicationId = x.Id,
                ApplicationTypeName = x.ApplicationType,
                EnteredDate = x.EnteredDate
            }).ToList();

            return models;
        }

        public GenericResponse<IEnumerable<CredentialRequestModel>> GetCredentialRequests(int credentialApplicationId)
        {
            return GetCredentialRequests(credentialApplicationId, new[] { CredentialRequestStatusTypeName.Deleted });
        }

        public GenericResponse<IEnumerable<CredentialRequestModel>> GetOtherCredentialRequests(int credentialApplicationId)
        {
            var serviceReponse = _applicationQueryService.GetOtherCredentialRequests(credentialApplicationId);
            var models = serviceReponse.Results.Select(MapCredentialRequest);
            return new GenericResponse<IEnumerable<CredentialRequestModel>>(models);
        }

        private GenericResponse<IEnumerable<CredentialRequestModel>> GetCredentialRequests(int credentialApplicationId,
            IEnumerable<CredentialRequestStatusTypeName> excludedStatuses)
        {
            CredentialRequestsResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetCredentialRequests(credentialApplicationId, excludedStatuses);

            var models = serviceReponse.Results.Select(MapCredentialRequest).OrderBy(x => x.ExternalCredentialName).ThenBy(x => x.Skill.DisplayName);
            return new GenericResponse<IEnumerable<CredentialRequestModel>>(models);
        }

        public GenericResponse<CredentialRequestModel> GetCredentialRequest(int credentialRequestId)
        {
            CredentialRequestResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetCredentialRequest(credentialRequestId);

            var model = MapCredentialRequest(serviceReponse.CredentialRequest);
            return new GenericResponse<CredentialRequestModel>(model);
        }

        public void UpdateCredential(CredentialModel credential)
        {
            var serverRequest = _autoMapperHelper.Mapper.Map<CredentialDto>(credential);

            _applicationQueryService.UpdateCredential(serverRequest);
        }

        private CredentialRequestModel MapCredentialRequest(CredentialRequestDto credentialRequest)
        {
            return new CredentialRequestModel
            {
                Id = credentialRequest.Id,
                Category = credentialRequest.Category,
                CategoryId = credentialRequest.CategoryId,
                CredentialName = credentialRequest.CredentialName,
                ExternalCredentialName = credentialRequest.ExternalCredentialName,
                Direction = credentialRequest.Direction,
                DirectionTypeId = credentialRequest.DirectionTypeId,
                Status = credentialRequest.Status,
                AutoCreated = credentialRequest.AutoCreated,
                ApplicationTypeDisplayName = credentialRequest.ApplicationTypeDisplayName,
                StatusTypeId = credentialRequest.StatusTypeId,
                SkillId = credentialRequest.SkillId,
                StatusChangeDate = credentialRequest.StatusChangeDate,
                ModifiedBy = credentialRequest.ModifiedBy,
                StatusChangeUserId = credentialRequest.StatusChangeUserId,
                CredentialTypeId = credentialRequest.CredentialTypeId,
                Supplementary = credentialRequest.Supplementary,
                ConcededFromCredentialRequestId = credentialRequest.ConcededFromCredentialRequestId,
                CredentialRequestPathTypeId = credentialRequest.CredentialRequestPathTypeId,
                CredentialType = _autoMapperHelper.Mapper.Map<CredentialTypeModel>(credentialRequest.CredentialType),
                Skill = _autoMapperHelper.Mapper.Map<SkillModel>(credentialRequest.Skill),
                Credentials = credentialRequest.Credentials.Select(MapCredential).ToList(),
                Fields = credentialRequest.Fields.Select(_autoMapperHelper.Mapper.Map<CredentialRequestFieldModel>).ToList(),
                Actions = _wizardLogicService.GetValidCredentialRequestActions((CredentialRequestStatusTypeName)credentialRequest.StatusTypeId, credentialRequest.Id).ToList<dynamic>(),
                TestSessions = credentialRequest.TestSessions.Select(_autoMapperHelper.Mapper.Map<CredentialRequestTestSessionModel>).ToList(),
                WorkPractices = new List<WorkPracticeDataModel>(),
                Briefs = new List<CandidateBriefModel>()
            };
        }

        private CredentialModel MapCredential(CredentialDto dto)
        {
            var credential = _autoMapperHelper.Mapper.Map<CredentialModel>(dto);
            credential.RecertificationStatus = _applicationBusinessLogicService.CalculateCredentialRecertificationStatus(dto.Id);
            return credential;
        }

        public bool HasValidCredentialRequest(int credentialApplicationId, int categoryId, int credentialTypeId, int skillId)
        {
            var credentialRequests = GetCredentialRequests(credentialApplicationId, new[] { CredentialRequestStatusTypeName.Cancelled, CredentialRequestStatusTypeName.Deleted, CredentialRequestStatusTypeName.Withdrawn });

            return credentialRequests.Data.Any(r => r.CategoryId == categoryId &&
                                                    r.CredentialTypeId == credentialTypeId &&
                                                    r.SkillId == skillId);
        }

        public bool CanWithdrawApplicationUnderPaidReview(int credentialRequestId)
        {
            var serviceReponse = _examinerQueryService.CanWithdrawApplicationUnderPaidReview(credentialRequestId);
            return serviceReponse;
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetLookupType(string lookupType)
        {
            LookupTypeResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetLookupType(lookupType);

            return new GenericResponse<IEnumerable<LookupTypeModel>>(serviceReponse.Results.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>));
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetCredentialApplicaionTypes(IEnumerable<int> skillTypeIds)
        {
            LookupTypeResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetCredentialApplicaionTypes(new GetCredentialApplicaionTypesRequest
            {
                SkillTypeIds = skillTypeIds
            });

            return new GenericResponse<IEnumerable<LookupTypeModel>>(serviceReponse.Results.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>));
        }

        public GenericResponse<IEnumerable<CredentialApplicationSectionModel>> GetApplicationFieldsData(
            int applicationId)
        {
            GetApplicationFieldsDataResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetApplicationFieldsData(applicationId);

            var hasAdminRole = _userService.HasPermission(SecurityNounName.Application, SecurityVerbName.Configure);
            var applicationStatus = (CredentialApplicationStatusTypeName)GetApplication(applicationId).Data.ApplicationStatusTypeId;

            var fieldIsLock = !hasAdminRole && applicationStatus == CredentialApplicationStatusTypeName.Completed;

            var fieldsDtos = serviceReponse.Result;
            var sectionModels = fieldsDtos.GroupBy(x => x.Section)
                .Select(g => new CredentialApplicationSectionModel
                {
                    Name = g.Key,
                    Fields = g.Select(v => new CredentialApplicationFieldModel
                    {
                        FieldDataId = v.FieldDataId,
                        FieldTypeId = v.FieldTypeId,
                        Name = v.Name,
                        DataTypeId = v.DataTypeId,
                        DefaultValue = v.DefaultValue,
                        Value = v.Value,
                        PerCredentialRequest = v.PerCredentialRequest,
                        Description = v.Description,
                        Mandatory = v.Mandatory,
                        Disabled = v.Disabled,
                        DisplayNone = string.IsNullOrEmpty(v.Value) && v.Disabled,
                        DisplayOrder = v.DisplayOrder,
                        FieldOptionId = v.FieldOptionId,
                        FieldEnable = !fieldIsLock,
                        Options = v.Options.Select(x => new CredentialApplicationFieldOptionModel { DisplayName = x.DisplayName, FieldOptionId = x.FieldOptionId })
                    }).OrderBy(y => y.DisplayOrder).ToList()
                });

            return new GenericResponse<IEnumerable<CredentialApplicationSectionModel>>(sectionModels);
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetCredentialTypeSkills(IEnumerable<int> credentialTypeIds)
        {
            var response = _applicationQueryService.GetCredentialTypeSkills(new GetCredentialTypeSkillsRequest { CredentialTypeIds = credentialTypeIds });
            return new GenericResponse<IEnumerable<LookupTypeModel>>(response.Results.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>));
        }

        public GenericResponse<List<SkillEmailTokenObject>> GetCredentialSkills(IEnumerable<int> credentialIds)
        {
            var response = _applicationQueryService.GetCredentialSkills(new GetCredentialSkillsRequest { CredentialIds = credentialIds });
            return response;
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetCredentialTypeDomains(IEnumerable<int> credentialTypeIds)
        {
            var response = _testMaterialQueryService.GetCredentialTypeDomains(credentialTypeIds.ToList());
            return new GenericResponse<IEnumerable<LookupTypeModel>>(response.Results.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>));
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetCredentialTypeSkillsTestSession(IEnumerable<int> credentialTypeIds)
        {
            var response = _applicationQueryService.GetCredentialTypeSkillsTestSession(new GetCredentialTypeSkillsRequest { CredentialTypeIds = credentialTypeIds });
            return new GenericResponse<IEnumerable<LookupTypeModel>>(response.Results.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>));
        }

        public GenericResponse<UpsertApplicationResultModel> UpsertApplication(UpsertApplicationRequestModel applicationRequestModel)
        {
            var applicationRequest = _autoMapperHelper.Mapper.Map<UpsertCredentialApplicationRequest>(applicationRequestModel.ApplicationInfo);

            applicationRequest.Fields = applicationRequestModel.Sections?.SelectMany(section =>
                section.Fields?.Select(_autoMapperHelper.Mapper.Map<ApplicationFieldData>));

            var applicationCredentials = applicationRequestModel.CredentialRequests;

            applicationRequest.CredentialRequests = applicationCredentials.Select(_autoMapperHelper.Mapper.Map<CredentialRequestData>);
            applicationRequest.StandardTestComponents = applicationRequestModel.StandardTestComponents?.Select(_autoMapperHelper.Mapper.Map<StandardTestComponentContract>);
            applicationRequest.RubricTestComponents = applicationRequestModel.RubricTestComponents?.Select(_autoMapperHelper.Mapper.Map<RubricTestComponentContract>);
            applicationRequest.Recertification = _autoMapperHelper.Mapper.Map<RecertificationDto>(applicationRequestModel.Recertification);
            applicationRequest.PdActivities = applicationRequestModel.PdActivities?.Select(_autoMapperHelper.Mapper.Map<PdActivityData>);

            applicationRequest.Notes = applicationRequestModel.Notes.Select(x => new ApplicationNoteData
            {
                NoteId = x.NoteId ?? 0,
                CreatedDate = x.CreatedDate ?? DateTime.Now,
                Description = x.Note,
                Highlight = x.Highlight,
                ReadOnly = x.ReadOnly,
                UserId = x.UserId,
                ModifiedDate = x.ModifiedDate ?? DateTime.Now
            });

            applicationRequest.PersonNotes = applicationRequestModel.PersonNotes.Select(x => new PersonNoteData
            {
                NoteId = x.NoteId ?? 0,
                CreatedDate = x.CreatedDate ?? DateTime.Now,
                Description = x.Note,
                Highlight = x.Highlight,
                ReadOnly = x.ReadOnly,
                UserId = x.UserId,
                ModifiedDate = x.ModifiedDate ?? DateTime.Now
            });

            applicationRequest.ProcessedFees =
                applicationRequestModel.ProcessFee == null
                    ? null
                    : new List<ProcessFeeDto>
                      {
                          new ProcessFeeDto
                          {
                              CredentialWorkflowFeeId = applicationRequestModel.ProcessFee.CredentialWorkflowFeeId,
                              Type = applicationRequestModel.ProcessFee.Type,
                              PaymentReference = applicationRequestModel.ProcessFee.PaymentReference,
                              TransactionId = applicationRequestModel.ProcessFee.TransactionId,
                              OrderNumber = applicationRequestModel.ProcessFee.OrderNumber
                          }
                      };

            UpsertApplicationResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.UpsertApplication(applicationRequest);

            return UpsertApplicationResultModel(serviceReponse);
        }

        public GenericResponse<UpsertApplicationResultModel> CreateApplication(UpsertApplicationRequestModel model)
        {
            return CreateApplication<ApplicationActionWizardModel>(model);
        }

        public GenericResponse<UpsertApplicationResultModel> CreateMyNaatiApplication(UpsertApplicationRequestModel model)
        {
            return CreateApplication<MyNaatiApplicationActionWizardModel>(model);
        }

        private GenericResponse<UpsertApplicationResultModel> CreateApplication<T>(UpsertApplicationRequestModel model) where T : ApplicationActionWizardModel
        {
            GetCredentialApplicationTypeResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetCredentialApplicationType(model.ApplicationInfo.ApplicationTypeId);

            var applicationModel = new CredentialApplicationDetailedModel
            {
                ApplicationInfo = model.ApplicationInfo,
                ApplicationType = _autoMapperHelper.Mapper.Map<CredentialApplicationTypeModel>(serviceReponse.Result),
                ApplicationStatus = new LookupTypeDetailedModel
                {
                    DisplayName = CredentialApplicationStatusTypeName.None.ToString(),
                    Id = (int)CredentialApplicationStatusTypeName.None,
                    Name = CredentialApplicationStatusTypeName.None.ToString()
                },

                Notes = new List<ApplicationNoteModel>(),
                PersonNotes = new List<PersonNoteModel>()
            };

            var wizardModel = Activator.CreateInstance<T>();
            wizardModel.ActionType = (int)SystemActionTypeName.CreateApplication;
            wizardModel.ApplicationId = 0;

            var response = PerformAction(wizardModel, applicationModel, out var output);

            if (response.Success)
            {
                if (output.PendingCredentialRequests?.Any() == true)
                {
                    foreach (var cr in output.PendingCredentialRequests)
                    {
                        LoggingHelper.LogDebug("Auto-creating Credential Request");

                        var input = _autoMapperHelper.Mapper.Map<AutoCreateCredentialRequestNonWizardModel>(cr);
                        input.ApplicationId = response.Data.CredentialApplicationId;
                        input.ActionType = (int)SystemActionTypeName.NewCredentialRequest;

                        var crResponse = PerformAction(input);

                        response = new[] { response, crResponse }.CombineResponses<UpsertApplicationResultModel>();
                    }
                }
            }

            return response;
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetVenue(IEnumerable<int> testLocation)
        {
            LookupTypeResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetVenue(testLocation);

            return new GenericResponse<IEnumerable<LookupTypeModel>>(serviceReponse.Results.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>));
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetVenuesShowingInactive(IEnumerable<int> testLocation)
        {
            LookupTypeResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetVenuesShowingInactive(testLocation);

            return new GenericResponse<IEnumerable<LookupTypeModel>>(serviceReponse.Results.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>));
        }

        private UpsertApplicationResultModel UpsertApplicationResultModel(UpsertApplicationResponse serviceResponse)
        {
            return new UpsertApplicationResultModel
            {
                CredentialApplicationId = serviceResponse.CredentialApplicationId,
                CredentialApplicationFieldDataIds = serviceResponse.ApplicationFieldDataIds,
                CredentialRequestIds = serviceResponse.CredentialRequestIds,
                NoteIds = serviceResponse.NoteIds
            };
        }

        public GenericResponse<CredentialApplicationInfoModel> GetApplication(int applicationId)
        {
            var serviceReponse = _applicationQueryService.GetApplication(applicationId);

            if(serviceReponse == null)
            {
                return new GenericResponse<CredentialApplicationInfoModel>()
                {
                    Success = false,
                    Messages = new List<string> { "ApplicationId was not found"}
                };
            }

            var model = _autoMapperHelper.Mapper.Map<CredentialApplicationInfoModel>(serviceReponse.Result);
            model.StatusChangeDate = serviceReponse.Result.StatusChangeDate;
            model.CredentialApplicationTypeCategoryId = (int)serviceReponse.Result.ApplicationTypeCategory;
            return model;
        }

        private CredentialApplicationDetailedModel GetApplicationDetailsByCredentialRequestId(int credentialRequestId)
        {
            var request = new GetApplicationDetailsRequest
            {
                CredentialRequestId = credentialRequestId,
                ExcludedRequestStauses = new[] { CredentialRequestStatusTypeName.Cancelled, CredentialRequestStatusTypeName.Deleted }
            };

            return GetApplicationDetails(request);
        }

        private CredentialApplicationDetailedModel GetApplicationDetailsByTestSittingId(int testSittingId)
        {
            var request = new GetApplicationDetailsRequest
            {
                TestSittingId = testSittingId,
                ExcludedRequestStauses = new[] { CredentialRequestStatusTypeName.Cancelled, CredentialRequestStatusTypeName.Deleted }
            };

            return GetApplicationDetails(request);
        }


        private CredentialApplicationDetailedModel GetApplicationDetails(GetApplicationDetailsRequest request)
        {
            GetApplicationDetailsResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetApplicationDetails(request);
            var model = _autoMapperHelper.Mapper.Map<CredentialApplicationDetailedModel>(serviceReponse);

            foreach (var credentialRequest in model.CredentialRequests)
            {
                var credentialApplicationTypeCredentialType = credentialRequest.CredentialType.CredentialApplicationTypeCredentialTypes.FirstOrDefault(c => c.CredentialApplicationTypeId == model.ApplicationInfo.ApplicationTypeId);

                if (credentialApplicationTypeCredentialType != null)
                {
                    credentialApplicationTypeCredentialType.HasTestFee = HasTestFee(credentialApplicationTypeCredentialType.CredentialApplicationTypeId, credentialApplicationTypeCredentialType.CredentialTypeId);
                }

                credentialRequest.WorkPractices = new List<WorkPracticeDataModel>();
                credentialRequest.Briefs = new List<CandidateBriefModel>();
            }

            if (model.Notes == null)
            {
                model.Notes = new List<ApplicationNoteModel>();
            }
            if (model.PersonNotes == null)
            {
                model.PersonNotes = new List<PersonNoteModel>();
            }

            if (model.PdActivities == null)
            {
                model.PdActivities = new List<PdActivityFieldModel>();
            }

            return model;
        }

        public bool HasTestFee(int credentialApplicationTypeId, int credentialTypeId)
        {
            var serviceReponse = false;
            serviceReponse = _applicationQueryService.HasTestFee(credentialApplicationTypeId, credentialTypeId);
            return serviceReponse;
        }

        public bool HasTest(int credentialApplicationTypeId, int credentialTypeId)
        {
            var serviceReponse = false;
            serviceReponse = _applicationQueryService.HasTest(credentialApplicationTypeId, credentialTypeId);
            return serviceReponse;
        }

        public CredentialApplicationDetailedModel GetApplicationDetails(int applicationId)
        {
            var request = new GetApplicationDetailsRequest
            {
                ApplicationId = applicationId,
                ExcludedRequestStauses = new[] { CredentialRequestStatusTypeName.Cancelled, CredentialRequestStatusTypeName.Deleted }
            };

            return GetApplicationDetails(request);
        }

        public BusinessServiceResponse PerformAction(ApplicationActionWizardModel wizardModel)
        {
            CredentialApplicationDetailedModel applicationModel = wizardModel.ApplicationId == 0 ? null : GetApplicationDetails(wizardModel.ApplicationId);
            return PerformAction(wizardModel, applicationModel, out var output);
        }

        public BusinessServiceResponse PerformCredentialRequestsBulkAction(CredentialRequestsBulkActionWizardModel wizardModel)
        {
            var credentialRequestIds = wizardModel.CredentialRequestIds;

            LoggingHelper.LogInfo($"Perform bulk Action for credential requestIds {string.Join(",", credentialRequestIds)}");
            var response = new BusinessServiceResponse();

            foreach (var credentialRequestId in credentialRequestIds)
            {
                try
                {
                    var applicationModel = GetApplicationDetailsByCredentialRequestId(credentialRequestId);
                    wizardModel.ApplicationId = applicationModel.ApplicationInfo.ApplicationId;
                    wizardModel.CredentialRequestId = credentialRequestId;
                    var result = PerformAction(wizardModel, applicationModel, out var output);
                    response.Errors.AddRange(result.Errors);
                }

                catch (UserFriendlySamException ex)
                {
                    LoggingHelper.LogWarning(ex, ex.Message);
                    response.Errors.Add(ex.Message);
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(ex);
                    var generalError = $"Error processing Credential Request: {credentialRequestId} ";

                    response.Errors.Add(generalError);
                }
            }

            response.Success = !response.Errors.Any();
            return response;
        }

        public BusinessServiceResponse PerformTestMaterialAssignementBulkAction(TestMaterialAssignmentBulkModel wizardModel)
        {
            var testSessionIds = wizardModel.TestSessionIds;
            var testMaterialAssignments = wizardModel.TestMaterialAssignments;
            var summary = _testMaterialQueryService.GetTestMaterialsSummary(new TestMaterialsSummaryRequest
            {
                TestSessionIds = testSessionIds,
                TestMaterialAssignments = testMaterialAssignments.Select(_autoMapperHelper.Mapper.Map<TestMaterialAssignmentDto>),
                SkillId = (int)wizardModel.SkillId,
                TestSpecificationId = wizardModel.TestSpecificationId
            });
            var testSittingIds = summary.TestSittingIds;
            var response = new BusinessServiceResponse();

            foreach (var testSittingId in testSittingIds)
            {
                try
                {
                    wizardModel.SetTestSittingId(testSittingId);
                    var applicationModel = GetApplicationDetailsByTestSittingId(testSittingId);
                    wizardModel.ApplicationId = applicationModel.ApplicationInfo.ApplicationId;
                    var credentialRequest = applicationModel.CredentialRequests.First(x => x.TestSessions.Any(y => y.CredentialTestSessionId == testSittingId));
                    wizardModel.CredentialRequestId = credentialRequest.Id;
                    var result = PerformAction(wizardModel, applicationModel, out var output);
                    response.Errors.AddRange(result.Errors);
                }

                catch (UserFriendlySamException ex)
                {
                    response.Errors.Add(ex.Message);
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(ex);
                    var generalError = $"Error processing Test Sitting: {testSittingId} ";

                    response.Errors.Add(generalError);
                }
            }

            response.Success = !response.Errors.Any();
            return response;
        }

        public void RollbackIssueCredential(RollbackIssueCredentialModel model)
        {
            var request = _autoMapperHelper.Mapper.Map<RollbackIssueCredentialRequest>(model);
            _applicationQueryService.RollbackIssueCredential(request);
        }

        public CheckOptionData GetCheckOptionMessage()
        {
            return new CheckOptionData
            {
                Checked = true,
                Message = Naati.Resources.Application.SendEmailOption,
                OnDisableMessage = Naati.Resources.Application.SkipEmailWarning
            };
        }

        public CheckOptionData GetIncompletePrerequisiteCheckOptionData()
        {
            return new CheckOptionData
            {
                Checked = false,
                Message = Naati.Resources.Application.IncompleteCredentialPrerequisiteCheckMessage,
                OnDisableMessage = Naati.Resources.Application.IncompleteCredentialPrerequisiteUncheckMessage
            };
        }

        public CheckOptionData GetIssueOnHoldCredentialsCheckOptionData()
        {
            return new IssueOnHoldCredentialCheckOptionData
            {
                Checked = false,
                Message = Naati.Resources.Application.IssueOnHoldCredentialsStepMessage,
                OnDisableMessage = Naati.Resources.Application.IssueOnHoldCredentialsUnCheckMesssage,
                OnEnableMessage = Naati.Resources.Application.IssueOnHoldCredentialsCheckMesssage
            };
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetEndorsementQualificationLookup(GetEndorsementQualificationLookupRequestModel request)
        {
            var getRequest = new GetEndorsementQualificationLookupRequest { Locations = request.Locations, InstitutionNaatiNumbers = request.InstitutionNaatiNumbers ?? new int[] { } };
            var response = _applicationQueryService.GetEndorsementQualificationLookup(getRequest);

            var models = response.Results.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>).ToList();
            return models;
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetEndorsementLocationLookup(GetEndorsementLocationLookupRequestModel request)
        {
            var getRequest = new GetEndorsementLocationLookupRequest { InstitutionNaatiNumbers = request.InstitutionNaatiNumbers ?? new int[] { } };
            var response = _applicationQueryService.GetEndorsementLocationLookup(getRequest);

            var models = response.Results.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>).ToList();
            return new GenericResponse<IEnumerable<LookupTypeModel>>(models);
        }

        private GenericResponse<UpsertApplicationResultModel> PerformAction(ApplicationActionWizardModel wizardModel, CredentialApplicationDetailedModel applicationModel, out ApplicationActionOutput output)
        {
            var action = ApplicationStateAction.CreateAction((SystemActionTypeName)wizardModel.ActionType, applicationModel, wizardModel);

            if (!action.ArePreconditionsMet())
            {
                var response = new GenericResponse<UpsertApplicationResultModel>();
                output = null;
                response.Errors.AddRange(action.ValidationErrors.Select(x => x.Message));
                response.Success = false;
                return response;
            }

            action.Perform();
            action.SaveChanges();
            output = action.GetOutput();

            // UpsertApplications will be null if no prerequisite application was selected to be created in the CredentialRequestCreatePreRequisiteApplicationsAction
            if (output.UpsertResults == null)
            {
                output.UpsertResults = new GenericResponse<UpsertApplicationResultModel>()
                {
                    Success = true
                };
            }

            return output.UpsertResults;
        }



        public GenericResponse<IEnumerable<ValidationResultModel>> ValidateActionPreconditions(ApplicationActionWizardModel wizardModel)
        {
            LoggingHelper.LogVerbose("Validating Action {Action}, {@WizardModel}", (SystemActionTypeName)wizardModel.ActionType, wizardModel);

            CredentialApplicationDetailedModel applicationModel = GetApplicationDetails(wizardModel.ApplicationId);

            var action = ApplicationStateAction.CreateAction((SystemActionTypeName)wizardModel.ActionType, applicationModel, wizardModel);
            try
            {
                action.ValidatePreconditions();
            }
            catch (UserFriendlySamException ex)
            {
                action.ValidationErrors.Add(new ValidationResultModel { Field = string.Empty, Message = ex.Message });
            }
            catch (Exception ex)
            {
                action.ValidationErrors.Add(new ValidationResultModel { Field = string.Empty, Message = "Error Validating Application" });
                LoggingHelper.LogException(ex, "Error validating application APP{ApplicationId}", wizardModel.ApplicationId);
            }

            return new GenericResponse<IEnumerable<ValidationResultModel>>(action.ValidationErrors);
        }


        public GenericResponse<IEnumerable<CredentialLookupTypeModel>> GetCredentialTypesForApplication(int applicationId, int categoryId)
        {
            CredentialLookupTypeResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetCredentialTypesForApplication(applicationId, categoryId);

            var data = serviceReponse.Results.Select(_autoMapperHelper.Mapper.Map<CredentialLookupTypeModel>);
            return new GenericResponse<IEnumerable<CredentialLookupTypeModel>>(data);
        }

        public GenericResponse<IEnumerable<CredentialTypeUpgradePathModel>> GetValidCredentialTypeUpgradePaths(CredentialPathRequestModel request)
        {
            var response = _applicationQueryService.GetValidCredentialTypeUpgradePaths(new GetUpgradeCredentialPathRequest
            {
                CredentialTypesIdsFrom = request.CredentialTypesIdsFrom
            });
            var data = response.Results.Select(_autoMapperHelper.Mapper.Map<CredentialTypeUpgradePathModel>);
            return new GenericResponse<IEnumerable<CredentialTypeUpgradePathModel>>(data);
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetCredentialTypesForApplicationType(int applicationTypeId)
        {
            var respone = _applicationQueryService.GetCredentialTypesForApplicationType(applicationTypeId);
            var data = respone.Results.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>);
            return new GenericResponse<IEnumerable<LookupTypeModel>>(data);
        }

        public GenericResponse<IEnumerable<SkillLookupTypeModel>> GetSkillsForCredentialType(IEnumerable<int> credentialTypes, IEnumerable<int> credentialApplicationTypes)
        {
            GetSkillsForCredentialTypeResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetSkillsForCredentialType(new GetSkillsForCredentialTypeRequest { CredentialTypeIds = credentialTypes, CredentialApplicationTypeIds = credentialApplicationTypes });
            var data = serviceReponse.Results.Select(_autoMapperHelper.Mapper.Map<SkillLookupTypeModel>).ToList();
            return new GenericResponse<IEnumerable<SkillLookupTypeModel>>(data);
        }

        public GenericResponse<IEnumerable<TestTaskLookupTypeModel>> GetTestTask(IEnumerable<int> credentialTypes)
        {
            var serviceReponse = _applicationQueryService.GetTestTask(new GetTestTaskRequest { CredentialTypeIds = credentialTypes });
            var data = serviceReponse.Results.Select(_autoMapperHelper.Mapper.Map<TestTaskLookupTypeModel>);
            return new GenericResponse<IEnumerable<TestTaskLookupTypeModel>>(data);
        }


        public IEnumerable<CredentialApplicationAttachmentModel> ListAttachments(int applicationId)
        {
            var request = new GetApplicationAttachmentsRequest
            {
                ApplicationId = applicationId,
            };

            GetApplicationAttachmentsResponse response = null;

            try
            {
                response = _applicationQueryService.GetAttachments(request);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return response?.Attachments.Select((Func<CredentialApplicationAttachmentDto, CredentialApplicationAttachmentModel>)(a =>
            {
                var model = _autoMapperHelper.Mapper.Map<CredentialApplicationAttachmentModel>(a);
                model.FileType = Path.GetExtension(a.FileName)?.Trim('.');
                model.Title = a.Description;
                return model;
            })).ToArray() ?? new CredentialApplicationAttachmentModel[0];
        }

        public int CreateOrReplaceAttachment(CredentialApplicationAttachmentModel request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<CreateOrReplaceApplicationAttachmentRequest>(request);

            CreateOrReplaceApplicationAttachmentResponse response = null;

            try
            {
                response = _applicationQueryService.CreateOrReplaceAttachment(serviceRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return response.StoredFileId;
        }

        public void DeleteAttachment(int storedFileId)
        {
            var serviceRequest = new DeleteApplicationAttachmentRequest
            {
                StoredFileId = storedFileId
            };

            DeleteApplicationAttachmentResponse response = null;

            try
            {
                response = _applicationQueryService.DeleteAttachment(serviceRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetCredentialCategoriesForApplicationType(int applicationId)
        {
            LookupTypeResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetCredentialCategoriesForApplicationType(applicationId);

            var data = serviceReponse.Results.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>);
            return new GenericResponse<IEnumerable<LookupTypeModel>>(data);
        }

        public GenericResponse<IEnumerable<string>> GetDocumentTypesForApplicationType(int applicationId)
        {
            GetDocumentTypesForApplicationTypeResponse serviceReponse = null;
            serviceReponse = _applicationQueryService.GetDocumentTypesForApplicationType(applicationId);
            return new GenericResponse<IEnumerable<string>>(serviceReponse.Results);
        }

        public GenericResponse<IEnumerable<DocumentData>> CreateCredentialDocuments(CreateCredentialDocumentsRequestModel request)
        {
            var tempFilePath = ConfigurationManager.AppSettings["tempFilePath"];

            var getTemplateResponse = _applicationQueryService.GetCredentialTypeTemplates(new GetCredentialTypeTemplateRequest
            {
                CredentialTypeId = request.CredentialTypeId,
                TempFilePath = tempFilePath,
                CredentialId = request.CredentialId
            });

            var credentialTypeTemplates = getTemplateResponse.Results;
            var pdfCreatorHelper = new PdfCreatorHelper(_tokenReplacementService);

            try
            {
                var data = pdfCreatorHelper.CreateCredentialDocuments(request, getTemplateResponse.Results, tempFilePath);
                return new GenericResponse<IEnumerable<DocumentData>>(data);
            }
            finally
            {
                foreach (var filePath in credentialTypeTemplates.Select(x => x.FilePath))
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }

        public CreateCredentialCertificateResponse SaveCredentialDocuments(CreateCredentialDocumentsRequestModel request)
        {
            var documents = CreateCredentialDocuments(request);

            return _applicationQueryService.CreateCredentialDocuments(new CreateCredentialDocumentsRequest
            {
                Documents = documents.Data,
                CredentialId = request.CredentialId,
                ApplicationId = request.ApplicationId,
                UserId = request.UserId
            });
        }

        public GenericResponse<IEnumerable<CredentialApplicationEmailMessageModel>> GetEmailPreview(ApplicationActionWizardModel wizardModel)
        {
            CredentialApplicationDetailedModel applicationModel = GetApplicationDetails(wizardModel.ApplicationId);
            var action = ApplicationStateAction.CreateAction((SystemActionTypeName)wizardModel.ActionType, applicationModel, wizardModel);
            var emails = action.GetEmailPreviews();
            return new GenericResponse<IEnumerable<CredentialApplicationEmailMessageModel>>(emails);
        }

        public GenericResponse<IEnumerable<EmailTemplateModel>> GetEmailTemplates(ApplicationActionWizardModel wizardModel)
        {
            CredentialApplicationDetailedModel applicationModel = GetApplicationDetails(wizardModel.ApplicationId);
            var action = ApplicationStateAction.CreateAction((SystemActionTypeName)wizardModel.ActionType, applicationModel, wizardModel);
            var emailTemplates = action.GetEmailTemplates();
            return new GenericResponse<IEnumerable<EmailTemplateModel>>(emailTemplates);
        }

        public GenericResponse<IEnumerable<CredentialApplicationEmailMessageModel>> GetCredentialRequestBulkActionEmailPreview(CredentialRequestsBulkActionWizardModel wizardModel)
        {
            var credentialRequests = wizardModel.CredentialRequestIds;
            if (credentialRequests.Length == 0)
            {
                return new GenericResponse<IEnumerable<CredentialApplicationEmailMessageModel>>(Enumerable.Empty<CredentialApplicationEmailMessageModel>());
            }

            wizardModel.CredentialRequestId = credentialRequests[0];
            CredentialApplicationDetailedModel applicationModel = GetApplicationDetailsByCredentialRequestId(wizardModel.CredentialRequestId);
            wizardModel.ApplicationId = applicationModel.ApplicationInfo.ApplicationId;
            var action = ApplicationStateAction.CreateAction((SystemActionTypeName)wizardModel.ActionType, applicationModel, wizardModel);
            var emails = action.GetEmailPreviews();
            return new GenericResponse<IEnumerable<CredentialApplicationEmailMessageModel>>(emails);
        }

        public ApplicationInvoiceCreateRequestModel GetMyNaatiInvoicePreview(ApplicationActionWizardModel wizardModel)
        {
            CredentialApplicationDetailedModel applicationModel = GetApplicationDetails(wizardModel.ApplicationId);
            var action = ApplicationStateAction.CreateAction((SystemActionTypeName)wizardModel.ActionType, applicationModel, wizardModel);

            var invoice = action.GetInvoicePreview();
            invoice.CancelOperationIfError = false;
            return invoice;
        }

        public GenericResponse<InvoicePreviewModel> GetInvoicePreview(ApplicationActionWizardModel wizardModel)
        {
            CredentialApplicationDetailedModel applicationModel = GetApplicationDetails(wizardModel.ApplicationId);
            var action = ApplicationStateAction.CreateAction((SystemActionTypeName)wizardModel.ActionType, applicationModel, wizardModel);

            var invoice = action.GetInvoicePreview() ?? new ApplicationInvoiceCreateRequestModel();

            InstitutionModel sponsor = null;
            if (applicationModel.ApplicationInfo.SponsorInstitutionNaatiNumber > 0)
            {
                sponsor = _institutionService.GetInstitution(applicationModel.ApplicationInfo.SponsorInstitutionNaatiNumber).Data;
            }


            var user = ServiceLocator.Resolve<IUserService>().Get();
            var office = ServiceLocator.Resolve<IApplicationService>()
                .GetLookupType(LookupType.OfficeAbbreviation.ToString()).Data.FirstOrDefault(x => x.Id == user.OfficeId);
            var preview = new InvoicePreviewModel
            {
                //BrandingTheme = brandingTheme,
                DueDate = invoice.DueDate,
                InvoiceTo = sponsor != null
                                      ? $"{sponsor.NaatiNumber} - {sponsor.Name}"
                                      : $"{applicationModel.ApplicantDetails.NaatiNumber} - {applicationModel.ApplicantDetails.GivenName} {applicationModel.ApplicantDetails.FamilyName}",
                LineItems = invoice.LineItems,
                UserOfficeAbbreviation = office?.DisplayName

            };

            return preview;
        }

        public GenericResponse<bool> CheckDuplicatedApplication(int naatiNumber, int credentialApplicationTypeId)
        {
            return _applicationQueryService.CheckDuplicatedApplication(naatiNumber, credentialApplicationTypeId);
        }

        public GenericResponse<IEnumerable<DocumentData>> GetIssueCredentialPreview(DocumentsPreviewRequestModel previewModel)
        {
            var wizardModel = new ApplicationActionWizardModel
            {
                ApplicationId = previewModel.ApplicationId,
                CredentialRequestId = previewModel.CredentialRequestId,
                ActionType = previewModel.ActionId,
                Data = null
            };
            var applicationModel = GetApplicationDetails(wizardModel.ApplicationId);
            var action = ApplicationStateAction.CreateAction((SystemActionTypeName)previewModel.ActionId, applicationModel, wizardModel);
            var docs = action.GetDocumentsPreview(previewModel);
            return new GenericResponse<IEnumerable<DocumentData>>(docs);
        }

        public GenericResponse<WizardIssueCredentialStepModel> GetWizardIssueCredentialData(int applicationId, int credentialRequestId, int actionId)
        {
            // logic comes from UC5021 BR20, BR21, BR23
            var application = GetApplicationDetails(applicationId);
            var credentialRequest = application.CredentialRequests.Single(x => x.Id == credentialRequestId);

            bool originalShowInOnlineDirectory = false;

            if (credentialRequest.CredentialType.Certification)
            {
                var originalCredential = GetCredentialRecertifications(application.ApplicantDetails.NaatiNumber, credentialRequest.SkillId, credentialRequest.CredentialTypeId);
                if (originalCredential != null)
                {
                    originalShowInOnlineDirectory = originalCredential.ShowInOnlineDirectory;
                }
            }

            var model = new WizardIssueCredentialStepModel
            {
                StartDate = DateTime.Now.Date,
                // 195595
                ShowInOnlineDirectory = originalShowInOnlineDirectory,
                //ShowInOnlineDirectory = credentialRequest.CredentialType.Certification,
            };

            if (!credentialRequest.CredentialType.Certification && credentialRequest.CredentialType.DefaultExpiry.HasValue)
            {
                model.ExpiryDate = DateTime.Now.Date.AddYears(credentialRequest.CredentialType.DefaultExpiry.Value).AddDays(-1);
            }

            if (!String.IsNullOrEmpty(application.ApplicantDetails.PractitionerNumber))
            {
                model.PractitionerNumber = application.ApplicantDetails.PractitionerNumber;
            }
            else if (credentialRequest.CredentialType.Certification)
            {
                var pnHelper = new PractitionerNumberHelper(_systemService, _personQueryService);
                model.PractitionerNumber = pnHelper.GetNewPractitionerNumber();
            }

            if (credentialRequest.CredentialType.Certification)
            {
                LoadCertificationPeriods(actionId, model, application, credentialRequest);
            }
            else
            {
                model.CertificationPeriods = new List<CertificationPeriodModel>();
                if (actionId == (int)SystemActionTypeName.ReissueCredential)
                {
                    var existingCredential = credentialRequest.Credentials.OrderBy(x => x.Id).LastOrDefault();
                    if (existingCredential != null)
                    {
                        model.StartDate = existingCredential.StartDate;
                        model.ExpiryDate = existingCredential.ExpiryDate;
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Implements logic from UC5021 BR20, BR21, BR23
        /// </summary>
        private void LoadCertificationPeriods(int actionId,
            WizardIssueCredentialStepModel model,
            CredentialApplicationDetailedModel application, CredentialRequestModel credentialRequest)
        {
            var systemValues = _systemService.GetAllSystemValues();
            var certificationPeriodNextPeriodSystemValue = systemValues.FirstOrDefault(x => x.ValueKey == "CertificationPeriodNextPeriod")?.Value;
            certificationPeriodNextPeriodSystemValue.NotNull("Missing system value: CertificationPeriodNextPeriod");
            var certificationPeriodDefaultDurationSystemValue = systemValues.FirstOrDefault(x => x.ValueKey == "CertificationPeriodDefaultDuration")?.Value;
            certificationPeriodDefaultDurationSystemValue.NotNull("Missing system value: CertificationPeriodDefaultDuration");
            var certificationPeriodRecertifySystemValue = systemValues.FirstOrDefault(x => x.ValueKey == "CertificationPeriodRecertifyExpiry")?.Value;
            certificationPeriodRecertifySystemValue.NotNull("Missing system value: CertificationPeriodRecertifyExpiry");

            var certificationPeriodNextPeriodThresholdMonths = Int32.Parse(certificationPeriodNextPeriodSystemValue);
            var certificationPeriodDefaultDurationYears = Int32.Parse(certificationPeriodDefaultDurationSystemValue);
            var cpResponse = _personQueryService.GetCertificationPeriods(new GetCertificationPeriodsRequest
            {
                PersonId = application.ApplicantDetails.PersonId,
                CertificationPeriodStatus = new List<CertificationPeriodStatus>
                {
                    CertificationPeriodStatus.Expired,
                    CertificationPeriodStatus.Current,
                    CertificationPeriodStatus.Future
                }
            });

            // display all current and future periods; ordered by Start Date
            model.CertificationPeriods = cpResponse.Results
                .Where(x => x.EndDate.Date >= DateTime.Today.Date)
                .Select(_autoMapperHelper.Mapper.Map<CertificationPeriodModel>)
                .OrderBy(x => x.StartDate)
                .ToList();


            var newPeriod = new CertificationPeriodModel();
            newPeriod.StartDate = DateTime.Today.Date;
            newPeriod.CredentialApplicationId = application.ApplicationInfo.ApplicationId;

            // if there are no Future Certification Periods, display New Certification Period 
            if (!cpResponse.Results.Any(x => x.StartDate.Date > DateTime.Today.Date))
            {
                model.CertificationPeriods.Add(newPeriod);
                // if there is a current or recent period, the new period starts immediately after the end of the most recent period, as long as the 
                // gap is less than CertificationPeriodRecertifyExpiry months (see BR21). if the gap is longer, the new period will start today.
                var minimumCertificationPeriodsGap = DateTime.Now.Date.AddMonths(-int.Parse(certificationPeriodRecertifySystemValue));
                var previousEndDate = cpResponse.Results.Where(x => x.OriginalEndDate >= minimumCertificationPeriodsGap)
                    .OrderByDescending(x => x.OriginalEndDate)
                    .FirstOrDefault()
                    ?.OriginalEndDate.Date;

                if (previousEndDate.HasValue)
                {
                    newPeriod.StartDate = previousEndDate.Value.AddDays(1);
                }
            }

            newPeriod.EndDate = newPeriod.StartDate.GetLeapYearAdjustedEndDate(certificationPeriodDefaultDurationYears);
            newPeriod.OriginalEndDate = newPeriod.EndDate;

            // BR20. certification period selection
            var currentPeriods = cpResponse.Results.Where(x => x.StartDate.Date <= DateTime.Today.Date && x.EndDate.Date >= DateTime.Today.Date).ToList();
            switch (currentPeriods.Count)
            {
                case 0:
                    // if there are no current periods, select New/Future
                    model.SelectedCertificationPeriodId = model.CertificationPeriods.Last().Id;
                    break;
                case 1:
                    model.SelectedCertificationPeriodId =
                        // if the current period original end date is still more than CertificationPeriodNextPeriod months from today, select it
                        currentPeriods.Single().OriginalEndDate.Date.AddMonths(-certificationPeriodNextPeriodThresholdMonths) > DateTime.Today.Date
                            ? currentPeriods.Single().Id
                            // otherwise select New/Future
                            : model.CertificationPeriods.Last().Id;
                    break;
                default:
                    // select the latest current period
                    model.SelectedCertificationPeriodId = currentPeriods.Last().Id;
                    break;
            }

            // If this person has a Credential which is already issued within the same application, then default select the same Certification Period as one of the previously issued Credential
            // this rule overrides the above logic
            var existingCertificationPeriod =
                application.CredentialRequests
                    .SelectMany(x => x.Credentials)
                    .Select(x => x.CertificationPeriod)
                    .OrderBy(x => x.StartDate)
                    .LastOrDefault();

            if (existingCertificationPeriod != null && model.CertificationPeriods.Any(x => x.Id == existingCertificationPeriod.Id))
            {
                model.SelectedCertificationPeriodId = existingCertificationPeriod.Id;
                // BR21.3.b
                newPeriod.StartDate = existingCertificationPeriod.OriginalEndDate.Date.AddDays(1);
            }

            model.CertificationPeriodDefaultDuration = certificationPeriodDefaultDurationYears;

            if (actionId == (int)SystemActionTypeName.ReissueCredential)
            {
                var existingCredential = credentialRequest.Credentials.OrderBy(x => x.Id).LastOrDefault();
                if (existingCredential != null && model.CertificationPeriods.Any(x => x.Id == existingCredential.CertificationPeriod.Id))
                {
                    model.StartDate = existingCredential.StartDate;
                    model.SelectedCertificationPeriodId = existingCredential.CertificationPeriod.Id;
                }
            }
            else if (credentialRequest.CredentialRequestPathTypeId == (int)CredentialRequestPathTypeName.Recertify)
            {
                // when recertifying, keep the start date of the existing credential
                var existingCredential = GetCredentialBeingRecertified(application.ApplicantDetails.NaatiNumber, credentialRequest.SkillId, credentialRequest.CredentialTypeId);
                existingCredential.NotNull("Can't find existing credential being recertified");
                model.ShowInOnlineDirectory = existingCredential.ShowInOnlineDirectory;
                model.StartDate = existingCredential.StartDate;
                model.DisallowEditCredentialStartDate = true;
            }

            // the start date of the new period can be modified if there are no other periods
            if (model.CertificationPeriods.Count == 1 && model.CertificationPeriods[0].Id == 0)
            {
                model.AllowEditCertificationPeriodStartDate = true;
            }
        }

        /// <summary>
        /// after writing the code the long way and constructing the unit tests it 
        /// became obvious the simple solution was add days.Add Years does not work the same way
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="policyYears"></param>
        /// <returns></returns>
        public DateTime GetLeapYearAdjustedEndDate(DateTime startDate, int policyYears)
        {
            // a 3 year certification is actually 1095 days
            var policyDays = 365 * policyYears;

            return startDate.AddDays(policyDays).AddDays(-1);

            //var adjustDays = 0;
            //if(DateTime.IsLeapYear(startDate.Year))
            //{
            //   if(DateTime.Compare(startDate,DateTime.Parse($"28-Feb-{startDate.Year}")) <= 0) //use <= in case it's the 28th Feb. 29th should be OK
            //    {
            //        adjustDays--;
            //    }
            //}
            //for(int i= 2; i< policyYears;i++)
            //{
            //    if (DateTime.IsLeapYear((startDate.AddYears(i)).Year))
            //    {
            //        adjustDays--;
            //    }
            //}

            //if (DateTime.IsLeapYear(startDate.Year + policyYears))
            //{
            //    if (DateTime.Compare(startDate.AddYears(policyYears), DateTime.Parse($"28-Feb{startDate.Year + policyYears}")) >= 0)
            //    {
            //        adjustDays--;
            //    }
            //}

            //return startDate.AddYears(policyYears).AddDays(-1).AddDays(adjustDays);
        }

        /// <summary>
        /// Gets the credential that a given person is currently recertifying, based on skill and credential type.
        /// </summary>
        public CredentialModel GetCredentialBeingRecertified(int naatinumber, int skillId, int credentialTypeId)
        {
            var credentialsResponse = _personService.GetPersonCredentials(naatinumber, false);

            // there is no direct link between the recertification credential request and the credential being recertified yet,
            // so we need to find it by a few identifying properties
            var credentials = credentialsResponse.Data
                .Where(x => x.SkillId == skillId &&
                            x.CredentialTypeId == credentialTypeId &&
                            x.RecertificationStatus == RecertificationStatus.BeingAssessed &&
                            x.TerminationDate == null // TFS219048 - added termination date
                       )
                .ToList();
            credentials.Requires(x => x.Count <= 1, $"Expected no more than one matching credential, but found {credentials.Count}");

            return credentials.SingleOrDefault();
        }
        public CredentialModel GetCredentialRecertifications(int naatinumber, int skillId, int credentialTypeId)
        {
            var credentialsResponse = _personService.GetPersonCredentials(naatinumber, false);

            // there is no direct link between the recertification credential request and the credential being recertified yet,
            // so we need to find it by a few identifying properties
            var credentials = credentialsResponse.Data
                .Where(x => x.SkillId == skillId && x.CredentialTypeId == credentialTypeId && x.TerminationDate == null) //TFS219048 - added termination date
                .ToList();
            credentials.Requires(x => x.Count <= 1, $"Expected no more than one matching credential, but found {credentials.Count}");

            return credentials.SingleOrDefault();
        }

        public GenericResponse<IEnumerable<InvoiceModel>> GetWorkflowApplicationActionInvoices(int actionId, int applicationId)
        {
            var serviceRequest = new GetActionInvoicesRequest { ActionId = actionId, ApplicationId = applicationId };

            return GetActionInvoices(serviceRequest);
        }

        public BusinessServiceResponse UpdateOutstandingRefunds(OutstandingRefundsRequestModel request)
        {
            if (!request.CreditNotes?.Any() ?? false)
            {
                throw new ArgumentException(nameof(request.CreditNotes));
            }

            var refundRequests = _applicationQueryService.GetOutstandingRefunds(
                new GetOutstandingRefundRequest { CreditNotes = request.CreditNotes });

            var errors = new List<string>();
            foreach (var refundRequest in refundRequests.Results)
            {
                if (refundRequest.OnPaymentCreatedSystemActionTypeId.HasValue)
                {
                    try
                    {
                        var applicationWizardModel = new ApplicationActionWizardModel
                        {
                            ApplicationId = refundRequest.CredentialApplicationId,
                            CredentialRequestId = refundRequest.CredentialRequestId,
                            ActionType = (int)refundRequest.OnPaymentCreatedSystemActionTypeId
                        };
                        PerformAction(applicationWizardModel);
                    }
                    catch (Exception ex)
                    {
                        var hex = new HandledException(
                            $"Error processing Refund {refundRequest.Id}: {ex.Message}",
                            ex.StackTrace,
                            "NCMS");
                        LoggingHelper.LogException(
                            hex,
                            "Error processing refund {refundId}, CreditNote: {creditNoteNumber}: {Message}",
                            refundRequest.Id,
                            refundRequest.CreditNoteNumber,
                            ex.Message);

                        var message = (ex as UserFriendlySamException)?.Message ??
                                      "Internal error. Please contact support.";
                        errors.Add(
                            string.Format(
                                Naati.Resources.Application.UpdateCreditNoteError,
                                refundRequest.CreditNoteNumber,
                                message));
                    }
                }
            }

            return new BusinessServiceResponse() { Errors = errors };
        }

        public GenericResponse<IEnumerable<int>> UpdateOutstandingInvoices(UpdateOutstandingInvoicesRequestModel request)
        {
            var getinvoicesRequest = new GetOutstandingInvoicesRequest
            {
                InvoiceNumber = request.InvoiceNumber
            };

            var errors = new List<string>();
            var updatedFees = new List<int>();

            var outstandingInvoicesResponse = _applicationQueryService.GetOutstandingInvoices(getinvoicesRequest);

            if (outstandingInvoicesResponse.CredentialWorkflowFees.Any())
            {
                // do some housework
                try
                {
                    var deletedInvoices = outstandingInvoicesResponse.CredentialWorkflowFees
                        .Where(x => x.Invoice?.Status == InvoiceStatus.Canceled && x.PaymentActionProcessedDate == null)
                        .ToList();

                    if (deletedInvoices.Any())
                    {
                        _applicationQueryService.DeleteWorkflowFees(new DeleteVoidedInvoicesRequest
                        {
                            CredentialWorkflowFees = deletedInvoices
                        });
                    }
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(new HandledException($"Error removing voided invoices from workflow fee table: {ex.Message}", ex.StackTrace, "NCMS"),
                        "Error removing voided invoices from workflow fee table");
                }



                foreach (var workflowFee in outstandingInvoicesResponse.CredentialWorkflowFees)
                {
                    if (workflowFee.Invoice == null || !(workflowFee.Invoice.Balance <= 0 || workflowFee.Invoice.Status == InvoiceStatus.Paid))
                    {
                        continue;
                    }

                    if (workflowFee.OnPaymentActionType.HasValue)
                    {
                        try
                        {
                            var applicationWizarModel = new ApplicationActionWizardModel();
                            applicationWizarModel.ApplicationId = workflowFee.CredentialApplicationId;
                            applicationWizarModel.CredentialRequestId = workflowFee.CredentialRequestId.GetValueOrDefault();
                            applicationWizarModel.ActionType = (int)workflowFee.OnPaymentActionType;
                            if (workflowFee.Invoice?.InvoiceNumber == request.InvoiceNumber)
                            {
                                applicationWizarModel.SetPaymentReference(request.PaymentReference, null, request.TransactionId, request.OrderNumber);
                            }
                            PerformAction(applicationWizarModel);
                        }
                        catch (Exception ex)
                        {
                            var hex = new HandledException($"Error processing fee {workflowFee.Id}: {ex.Message}", ex.StackTrace, "NCMS");
                            LoggingHelper.LogException(hex, "Error processing fee {WorkflowFeeId}: {Message}", workflowFee.Id, ex.Message);

                            var message = (ex as UserFriendlySamException)?.Message ?? "Internal error. Please contact support.";
                            errors.Add(string.Format(Naati.Resources.Application.UpdateInvoiceError, workflowFee.Invoice.InvoiceNumber, message));
                        }
                    }
                    updatedFees.Add(workflowFee.Id);
                }
            }

            _applicationQueryService.UpdateProcessedWorkflowFees(new UpdateProcessedWorkflowFeeRequest { Fees = updatedFees.Select(x => new ProcessFeeDto { CredentialWorkflowFeeId = x, Type = ProcessTypeName.Payment }) });

            var response = new GenericResponse<IEnumerable<int>> { Data = updatedFees, Errors = errors };
            return response;
        }

        public GenericResponse<IEnumerable<CredentialRequestSummarySearchResultModel>> SearchCredentialRequestSummary(CredentialRequestSummarySearchRequest request)
        {
            var getRequest = _autoMapperHelper.Mapper.Map<GetCredentialRequestSummarySearchRequest>(request);
            getRequest.Filters = request.Filter.ToFilterList<CredentialRequestSummarySearchCriteria, CredentialRequestSummaryFilterType>();
            var serviceReponse = _applicationQueryService.SearchCredentialRequestSummary(getRequest);

            var models = serviceReponse.Results.Select(MapCredentialRequestSummary).ToList();
            var response = new GenericResponse<IEnumerable<CredentialRequestSummarySearchResultModel>>(models);

            if (request.Take.HasValue && models.Count == request.Take.Value)
            {
                response.Warnings.Add($"Search result were limited to {request.Take.Value} records.");
            }

            if (request.Skip.HasValue)
            {
                response.Warnings.Add($"First {request.Skip.Value} records were skipped.");
            }

            return response;
        }

        public GenericResponse<CredentialRequestSummaryItemModel> GetCredentialRequestSummaryItem(CredentialRequestBulkActionRequest request)
        {
            var filters = new[]
            {
                new CredentialRequestSummarySearchCriteria { Filter = CredentialRequestSummaryFilterType.ApplicationTypeIntList,  Values = new []{ request.CredentialApplicationTypeId.ToString()}},
                new CredentialRequestSummarySearchCriteria { Filter = CredentialRequestSummaryFilterType.CredentialRequestStatusIntList,  Values = new []{ request.CredentialRequestStatusTypeId.ToString()}},
                new CredentialRequestSummarySearchCriteria { Filter = CredentialRequestSummaryFilterType.CredentialRequestTypeIntList,  Values = new []{ request.CredentialTypeId.ToString()}},
                new CredentialRequestSummarySearchCriteria { Filter = CredentialRequestSummaryFilterType.PreferredTestLocationIntList,  Values = new []{ request.TestLocationId.ToString()}},
                new CredentialRequestSummarySearchCriteria { Filter = CredentialRequestSummaryFilterType.SkillIntList,  Values = new []{ request.SkillId.ToString()}},
            };

            var serviceReponse = _applicationQueryService.SearchCredentialRequestSummary(new GetCredentialRequestSummarySearchRequest { Filters = filters, Take = 1 });
            var item = serviceReponse.Results.Select(MapCredentialRequestSummaryItem).FirstOrDefault();
            if (item != null)
            {
                item.Applicants = GetCredentialRequestApplicants(request);
            }
            var response = item;
            return response;

        }

        public GenericResponse<bool> UpdateOnHoldCredentialToOnHoldToBeIssued(OnHoldCredential onHoldCredential)
        {
            if (onHoldCredential.CredentialRequestId == 0)
            {
                return new GenericResponse<bool>(false)
                {
                    Success = false,
                    Errors = new List<string>() { $"Credential request id was 0 when trying to update on hold credential to on hold to be issued." }
                };
            }

            var response = _applicationQueryService.UpdateOnHoldCredentialToOnHoldToBeIssued(onHoldCredential);

            if (!response.Success)
            {
                return new GenericResponse<bool>(response.Data)
                {
                    Success = response.Success,
                    Errors = response.Errors
                };
            }

            return new GenericResponse<bool>(true);
        }

        private IEnumerable<CredentialRequestApplicantModel> GetCredentialRequestApplicants(CredentialRequestBulkActionRequest request)
        {
            var testSessionService = ServiceLocator.Resolve<ITestSessionService>();
            var applicants = request.TestSessionId != 0 ? testSessionService.GetTestSessionById(request.TestSessionId).Data.Applicants.ToList() : new List<TestSessionApplicantModel>();

            var getRequest = _autoMapperHelper.Mapper.Map<CredentialRequestApplicantsRequest>(request);
            getRequest.SkillIds = new[] { request.SkillId };
            var result = _applicationQueryService.GetCredentialRequestApplicants(getRequest).Results;
            var mappedValues = result.Select(_autoMapperHelper.Mapper.Map<CredentialRequestApplicantModel>).ToList();
            mappedValues.ForEach(v => SetSessionApplicantStatus(applicants, v));
            return mappedValues;
        }

        private void SetSessionApplicantStatus(IEnumerable<TestSessionApplicantModel> applicants, CredentialRequestApplicantModel newApplicant)
        {
            newApplicant.Status = applicants.Any(a => a.CredentialRequestId == newApplicant.CredentialRequestId && a.Rejected)
                ? Naati.Resources.Shared.SessionRejected
                : newApplicant.Status;
        }

        private CredentialRequestSummarySearchResultModel MapCredentialRequestSummary(CredentialRequestSummarySearchDto credentialRequestSummarySearchDto)
        {
            return new CredentialRequestSummarySearchResultModel
            {
                CompositeId = credentialRequestSummarySearchDto.CredentialApplicationTypeId + "-" + credentialRequestSummarySearchDto.CredentialTypeId + "-" + credentialRequestSummarySearchDto.SkillId + "-" + credentialRequestSummarySearchDto.CredentialRequestStatusTypeId,
                DaysSinceSubmissionInt = Math.Round((DateTime.Now - credentialRequestSummarySearchDto.EnteredDate).TotalDays),
                CredentialApplicationTypeId = credentialRequestSummarySearchDto.CredentialApplicationTypeId,
                CredentialTypeId = credentialRequestSummarySearchDto.CredentialTypeId,
                SkillId = credentialRequestSummarySearchDto.SkillId,
                CredentialRequestStatusTypeId = credentialRequestSummarySearchDto.CredentialRequestStatusTypeId,
                DaysSinceSubmission = GetDaysSinceSubmission(credentialRequestSummarySearchDto.EnteredDate),
                ApplicationTypeName = credentialRequestSummarySearchDto.ApplicationType,
                CredentialTypeName = credentialRequestSummarySearchDto.CredentialType,
                Skill = GetSkill(credentialRequestSummarySearchDto.DirectionDisplayName, credentialRequestSummarySearchDto.Language1Name, credentialRequestSummarySearchDto.Language2Name),
                PreferredTestLocation = GetPreferredTestLocation(credentialRequestSummarySearchDto.PreferredTestLocation, credentialRequestSummarySearchDto.StateAbbr),
                NumberOfApplicants = credentialRequestSummarySearchDto.NumberOfApplicants,
                RequestStatus = credentialRequestSummarySearchDto.CredentialRequestStatus,
                ShowAllocateTestSession = credentialRequestSummarySearchDto.CredentialRequestStatus == CredentialRequestStatusType.TestAccepted.ToString(),
                EarliestApplication = credentialRequestSummarySearchDto.EarliestApplicationEnteredDate,
                LastApplication = credentialRequestSummarySearchDto.LastApplicationEnteredDate,
                TestLocationId = credentialRequestSummarySearchDto.TestLocationId
            };
        }

        private CredentialRequestSummaryItemModel MapCredentialRequestSummaryItem(CredentialRequestSummarySearchDto credentialRequestSummarySearchDto)
        {
            return new CredentialRequestSummaryItemModel
            {
                CompositeId = credentialRequestSummarySearchDto.CredentialApplicationTypeId + "-" + credentialRequestSummarySearchDto.CredentialTypeId + "-" + credentialRequestSummarySearchDto.SkillId + "-" + credentialRequestSummarySearchDto.CredentialRequestStatusTypeId,
                DaysSinceSubmissionInt = Math.Round((DateTime.Now - credentialRequestSummarySearchDto.EnteredDate).TotalDays),
                CredentialApplicationTypeId = credentialRequestSummarySearchDto.CredentialApplicationTypeId,
                CredentialTypeId = credentialRequestSummarySearchDto.CredentialTypeId,
                SkillId = credentialRequestSummarySearchDto.SkillId,
                CredentialRequestStatusTypeId = credentialRequestSummarySearchDto.CredentialRequestStatusTypeId,
                DaysSinceSubmission = GetDaysSinceSubmission(credentialRequestSummarySearchDto.EnteredDate),
                ApplicationTypeName = credentialRequestSummarySearchDto.ApplicationType,
                CredentialTypeName = credentialRequestSummarySearchDto.CredentialType,
                Skill = GetSkill(credentialRequestSummarySearchDto.DirectionDisplayName, credentialRequestSummarySearchDto.Language1Name, credentialRequestSummarySearchDto.Language2Name),
                PreferredTestLocation = GetPreferredTestLocation(credentialRequestSummarySearchDto.PreferredTestLocation, credentialRequestSummarySearchDto.StateAbbr),
                RequestStatus = credentialRequestSummarySearchDto.CredentialRequestStatus,
                ShowAllocateTestSession = credentialRequestSummarySearchDto.CredentialRequestStatus == CredentialRequestStatusType.TestAccepted.ToString(),
                EarliestApplication = credentialRequestSummarySearchDto.EarliestApplicationEnteredDate,
                LastApplication = credentialRequestSummarySearchDto.LastApplicationEnteredDate,
                TestLocationId = credentialRequestSummarySearchDto.TestLocationId
            };
        }


        private string GetPreferredTestLocation(string preferredTestLocation = null, string stateAbbr = null)
        {
            if (!string.IsNullOrEmpty(preferredTestLocation))
                return preferredTestLocation + ", " + stateAbbr;

            return string.Empty;
        }

        private string GetSkill(string directionDisplayName, string language1Name = null, string language2Name = null)
        {
            var skill = directionDisplayName.Replace("[Language 1]", language1Name);
            skill = skill.Replace("[Language 2]", language2Name);
            return skill;
        }

        private string GetDaysSinceSubmission(DateTime enteredDate)
        {
            var noOfDays = Math.Round((DateTime.Now - enteredDate).TotalDays);

            var displayDaysSinceSubmission = string.Empty;

            if (noOfDays > 30)
            {
                var month = Math.Round(noOfDays / 30);

                if (month == 1)
                {
                    displayDaysSinceSubmission = Math.Round(noOfDays) + " days " + "(1 Month)";
                }
                else if (month > 1)
                {
                    displayDaysSinceSubmission = Math.Round(noOfDays) + " days " + "(" + month + " Months)";
                }
            }
            else
            {
                if (noOfDays == 0)
                {
                    displayDaysSinceSubmission = "0 days (Submitted Today)";
                }
                else if (noOfDays == 1)
                {
                    displayDaysSinceSubmission = Math.Round(noOfDays) + " day";
                }
                else if (noOfDays > 1 && noOfDays < 30)
                {
                    displayDaysSinceSubmission = Math.Round(noOfDays) + " days";
                }
            }
            return displayDaysSinceSubmission;
        }

        public GenericResponse<IEnumerable<InvoiceModel>> GetWorkflowCredentialRequestActionInvoices(int actionId, int credentialRequestId)
        {
            var serviceRequest = new GetActionInvoicesRequest { ActionId = actionId, CredentialRequestId = credentialRequestId };
            return GetActionInvoices(serviceRequest);
        }

        private GenericResponse<IEnumerable<InvoiceModel>> GetActionInvoices(GetActionInvoicesRequest serviceRequest)
        {
            GetInvoicesResponse serviceResponse = null;
            var response = new GenericResponse<IEnumerable<InvoiceModel>>();

            serviceResponse = _applicationQueryService.GetActionInvoices(serviceRequest);

            if (!string.IsNullOrEmpty(serviceResponse.WarningMessage))
            {
                response.Warnings.Add(serviceResponse.WarningMessage);
            }

            if (!serviceResponse.Error)
            {
                response.Data = serviceResponse.Invoices.Select(_autoMapperHelper.Mapper.Map<InvoiceModel>);
            }
            else
            {
                response.Success = false;
                response.Errors.Add(serviceResponse.ErrorMessage);

                if (serviceResponse.ApiKeyError)
                {
                    response.Warnings.Add("Cannot retrieve invoices until Wiise API key is set correctly.");
                }
            }

            return response;
        }

        public bool HasAnyFee(int applicationTypeId)
        {
            var hasAnyFee = _applicationQueryService.HasAnyFee(applicationTypeId);
            return hasAnyFee;
        }

        public ProductSpecificationModel GetApplicationFee(int applicationId)
        {
            applicationId.Requires(x => x > 0, "ApplicationId not provided.");

            var application = GetApplicationDetails(applicationId);

            var feesResponse = _applicationQueryService.GetApplicationTypeFees(application.ApplicationType.Id, FeeTypeName.Application);

            if (feesResponse.FeeProducts == null || !feesResponse.FeeProducts.Any())
            {
                return null;
            }

            return GetProductSpecificationFee(feesResponse, application);
        }

        public ProductSpecificationModel GetApplicationAssessmentFee(int applicationId)
        {
            applicationId.Requires(x => x > 0, "ApplicationId not provided.");

            var application = GetApplicationDetails(applicationId);

            var feesResponse = _applicationQueryService.GetApplicationTypeFees(application.ApplicationType.Id, FeeTypeName.ApplicationAssessment);

            if (feesResponse.FeeProducts == null || !feesResponse.FeeProducts.Any())
            {
                return null;
            }

            return GetProductSpecificationFee(feesResponse, application);
        }

        private ProductSpecificationModel GetProductSpecificationFee(GetApplicationTypeFeesResponse feesResponse, CredentialApplicationDetailedModel application)
        {
            var credentiaFeeProduct = feesResponse.FeeProducts.Where(x => x.CredentialTypeId == null)
                                                     //.Select(x => x.ProductSpecification)
                                                     .OrderByDescending(x => x.ProductSpecification.CostPerUnit)
                                                     .FirstOrDefault();

            if (credentiaFeeProduct == null)
            {
                var credentialTypesInApplication = application.CredentialRequests
                    .Select(x => x.CredentialTypeId)
                    .ToList();

                var maxFee = feesResponse.FeeProducts.Where(x => x.CredentialTypeId.HasValue && credentialTypesInApplication.Contains(x.CredentialTypeId.Value))
                                                     .OrderByDescending(x => x.ProductSpecification.CostPerUnit)
                                                     .FirstOrDefault();

                if (maxFee == null)
                {
                    return null;
                }

                var maxFeeProductSpec = _autoMapperHelper.Mapper.Map<ProductSpecificationModel>(maxFee.ProductSpecification);
                maxFeeProductSpec.CredentialFeeProductId = maxFee.Id;

                return maxFeeProductSpec;
            }

            var credentiaFeeProductSpec = _autoMapperHelper.Mapper.Map<ProductSpecificationModel>(credentiaFeeProduct.ProductSpecification);
            credentiaFeeProductSpec.CredentialFeeProductId = credentiaFeeProduct.Id;

            return credentiaFeeProductSpec;
        }

        public ProductSpecificationModel GetSupplementaryTestFee(int applicationTypeId, int credentialTypeId)
        {
            applicationTypeId.Requires(x => x > 0, Naati.Resources.Application.ApplicationTypeIdValidation);
            credentialTypeId.Requires(x => x > 0, Naati.Resources.Application.CredentialTypeIdValidation);

            return _applicationBusinessLogicService.GetProductSpecificationFee(applicationTypeId, credentialTypeId, FeeTypeName.SupplementaryTest);
        }

        public ProductSpecificationModel GetPaidReviewFee(int applicationTypeId, int credentialTypeId)
        {
            applicationTypeId.Requires(x => x > 0, Naati.Resources.Application.ApplicationTypeIdValidation);
            credentialTypeId.Requires(x => x > 0, Naati.Resources.Application.CredentialTypeIdValidation);

            return _applicationBusinessLogicService.GetProductSpecificationFee(applicationTypeId, credentialTypeId, FeeTypeName.PaidTestReview);
        }

        public ProductSpecificationModel GetTestFee(int applicationTypeId, int credentialTypeId)
        {
            applicationTypeId.Requires(x => x > 0, Naati.Resources.Application.ApplicationTypeIdValidation);
            credentialTypeId.Requires(x => x > 0, Naati.Resources.Application.CredentialTypeIdValidation);

            return _applicationBusinessLogicService.GetProductSpecificationFee(applicationTypeId, credentialTypeId, FeeTypeName.Test);
        }

        public void ProcessInvoiceFee(int feeId)
        {
            ProcessFees(new[] { new ProcessFeeDto() { CredentialWorkflowFeeId = feeId, Type = ProcessTypeName.Invoice } });
        }

        private void ProcessFees(IEnumerable<ProcessFeeDto> fees)
        {
            var request = new UpdateProcessedWorkflowFeeRequest
            {
                Fees = fees
            };
            _applicationQueryService.UpdateProcessedWorkflowFees(request);
        }

        public void ProcessPaymentFee(int feeId)
        {
            ProcessFees(new[] { new ProcessFeeDto() { CredentialWorkflowFeeId = feeId, Type = ProcessTypeName.Payment } });
        }

        public GetEmailTemplateResponse GetCredentialApplicationEmailTemplate(GetEmailTemplateRequest request)
        {
            return _applicationQueryService.GetEmailTemplate(request);
        }

        public void RemoveFee(int feeId)
        {
            var request = new RemoveWorkflowFeeRequest
            {
                CredentialWorkflowFeeIds = new[] { feeId }
            };

            _applicationQueryService.RemoveWorkFlowFees(request);
        }

        public IList<ValidationResultModel> ValidateNewCertificationPeriod(string practitionerNumber, DateTime periodStart, DateTime periodEnd)
        {
            var errors = new List<ValidationResultModel>();
            var systemValue = _systemService.GetSystemValue("CertificationPeriodRecertifyMonths");
            var recertifyMonths = int.Parse(systemValue);
            var periods = _personQueryService.GetCertificationPeriods(new GetCertificationPeriodsRequest
            {
                PractitionerNumber = practitionerNumber,
                CertificationPeriodStatus = new[] { CertificationPeriodStatus.Current, CertificationPeriodStatus.Future }
            }).Results.ToList();

            if (periodEnd <= periodStart)
            {
                errors.Add(
                    new ValidationResultModel
                    {
                        Field = "CertificationPeriodEnd",
                        Message = "Certification Period End must be later than Certification Period Start"
                    });
            }

            // UC-BackOffice-5021 BR21 VR4
            if (!periods.Any())
            {
                var minimumValidEnd = periodStart.AddMonths(recertifyMonths).AddDays(-1).Date;
                if (periodEnd.Date < minimumValidEnd)
                {
                    errors.Add(
                        new ValidationResultModel
                        {
                            Field = "CertificationPeriodEnd",
                            Message = $"Certification Period End must be at least {minimumValidEnd:dd/MM/yyyy} ({recertifyMonths} months later than Certification Period Start)"
                        });
                }
            }

            var currentPeriod = periods.LastOrDefault(x => x.StartDate.Date <= DateTime.Today && x.EndDate.Date >= DateTime.Today);
            if (currentPeriod != null)
            {
                var minimumValidEnd = currentPeriod.OriginalEndDate.AddMonths(recertifyMonths).AddDays(-1).Date;
                if (periodEnd.Date < minimumValidEnd)
                {
                    errors.Add(
                        new ValidationResultModel
                        {
                            Field = "CertificationPeriodEnd",
                            Message = $"Certification Period End must be at least {minimumValidEnd:dd/MM/yyyy} ({recertifyMonths} months later than the original end date of the current Certification Period)"
                        });
                }
            }

            return errors;
        }

        public GenericResponse<IEnumerable<CredentialModel>> CredentialsForRecertification(int applicationId, int naatiNumber)
        {
            var application = _applicationQueryService.GetApplicationDetails(new GetApplicationDetailsRequest { ApplicationId = applicationId });

            var credentials = GetCredentialsAvailableForRecertification(naatiNumber, application.ApplicationInfo.CertificationPeriodId).Where(x => x.TerminationDate == null || x.TerminationDate > DateTime.Now);
            var notAddedCredentials = ExcludeAlreadyAddedCredentials(credentials, application.CredentialRequests);

            return new GenericResponse<IEnumerable<CredentialModel>>(notAddedCredentials.Select(e => _autoMapperHelper.Mapper.Map<CredentialModel>(e)));
        }

        private IEnumerable<CredentialDto> ExcludeAlreadyAddedCredentials(IEnumerable<CredentialDto> credentials, IEnumerable<CredentialRequestDto> credentialRequests)
        {
            //Deleted, Rejected, Cancelled, or Request Withdrawn
            return credentials.Where(c => !credentialRequests.Any(cr =>
                cr.CategoryId == c.CategoryId
                && cr.SkillId == c.SkillId
                && cr.CredentialTypeId == c.CredentialTypeId
                && cr.StatusTypeId != (int)CredentialRequestStatusTypeName.Deleted
                && cr.StatusTypeId != (int)CredentialRequestStatusTypeName.Rejected
                && cr.StatusTypeId != (int)CredentialRequestStatusTypeName.Cancelled
                && cr.StatusTypeId != (int)CredentialRequestStatusTypeName.Withdrawn
            ));
        }

        private IEnumerable<CredentialDto> GetCredentialsAvailableForRecertification(int naatiNumber, int certificationPeriodId)
        {
            var credentials = _personQueryService.GetPersonCredentials(naatiNumber);
            var certficationPeriodCredentials = credentials.Data.Where(c => c.CertificationPeriod?.Id == certificationPeriodId);
            return certficationPeriodCredentials.Where(x =>
            {
                var status = _applicationBusinessLogicService.CalculateCredentialRecertificationStatus(x.Id);
                return status == RecertificationStatus.EligibleForNew ||
                       status == RecertificationStatus.EligibleForExisting;
            }).ToList();
        }

        public void MoveCredential(MoveCredentialModel model)
        {
            var request = _autoMapperHelper.Mapper.Map<MoveCredentialRequest>(model);
            request.UserId = _userService.Get().Id;
            _applicationQueryService.MoveCredential(request);
        }

        public IEnumerable<int> GetAvailableCredentialsToRecertify(int applicationId)
        {
            var credentialIds = _applicationQueryService.GetCertificationPeriodCredentialFromApplicationId(applicationId);
            var availableCredentialIds = new List<int>();
            foreach (var credentialId in credentialIds)
            {
                var credentialStatus =
                    _applicationBusinessLogicService.CalculateCredentialRecertificationStatus(credentialId);

                if (credentialStatus == RecertificationStatus.EligibleForExisting || credentialStatus == RecertificationStatus.EligibleForNew)
                {
                    availableCredentialIds.Add(credentialId);
                }
            }

            return availableCredentialIds;
        }

        public GenericResponse<BasicCertificationPeriodModel> GetCertificationPeriod(int applicationId)
        {
            var certificationPeriod = _applicationQueryService.GetCertificationPeriodDetailsByApplicationId(applicationId).CertificationPeriod;

            return new BasicCertificationPeriodModel()
                {
                    Id = certificationPeriod.Id,
                    StartDate = certificationPeriod.StartDate,
                    EndDate = certificationPeriod.EndDate,
                    OriginalEndDate = certificationPeriod.OriginalEndDate
                };
        }

        public void UpdateCredentialApplicationRefundRequest(RefundModel refundModel)
        {
            var refundDto = _autoMapperHelper.Mapper.Map<UpdateCredentialApplicationRefundRequest>(refundModel);

            _applicationQueryService.UpdateCredentialApplicationRefundRequest(refundDto);
        }

        public GenericResponse<IEnumerable<RefundRequestsGroupingModel>> GetApprovalPendingRefundRequests()
        {
            var refundRequests = _applicationQueryService.GetApprovalPendingRefundRequests().ToList();

            if (refundRequests.Any())
            {
                //get initial paid amount if not available.
                //added refundable amount for PayPal. This will mean all of them for the moment
                IList<ApprovalPendingRefundRequestDto> requestsWithNoPaidAmount = refundRequests.Where(refundRequest => (refundRequest.InitialPaidAmount == null || refundRequest.RefundableAmount == null)).ToList();
                var notNullPayments = refundRequests.Where(refundRequest => (refundRequest.InitialPaidAmount != null && refundRequest.RefundableAmount != null)).ToList();
                if (requestsWithNoPaidAmount.Any())
                {
                    requestsWithNoPaidAmount = SetInitialPaidAmount(requestsWithNoPaidAmount);
                }

                var validRefunds = notNullPayments.Concat(requestsWithNoPaidAmount.Where(x => x.InitialPaidAmount != null)).ToList();
                var result = new GenericResponse<IEnumerable<RefundRequestsGroupingModel>>(new List<RefundRequestsGroupingModel> {new RefundRequestsGroupingModel
                {
                    Items = validRefunds.Select(_autoMapperHelper.Mapper.Map<RefundApprovalModel>).ToList()
                }});
                return result;
            }
            return new GenericResponse<IEnumerable<RefundRequestsGroupingModel>>();
        }

        public GenericResponse<bool> ApproveRefundRequests(IEnumerable<RefundRequestsGroupingModel> items)
        {
            var approvedRequests = items.SelectMany(x => x.Items.Where(y => y.Approved));

            foreach (var request in approvedRequests)
            {
                try
                {
                    var workflowFee = _applicationQueryService.GetCredentialWrokflowFeeById(request.CredentialWorkflowFeeId);

                    var wizardModel = new ApplicationActionWizardModel()
                    {
                        ActionType = (int)SystemActionTypeName.ApproveRefund,
                        ApplicationId = workflowFee.CredentialApplicationId,
                        CredentialRequestId = workflowFee.CredentialRequestId.GetValueOrDefault()
                    };

                    wizardModel.SetRefundDetails(
                        request.RefundPercentage,
                        request.RefundMethodTypeId,
                        request.CredentialWorkflowFeeId,
                        request.RefundAmount,
                        null,
                        null);

                    PerformAction(wizardModel);
                }
                catch (UserFriendlySamException ex)
                {
                    var response = new GenericResponse<bool>(true);
                    response.Messages.Add(ex.Message);
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(ex);
                    var response = new GenericResponse<bool>(true);
                    response.Messages.Add(string.Format(Naati.Resources.MaterialRequest.ErrorProcessingRequest, request.CredentialWorkflowFeeId));
                }
            }

            return true;
        }

        private IList<ApprovalPendingRefundRequestDto> SetInitialPaidAmount(IList<ApprovalPendingRefundRequestDto> requestsWithNoPaidAmount)
        {
            var invoicesResponse = _financeService.GetInvoices(new GetInvoicesRequest
            {
                InvoiceNumber = requestsWithNoPaidAmount.Select(refundRequest => refundRequest.InvoiceNumber).ToArray()
            });

            var invoices = invoicesResponse.Invoices.ToDictionary(invoice => invoice.InvoiceNumber, invoice => invoice);

            foreach (var request in requestsWithNoPaidAmount)
            {
                if (invoices.TryGetValue(request.InvoiceNumber, out var invoice))
                {
                    request.InitialPaidTax = invoice.TotalTax;
                    request.InitialPaidAmount = invoice.Payment;
                    if (request.RefundPercentage == null)
                    {
                        request.RefundAmount = (decimal?)null;
                    }
                    else
                    {
                        var glCode = _systemService.GetSystemValue("PayPalGlCode");
                        var externalAccountId = _financeService.GetExternalAccountIdByCode(glCode);
                        request.RefundAmount = NcmsRefundCalculator.CalculatePaidAmount(invoice, externalAccountId) * (decimal)request.RefundPercentage.Value;
                        request.RefundableAmount = NcmsRefundCalculator.CalculatePaidAmount(invoice, externalAccountId);
                    }
                    _applicationQueryService.UpdateCredentialApplicationRefundRequest(_autoMapperHelper.Mapper.Map<UpdateCredentialApplicationRefundRequest>(request));
                }
                else
                {
                    LoggingHelper.LogWarning("Invoice {invoiceNumber} was not found in Wiise", request.InvoiceNumber);
                }
            }

            return requestsWithNoPaidAmount;
        }

        public GenericResponse<IEnumerable<PaymentMethodTypeModel>> GetPaymentMethodTypes()
        {
            return new GenericResponse<IEnumerable<PaymentMethodTypeModel>>(_applicationQueryService.GetPaymentMethodTypes());
        }

        public GenericResponse<PrerequisiteSummaryResult> GetPrerequisiteSummary(int credentialRequestId)
        {

            return _credentialPrerequisiteService.GetPreReqsForCredRequest(credentialRequestId);
        }

        public GenericResponse<PrerequisiteApplicationsResult> GetPrerequisiteApplications(int applicationId)
        {
            return _prerequisiteApplicationDalService.GetPrerequisiteApplications(applicationId);
        }

        public GenericResponse<List<PrerequisiteApplicationsNullableApplicationsModel>> GetPrerequisiteApplicationsNullableApplications(int credentialRequestId)
        {
            var response = _prerequisiteApplicationDalService.GetPrerequisiteApplicationsNullableApplications(credentialRequestId);

            if (!response.Success)
            {
                return new GenericResponse<List<PrerequisiteApplicationsNullableApplicationsModel>>(null)
                {
                    Success = false,
                    Errors = response.Errors
                };
            }

            var prerequisiteApplicationsNullableApplicationsModels = new List<PrerequisiteApplicationsNullableApplicationsModel>();

            var prerequisiteApplicationsNullableApplicationsDtos = response.Data.PrerequisiteApplicationsNullableApplicationsDtos;

            foreach (var prerequisiteApplicationsNullableApplicationsDto in prerequisiteApplicationsNullableApplicationsDtos)
            {
                prerequisiteApplicationsNullableApplicationsModels.Add(
                    new PrerequisiteApplicationsNullableApplicationsModel()
                    {
                        PrerequisiteCredentialName = prerequisiteApplicationsNullableApplicationsDto.PrerequisiteCredentialName,
                        PrerequisiteCredentialLanguage1 = prerequisiteApplicationsNullableApplicationsDto.PrerequisiteCredentialLanguage1,
                        PrerequisiteCredentialLanguage2 = prerequisiteApplicationsNullableApplicationsDto.PrerequisiteCredentialLanguage2,
                        PrerequisiteSkillMatch = prerequisiteApplicationsNullableApplicationsDto.PrerequisiteSkillMatch,
                        PrerequistieDirection = prerequisiteApplicationsNullableApplicationsDto.PrerequistieDirection,
                        ExistingApplicationId = prerequisiteApplicationsNullableApplicationsDto.ExistingApplicationId,
                        ExistingApplicationStatusName = prerequisiteApplicationsNullableApplicationsDto.ExistingApplicationStatusName,
                        ExistingApplicationTypeName = prerequisiteApplicationsNullableApplicationsDto.ExistingApplicationTypeName,
                        MatchingCredentialName = prerequisiteApplicationsNullableApplicationsDto.MatchingCredentialName,
                        MatchingCredentialLanguage1 = prerequisiteApplicationsNullableApplicationsDto.MatchingCredentialLanguage1,
                        MatchingCredentialLanguage2 = prerequisiteApplicationsNullableApplicationsDto.MatchingCredentialLanguage2,
                        MatchingCredentialStatusTypeId = prerequisiteApplicationsNullableApplicationsDto.MatchingCredentialStatusTypeId,
                        MatchingCredentialStatusName = prerequisiteApplicationsNullableApplicationsDto.MatchingCredentialStatusName,
                        MatchingCredentialDirection = prerequisiteApplicationsNullableApplicationsDto.MatchingCredentialDirection,
                        ApplicationTypePrerequisiteId = prerequisiteApplicationsNullableApplicationsDto.ApplicationTypePrerequisiteId,
                        PrerequisiteSkillId = prerequisiteApplicationsNullableApplicationsDto.PrerequisiteSkillId
                    }
                );
            }

            return prerequisiteApplicationsNullableApplicationsModels;
        }

        public GenericResponse<bool> CreateApplicationNotesForOnHoldToBeIssued(ApplicationNoteModel noteModel)
        {
            var noteDto = new NoteDto()
            {
                NoteId = noteModel.NoteId.HasValue ? noteModel.NoteId.Value : 0,
                Description = noteModel.Note,
                CreatedDate = noteModel.CreatedDate.HasValue ? noteModel.CreatedDate.Value : DateTime.Now,
                ModifiedDate = noteModel.ModifiedDate.HasValue ? noteModel.ModifiedDate.Value : DateTime.Now,
                Highlight = noteModel.Highlight,
                ReadOnly = noteModel.ReadOnly,
                UserId = noteModel.UserId
            };

            var applicationNoteDto = new ApplicationNoteDto()
            {
                CredentialApplicationId = noteModel.ApplicationId,
                Note = noteDto
            };

            var createApplicationNoteResponse = _applicationQueryService.CreateNotesForOnHoldToBeIssued(applicationNoteDto);

            if (!createApplicationNoteResponse.Success)
            {
                return new GenericResponse<bool>(false)
                {
                    Success = false,
                    Errors = createApplicationNoteResponse.Errors
                };
            }

            return true;
        }
    }
}
