using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.ExtensionHelpers;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.RolePlayerActions;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.CredentialRequest;
using Ncms.Contracts.Models.RolePlayer;
using Ncms.Contracts.Models.System;
using Ncms.Contracts.Models.Test;
using IApplicationService = Ncms.Contracts.IApplicationService;
using TestTaskModel = Ncms.Contracts.TestTaskModel;
using IUserService = Ncms.Contracts.IUserService;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Security;
using System.Threading;

namespace Ncms.Bl
{
    public class TestSessionService : ITestSessionService
    {
        private IServiceLocator _serviceLocatorInstance { get; set; }
        private readonly ITestSessionQueryService _testSessionQueryService;
        private readonly IApplicationQueryService _applicationQueryService;
        private readonly ITestMaterialQueryService _testMaterialQueryService;
        private readonly ICredentialQueryService _credentialQueryService;
        private readonly ITestSpecificationQueryService _testSpecificationQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly INoteService _noteService; 

        private IUserService _userService => _serviceLocatorInstance.Resolve<IUserService>();

        public TestSessionService(ITestSessionQueryService testSessionQueryService,
            IApplicationQueryService applicationQueryService,
            ITestMaterialQueryService testMaterialQueryService,
            ICredentialQueryService credentialQueryService,
            ITestSpecificationQueryService testSpecificationQueryService, 
            IAutoMapperHelper autoMapperHelper,
            INoteService noteService)
        {
            _testSessionQueryService = testSessionQueryService;
            _applicationQueryService = applicationQueryService;
            _testMaterialQueryService = testMaterialQueryService;
            _credentialQueryService = credentialQueryService;
            _testSpecificationQueryService = testSpecificationQueryService;
            _autoMapperHelper = autoMapperHelper;
            _noteService = noteService; 

            _serviceLocatorInstance = ServiceLocator.GetInstance();
        }

        public GenericResponse<bool> CheckCapacityTestSession(int credentialTypeId,
            int skillId,
            int testSessionId,
            int credentialStatusTypeId,
            int preferredTestLocationId)
        {
            return _testSessionQueryService.CheckCapacityTestSession(credentialTypeId, skillId, testSessionId, credentialStatusTypeId, preferredTestLocationId);
        }

        private ValidationMessage CreateValidationMessage(int issueNumber, string description, IList<string> details)
        {
            var maxDetails = 5;
            return new ValidationMessage
            {
                IssueNumber = issueNumber,
                Message = description,
                Details = details.Take(maxDetails),
                TotalFound = details.Count
            };
        }

        public GenericResponse<IEnumerable<ValidationMessage>> ValidatestTestSitting(int testSessionId)
        {
            var issues = new List<ValidationMessage>();

            var alreadySatApplicants = _testMaterialQueryService
                .GetApplicantWithAlreadySatMaterialsForTestSession(testSessionId)
                .ToList();
            if (alreadySatApplicants.Any())
            {
                var details = alreadySatApplicants.Select(x =>
                    {
                        var testMaterials = string.Join(", ", x.ConflictingTestMaterialsIds.Select(z => "#" + z));
                        LoggingHelper.LogError(
                            $"Customer {{NaatiNumber}} has following Test Materials which have sat in the past {testMaterials}",
                            x.NaatiNumber);
                        return $"{x.NaatiNumber} - {Naati.Resources.TestMaterial.TestMaterials + " " + testMaterials}";
                    })
                    .ToList();

                issues.Add(CreateValidationMessage(1, Naati.Resources.TestSession.ValidationIssue1Description, details));

            }

            var testSittings = _testSessionQueryService.GetApplicantsById(testSessionId, false).Results.ToList();

            var applicantsWithTaskWithoutMaterial = testSittings
                .Where(x => x.TestTasks.Any(y => !y.TestMaterialId.HasValue))
                .ToList();

            if (applicantsWithTaskWithoutMaterial.Any())
            {
                var details = applicantsWithTaskWithoutMaterial
                    .Select(
                        x =>
                            $"{x.CustomerNo} - {Naati.Resources.TestSession.TestTasks + " " + string.Join(", ", x.TestTasks.Where(y => !y.TestMaterialId.HasValue).Select(z => z.TestComponentTypeLabel + z.Label))}")
                    .ToList();
                issues.Add(CreateValidationMessage(2, Naati.Resources.TestSession.ValidationIssue2Description, details));
            }

            var applicantWithTaskWithoutAttachment = testSittings
                .Where(x => x.TestTasks.Any(y => y.TestMaterialId.HasValue && !y.HasTestMaterialAttachment))
                .ToList();

            if (applicantWithTaskWithoutAttachment.Any())
            {
                var details = applicantWithTaskWithoutAttachment
                    .Select(
                        x =>
                            $"{x.CustomerNo} - {string.Join(", ", x.TestTasks.Where(y => y.TestMaterialId.HasValue && !y.HasTestMaterialAttachment).Select(z => Naati.Resources.TestSession.TestTask + " " + z.TestComponentTypeLabel + z.Label + " - #" + z.TestMaterialId))}")
                    .ToList();
                issues.Add(CreateValidationMessage(3, Naati.Resources.TestSession.ValidationIssue3Description, details));
            }

            var sameApplicantInSameTestSession = testSittings.GroupBy(x => x.CustomerNo)
                .Where(y => y.Count() > 1)
                .ToList();

            if (sameApplicantInSameTestSession.Any())
            {
                var details = sameApplicantInSameTestSession.Select(x => $"{x.Key}").ToList();
                issues.Add(CreateValidationMessage(4, Naati.Resources.TestSession.ValidationIssue4Description, details));
            }

            var applicantsWithTestSessionsOnSame = _testMaterialQueryService
                .GetPeopleWithOtherTestSittingAssingnedForTheSameDay(testSessionId)
                .ToList();
            if (applicantsWithTestSessionsOnSame.Any())
            {
                var details = applicantsWithTestSessionsOnSame.GroupBy(x => x.NaatiNumber)
                    .Select(x => $"{x.Key} - {string.Join(", ", x.Select(z => "TS" + z.TestSessionId))}")
                    .ToList();
                issues.Add(CreateValidationMessage(5, Naati.Resources.TestSession.ValidationIssue5Description, details));
            }

            var materialTypeDifferentToTaskType = testSittings
                .Where(x => x.TestTasks.Any(y => y.TestMaterialId.HasValue &&
                                                 y.TaskComponentTypeId != y.MaterialComponentTypeId))
                .ToList();

            if (materialTypeDifferentToTaskType.Any())
            {
                var details = materialTypeDifferentToTaskType.Select(x =>
                    {
                        var testMaterials = string.Join(", ",
                            x.TestTasks.Where(y => y.TestMaterialId.HasValue &&
                                                   y.TaskComponentTypeId != y.MaterialComponentTypeId)
                                .Select(z => "#" + z.TestMaterialId));
                        LoggingHelper.LogWarning(
                            $"Customer {{NaatiNumber}} has following Test Materials which have sat in the past {testMaterials}",
                            x.CustomerNo);
                        return $"{x.CustomerNo} - {Naati.Resources.TestMaterial.TestMaterials + " " + testMaterials}";

                    })
                    .ToList();
                issues.Add(CreateValidationMessage(6, Naati.Resources.TestSession.ValidationIssue6Description, details));
            }

            var testSittingWithRepeatedTestMaterials = testSittings.Where(x => x.TestTasks
                    .Where(y => y.TestMaterialId.HasValue)
                    .GroupBy(y => y.TestMaterialId)
                    .Any(z => z.Count() > 1))
                .ToList();

            if (testSittingWithRepeatedTestMaterials.Any())
            {
                var details = testSittingWithRepeatedTestMaterials
                    .Select(
                        x =>
                            $"{x.CustomerNo} - {Naati.Resources.TestMaterial.TestMaterials + " " + string.Join(", ", x.TestTasks.Where(y => y.TestMaterialId.HasValue).GroupBy(w => w.TestMaterialId).Where(tm => tm.Count() > 1).Select(z => "#" + z.Key))}")
                    .ToList();
                issues.Add(CreateValidationMessage(7, Naati.Resources.TestSession.ValidationIssue7Description, details));
            }

            return new GenericResponse<IEnumerable<ValidationMessage>>(issues);
        }

        public GenericResponse<IEnumerable<TestSessionSearchResultModel>> List(TestSessionSearchRequest request)
        {
            var getRequest = new GetTestSessionSearchRequest
            {
                Take = request.Take,
                Skip = request.Skip,
                Filters = request.Filter.ToFilterList<TestSessionSearchCriteria, TestSessionFilterType>()
            };

            var result = _testSessionQueryService.Search(getRequest);
            var models = result.TestSessions.Select(_autoMapperHelper.Mapper.Map<TestSessionSearchResultModel>).ToList();

            var response = new GenericResponse<IEnumerable<TestSessionSearchResultModel>>(models);

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

        public GenericResponse<TestSessionDetails> GetTestSessionDetailsById(int testSessionId)
        {
            GetTestSessionDetailsResponse serviceReponse = null;
            serviceReponse = _testSessionQueryService.GetTestSessionDetailsById(testSessionId);

            var model = MapTestSessionDetailsResponse(serviceReponse.Result);
            return model;
        }

        public GenericResponse<GetTestSessionSkillsResponse> GetSkillAttendeesCount(int testSessionId)
        {
            return _testSessionQueryService.GetSkillAttendeesCount(testSessionId);
        }

        public GenericResponse<string> MarkAsCompleteTestSession(int testSessionId)
        {
            var markIncompleteRequirement =
                _testSessionQueryService.AllTestSittingsMarkCompleteRequirements(testSessionId);

            var hasPendingRolePlayers = _testSessionQueryService.GetTestSessionRolePlayerBySessionId(testSessionId)
                .Data.Any(x => x.RolePlayerStatusId != (int)RolePlayerStatusTypeName.Rehearsed &&
                               x.RolePlayerStatusId != (int)RolePlayerStatusTypeName.Attended &&
                               x.RolePlayerStatusId != (int)RolePlayerStatusTypeName.NoShow);
            var model = string.Empty;

            if (markIncompleteRequirement == "NotSit")
            {
                model = "ApplicantNotSit";
            }
            else if (markIncompleteRequirement == "TestMaterialNotAssigned")
            {
                model = "ApplicantTestMaterialNotAssigned";
            }

            if (hasPendingRolePlayers)
            {
                throw new UserFriendlySamException(Naati.Resources.TestSession.PengingRolePlayersToResolve);
            }

            if (string.IsNullOrEmpty(markIncompleteRequirement))
            {
                _testSessionQueryService.MarkAsCompleteTestSession(testSessionId);
            }

            return model;
        }

        public void ReopenTestSession(int testSessionId)
        {
            _testSessionQueryService.ReopenTestSession(testSessionId);
        }

        public GenericResponse<IEnumerable<ApplicantModel>> GetApplicantsByTestSessionId(int testSessionId,
            bool includeRejected)
        {
            GetTestSessionApplicantsResponse serviceReponse = null;
            serviceReponse = _testSessionQueryService.GetApplicantsById(testSessionId, includeRejected);

            var models = serviceReponse.Results.Select(_autoMapperHelper.Mapper.Map<ApplicantModel>).ToList();

            return new GenericResponse<IEnumerable<ApplicantModel>>(models);
        }

        public GenericResponse<TestSessionModel> GetTestSessionById(int testSessionId)
        {
            var response = _testSessionQueryService.GetTestSessionById(testSessionId);

            var model = _autoMapperHelper.Mapper.Map<TestSessionModel>(response.Result);
            model.TestTime = response.Result.TestDate.ToString("hh:mm tt");
            model.RehearsalDate = response.Result.RehearsalDateTime;
            model.RehearsalTime = response.Result.RehearsalDateTime.GetValueOrDefault().ToString("hh:mm tt");
            model.Applicants = response.Result.TestSessionApplicants
                .Select(_autoMapperHelper.Mapper.Map<TestSessionApplicantModel>)
                .ToList();

            return model;
        }

        public void UpdateTestSession(TestSessionModel testSessionModel)
        {
            if (testSessionModel.TestDate < MinDate.Value)
            {
                throw new UserFriendlySamException("Test date is invalid.");
            }

            var request = MapTestSessionModel(testSessionModel);

            try
            {
                _testSessionQueryService.UpdateTestSession(request);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }

        public GenericResponse<IEnumerable<TestSessionItemModel>> GetActiveTestSessions(
            CredentialRequestBulkActionRequest request)
        {
            var result = _testSessionQueryService.GetActiveTestSessions(new GetActiveTestSessionRequest
            {
                CredentialTypeId = request.CredentialTypeId,
                TestLocationId = request.TestLocationId
            });
            var models = result.Result.Select(MapTestSession).ToList();
            return new GenericResponse<IEnumerable<TestSessionItemModel>>(models);
        }

        private TestSessionItemModel MapTestSession(TestSessionBasicDto dto)
        {
            return new TestSessionItemModel
            {
                Id = dto.TestSessionId,
                Session = dto.Name,
                TestDate = dto.TestDate,
                Venue = dto.VenueName,
                Capacity = dto.OverrideCapacity ? dto.OverridenCapacity.GetValueOrDefault() : dto.VenueCapacity,
                PreparationTime = dto.ArrivalTime,
                SessionDuration = dto.Duration,
                Notes = dto.PublicNotes,
                Skills = dto.Skills,
                TotalApplicants = dto.TotalApplicants
            };
        }

        public GenericResponse<int> CreateOrUpdateTestSession(TestSessionRequestModel testSessionModel)
        {
            var dateAndTime = testSessionModel.TestDate.Date().GetValueOrDefault();
            dateAndTime = dateAndTime.AddHours(int.Parse(testSessionModel.TestTime.Substring(0, 2)));
            dateAndTime = dateAndTime.AddMinutes(int.Parse(testSessionModel.TestTime.Substring(3, 2)));

            DateTime? rehearsalDateAndTime = null;
            if (testSessionModel.TestDate != null && !string.IsNullOrWhiteSpace(testSessionModel.RehearsalTime))
            {
                rehearsalDateAndTime = testSessionModel.RehearsalDate.Date().GetValueOrDefault();
                rehearsalDateAndTime =
                    rehearsalDateAndTime.Value.AddHours(int.Parse(testSessionModel.RehearsalTime.Substring(0, 2)));
                rehearsalDateAndTime =
                    rehearsalDateAndTime.Value.AddMinutes(int.Parse(testSessionModel.RehearsalTime.Substring(3, 2)));
            }

            if (testSessionModel.DefaultTestSpecificationId == 0)
            {
                throw new Exception($"{nameof(testSessionModel.DefaultTestSpecificationId)} not specified");
            }

            //The allowNotify for the testsession will be set to true if the credential type allows notification and the test session allows self assign
            //and the test is new or has a new date.
            bool allowNotify = false;
            bool credentialTypeAllowNotify = false;

            var credentialType = _credentialQueryService.GetCredentialType(testSessionModel.CredentialTypeId).Data;
            if (credentialType.AllowAvailabilityNotice)
            {
                credentialTypeAllowNotify = true;
            }

            if (testSessionModel.AllowSelfAssign && credentialTypeAllowNotify)
            {
                if (testSessionModel.Id == null || testSessionModel.Id == 0)
                {
                    allowNotify = true;
                }
                else
                {
                    var existingDate = _testSessionQueryService.GetTestSessionById(testSessionModel.Id.Value).Result.TestDate;

                    if (testSessionModel.TestDate != existingDate)
                    {
                        allowNotify = true;
                    }
                }
            }

            var request = new CreateOrUpdateTestSessionRequest
            {
                Id = testSessionModel.Id.GetValueOrDefault(),
                Name = testSessionModel.SessionName,
                ArrivalTime = testSessionModel.PreparationTime,
                Duration = testSessionModel.SessionDuration,
                CredentialApplicationTypeId = testSessionModel.CredentialApplicationTypeId,
                CredentialTypeId = testSessionModel.CredentialTypeId,
                Completed = testSessionModel.Completed.GetValueOrDefault(),
                PublicNote = testSessionModel.Notes,
                TestDateTime = dateAndTime,
                VenueId = testSessionModel.VenueId.GetValueOrDefault(),
                AllowSelfAssign = testSessionModel.AllowSelfAssign,
                OverrideVenueCapacity = testSessionModel.OverrideVenueCapacity,
                Capacity = testSessionModel.Capacity,
                Skills = testSessionModel.Skills.Select(x => new TestSessionSkillDto
                {
                    Id = x.Id,
                    Capacity = x.MaximumCapacity,
                    SkillId = x.SkillId,
                    TestSessionId = testSessionModel.Id.GetValueOrDefault()
                })
                    .ToArray(),
                RehearsalDateTime = rehearsalDateAndTime,
                RehearsalNotes = testSessionModel.RehearsalNotes,
                DefaultTestSpecificationId = testSessionModel.DefaultTestSpecificationId,
                AllowAvailabilityNotice = allowNotify,
                NewCandidatesOnly = testSessionModel.NewCandidatesOnly,
                IsActive = testSessionModel.IsActive,
            };

            var result = _testSessionQueryService.CreateOrUpdateTestSession(request);

            return result.TestSessionId;
        }

        public GenericResponse<IEnumerable<TestSessionSkillModel>> GetTestSessionSkills(int testSessionId,
            int credentialTypeId)
        {
            var testSessionSkillsBySkillId = _testSessionQueryService.GetTestSessionSkillsForTestSession(testSessionId)
                .Results.ToDictionary(x => x.SkillId, y => y);
            ;

            var credentialTypeSkills = _applicationQueryService.GetCredentialTypeSkills(
                new GetCredentialTypeSkillsRequest { CredentialTypeIds = new[] { credentialTypeId } });

            var models = credentialTypeSkills.Results.Select(x =>
                {
                    TestSessionSkillDto skill;
                    if (!testSessionSkillsBySkillId.TryGetValue(x.Id, out skill))
                    {
                        return new TestSessionSkillModel { SkillId = x.Id, Name = x.DisplayName };
                    }

                    return MapTestSessionSkillDto(skill);
                })
                .ToList();

            return new GenericResponse<IEnumerable<TestSessionSkillModel>>(models);
        }

        public GenericResponse<IEnumerable<TestSessionSkillModel>> GetSelectedTestSessionSkillDetails(int testSessionId)
        {
            var testSessionSkills = _testSessionQueryService.GetTestSessionSkillsForTestSession(testSessionId)
                .Results.Select(x => new TestSessionSkillModel()
                {
                    Name = x.Name,
                    MaximumCapacity = x.Capacity
                });

            return new GenericResponse<IEnumerable<TestSessionSkillModel>>(testSessionSkills);

        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetSelectedTestSessionSkills(int testSessionId)
        {
            var testSessionSkills = _testSessionQueryService.GetTestSessionSkillsForTestSession(testSessionId)
                .Results.Select(x => new LookupTypeModel()
                {
                    Id = x.SkillId,
                    DisplayName = x.Name
                });

            return new GenericResponse<IEnumerable<LookupTypeModel>>(testSessionSkills);

        }

        private TestSessionSkillModel MapTestSessionSkillDto(TestSessionSkillDto dto)
        {
            return new TestSessionSkillModel
            {
                Id = dto.Id,
                MaximumCapacity = dto.Capacity,
                Selected = true,
                SkillId = dto.SkillId,
                TestSessionId = dto.TestSessionId,
                Name = dto.Name
            };
        }

        public GenericResponse<IEnumerable<object>> ValidateTestSession(TestSessionRequestModel testSessionModel)
        {
            var errors = new List<object>();
            ValidateNullValue(nameof(testSessionModel.SessionName), testSessionModel.SessionName, errors);
            ValidateNullValue(nameof(testSessionModel.TestDate), testSessionModel.TestDate, errors);
            ValidateNullValue(nameof(testSessionModel.TestTime), testSessionModel.TestTime, errors);
            ValidateNullValue(nameof(testSessionModel.SessionDuration), testSessionModel.SessionDuration, errors);
            ValidateNullValue(nameof(testSessionModel.VenueId), testSessionModel.VenueId, errors);
            ValidateNullValue(nameof(testSessionModel.DefaultTestSpecificationId),
                testSessionModel.DefaultTestSpecificationId, errors);

            if (string.IsNullOrWhiteSpace(testSessionModel.SessionName))
            {
                AddError(nameof(testSessionModel.SessionName), Naati.Resources.Shared.RequiredFieldValidationError, errors);
            }
            if (errors.Any())
            {
                return new GenericResponse<IEnumerable<object>>(errors);
            }

            var date = testSessionModel.TestDate.GetValueOrDefault();
            date = date.AddHours(int.Parse(testSessionModel.TestTime.Substring(0, 2)));
            date = date.AddMinutes(int.Parse(testSessionModel.TestTime.Substring(3, 2)));
            if (date < DateTime.Now)
            {
                AddError(nameof(testSessionModel.TestDate), Naati.Resources.Shared.DateMustBeLaterThanToday, errors);
            }

            var minPreprartionTime = 0;
            var maxPreparationTime = 180;
            if (testSessionModel.PreparationTime.GetValueOrDefault() < minPreprartionTime ||
                testSessionModel.PreparationTime.GetValueOrDefault() > maxPreparationTime)
            {
                AddError(nameof(testSessionModel.PreparationTime),
                    string.Format(Naati.Resources.Shared.InvalidPreparationTime, minPreprartionTime, maxPreparationTime),
                    errors);
            }

            var minSessionTime = 15;
            var maxSessionTime = 480;
            if (testSessionModel.SessionDuration.GetValueOrDefault() < minSessionTime ||
                testSessionModel.SessionDuration.GetValueOrDefault() > maxSessionTime)
            {
                AddError(nameof(testSessionModel.SessionDuration),
                    string.Format(Naati.Resources.Shared.InvalidSessionDuration, minSessionTime, maxSessionTime), errors);
            }

            var maxSessionNameLength = 100;
            if (!string.IsNullOrEmpty(testSessionModel.SessionName) &&
                testSessionModel.SessionName.Length > maxSessionNameLength)
            {
                AddError(nameof(testSessionModel.SessionName),
                    string.Format(Naati.Resources.Shared.InvalidTestSessionNameLength, maxSessionNameLength), errors);
            }

            var maxNotesLength = 1000;
            if (!string.IsNullOrEmpty(testSessionModel.Notes) && testSessionModel.Notes.Length > maxNotesLength)
            {
                AddError(nameof(testSessionModel.Notes),
                    string.Format(Naati.Resources.Shared.InvalidTestSessionNotesLength, maxNotesLength), errors);
            }

            if (testSessionModel.Id.GetValueOrDefault() > 0)
            {
                var testSession = _testSessionQueryService
                    .GetTestSessionById(testSessionModel.Id.GetValueOrDefault())
                    .Result;

                var testSittings = _testSessionQueryService.GetApplicantsById(testSession.TestSessionId, false).Results.ToList();

                if (testSession.DefaultTestSpecificationId != testSessionModel.DefaultTestSpecificationId && testSession.HasRolePlayers)
                {
                    AddError(nameof(testSessionModel.DefaultTestSpecificationId), Naati.Resources.Shared.SessionHasRolePlayers, errors);
                }

                var materialsAssigned = testSittings.Any(x => x.TestTasks.Any(y => y.TestMaterialId.HasValue));

                if (testSession.DefaultTestSpecificationId != testSessionModel.DefaultTestSpecificationId && materialsAssigned)
                {
                    AddError(nameof(testSessionModel.DefaultTestSpecificationId), Naati.Resources.Shared.MaterialsAssigned, errors);
                }

            }

            return new GenericResponse<IEnumerable<object>>(errors);
        }

        public GenericResponse<IEnumerable<object>> ValidateTestSession(TestSessionDetails testSessionModel)
        {
            var errors = new List<object>();
            ValidateNullValue(nameof(testSessionModel.Name), testSessionModel.Name, errors);
            ValidateNullValue(nameof(testSessionModel.TestDate), testSessionModel.TestDate, errors);
            ValidateNullValue(nameof(testSessionModel.TestTime), testSessionModel.TestTime, errors);
            ValidateNullValue(nameof(testSessionModel.SessionDuration), testSessionModel.SessionDuration, errors);
            ValidateZeroValue(nameof(testSessionModel.TestLocationId), testSessionModel.TestLocationId, errors);
            ValidateZeroValue(nameof(testSessionModel.VenueId), testSessionModel.VenueId.GetValueOrDefault(), errors);
            ValidateZeroValue(nameof(testSessionModel.CredentialTypeId), testSessionModel.CredentialTypeId, errors);
            ValidateZeroValue(nameof(testSessionModel.DefaultTestSpecificationId),
                testSessionModel.DefaultTestSpecificationId, errors);

            if (string.IsNullOrWhiteSpace(testSessionModel.Name))
            {
                AddError(nameof(testSessionModel.Name), Naati.Resources.Shared.RequiredFieldValidationError, errors);
            }
            if (errors.Any())
            {
                return new GenericResponse<IEnumerable<object>>(errors);
            }

            var credentialType = _credentialQueryService.GetCredentialType(testSessionModel.CredentialTypeId).Data;
            if (!credentialType.AllowBackdating)
            {
                var date = testSessionModel.TestDate.GetValueOrDefault();
                date = date.AddHours(int.Parse(testSessionModel.TestTime.Substring(0, 2)));
                date = date.AddMinutes(int.Parse(testSessionModel.TestTime.Substring(3, 2)));
                if (date < DateTime.Now.AddDays(-1))
                {
                    AddError(nameof(testSessionModel.TestDate), Naati.Resources.Shared.DateMustBeTodayorLater, errors);
                }
            }

            var minPreprartionTime = 0;
            var maxPreparationTime = 180;
            if (testSessionModel.PreparationTime.GetValueOrDefault() < minPreprartionTime ||
                testSessionModel.PreparationTime.GetValueOrDefault() > maxPreparationTime)
            {
                AddError(nameof(testSessionModel.PreparationTime),
                    string.Format(Naati.Resources.Shared.InvalidPreparationTime, minPreprartionTime, maxPreparationTime),
                    errors);
            }

            var minSessionTime = 15;
            var maxSessionTime = 480;
            if (testSessionModel.SessionDuration.GetValueOrDefault() < minSessionTime ||
                testSessionModel.SessionDuration.GetValueOrDefault() > maxSessionTime)
            {
                AddError(nameof(testSessionModel.SessionDuration),
                    string.Format(Naati.Resources.Shared.InvalidSessionDuration, minSessionTime, maxSessionTime), errors);
            }

            var maxSessionNameLength = 100;
            if (!string.IsNullOrEmpty(testSessionModel.Name) && testSessionModel.Name.Length > maxSessionNameLength)
            {
                AddError(nameof(testSessionModel.Name),
                    string.Format(Naati.Resources.Shared.InvalidTestSessionNameLength, maxSessionNameLength), errors);
            }

            var maxNotesLength = 1000;
            if (!string.IsNullOrEmpty(testSessionModel.PublicNote) &&
                testSessionModel.PublicNote.Length > maxNotesLength)
            {
                AddError(nameof(testSessionModel.PublicNote),
                    string.Format(Naati.Resources.Shared.InvalidTestSessionNotesLength, maxNotesLength), errors);
            }

            var testSpecifications = _testSpecificationQueryService.GetTestSpecificationByCredentialTypeId(
                new SpecificationByCredentialTypeRequest() { CredentialTypeId = testSessionModel.CredentialTypeId });

            var selectedTestSpecification = _testSpecificationQueryService.GetTestSpecificationById(testSessionModel.DefaultTestSpecificationId);

            if (!selectedTestSpecification.Active && testSessionModel.IsActive)
            {
                AddError(nameof(testSessionModel.DefaultTestSpecificationId), Naati.Resources.TestSession.InactiveTestSpecificationActiveSessionIssue, errors);
            }


            if (testSessionModel.Id > 0)
            {
                var testSession = _testSessionQueryService.GetTestSessionById(testSessionModel.Id.GetValueOrDefault()).Result;
                var hasTestSittings = testSession.TestSessionApplicants.Any(x => !x.Rejected);
                if (testSession.HasRolePlayers && testSessionModel.CredentialTypeId !=
                    testSession.CredentialTypeId)
                {
                    AddError(nameof(testSessionModel.CredentialTypeId), Naati.Resources.Shared.TestSessionHasRolePlayers,
                        errors);
                }
                if (hasTestSittings && testSessionModel.CredentialTypeId != testSession.CredentialTypeId)
                {
                    AddError(nameof(testSessionModel.CredentialTypeId), Naati.Resources.Shared.TestSessionHasTestSittings,
                        errors);
                }
                if (hasTestSittings && !testSessionModel.IsActive)
                {
                    AddError(nameof(testSessionModel.CredentialTypeId), Naati.Resources.Shared.TestSessionHasTestSittings,
                        errors);
                }
                var testSittings = _testSessionQueryService.GetApplicantsById(testSession.TestSessionId, false).Results.ToList();

                if (testSession.DefaultTestSpecificationId != testSessionModel.DefaultTestSpecificationId && testSession.HasRolePlayers)
                {
                    AddError(nameof(testSessionModel.DefaultTestSpecificationId), Naati.Resources.Shared.SessionHasRolePlayers, errors);
                }

                var materialsAssigned = testSittings.Any(x => x.TestTasks.Any(y => y.TestMaterialId.HasValue));

                if (testSession.DefaultTestSpecificationId != testSessionModel.DefaultTestSpecificationId && materialsAssigned)
                {
                    AddError(nameof(testSessionModel.DefaultTestSpecificationId), Naati.Resources.Shared.MaterialsAssigned, errors);
                }
            }

            return new GenericResponse<IEnumerable<object>>(errors);
        }

        public GenericResponse<IEnumerable<object>> ValidateTestSessionRehearsal(TestSessionDetails testSessionModel)
        {
            var errors = new List<object>();

            ValidateNullValue(nameof(testSessionModel.TestDate), testSessionModel.TestDate, errors);
            ValidateNullValue(nameof(testSessionModel.TestTime), testSessionModel.TestTime, errors);
            ValidateZeroValue(nameof(testSessionModel.CredentialTypeId), testSessionModel.CredentialTypeId, errors);

            var maxRehearsalNotesLength = 4000;
            if (!string.IsNullOrWhiteSpace(testSessionModel.RehearsalNotes) &&
                testSessionModel.RehearsalNotes.Length > maxRehearsalNotesLength)
            {
                AddError(nameof(testSessionModel.RehearsalNotes),
                    string.Format(Naati.Resources.Shared.RehearsalNotesLength, maxRehearsalNotesLength), errors);
            }

            if (errors.Any())
            {
                return new GenericResponse<IEnumerable<object>>(errors);
            }

            if (!string.IsNullOrWhiteSpace(testSessionModel.RehearsalTime) && testSessionModel.RehearsalDate == null)
            {
                AddError(nameof(testSessionModel.RehearsalDate), Naati.Resources.Shared.RequiredFieldValidationError, errors);
            }

            if (testSessionModel.RehearsalDate != null && string.IsNullOrWhiteSpace(testSessionModel.RehearsalTime))
            {
                AddError(nameof(testSessionModel.RehearsalTime), Naati.Resources.Shared.RequiredFieldValidationError, errors);
            }

            if (errors.Any())
            {
                return new GenericResponse<IEnumerable<object>>(errors);
            }
            var testSpecifications = _testSpecificationQueryService.GetTestSpecificationByCredentialTypeId(
                new SpecificationByCredentialTypeRequest() { CredentialTypeId = testSessionModel.CredentialTypeId });
            var activeTestSpecification = testSpecifications.List.Where(x => x.Active).Select(x => x.Id).Single();
            var testTasksResponse = _testSpecificationQueryService.GetTestComponentsBySpecificationId(
                new TestSpecificationRequest() { Id = activeTestSpecification });

            var requireRolePlayers = testTasksResponse.List.Any(x => x.RolePlayersRequired);

            if (requireRolePlayers && testSessionModel.RehearsalDate == null)
            {
                AddError(nameof(testSessionModel.RehearsalDate), Naati.Resources.Shared.RequiredFieldValidationError, errors);
            }

            if (requireRolePlayers && string.IsNullOrWhiteSpace(testSessionModel.RehearsalTime))
            {
                AddError(nameof(testSessionModel.RehearsalTime), Naati.Resources.Shared.RequiredFieldValidationError, errors);
            }

            if (!string.IsNullOrWhiteSpace(testSessionModel.RehearsalTime) && testSessionModel.RehearsalDate != null)
            {
                var sessionDateTime = testSessionModel.TestDate.Date().GetValueOrDefault();
                sessionDateTime = sessionDateTime.AddHours(int.Parse(testSessionModel.TestTime.Substring(0, 2)));
                sessionDateTime = sessionDateTime.AddMinutes(int.Parse(testSessionModel.TestTime.Substring(3, 2)));

                var rehearsalDateTime = testSessionModel.RehearsalDate.Date().GetValueOrDefault();
                rehearsalDateTime =
                    rehearsalDateTime.AddHours(int.Parse(testSessionModel.RehearsalTime.Substring(0, 2)));
                rehearsalDateTime =
                    rehearsalDateTime.AddMinutes(int.Parse(testSessionModel.RehearsalTime.Substring(3, 2)));

                if (rehearsalDateTime >= sessionDateTime)
                {
                    AddError(nameof(testSessionModel.RehearsalTime), Naati.Resources.Shared.InvalidRehearsalDateTime, errors);
                    AddError(nameof(testSessionModel.RehearsalDate), Naati.Resources.Shared.InvalidRehearsalDateTime, errors);
                }

                if (rehearsalDateTime < DateTime.Now)
                {
                    var credentialType = _credentialQueryService.GetCredentialType(testSessionModel.CredentialTypeId)
                        .Data;
                    if (!credentialType.AllowBackdating)
                    {
                        AddError(nameof(testSessionModel.RehearsalTime), Naati.Resources.Shared.RehearsalTimeInThePast,
                            errors);
                        AddError(nameof(testSessionModel.RehearsalDate), Naati.Resources.Shared.RehearsalTimeInThePast,
                            errors);
                    }
                }
            }

            if (testSessionModel.Id > 0)
            {
                var hasActiveRolePlayers = _testSessionQueryService
                    .GetTestSessionById(testSessionModel.Id.GetValueOrDefault())
                    .Result.HasRolePlayers;

                if (hasActiveRolePlayers && testSessionModel.RehearsalDate == null)
                {
                    AddError(nameof(testSessionModel.RehearsalDate), Naati.Resources.Shared.RehearsalDateCanNotBeRemoved,
                        errors);
                }
                if (hasActiveRolePlayers && testSessionModel.RehearsalTime == null)
                {
                    AddError(nameof(testSessionModel.RehearsalTime), Naati.Resources.Shared.RehearsalDateCanNotBeRemoved,
                        errors);
                }
            }

            return new GenericResponse<IEnumerable<object>>(errors);
        }

        public GenericResponse<TestSessionSkillValidationModel> ValidateTestSessionSkills(
            TestSessionSkillValidationRequest testSessionSkills)
        {
            var validationModel =
                new TestSessionSkillValidationModel { Errors = new List<object>(), Warnings = new List<object>() };
            var capacity = testSessionSkills.Capacity;

            var negativeCapacities =
                (testSessionSkills.Skills ?? Enumerable.Empty<TestSessionSkillModel>()).Where(
                    x => x.Selected && x.MaximumCapacity.GetValueOrDefault() < 0);
            negativeCapacities.ForEach(x => AddError(nameof(testSessionSkills.Skills),
                string.Format(Naati.Resources.Shared.InvalidMaximumCapacity, x.Name), validationModel.Errors));

            var biggerThanVenueCapacity =
                (testSessionSkills.Skills ?? Enumerable.Empty<TestSessionSkillModel>()).Where(
                    x => x.Selected && x.MaximumCapacity.GetValueOrDefault() > capacity);
            biggerThanVenueCapacity.ForEach(x => AddError(nameof(testSessionSkills.Skills),
                string.Format(Naati.Resources.Shared.CapacityBiggerThanVenueCapacity, capacity), validationModel.Errors));

            var notSelectedCapacities =
                (testSessionSkills.Skills ?? Enumerable.Empty<TestSessionSkillModel>()).Where(
                    x => !x.Selected && x.MaximumCapacity.GetValueOrDefault() > 0);
            notSelectedCapacities.ForEach(x => AddError(nameof(testSessionSkills.Skills),
                string.Format(Naati.Resources.Shared.NotSelectedSkill, x.Name, x.MaximumCapacity), validationModel.Warnings));

            var selectedSkillsDictionary = (testSessionSkills.Skills ?? Enumerable.Empty<TestSessionSkillModel>())
                .ToDictionary(x => x.SkillId, y => y);

            if ((testSessionSkills.RehearsalDate.HasValue ||
                 !string.IsNullOrWhiteSpace(testSessionSkills.RehearsalTime)) &&
                !((testSessionSkills.Skills ?? Enumerable.Empty<TestSessionSkillModel>()).Any(x => x.Selected)))
            {
                AddError(nameof(testSessionSkills.Skills),
                    string.Format(Naati.Resources.Shared.NotSelectedSkillForRolePlayers), validationModel.Warnings);
            }

            if (testSessionSkills.TestSessionId > 0)
            {
                var testSessionRolePlayersSkills =
                    _testSessionQueryService.GetTestSessionRolePlayerBySessionId(testSessionSkills.TestSessionId)
                        .Data.SelectMany(x => x.Details.Select(y => y.SkillId))
                        .Distinct();

                var removedSkills =
                    testSessionRolePlayersSkills.Where(x => !selectedSkillsDictionary.ContainsKey(x) ||
                                                            !selectedSkillsDictionary[x].Selected);
                removedSkills.ForEach(x =>
                {
                    TestSessionSkillModel skill;
                    selectedSkillsDictionary.TryGetValue(x, out skill);
                    AddError(nameof(testSessionSkills.Skills),
                        string.Format(Naati.Resources.Shared.RolePlayerAssignedToSkill, skill?.Name ?? x.ToString()),
                        validationModel.Errors);
                });

            }

            return validationModel;
        }

        private void ValidateNullValue(string property, object value, List<object> errors)
        {
            if (value == null)
            {
                AddError(property, Naati.Resources.Shared.RequiredFieldValidationError, errors);
            }
        }

        private void ValidateZeroValue(string property, int value, List<object> errors)
        {
            if (value == 0)
            {
                AddError(property, Naati.Resources.Shared.RequiredFieldValidationError, errors);
            }
        }

        private void AddError(string property, string message, IList<object> errors)
        {
            errors.Add(new
            {
                FieldName = property,
                Message = message
            });
        }

        public GenericResponse<IEnumerable<object>> ValidateNewTestSessionApplicants(
            CredentialRequestBulkActionRequest testSessionModel)
        {
            var errors = new List<object>();
            ValidateNullValue(nameof(testSessionModel.CredentialRequestIds), testSessionModel, errors);
            ValidateNullValue(nameof(testSessionModel.CredentialRequestIds), testSessionModel.CredentialRequestIds,
                errors);
            if (testSessionModel.CredentialRequestIds.Length == 0)
            {
                AddError(nameof(testSessionModel.CredentialRequestIds), Naati.Resources.Shared.NoCredentialRequestsSelected,
                    errors);
            }

            return new GenericResponse<IEnumerable<object>>(errors);
        }

        public GenericResponse<IEnumerable<CredentialRequestApplicantModel>> GetCredentialRequestApplicants(
            TestSessionRequest request)
        {
            var getRequest = _autoMapperHelper.Mapper.Map<CredentialRequestApplicantsRequest>(request);
            getRequest.CredentialRequestStatusTypeId = (int)CredentialRequestStatusTypeName.TestAccepted;
            getRequest.SkillIds = request.SkillIds;
            getRequest.CredentialApplicationTypeId = request.ApplicationTypeId;

            var result = _applicationQueryService.GetCredentialRequestApplicants(getRequest).Results;

            var mappedValues = result.Select(_autoMapperHelper.Mapper.Map<CredentialRequestApplicantModel>).ToList();
            return new GenericResponse<IEnumerable<CredentialRequestApplicantModel>>(mappedValues);
        }

        private UpdateTestSessionRequest MapTestSessionModel(TestSessionModel testSessionModel)
        {
            return new UpdateTestSessionRequest
            {
                TestSessionId = testSessionModel.TestSessionId,
                VenueId = testSessionModel.VenueId,
                Name = testSessionModel.Name,
                TestDateTime = testSessionModel.TestDate,
                ArrivalTime = testSessionModel.ArrivalTime,
                Duration = testSessionModel.Duration
            };
        }

        private TestSessionDetails MapTestSessionDetailsResponse(TestSessionDetailsDto testSessionTopSectionDto)
        {

            return new TestSessionDetails
            {
                Id = testSessionTopSectionDto.Id,
                Name = testSessionTopSectionDto.Name,
                TestDate = testSessionTopSectionDto.TestDate,
                ApplicationType = testSessionTopSectionDto.ApplicationType,
                CredentialType = testSessionTopSectionDto.CredentialType,
                CredentialTypeInternalName = testSessionTopSectionDto.CredentialTypeInternalName,
                TestSpecificationDescription = testSessionTopSectionDto.TestSpecificationDescription,
                IsActiveTestSpecification = testSessionTopSectionDto.IsActiveTestSpecification,
                ArrivalTime = GetTestSessionArriavalTime(testSessionTopSectionDto.TestDate,
                    testSessionTopSectionDto.ArrivalTime),
                TestEnd = GetTestSessionEndTime(testSessionTopSectionDto.TestDate, testSessionTopSectionDto.Duration),
                VenueName = testSessionTopSectionDto.VenueName,
                Capacity = testSessionTopSectionDto.Capacity,
                Attendees = testSessionTopSectionDto.Attendees,
                IsCompletedStatus = testSessionTopSectionDto.Completed,
                VenueAddress = testSessionTopSectionDto.VenueAddress,
                PreparationTime = testSessionTopSectionDto.ArrivalTime,
                SessionDuration = testSessionTopSectionDto.Duration,
                AllowSelfAssign = testSessionTopSectionDto.AllowSelfAssign,
                ApplicationTypeId = testSessionTopSectionDto.ApplicationTypeId,
                CredentialTypeId = testSessionTopSectionDto.CredentialTypeId,
                TestLocationId = testSessionTopSectionDto.TestLocationId,
                VenueId = testSessionTopSectionDto.VenueId,
                PublicNote = testSessionTopSectionDto.PublicNote,
                TestTime = testSessionTopSectionDto.TestDate.ToString("HH:mm"),
                ConfirmedAttendees = testSessionTopSectionDto.TestSessionApplicants.Count(
                    x => x.StatusId == (int)CredentialRequestStatusTypeName.TestSessionAccepted && !x.Sat &&
                         !x.Rejected),
                AwaitingPaymentAttendees = testSessionTopSectionDto.TestSessionApplicants.Count(
                    x => x.StatusId == (int)CredentialRequestStatusTypeName.AwaitingTestPayment && !x.Sat &&
                         !x.Rejected),
                ProcessingInvoiceAttendees = testSessionTopSectionDto.TestSessionApplicants.Count(
                    x => x.StatusId == (int)CredentialRequestStatusTypeName.ProcessingTestInvoice && !x.Sat &&
                         !x.Rejected),
                SatAttendees = testSessionTopSectionDto.TestSessionApplicants.Count(x => x.Sat && !x.Rejected),
                CheckedInAttendees = testSessionTopSectionDto.TestSessionApplicants.Count(
                    x => x.StatusId == (int)CredentialRequestStatusTypeName.CheckedIn && !x.Sat && !x.Rejected),
                OverrideVenueCapacity = testSessionTopSectionDto.OverrideVenueCapacity,
                RehearsalDate = testSessionTopSectionDto.RehearsalDateTime,
                RehearsalTime = testSessionTopSectionDto.RehearsalDateTime.HasValue
                    ? testSessionTopSectionDto.RehearsalDateTime.GetValueOrDefault().ToString("HH:mm")
                    : null,
                RehearsalNotes = testSessionTopSectionDto.RehearsalNotes,
                HasRolePLayers = testSessionTopSectionDto.HasRolePLayers,
                RolePlayersRequired = testSessionTopSectionDto.RolePlayersRequired,
                DefaultTestSpecificationId = testSessionTopSectionDto.DefaultTestSpecificationId,
                IsPastTestSession = testSessionTopSectionDto.TestDate < DateTime.Now,
                NewCandidatesOnly = testSessionTopSectionDto.NewCandidatesOnly,
                IsActive = testSessionTopSectionDto.IsActive,
            };
        }

        private string GetTestSessionArriavalTime(DateTime testDate, int? arrivalTime)
        {
            var updateTestDate = testDate.AddMinutes(-arrivalTime ?? 0);
            var res = updateTestDate.ToString(CultureInfo.InvariantCulture);
            return res;
        }

        private string GetTestSessionEndTime(DateTime testDate, int? duration)
        {
            var updateTestDate = testDate.AddMinutes(duration ?? 0);
            var res = updateTestDate.ToString(CultureInfo.InvariantCulture);
            return res;
        }

        public BusinessServiceResponse CreateOrUpdateTestSession(TestSessionBulkActionWizardModel wizardModel)
        {
            var applicationService = ServiceLocator.Resolve<IApplicationService>();
            var testSession = wizardModel.TestSessionRequestModel;
            var isNewTestSession = testSession.Id.GetValueOrDefault() == 0;
            var testSessionResponse = CreateOrUpdateTestSession(testSession);
            wizardModel.SetNewTestSessionId(testSessionResponse.Data);

            if (isNewTestSession)
            {
                wizardModel.SetAction(SystemActionTypeName.AllocateTestSession);
                wizardModel.SetSendEmailFlag(wizardModel.SendEmailToApplicants);
                return applicationService.PerformCredentialRequestsBulkAction(wizardModel);
            }

            wizardModel.SetAction(SystemActionTypeName.NotifyTestSessionDetails);
            wizardModel.SetSendEmailFlag(wizardModel.SendEmailToApplicants);
            var credentialRequestIds = GetApplicantsByTestSessionId(testSession.Id.GetValueOrDefault(), false)
                .Data.Select(x => x.CredentialRequestId);
            wizardModel.SetCredentialRequests(credentialRequestIds.ToArray());
            var response = applicationService.PerformCredentialRequestsBulkAction(wizardModel);

            wizardModel.SetAction(SystemActionTypeName.NotifyTestSessionRehearsalDetails);
            wizardModel.SetSendEmailFlag(wizardModel.SendEmailToRolePlayers);
            var rolePlayersResponse = ExecuteRolePlayersAction(wizardModel);

            response = new[] { response, rolePlayersResponse }.CombineResponses<object>();
            return response;
        }

        public GenericResponse<IEnumerable<EmailMessageModel>> GetCreateOrUpdateTestSessionEmailPreview(
            TestSessionBulkActionWizardModel wizardModel)
        {
            var applicationService = ServiceLocator.Resolve<IApplicationService>();

            var emails = new GenericResponse<IEnumerable<EmailMessageModel>>();

            var testSession = wizardModel.TestSessionRequestModel;
            var isNewTestSession = testSession.Id.GetValueOrDefault() == 0;
            if (isNewTestSession)
            {
                wizardModel.SetAction(SystemActionTypeName.AllocateTestSession);
                wizardModel.SetSendEmailFlag(wizardModel.SendEmailToApplicants);
                emails.Data = applicationService.GetCredentialRequestBulkActionEmailPreview(wizardModel).Data;
            }
            else
            {
                wizardModel.SetAction(SystemActionTypeName.NotifyTestSessionDetails);
                wizardModel.SetSendEmailFlag(wizardModel.SendEmailToApplicants);
                var credentialRequestIds = GetApplicantsByTestSessionId(testSession.Id.GetValueOrDefault(), false)
                    .Data.Select(x => x.CredentialRequestId);
                wizardModel.SetCredentialRequests(credentialRequestIds.ToArray());
                emails.Data = applicationService.GetCredentialRequestBulkActionEmailPreview(wizardModel).Data;

                wizardModel.SetAction(SystemActionTypeName.NotifyTestSessionRehearsalDetails);
                wizardModel.SetSendEmailFlag(wizardModel.SendEmailToRolePlayers);
                var rolePlerEmails = GetRolePlayersEmailPreview(wizardModel);
                emails.Data = emails.Data.Concat(rolePlerEmails).ToList();
            }

            return emails;
        }

        private IList<EmailMessageModel> GetRolePlayersEmailPreview(TestSessionBulkActionWizardModel wizardModel)
        {
            var rolePlayer = _testSessionQueryService
                .GetTestSessionRolePlayerBySessionId(wizardModel.TestSessionId.GetValueOrDefault())
                .Data.FirstOrDefault();

            if (rolePlayer != null)
            {

                var actionModel = GetRolePlayerActionModel(rolePlayer);
                var action =
                    RolePlayerAction<TestSessionBulkActionWizardModel>.CreateAction(
                        (SystemActionTypeName)wizardModel.ActionType, actionModel, wizardModel);
                var emails = action.GetEmailPreviews();
                return emails;
            }
            return new List<EmailMessageModel>();

        }

        private IList<EmailMessageModel> GetEmailPreview(AssignRolePlayersWizardModel wizardModel)
        {

            var actionModel = GetRolePlayerActionModel(wizardModel);

            var action =
                RolePlayerAction<AssignRolePlayersWizardModel>.CreateAction(
                    (SystemActionTypeName)wizardModel.ActionType, actionModel, wizardModel);

            var emails = action.GetEmailPreviews();
            return emails;
        }

        public GenericResponse<IEnumerable<EmailMessageModel>> GetAllocateRolePlayerEmailPreview(
            RolePlayerBulkAssignmentWizard request)
        {

            IList<EmailMessageModel> emailMessages = new List<EmailMessageModel>();

            var roleplayerWizards = GetRolePlayerWizardModels(request);
            var deletedRolePlayer =
                roleplayerWizards.FirstOrDefault(x => x.ActionType ==
                                                      (int)SystemActionTypeName.RolePlayerRemoveFromTestSession);

            var addedRolePlayer =
                roleplayerWizards.FirstOrDefault(x => x.ActionType == (int)SystemActionTypeName.AllocateRolePlayer);

            var modifiedRolePlayes =
                roleplayerWizards.FirstOrDefault(x => x.ActionType ==
                                                      (int)SystemActionTypeName.RolePlayerNotifyAllocationUpdate);

            if (addedRolePlayer != null)
            {
                emailMessages = emailMessages.Concat(GetEmailPreview(addedRolePlayer)).ToList();
            }

            if (modifiedRolePlayes != null)
            {
                emailMessages = emailMessages.Concat(GetEmailPreview(modifiedRolePlayes)).ToList();
            }

            if (deletedRolePlayer != null)
            {
                emailMessages = emailMessages.Concat(GetEmailPreview(deletedRolePlayer)).ToList();
            }

            return new GenericResponse<IEnumerable<EmailMessageModel>>(emailMessages);
        }

        private IList<AssignRolePlayersWizardModel> GetRolePlayerWizardModels(RolePlayerBulkAssignmentWizard request)
        {
            var skillDetails = _testSessionQueryService.GetSkillDetails(request.SkillId);
            var existingRolePlayerRequest = new GetTestSessionRolePlayerRequest()
            {
                TestSessionId = request.TestSessionId,
                SkillId = request.SkillId,
                LanguageId = skillDetails.Language1Id,
                TestSpecificationId = request.TestSpecificationId,
                Take = 20,
                Skip = 0,
                Sorting = GetRolePlayerSorting()
            };

            var existingRolePlayersForLanguage1 = _testSessionQueryService
                .GetTestSessionRolePlayers(existingRolePlayerRequest)
                .RolePlayers;

            existingRolePlayerRequest.LanguageId = skillDetails.Language2Id;
            var existingRolePlayersForLanguage2 = _testSessionQueryService
                .GetTestSessionRolePlayers(existingRolePlayerRequest)
                .RolePlayers;

            var existingRolePlayersById = existingRolePlayersForLanguage1.Concat(existingRolePlayersForLanguage2)
                .GroupBy(x => x.RolePlayerId)
                .Select(g => MapTestSessionRolePlayerAvailabilityModel(g.ToArray()))
                .ToDictionary(y => y.RolePlayerId, w => w);

            var wizardRolePlayerById = request.RolePlayers.ToDictionary(x => x.RolePlayerId, y => y);

            var addedRolePlayers = new List<TestSessionRolePlayerAvailabilityModel>();
            var remvovedRolePlayers = new List<TestSessionRolePlayerAvailabilityModel>();
            var modifiedRolePlayers = new List<TestSessionRolePlayerAvailabilityModel>();

            foreach (var wizardRolePlayer in wizardRolePlayerById.Values)
            {
                if (!existingRolePlayersById.ContainsKey(wizardRolePlayer.RolePlayerId) &&
                    wizardRolePlayer.Details.Any())
                {
                    addedRolePlayers.Add(wizardRolePlayer);

                }
                else if (existingRolePlayersById.ContainsKey(wizardRolePlayer.RolePlayerId))
                {
                    if (!wizardRolePlayer.Details.Any())
                    {
                        remvovedRolePlayers.Add(wizardRolePlayer);
                    }
                    else
                    {
                        var existingRolePlayerDetailsByTaskId = existingRolePlayersById[wizardRolePlayer.RolePlayerId]
                            .Details.ToDictionary(x => x.TestComponentId, y => y);
                        foreach (var wizardRolePlayerDetail in wizardRolePlayer.Details)
                        {
                            TestSessionRolePlayerTaskModel existingTask;
                            if (!existingRolePlayerDetailsByTaskId.TryGetValue(wizardRolePlayerDetail.TestComponentId,
                                out existingTask))
                            {
                                modifiedRolePlayers.Add(wizardRolePlayer);
                                break;
                            }

                            if (wizardRolePlayerDetail.LanguageId != existingTask.LanguageId
                                || wizardRolePlayerDetail.RolePlayerRoleTypeId != existingTask.RolePlayerRoleTypeId)
                            {
                                modifiedRolePlayers.Add(wizardRolePlayer);
                                break;
                            }
                        }

                        foreach (var existingTask in existingRolePlayerDetailsByTaskId.Values)
                        {
                            if (wizardRolePlayer.Details.All(x => x.TestComponentId != existingTask.TestComponentId))
                            {
                                modifiedRolePlayers.Add(wizardRolePlayer);
                                break;
                            }
                        }
                    }

                }

            }

            var models = addedRolePlayers
                .Select(x => MapAssignRolePlayersWizardModel(x, SystemActionTypeName.AllocateRolePlayer, request.Data))
                .OfType<AssignRolePlayersWizardModel>()
                .Concat(modifiedRolePlayers
                    .Select(x => MapAssignRolePlayersWizardModel(x,
                        SystemActionTypeName.RolePlayerNotifyAllocationUpdate, request.Data))
                    .OfType<AssignRolePlayersWizardModel>());

            models = models.Concat(remvovedRolePlayers
                .Select(x => MapAssignRolePlayersWizardModel(x, SystemActionTypeName.RolePlayerRemoveFromTestSession,
                    request.Data))
                .OfType<AssignRolePlayersWizardModel>());
            return models.ToList();
        }

        private AssignRolePlayersWizardModel MapAssignRolePlayersWizardModel(
            TestSessionRolePlayerAvailabilityModel rolePlayer, SystemActionTypeName actionType, dynamic data)
        {
            return new AssignRolePlayersWizardModel()
            {
                ActionType = (int)actionType,
                RolePlayer = rolePlayer,
                Data = data
            };
        }

        private RolePlayerActionModel GetRolePlayerActionModel(AssignRolePlayersWizardModel wizardModel)
        {
            var personDetails = ServiceLocator.Resolve<IPersonService>()
                .GetPersonBasic(wizardModel.RolePlayer.CustomerNo)
                .Data;

            var testSessionRolePlayer = new TestSessionRolePlayerModel()
            {
                TestSessionId = wizardModel.TestSessionId,
                RolePlayerId = wizardModel.RolePlayer.RolePlayerId,
                Details = new List<TestSessionRolePlayerDetailModel>()
            };

            if (wizardModel.RolePlayer.TestSessionRolePlayerId > 0)
            {
                var result =
                    _testSessionQueryService.GetTestSessionRolePlayer(wizardModel.RolePlayer.TestSessionRolePlayerId);
                testSessionRolePlayer = _autoMapperHelper.Mapper.Map<TestSessionRolePlayerModel>(result.Data);
            }

            var actionModel = new RolePlayerActionModel
            {
                PersonDetails = personDetails,
                PersonNotes = new List<PersonNoteModel>(),
                TestSessionRolePlayer = testSessionRolePlayer
            };

            return actionModel;
        }

        private RolePlayerActionModel GetRolePlayerActionModel(TestSessionRolePlayerDto testSessionRolePlayerDto)
        {
            var personDetails = ServiceLocator.Resolve<IPersonService>()
                .GetPersonBasic(testSessionRolePlayerDto.NaatiNumber)
                .Data;
            var testSessionRolePlayer = _autoMapperHelper.Mapper.Map<TestSessionRolePlayerModel>(testSessionRolePlayerDto);

            var actionModel = new RolePlayerActionModel
            {
                PersonDetails = personDetails,
                PersonNotes = new List<PersonNoteModel>(),
                TestSessionRolePlayer = testSessionRolePlayer
            };

            return actionModel;
        }

        public GenericResponse<RolePlayerAllocationDetails> GetRolePlayerAllocationTasks(int testSessionId, int skillId)
        {
            // TODO: This methods need to be adjusted when TestSpecification Is moved from the test sitting to the test session
            var testSepecificationId = _testSessionQueryService.GetTestSpecificationDetails(
                                               new TestSpecificationDetailsRequest()
                                               {
                                                   TestSessionIds = new[] { testSessionId }

                                               })
                                           .FirstOrDefault()
                                           ?.Id ?? 0;
            return GetRolePlayerAllocationDetails(new RolePlayerAllocationDetailsRequest
            {
                TestSpecificationId = testSepecificationId,
                SkillId = skillId
            });
        }

        public GenericResponse<IList<TestSessionRolePlayerModel>> GetPersonRolePlays(int naatiNumber)
        {
            var rolePlays = _testSessionQueryService.GetPersonRolePlays(naatiNumber).Data;
            var models = rolePlays.Select(_autoMapperHelper.Mapper.Map<TestSessionRolePlayerModel>).ToList();
            return new GenericResponse<IList<TestSessionRolePlayerModel>>(models);
        }

        public BusinessServiceResponse ExecuteRolePlayersAction(RolePlayerBulkAssignmentWizard wizardModel)
        {
            var wizardModels = GetRolePlayerWizardModels(wizardModel);

            var response = new BusinessServiceResponse();

            foreach (var rolePlayerWizardModel in wizardModels)
            {
                try
                {
                    var actionModel = GetRolePlayerActionModel(rolePlayerWizardModel);

                    var actionResponse = PerformRolePlayerAction(rolePlayerWizardModel, actionModel);
                    response.Errors.AddRange(actionResponse.Errors);

                }

                catch (UserFriendlySamException ex)
                {
                    LoggingHelper.LogWarning(ex, ex.Message);
                    response.Errors.Add(ex.Message);
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(ex);
                    var generalError =
                        $"Error processing role-player action for customer number: {rolePlayerWizardModel.RolePlayer.CustomerNo} ";

                    response.Errors.Add(generalError);
                }
            }

            response.Success = !response.Errors.Any();
            return response;
        }

        public BusinessServiceResponse ExecuteRolePlayersAction(TestSessionBulkActionWizardModel wizardModel)
        {
            var rolePlayers = _testSessionQueryService
                .GetTestSessionRolePlayerBySessionId(wizardModel.TestSessionId.GetValueOrDefault())
                .Data;

            var response = new BusinessServiceResponse();

            foreach (var rolePlayer in rolePlayers)
            {
                try
                {
                    var actionModel = GetRolePlayerActionModel(rolePlayer);

                    var actionResponse = PerformRolePlayerAction(wizardModel, actionModel);
                    response.Errors.AddRange(actionResponse.Errors);

                }

                catch (UserFriendlySamException ex)
                {
                    LoggingHelper.LogWarning(ex, ex.Message);
                    response.Errors.Add(ex.Message);
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(ex);
                    var generalError =
                        $"Error processing role-player action for customer number: {rolePlayer.NaatiNumber} ";

                    response.Errors.Add(generalError);
                }
            }

            response.Success = !response.Errors.Any();
            return response;
        }

        public GenericResponse<UpsertRolePlayerResultModel> UpsertTestSessionRolePlayer(
            UpsertSessionRolePlayerRequest request)
        {
            var response = _testSessionQueryService.UpsertTestSessionRolePlayer(request);
            var result = new UpsertRolePlayerResultModel
            {
                TestSessionRolePLayerId = response.TestSessionRolePlayerId
            };

            return result;
        }

        public GenericResponse<UpsertRolePlayerResultModel> PerformRolePlayerAction<T>(T wizardModel,
            RolePlayerActionModel rolePlayerModel) where T : SystemActionWizardModel
        {
            var action = RolePlayerAction<T>.CreateAction((SystemActionTypeName)wizardModel.ActionType,
                rolePlayerModel, wizardModel);

            if (!action.ArePreconditionsMet())
            {
                var response = new GenericResponse<UpsertRolePlayerResultModel>();
                response.Errors.AddRange(action.ValidationErrors.Select(x => x.Message));
                response.Success = false;
                return response;
            }

            action.Perform();
            action.SaveChanges();
            var output = action.GetOutput();
            return output.UpsertResults;
        }

        private bool CanOverrideEmail()
        {
            return ServiceLocator.Resolve<IUserService>().HasPermission(SecurityNounName.Email, SecurityVerbName.Override);
        }

        public TestSessionCheckOptionData GetCheckOptionMessage(TestSessionDetails testSessionDetails)
        {
            //SR-190303 TFS 241350
            var notifyApplicants = false;
            var notifyRolePlayers = false;
            var isRolePlayerAvailable = _testSessionQueryService.IsRolePlayerAvailable();
            if (testSessionDetails.Id > 0)
            {
                var testSession = _testSessionQueryService.GetTestSessionById(testSessionDetails.Id.GetValueOrDefault())
                    .Result;
                notifyApplicants = NotifiyTestSessionChangesToApplicants(testSessionDetails, testSession);
                notifyRolePlayers = NotifiyTestSessionChangesToRolePlayers(testSessionDetails, testSession);
            }

            return new TestSessionCheckOptionData
            {
                NotifyApplicantsChecked = notifyApplicants,
                NotifyRolePlayersChecked = notifyRolePlayers,
                NotifyApplicantsMessage = Naati.Resources.TestSession.TestSessionCheckBoxMessage,
                NotifyRolePlayersMessage = Naati.Resources.TestSession.TestSessionRolePlayerCheckBoxMessage,
                OnDisableApplicantsMessage = Naati.Resources.TestSession.SendEmailNotificationDisabled,
                OnDisableRolePlayersMessage = Naati.Resources.TestSession.RolePlayerSendEmailNotificationDisabled,
                ReadOnly = !_userService.HasPermission(SecurityNounName.Email, SecurityVerbName.Override),
                IsRolePlayerAvailable = isRolePlayerAvailable
            };
        }

        public TestSessionCheckOptionData GetAllocateRolePlayersCheckOptionMessage(
            TestSessionDetails testSessionDetails)
        {
            return new TestSessionCheckOptionData
            {
                NotifyApplicantsChecked = false,
                NotifyRolePlayersChecked = true,
                NotifyApplicantsMessage = Naati.Resources.TestSession.TestSessionCheckBoxMessage,
                NotifyRolePlayersMessage = Naati.Resources.TestSession.TestSessionRolePlayerCheckBoxMessage,
                OnDisableApplicantsMessage = Naati.Resources.TestSession.SendEmailNotificationDisabled,
                OnDisableRolePlayersMessage = Naati.Resources.TestSession.RolePlayerSendEmailNotificationDisabled,
                ReadOnly = !CanOverrideEmail()
            };
        }

        private bool NotifiyTestSessionChangesToApplicants(TestSessionDetails testSessionDetails,
            TestSessionDto initialDetails)
        {
            var sessionDateTicks = testSessionDetails.TestDate.GetValueOrDefault()
                .Date.AddTicks(TimeSpan.Parse(testSessionDetails.TestTime).Ticks)
                .Ticks;
            var optionChecked = initialDetails.VenueId != testSessionDetails.VenueId ||
                                initialDetails.Duration != testSessionDetails.SessionDuration ||
                                initialDetails.TestDate.Ticks != sessionDateTicks;
            return optionChecked;
        }

        private bool NotifiyTestSessionChangesToRolePlayers(TestSessionDetails testSessionDetails,
            TestSessionDto initialDetails)
        {
            var optionChecked = NotifiyTestSessionChangesToApplicants(testSessionDetails, initialDetails);

            if (initialDetails.RehearsalDateTime.HasValue &&
                !string.IsNullOrWhiteSpace(testSessionDetails.RehearsalTime))
            {
                var rehearsalDateTicks = testSessionDetails.RehearsalDate.GetValueOrDefault()
                    .Date.AddTicks(TimeSpan.Parse(testSessionDetails.RehearsalTime).Ticks)
                    .Ticks;
                optionChecked = optionChecked || initialDetails.RehearsalDateTime.GetValueOrDefault().Ticks !=
                                rehearsalDateTicks;
            }

            return optionChecked;
        }

        private IEnumerable<RolePlayerSorting> GetRolePlayerSorting()
        {
            return new[]
            {
                new RolePlayerSorting
                {
                    SortType = RolePlayerSortingType.TestLocation,
                    Direction = SortDirection.Descending
                },
                new RolePlayerSorting
                {
                    SortType = RolePlayerSortingType.Availability,
                    Direction = SortDirection.Descending
                },
                new RolePlayerSorting
                {
                    SortType = RolePlayerSortingType.Capacity,
                    Direction = SortDirection.Descending
                },
                new RolePlayerSorting
                {
                    SortType = RolePlayerSortingType.Rating,
                    Direction = SortDirection.Descending
                },

            };
        }

        //public GenericResponse<IEnumerable<TestSessionRolePlayerAvailabilityModel>> GetAvailableRolePlayersForLanguage2(RolePlayerAssignmentRequest request)
        //{
        //    var skillDetails = _testSessionQueryService.GetSkillDetails(request.SkillId);
        //    var existingRolePlayers = GetTestSessionRolePlayers(new GetTestSessionRolePlayerRequest()
        //    {
        //        TestSessionId = request.TestSessionId,
        //        SkillId = request.SkillId,
        //        LanguageId = skillDetails.Language2Id,
        //        TestSpecificationId = request.TestSpecificationId

        //    });

        //    Mapper.CreateMap<RolePlayerAvailabilityDto, TestSessionRolePlayerAvailabilityModel>()
        //        .ForMember(x => x.CustomerNo, y => y.MapFrom(z => z.NaatiNumber))
        //        .ForMember(x => x.Name, y => y.MapFrom(z => $"{z.GivenName} {z.Surname}"))
        //        .ForMember(x => x.TestSessionRolePlayerId, y => y.Ignore())
        //        .ForMember(x => x.Attended, y => y.Ignore())
        //        .ForMember(x => x.Rejected, y => y.Ignore())
        //        .ForMember(x => x.Rehearsed, y => y.Ignore())
        //        .ForMember(x => x.RolePlayerStatusId, y => y.Ignore())
        //        .ForMember(x => x.Details, y => y.UseValue(new List<TestSessionRolePlayerTaskModel>()));


        //    var rolePlayers = _testSessionQueryService.GetAvailableRolePlayers(
        //        new GetRolePlayerRequest() { TestSessionId = request.TestSessionId, TestSpecificationId = request.TestSpecificationId, SkillId = request.SkillId, LanguageId = skillDetails.Language2Id, Sorting = GetRolePlayerSorting(), Skip = 0, Take = 50 }).RolePlayers;
        //    rolePlayers = rolePlayers.Where(x => existingRolePlayers.Data.All(y => y.RolePlayerId != x.RolePlayerId));
        //    var result = rolePlayers.Select(Mapper.DynamicMap<TestSessionRolePlayerAvailabilityModel>).ToList();

        //    return OrderRolePlayers(existingRolePlayers.Data.Concat(result));

        //}

        private GenericResponse<IEnumerable<TestSessionRolePlayerAvailabilityModel>> OrderRolePlayers(
            IList<TestSessionRolePlayerAvailabilityModel> rolePlayers)
        {

            IEnumerable<TestSessionRolePlayerAvailabilityModel> orderedList = rolePlayers;
            foreach (var sorting in GetRolePlayerSorting())
            {

                switch (sorting.SortType)
                {
                    case RolePlayerSortingType.TestLocation:
                        orderedList = orderedList.OrderUsing(x => x.IsInTestLocation, sorting.Direction);
                        break;
                    case RolePlayerSortingType.Availability:
                        orderedList = orderedList.OrderUsing(x => x.Available, sorting.Direction);
                        break;
                    case RolePlayerSortingType.Capacity:
                        orderedList = orderedList.OrderUsing(x => x.HasCapacity, sorting.Direction);
                        break;
                    case RolePlayerSortingType.Gender:
                        orderedList = orderedList.OrderUsing(x => x.Gender, sorting.Direction);
                        break;
                    case RolePlayerSortingType.Rating:
                        orderedList = orderedList.OrderUsing(x => x.Rating.GetValueOrDefault(), sorting.Direction);
                        break;

                    default:
                        throw new NotSupportedException($"sort type '{sorting.SortType}' Not suported");
                }

            }
            var order = 0;
            orderedList = orderedList.Select(x =>
            {
                x.DisplayOrder = order++;
                return x;
            });
            var response =
                new GenericResponse<IEnumerable<TestSessionRolePlayerAvailabilityModel>>(orderedList.ToList());

            return response;
        }

        public GenericResponse<IEnumerable<TestSessionRolePlayerAvailabilityModel>> GetTestSessionRolePlayers(
            TestSessionRolePlayersRequest request)
        {
            // TODO: This methods need to be adjusted when TestSpecification Is moved from the test sitting to the test session
            var testSepecificationId = _testSessionQueryService.GetTestSpecificationDetails(
                                               new TestSpecificationDetailsRequest()
                                               {
                                                   TestSessionIds = new[] { request.TestSessionId }

                                               })
                                           .FirstOrDefault()
                                           ?.Id ?? 0;
            var skillDetails = _testSessionQueryService.GetSkillDetails(request.SkillId);
            var existingRolePlayerRequest = new GetTestSessionRolePlayerRequest()
            {
                TestSessionId = request.TestSessionId,
                SkillId = request.SkillId,
                LanguageId = skillDetails.Language1Id,
                TestSpecificationId = testSepecificationId,
                Take = 20,
                Skip = 0,
                IncludeRejected = true,
                Sorting = GetRolePlayerSorting()
            };

            var existingRolePlayersForLanguage1 = _testSessionQueryService
                .GetTestSessionRolePlayers(existingRolePlayerRequest)
                .RolePlayers;

            existingRolePlayerRequest.LanguageId = skillDetails.Language2Id;
            var existingRolePlayersForLanguage2 = _testSessionQueryService
                .GetTestSessionRolePlayers(existingRolePlayerRequest)
                .RolePlayers;

            var existingRolePlayers = existingRolePlayersForLanguage1.Concat(existingRolePlayersForLanguage2).ToList();

            var mappedRolePlayers = existingRolePlayers.GroupBy(x => x.RolePlayerId)
                .Select(g => MapTestSessionRolePlayerAvailabilityModel(g.ToArray()))
                .ToList();

            return new GenericResponse<IEnumerable<TestSessionRolePlayerAvailabilityModel>>(mappedRolePlayers);
        }

        public BusinessServiceResponse PerformRolePlayerAction(int testSessionRolePlayerId,
            RolePlayerUpdateWizardModel wizardModel)
        {
            var rolePlayer = _testSessionQueryService.GetTestSessionRolePlayer(testSessionRolePlayerId).Data;

            var actionModel = GetRolePlayerActionModel(rolePlayer);
            var actionResponse = PerformRolePlayerAction(wizardModel, actionModel);

            return actionResponse;
        }

        public IList<SystemActionNameModel> GetValidRolePlayerActions(SystemActionTypeName lastSystemActionType)
        {

            var actionId = (int)lastSystemActionType;
            var mapping = GetViewRolePlayerActionMapping().First(x => x.Value == actionId);
            var result = ServiceLocator.Resolve<IApplicationWizardLogicService>()
                .GetValidRolePlayerActions((RolePlayerStatusTypeName)mapping.Key);
            return new[]
                {
                    new SystemActionNameModel
                    {
                        Id = actionId,
                        Name = Naati.Resources.RolePlayer.ResourceManager.GetString(lastSystemActionType.ToString())
                    }
                }.Concat(result)
                .ToList();
        }

        private Dictionary<int, int> GetViewRolePlayerActionMapping()
        {
            var lastActioStatusnMapping = new Dictionary<int, int>()
            {
                {(int) RolePlayerStatusTypeName.Pending, (int) SystemActionTypeName.RolePlayerMarkAsPending},
                {(int) RolePlayerStatusTypeName.Accepted, (int) SystemActionTypeName.RolePlayerMarkAsAccepted},
                {(int) RolePlayerStatusTypeName.Attended, (int) SystemActionTypeName.RolePlayerMarkAsAttendedTest},
                {
                    (int) RolePlayerStatusTypeName.Rehearsed,
                    (int) SystemActionTypeName.RolePlayerMarkAsAttendedRehearsal
                },
                {(int) RolePlayerStatusTypeName.NoShow, (int) SystemActionTypeName.RolePlayerMarkAsNoShow},
                {(int) RolePlayerStatusTypeName.Rejected, (int) SystemActionTypeName.RolePlayerMarkAsRejected},
                {(int) RolePlayerStatusTypeName.None, (int) SystemActionTypeName.RolePlayerMarkAsRemoved},
            };

            return lastActioStatusnMapping;
        }

        /// <summary>
        /// This is for someone who has rejected a test session and we are manually assinging them back to that session or a different one
        /// </summary>
        /// <param name="allocateTestSessionRequest"></param>
        /// <returns></returns>
        public BusinessServiceResponse AllocatePastTestSession(AllocateTestSessionRequest allocateTestSessionRequest)
        {
            var response = new BusinessServiceResponse();

            //get credential request
            var credentialRequest = _applicationQueryService.GetCredentialRequest(allocateTestSessionRequest.CredentialRequestId).CredentialRequest;
            var testSitting = credentialRequest.TestSittings.Where(x => x.TestSessionId == allocateTestSessionRequest.TestSessionId).FirstOrDefault();

            //validate against credential request status
            if (credentialRequest.StatusTypeId == (int)CredentialRequestStatusTypeName.TestAccepted)
            {
                //if candidate assigned to a different session than the one nominated then
                if (testSitting == null)
                {
                    //user being allocated to different test session
                    try
                    {
                        CreateNewTestSitting(allocateTestSessionRequest, credentialRequest);
                        LoggingHelper.LogInfo($"Assign Past Test Session successful for {allocateTestSessionRequest.CredentialRequestId} to {allocateTestSessionRequest.TestSessionId}");
                    }
                    catch (Exception ex)
                    {
                        LoggingHelper.LogException(ex);
                        var generalError =
                            $"Error allocating to past test session (with new test sitting): {allocateTestSessionRequest.TestSessionId} ";

                        response.Errors.Add(generalError);
                    }
                }
                else if (!testSitting.Rejected)
                {
                    throw new UserFriendlySamException(Naati.Resources.TestSession.CredentialRequestNotAllocated);
                }
                else
                {
                    //reassigning to same session
                    try
                    {
                        _testSessionQueryService.UpdateTestSitting(new UpdateTestSittingRequest()
                        {
                            CredentialRequestId = allocateTestSessionRequest.CredentialRequestId,
                            TestSessionId = allocateTestSessionRequest.TestSessionId,
                            CredentialRequestStatusTypeId = (int)CredentialRequestStatusTypeName.TestSessionAccepted,
                            AllocatedDate = DateTime.Now,
                            IsRejected = false
                        });
                        LoggingHelper.LogInfo($"Assign Past Test Session successful for {allocateTestSessionRequest.CredentialRequestId} to {allocateTestSessionRequest.TestSessionId}");
                    }
                    catch (Exception ex)
                    {
                        LoggingHelper.LogException(ex);
                        var generalError =
                            $"Error allocating to past test session : {allocateTestSessionRequest.TestSessionId} ";

                        response.Errors.Add(generalError);
                    }
                }
            }
            else if (credentialRequest.StatusTypeId == (int)CredentialRequestStatusTypeName.EligibleForTesting)
            {
                //create new test sitting
                try
                {
                    CreateNewTestSitting(allocateTestSessionRequest, credentialRequest);
                    LoggingHelper.LogInfo($"Assign Past Test Session successful for {allocateTestSessionRequest.CredentialRequestId} to {allocateTestSessionRequest.TestSessionId}");
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(ex);
                    var generalError =
                        $"Error allocating to past test session (with new test sitting): {allocateTestSessionRequest.TestSessionId} ";

                    response.Errors.Add(generalError);
                }
            }
            else
            {
                throw new UserFriendlySamException(Naati.Resources.TestSession.NotAvailable);
            }

            //create note
            var application = _applicationQueryService.GetApplicationByCredentialRequestId(allocateTestSessionRequest.CredentialRequestId).Result;
            _noteService.CreateApplicationNote(new ApplicationNoteModel
            {
                ApplicationId = application.ApplicationId,
                CreatedDate = DateTime.Now,
                Note = string.Format(Naati.Resources.TestSession.PastTestSessionNote, allocateTestSessionRequest.TestSessionId, _userService.GetUserName()),
                ReadOnly = true,
                UserId = allocateTestSessionRequest.UserId
            });

            response.Success = !response.Errors.Any();
            return response;
        }

        private void CreateNewTestSitting(AllocateTestSessionRequest allocateTestSessionRequest, CredentialRequestDto credentialRequest)
        {
            var testSession = _testSessionQueryService.GetTestSessionById(allocateTestSessionRequest.TestSessionId);
            _testSessionQueryService.UpdateTestSitting(new UpdateTestSittingRequest()
            {
                CredentialRequestId = allocateTestSessionRequest.CredentialRequestId,
                TestSessionId = allocateTestSessionRequest.TestSessionId,
                CredentialRequestStatusTypeId = (int)CredentialRequestStatusTypeName.TestSessionAccepted,
                IsRejected = false,
                AllocatedDate = DateTime.Now,
                Supplementary = credentialRequest.Supplementary,
                TestSpecificationId = credentialRequest.CredentialType.ActiveTestSpecificationId ?? testSession.Result.DefaultTestSpecificationId
            });
        }

        public BusinessServiceResponse UpdateRolePlayers(UpdateRolePLayersRequest request)
        {
            var response = new BusinessServiceResponse();

            var wizardModel = new RolePlayerUpdateWizardModel();
            foreach (var rolePlayerAction in request.RolePlayerActions)
            {
                try
                {
                    wizardModel.ActionType = rolePlayerAction.SystemActionTypeId;
                    var rolePlayer = _testSessionQueryService
                        .GetTestSessionRolePlayer(rolePlayerAction.TestSessionRolePlayerId)
                        .Data;

                    var actionModel = GetRolePlayerActionModel(rolePlayer);

                    var actionResponse = PerformRolePlayerAction(wizardModel, actionModel);
                    response.Errors.AddRange(actionResponse.Errors);

                }

                catch (UserFriendlySamException ex)
                {
                    LoggingHelper.LogWarning(ex, ex.Message);
                    response.Errors.Add(ex.Message);
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(ex);
                    var generalError =
                        $"Error processing role-player action for Test Session role-player Id: {rolePlayerAction.TestSessionRolePlayerId} ";

                    response.Errors.Add(generalError);
                }
            }

            response.Success = !response.Errors.Any();
            return response;
        }

        public GenericResponse<IEnumerable<TestSessionRolePlayerAvailabilityModel>> GetAvailableAndExistingRolePlayers(
            RolePlayerAssignmentRequest request)
        {
            var skillDetails = _testSessionQueryService.GetSkillDetails(request.SkillId);
            var existingRolePlayerRequest = new GetTestSessionRolePlayerRequest()
            {
                TestSessionId = request.TestSessionId,
                SkillId = request.SkillId,
                LanguageId = skillDetails.Language1Id,
                TestSpecificationId = request.TestSpecificationId,
                Take = 20,
                Skip = 0,
                Sorting = GetRolePlayerSorting()
            };

            var existingRolePlayersForLanguage1 = _testSessionQueryService
                .GetTestSessionRolePlayers(existingRolePlayerRequest)
                .RolePlayers;

            existingRolePlayerRequest.LanguageId = skillDetails.Language2Id;
            var existingRolePlayersForLanguage2 = _testSessionQueryService
                .GetTestSessionRolePlayers(existingRolePlayerRequest)
                .RolePlayers;

            var existingRolePlayers = existingRolePlayersForLanguage1.Concat(existingRolePlayersForLanguage2).ToList();

            var rolePlayerRequests = new GetRolePlayerRequest()
            {
                TestSessionId = request.TestSessionId,
                TestSpecificationId = request.TestSpecificationId,
                SkillId = request.SkillId,
                LanguageId = skillDetails.Language1Id,
                Sorting = GetRolePlayerSorting(),
                Take = 20,
                Skip = 0,
            };

            var rolePlayersForLanguage1 = _testSessionQueryService.GetAvailableRolePlayers(rolePlayerRequests)
                .RolePlayers;

            rolePlayerRequests.LanguageId = skillDetails.Language2Id;
            var rolePlayersForLanguage2 = _testSessionQueryService.GetAvailableRolePlayers(rolePlayerRequests)
                .RolePlayers;

            var availableRolePlayers = rolePlayersForLanguage1.Concat(rolePlayersForLanguage2)
                .Where(x => !existingRolePlayers.Any(y => y.RolePlayerId == x.RolePlayerId &&
                                                          y.LanguageId == x.LanguageId));

            var rolePlayers = MergeExistingAndAvailableRolePlayers(existingRolePlayers, availableRolePlayers);
            return OrderRolePlayers(rolePlayers.ToList());

        }

        private IEnumerable<TestSessionRolePlayerAvailabilityModel> MergeExistingAndAvailableRolePlayers(
            IEnumerable<TestSessionRolePlayerAvailabilityDto> existingRolePlayers,
            IEnumerable<RolePlayerAvailabilityDto> availableRolePlayers)
        {
            var existingRolePlayersById = existingRolePlayers.GroupBy(x => x.RolePlayerId)
                .Select(g => MapTestSessionRolePlayerAvailabilityModel(g.ToArray()))
                .ToDictionary(y => y.RolePlayerId, w => w);

            foreach (var rolePlayer in availableRolePlayers)
            {
                TestSessionRolePlayerAvailabilityModel rolePlayerModel;
                if (!existingRolePlayersById.TryGetValue(rolePlayer.RolePlayerId, out rolePlayerModel))
                {
                    rolePlayerModel = MapRolePlayerAvailabilityDto(rolePlayer);
                    existingRolePlayersById[rolePlayer.RolePlayerId] = rolePlayerModel;
                }

                rolePlayerModel.LanguageIds.Add(rolePlayer.LanguageId);

            }

            return existingRolePlayersById.Values;
        }

        private TestSessionRolePlayerAvailabilityModel MapRolePlayerAvailabilityDto(
            RolePlayerAvailabilityDto rolePlayerAvailability)
        {
            var model = _autoMapperHelper.Mapper.Map<TestSessionRolePlayerAvailabilityModel>(rolePlayerAvailability);
            model.ReadOnly = !CanRemoveRolePlayer(model.RolePlayerStatusId);
            return model;
        }


        private TestSessionRolePlayerAvailabilityModel MapTestSessionRolePlayerAvailabilityModel(
            params TestSessionRolePlayerAvailabilityDto[] rolePlayers)
        {
            var mappedEntity = _autoMapperHelper.Mapper.Map<TestSessionRolePlayerAvailabilityModel>(rolePlayers[0]);
            mappedEntity.ReadOnly = !CanRemoveRolePlayer(mappedEntity.RolePlayerStatusId);

            var tasksDetails = new List<TestSessionRolePlayerTaskModel>();
            foreach (var rolePlayer in rolePlayers)
            {
                var mappedTasks = rolePlayer.Details.Select(x =>
                {
                    var mappedDetails = _autoMapperHelper.Mapper.Map<TestSessionRolePlayerTaskModel>(x);
                    mappedDetails.LanguageId = rolePlayer.LanguageId;
                    return mappedDetails;
                });
                tasksDetails.AddRange(mappedTasks);
                mappedEntity.LanguageIds.Add(rolePlayer.LanguageId);
            }
            mappedEntity.Details = tasksDetails;
            return mappedEntity;
        }

        private bool CanRemoveRolePlayer(int rolePLayerStatusTypeId)
        {

            return rolePLayerStatusTypeId == (int)RolePlayerStatusTypeName.Accepted ||
                   rolePLayerStatusTypeId == (int)RolePlayerStatusTypeName.Pending ||
                   rolePLayerStatusTypeId == (int)RolePlayerStatusTypeName.None;
        }

        public GenericResponse<RolePlayerAllocationDetails> GetRolePlayerAllocationDetails(
            RolePlayerAssignmentRequest request)
        {

            return GetRolePlayerAllocationDetails(new RolePlayerAllocationDetailsRequest
            {
                TestSpecificationId = request.TestSpecificationId,
                SkillId = request.SkillId
            });
        }

        public GenericResponse<RolePlayerAllocationDetails> GetRolePlayerAllocationDetails(
            RolePlayerAllocationDetailsRequest request)
        {
            var respponse =
                _testMaterialQueryService.GetTestSpecificationTasks(
                    new TestTaskRequest { TestSpecificationId = request.TestSpecificationId });

            var tasks = respponse.Where(x => x.RoleplayersRequired)
                .Select(_autoMapperHelper.Mapper.Map<TestTaskModel>)
                .ToList();
            var skillDetails = _testSessionQueryService.GetSkillDetails(request.SkillId);
            var skill = _autoMapperHelper.Mapper.Map<SkillDetailsModel>(skillDetails);
            var details = new RolePlayerAllocationDetails
            {
                Skill = skill,
                Tasks = tasks
            };
            return details;
        }

        public GetTestSessionSpecificationResponse GetTestSpecificationDetails(RolePlayerAssignmentRequest request)
        {
            var respponse = _testSessionQueryService.GetTestSpecificationDetails(
                new TestSpecificationDetailsRequest { TestSessionIds = new[] { request.TestSessionId } });


            return new GetTestSessionSpecificationResponse
            {
                Results = respponse.Select(_autoMapperHelper.Mapper.Map<TestSessionSpecificationDetailsModel>).ToList()
            };
        }

        public GetTestSessionLanguageResponse GetLanguageDetails(RolePlayerAssignmentRequest request)
        {
            var respponse =
                _testSessionQueryService.GetTestSessionLanguageDetails(
                    new TestSessionLanguageDetailsRequest
                    {
                        TestSessionIds = new[] { request.TestSessionId },
                        TestSpecificationId = request.TestSpecificationId
                    });

            return new GetTestSessionLanguageResponse
            {
                Results = respponse.Select(_autoMapperHelper.Mapper.Map<TestSessionLanguageDetailsModel>).ToList()
            };
        }

        private GenericResponse<IEnumerable<object>> Validate(Func<bool> validation, string propertyName,
            string message)
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

        public GenericResponse<IEnumerable<object>> ValidateTestSpecification(RolePlayerAssignmentRequest request)
        {
            var testSession = _testSessionQueryService.GetTestSessionById(request.TestSessionId).Result;
            if (!testSession.RehearsalDateTime.HasValue)
            {
                return Validate(() => false, nameof(request.TestSpecificationId),
                    Naati.Resources.Shared.RehearsalDateHasNotBeenSet);
            }

            return Validate(() => request.TestSpecificationId > 0, nameof(request.TestSpecificationId),
                Naati.Resources.TestSession.SelectTestSpecificationMessage);

        }

        public GenericResponse<IEnumerable<object>> ValidateLanguage(RolePlayerAssignmentRequest request)
        {
            return Validate(() => request.SkillId > 0, nameof(request.SkillId),
                Naati.Resources.TestSession.SelectLanguageMessage);
        }

        public GenericResponse<IEnumerable<object>> ValidateRolePlayersAllocation(RolePlayerAssignmentRequest request)
        {
            var errors = new List<object>();

            if (!request.RolePlayers?.Any() ?? false)
            {
                errors.Add(new
                {
                    FieldName = nameof(request.RolePlayers),
                    Message = "Role-players must be assgined"
                });

                return new GenericResponse<IEnumerable<object>>(errors);
            }

            foreach (var rolePlayer in request.RolePlayers.Where(x => x.Details.Any()))
            {
                foreach (var rolePlayerDetails in rolePlayer.Details)
                {
                    if (rolePlayerDetails.RolePlayerRoleTypeId == 0 || rolePlayerDetails.TestComponentId == 0)
                    {
                        errors.Add(new
                        {
                            FieldName = nameof(request.RolePlayers),
                            Message =
                            $"{rolePlayerDetails.RolePlayerRoleTypeId} and {rolePlayerDetails.TestComponentId} must be set"
                        });
                    }
                }
            }

            return new GenericResponse<IEnumerable<object>>(errors);

        }

        public GenericResponse<IEnumerable<TestSpecificationLookupTypeModel>> GetTestSpecificationsByCredentialTypeId(int credentialTypeId)
        {
            var results = _testSpecificationQueryService.GetTestSpecificationByCredentialTypeId(
                new SpecificationByCredentialTypeRequest() { CredentialTypeId = credentialTypeId }).List;

            var data = results.Select(y =>
            {
                var postFix = y.Active ? string.Empty : " - (Inactive)";
                return new TestSpecificationLookupTypeModel()
                {
                    Id = y.Id,
                    DisplayName = $"{y.Description}{postFix}",
                    Active = y.Active
                };
            });

            return new GenericResponse<IEnumerable<TestSpecificationLookupTypeModel>>(data);
        }
    }
}
