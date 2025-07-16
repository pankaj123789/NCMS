using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.RolePlayer;
using Ncms.Contracts.Models.Test;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;
using IApplicationService = Ncms.Contracts.IApplicationService;
using IPersonService = Ncms.Contracts.IPersonService;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/testsession")]
    public class TestSessionController : BaseApiController
    {
        private readonly ITestSessionService _testSessionService;
        private readonly ITestService _testService;
        private readonly IApplicationWizardLogicService _wizardLogicService;
        private readonly IApplicationService _applicationService;

        public TestSessionController(IApplicationService applicationService, IPersonService personService,
            ITestSessionService testSessionService, IApplicationWizardLogicService wizardLogicService, ITestService testService)
        {
            _applicationService = applicationService;
            _testSessionService = testSessionService;
            _wizardLogicService = wizardLogicService;
            _testService = testService;
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage Get([FromUri] TestSessionSearchRequest request)
        {
            return this.CreateSearchResponse(() => _testSessionService.List(request));
        }

        [HttpGet]
        [Route("{testsessionId}/validateTestSitting")]
        [NcmsAuthorize(SecurityVerbName.Validate, SecurityNounName.TestSitting)]
        public HttpResponseMessage ValidateTestSitting(int testSessionId)
        {
            return this.CreateResponse(() => _testSessionService.ValidatestTestSitting(testSessionId));
        }

        [HttpGet]
        [Route("{testsessionId}/topsection")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetTopSection(int testSessionId)
        {
            //TODO TEST SESSION WIZARD: Add and fills the missed properties on TestSessionDetails
            //UI is expecting: Id, Name, TestLocationId, VenueId, Capacity, ApplicationTypeId, CredentialTypeId, AllowSelfAssign, TestTime, TestDate, PreparationTime, SessionDuration
            return this.CreateResponse(() => _testSessionService.GetTestSessionDetailsById(testSessionId));
        }

        [HttpGet]
        [Route("{testsessionId}/barChart")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetSkillAttendeesCount(int testSessionId)
        {
            return this.CreateResponse(() => _testSessionService.GetSkillAttendeesCount(testSessionId));
        }


        [HttpGet]
        [Route("{testsessionId}/markascomplete")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.TestSitting)]
        public HttpResponseMessage MarkAsComplete(int testSessionId)
        {
            return this.CreateResponse(() => _testSessionService.MarkAsCompleteTestSession(testSessionId));
        }

        [HttpGet]
        [Route("{testsessionId}/reopentestsession")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.TestSitting)]
        public HttpResponseMessage ReopenTestSession(int testSessionId)
        {
            return this.CreateResponse(() => _testSessionService.ReopenTestSession(testSessionId));
        }

        [HttpGet]
        [Route("{testsessionId}/applicants")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetApplicantsByTestSessionId(int testSessionId)
        {
            return this.CreateResponse(() => _testSessionService.GetApplicantsByTestSessionId(testSessionId, false));
        }

        [HttpGet]
        [Route("{testsessionId}/allAppplicants")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetAllApplicantsByTestSessionId(int testSessionId)
        {
            return this.CreateResponse(() => _testSessionService.GetApplicantsByTestSessionId(testSessionId, true));
        }

        [HttpGet]
        [Route("{testsessionId}/applicantsbytestsitting")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSitting)]
        public HttpResponseMessage GetApplicantsByTestsitting(int testSessionId)
        {
            var list = new[]
              {
                (int)CredentialRequestStatusTypeName.TestSessionAccepted,
                (int)CredentialRequestStatusTypeName.CheckedIn,
                (int)CredentialRequestStatusTypeName.TestSat,
                (int)CredentialRequestStatusTypeName.TestFailed,
                (int)CredentialRequestStatusTypeName.CertificationIssued,
                (int)CredentialRequestStatusTypeName.IssuedPassResult,
                (int)CredentialRequestStatusTypeName.ProcessingPaidReviewInvoice,
                (int)CredentialRequestStatusTypeName.UnderPaidTestReview,
                (int)CredentialRequestStatusTypeName.AwaitingPaidReviewPayment,
            };
            var response = _testSessionService.GetApplicantsByTestSessionId(testSessionId, false).Data.Where(x => list.Contains(x.Status));

            return this.CreateResponse(() => response);
        }

        [HttpGet]
        [Route("actions")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.TestSession)]
        public HttpResponseMessage GetActions([FromUri]int testSittingId)
        {
            return this.CreateResponse(() => _wizardLogicService.GetValidTestSittingActions(testSittingId));
        }


        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage Get(int id)
        {
            return this.CreateResponse(() => _testSessionService.GetTestSessionById(id));
        }

        /// <summary>
        /// Lists lookup that make up the Application structure. This action is used to build dropdown elements
        /// </summary>
        /// <param name="GetVenues">List type (Credential Request Type, Credential Application Type, etc)</param>
        /// <returns>List of lookup types in simple format (Id, DisplayName)</returns>
        [HttpGet]
        [Route("GetVenues")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Venue)]
        public HttpResponseMessage GetVenues()
        {
            return this.CreateResponse(() => _testService.GetAllVenues());
        }

        /// <summary>
        /// Update an Application 
        /// </summary>
        /// <param name="model">Update TestSession</param>
        /// <returns>update model</returns>
        [HttpPost]
        [Route("update")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSession)]
        public HttpResponseMessage UpdateTestSession(TestSessionModel model)
        {
            model.TestDate = UpdateTestDateTime(model.TestDate, model.TestTime);
            return this.CreateResponse(() => _testSessionService.UpdateTestSession(model));
        }

        [HttpGet]
        [Route("wizard/steps")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSession)]
        public HttpResponseMessage GetWizardSteps([FromUri]int? testSessionId = null)
        {
            return this.CreateResponse(() => _wizardLogicService.GetTestSessionWizardSteps(testSessionId));

        }

        [HttpGet]
        [Route("wizard/skills")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetWizardSkills([FromUri]TestSessionDetails request)
        {
            return this.CreateResponse(() => _testSessionService.GetTestSessionSkills(request.Id.GetValueOrDefault(), request.CredentialTypeId));
        }

        [HttpPost]
        [Route("wizard/getmatchingapplicants")]
        [NcmsAuthorize(SecurityVerbName.Update , SecurityNounName.TestSession)]
        public HttpResponseMessage GetWizardMatchingApplicants(TestSessionRequest request)
        {
            return this.CreateResponse(() => _testSessionService.GetCredentialRequestApplicants(request));
        }

        [HttpGet]
        [Route("wizard/notes")]
        [NcmsAuthorize( SecurityVerbName.Update , SecurityNounName.TestSession)]
        public HttpResponseMessage GetWizardNotes([FromUri]TestSessionDetails request)
        {
            return this.CreateResponse(() => _wizardLogicService.GetNoteFieldRulesForAllocateTestSessionAction(request.ApplicationTypeId));
        }

        [HttpPost]
        [Route("wizard/previewemail")]
        [NcmsAuthorize(SecurityVerbName.Update , SecurityNounName.TestSession)]
        public HttpResponseMessage GetWizardPreviewEmail(dynamic request)
        {
            try
            {
                var wizardModel = new TestSessionBulkActionWizardModel { Data = request };
                return this.CreateResponse(() => _testSessionService.GetCreateOrUpdateTestSessionEmailPreview(wizardModel));

            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("wizard/details")]
        [NcmsAuthorize(SecurityVerbName.Validate , SecurityNounName.TestSession)]
        public HttpResponseMessage PostWizardDetails(TestSessionDetails testSessionDetails)
        {
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _testSessionService.ValidateTestSession(testSessionDetails).Data.ToList();
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
        [Route("wizard/rehearsalDetails")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSession)]
        public HttpResponseMessage PostWizardRehearsalDetails(TestSessionDetails testSessionDetails)
        {
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _testSessionService.ValidateTestSessionRehearsal(testSessionDetails).Data.ToList();
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
        [Route("wizard/skills")]
        [NcmsAuthorize(SecurityVerbName.Update , SecurityNounName.TestSession)]
        public HttpResponseMessage PostWizardSkills(TestSessionSkillValidationRequest request)
        {
            try
            {
                var validationModel = _testSessionService.ValidateTestSessionSkills(request).Data;
                return this.CreateResponse(() => new { InvalidFields = validationModel.Errors, WarningFields = validationModel.Warnings });
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpGet]
        [Route("wizard/canShowStep")]
        [NcmsAuthorize(SecurityVerbName.Update , SecurityNounName.TestSession)]
        public HttpResponseMessage CanShowStep([FromUri]SessionStepValidationModel request)
        {
            try
            {
                return this.CreateResponse(() => _wizardLogicService.CanShowSessionStep(request).Data);
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }


        [HttpPost]
        [Route("wizard/matchingapplicants")]
        [NcmsAuthorize(SecurityVerbName.Update , SecurityNounName.TestSession)]
        public HttpResponseMessage PostWizardMatchingApplicants(dynamic applicants)
        {
            var result = new
            {
                InvalidFields = new List<object>()
            };

            return this.CreateResponse(() => result);
        }

        [HttpPost]
        [Route("wizard/notes")]
        [NcmsAuthorize(SecurityVerbName.Update , SecurityNounName.TestSession)]
        public HttpResponseMessage PostWizardNotes(dynamic request)
        {
            var result = new
            {
                InvalidFields = new List<object>()
            };

            try
            {
                if (request.ApplicationTypeId == null)
                {
                    return this.CreateResponse(() => result);
                }

                var rules = _wizardLogicService.GetNoteFieldRulesForAllocateTestSessionAction((int)request.ApplicationTypeId.Value).Data;

                if (rules.RequirePublicNote && String.IsNullOrWhiteSpace(request.PublicNote?.Value))
                {
                    result.InvalidFields.Add(new
                    {
                        FieldName = "PublicNote",
                        Message = Naati.Resources.Shared.RequiredFieldValidationError
                    });
                }
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }

            return this.CreateResponse(() => result);
        }

        [HttpPost]
        [NcmsAuthorize( SecurityVerbName.Update , SecurityNounName.TestSession)]
        [Route("wizard")]
        public HttpResponseMessage PostWizard(dynamic request)
        {
            try
            {
                var wizardModel = new TestSessionBulkActionWizardModel() { Data = request };
                return this.CreateResponse(() => _testSessionService.CreateOrUpdateTestSession(wizardModel));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        #region Allocate Role-players
        [HttpGet]
        [Route("allocateroleplayerswizard/steps")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetAllocateRolePlayersWizardSteps([FromUri]int? testSessionId = null)
        {
            return this.CreateResponse(() => _wizardLogicService.GetAllocateRolePlayersSteps(testSessionId));
        }
        

        [HttpPost]
        [Route("allocateroleplayerswizard/AvailableAndExistingRolePlayers")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.RolePlayer)]
        public HttpResponseMessage GetGetAvailableAndExistingRolePlayers(RolePlayerAssignmentRequest request)
        {
            return this.CreateResponse(() => _testSessionService.GetAvailableAndExistingRolePlayers(request));
        }

        [HttpPost]
        [Route("allocateroleplayerswizard/getAllocationDetails")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.RolePlayer)]
        public HttpResponseMessage GetRolePlayerAllocationDetails(RolePlayerAssignmentRequest request)
        {
            return this.CreateResponse(() => _testSessionService.GetRolePlayerAllocationDetails(request));
        }

        [HttpPost]
        [Route("allocateroleplayerswizard/allocateroleplayers")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.RolePlayer)]
        public HttpResponseMessage PostAllocateRolePlayersWizardAllocateRolePlayers(RolePlayerAssignmentRequest request)
        {
            return this.CreateResponse(() => _testSessionService.ValidateRolePlayersAllocation(request));
        }

        [HttpPost]
        [Route("allocateroleplayerswizard/previewemail")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.RolePlayer)]
        public HttpResponseMessage GetAllocateRolePlayersPreviewEmail(dynamic data)
        {
            try
            {
                return this.CreateResponse(() => _testSessionService.GetAllocateRolePlayerEmailPreview(new RolePlayerBulkAssignmentWizard() { Data = data }));

            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("allocateroleplayerswizard")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSession)]
        public HttpResponseMessage PostAllocateRolePlayersWizard(dynamic data)
        {
            return this.CreateResponse(() => _testSessionService.ExecuteRolePlayersAction(new RolePlayerBulkAssignmentWizard() { Data = data }));
        }

        [HttpPost]
        [Route("allocateroleplayerswizard/getTestSpecifications")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.RolePlayer)]
        public HttpResponseMessage GetTestSpecifications(RolePlayerAssignmentRequest request)
        {
            return this.CreateResponse(() => _testSessionService.GetTestSpecificationDetails(request));
        }

        [HttpPost]
        [Route("allocateroleplayerswizard/testSpecifications")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.RolePlayer)]
        public HttpResponseMessage ValidateTestSpecification(RolePlayerAssignmentRequest request)
        {
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _testSessionService.ValidateTestSpecification(request).Data.ToList();
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
        [Route("allocateroleplayerswizard/getTestSessionSkills")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.RolePlayer)]
        public HttpResponseMessage GetLanguages(RolePlayerAssignmentRequest request)
        {
            return this.CreateResponse(() => _testSessionService.GetLanguageDetails(request));
        }

        [HttpPost]
        [Route("allocateroleplayerswizard/testSessionSkill")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.RolePlayer)]
        public HttpResponseMessage ValidateLanguage(RolePlayerAssignmentRequest request)
        {
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _testSessionService.ValidateLanguage(request).Data.ToList();
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
        #endregion

        [HttpGet]
        [Route("credentialtypes")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSession)]
        public HttpResponseMessage GetCredentialTypes([FromUri]int applicationTypeId)
        {

            return this.CreateResponse(
                () => _applicationService.GetCredentialTypesForApplicationType(applicationTypeId));
        }

        private DateTime UpdateTestDateTime(DateTime testDate, string testTime)
        {
            var dateString = testDate.ToString("dd/MM/yyyy");
            var dateTimeObject = DateTime.ParseExact(dateString + " " + testTime + ":00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            return dateTimeObject;
        }

        [HttpPost]
        [Route("getCheckOptionMessage")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetConfirmationMessage(TestSessionDetails testSessionDetails)
        {
            return this.CreateResponse(() => _testSessionService.GetCheckOptionMessage(testSessionDetails));
        }

        [HttpPost]
        [Route("getAllocateRolePlayersCheckOptionMessage")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.RolePlayer)]
        public HttpResponseMessage GetAllocateRolePlayersConfirmationMessage(TestSessionDetails testSessionDetails)
        {
            return this.CreateResponse(() => _testSessionService.GetAllocateRolePlayersCheckOptionMessage(testSessionDetails));
        }

        [HttpGet]
        [Route("viewRolePlayers/roleplayers")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.RolePlayer)]
        public HttpResponseMessage GetTestSessionRoleplayers([FromUri]TestSessionRolePlayersRequest request)
        {
            return this.CreateResponse(() => _testSessionService.GetTestSessionRolePlayers(request));
        }

        [HttpGet]
        [Route("viewRolePlayers/{lastSystemActionTypeId}/testSessionRolePLayerActions")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.RolePlayer)]
        public HttpResponseMessage GetTestSessionRoleplayersActions(int lastSystemActionTypeId)
        {
            return this.CreateResponse(() => _testSessionService.GetValidRolePlayerActions((SystemActionTypeName)lastSystemActionTypeId));
        }
        [HttpGet]
        [Route("testSpecificationByCredentialType/{credentialTypeId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetTestSpecificationByCredentialTypeId(int credentialTypeId)
        {
            return this.CreateResponse(() => _testSessionService.GetTestSpecificationsByCredentialTypeId(credentialTypeId).Data);
        }


        [HttpGet]
        [Route("viewRolePlayers/{testSessionId}/testSessionSkills")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetTestSessionSkills(int testSessionId)
        {
            return this.CreateResponse(() => _testSessionService.GetSelectedTestSessionSkills(testSessionId).Data);
        }

        [HttpGet]
        [Route("{testSessionId}/testSessionSkillDetails")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetTestSessionSkillsDetails(int testSessionId)
        {
            return this.CreateResponse(() => _testSessionService.GetSelectedTestSessionSkillDetails(testSessionId).Data);
        }

        [HttpGet]
        [Route("viewRolePlayers/{testSessionId}/{skillId}/skillDetails")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetTestSessionSkillDetails(int testSessionId, int skillId)
        {
            return this.CreateResponse(() => _testSessionService.GetRolePlayerAllocationTasks(testSessionId, skillId).Data);
        }

        [HttpPost]
        [Route("viewRolePlayers/Update")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.TestSession)]
        public HttpResponseMessage GetTestSessionSkillDetails(UpdateRolePLayersRequest request)
        {
            return this.CreateResponse(() => _testSessionService.UpdateRolePlayers(request));
        }

        [HttpPost]
        [Route("allocatePastTestSession")]
        [NcmsAuthorize(SecurityVerbName.AssignPastSession, SecurityNounName.Application)]
        public HttpResponseMessage AllocatePastTestSession(AllocateTestSessionRequest request)
        {
            request.UserId = CurrentUser.Id;
            return this.CreateResponse(() => _testSessionService.AllocatePastTestSession(request));
        }

    }
}