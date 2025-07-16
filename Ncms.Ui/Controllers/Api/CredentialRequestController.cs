using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.CredentialRequest;
using Ncms.Ui.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using IApplicationService = Ncms.Contracts.IApplicationService;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/credentialrequest")]
    public class CredentialRequestController : BaseApiController
    {
        private readonly IApplicationService _applicationService;
        private readonly ITestService _testService;
        private readonly ITestSessionService _testSessionService;
        private readonly IApplicationWizardLogicService _applicationWizardLogicService;

        public CredentialRequestController(IApplicationService applicationService, ITestService testService, ITestSessionService testSessionService, IApplicationWizardLogicService applicationWizardLogicService)
        {
            _applicationService = applicationService;
            _testService = testService;
            _testSessionService = testSessionService;
            _applicationWizardLogicService = applicationWizardLogicService;
        }

        [HttpGet]
        [Route("summaries")]
        [NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.CredentialRequest)]
        public HttpResponseMessage GetSummaries([FromUri] CredentialRequestSummarySearchRequest request)
        {
            return this.CreateResponse(() => _applicationService.SearchCredentialRequestSummary(request));
        }

        [HttpPost]
        [Route("summary")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.CredentialRequest)]
        public HttpResponseMessage PostSummary(CredentialRequestBulkActionRequest request)
        {
            return this.CreateResponse(() => _applicationService.GetCredentialRequestSummaryItem(request).Data);
        }

        [HttpGet]
        [Route("steps")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.Application)]
        public HttpResponseMessage GetSteps([FromUri]CredentialRequestBulkActionRequest request)
        {
            return this.CreateResponse(() => _applicationWizardLogicService.GetCredentialRequestWizardSteps(request.Action, request.CredentialApplicationTypeId, request.CredentialRequestStatusTypeId));
        }

        [HttpGet]
        [Route("testsession")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetTestSession([FromUri]CredentialRequestBulkActionRequest request)
        {
            return this.CreateResponse(() => _testSessionService.GetActiveTestSessions(request));
        }

        [HttpPost]
        [Route("testsession")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.Application)]
        public HttpResponseMessage PostTestSession(TestSessionRequestModel request)
        {
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                if (request.Id > 0)
                {
                    return sucessResponse;
                }

                var validationErrors = _testSessionService.ValidateTestSession(request).Data.ToList();
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
        [Route("emailpreview")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.Application)]
        public HttpResponseMessage PostWizardEmailPreview(dynamic request)
        {
            try
            {
                var wizardModel = new CredentialRequestsBulkActionWizardModel { Data = request };
                return this.CreateResponse(() => _applicationService.GetCredentialRequestBulkActionEmailPreview(wizardModel));
               
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("wizard")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.Application)]
        public HttpResponseMessage PostWizard(dynamic request)
        {
            try
            {
                var wizardModel = new CredentialRequestsBulkActionWizardModel { Data = request };
                return
                    this.CreateResponse(() => _applicationService.PerformCredentialRequestsBulkAction(wizardModel));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpGet]
        [Route("actions")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.CredentialRequest)]
        public HttpResponseMessage GetActions([FromUri]CredentialRequestBulkActionRequest request)
        {
            return this.CreateResponse(() => _applicationWizardLogicService.GetValidCredentialRequestSummaryActions(request.CredentialRequestStatusTypeId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Venue)]
        [Route("venues")]
        public HttpResponseMessage GetVenues([FromUri] CredentialRequestBulkActionRequest request)
        {
            return this.CreateResponse(() => _testService.GetVenues(request.TestLocationId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Venue)]
        [Route("venuesShowingInactive")]
        public HttpResponseMessage GetVenuesShowingInactive([FromUri] CredentialRequestBulkActionRequest request)
        {
            return this.CreateResponse(() => _testService.GetVenuesShowingInactive(request.TestLocationId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Venue)]
        [Route("activeVenues")]
        public HttpResponseMessage GetActiveVenues([FromUri] CredentialRequestBulkActionRequest request)
        {
            return this.CreateResponse(() => _testService.GetVenues(request.TestLocationId,true));
        }

        [HttpGet]
        [Route("existingapplicants")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.Application)]
        public HttpResponseMessage GetExistingApplicants([FromUri] CredentialRequestBulkActionRequest request)
        {
            
            return this.CreateResponse(() => request.TestSessionId == 0 ? new TestSessionApplicantModel[] {} :
                _testSessionService.GetTestSessionById(request.TestSessionId).Data.Applicants);
        }

        [HttpGet]
        [Route("newapplicants")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.TestSession)]
        public HttpResponseMessage GetNewApplicants([FromUri] CredentialRequestBulkActionRequest request)
        {
            return this.CreateResponse(() => _applicationService.GetCredentialRequestSummaryItem(request).Data?.Applicants?? Enumerable.Empty<CredentialRequestApplicantModel>());
        }

        [HttpPost]
        [Route("notes")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.Application)]
        public HttpResponseMessage ValidateNotes(dynamic request)
        {
            var result = new
            {
                InvalidFields = new List<object>()
            };

            try
            {
                var rules = _applicationWizardLogicService.GetNoteFieldRulesByApplicationTypeId((int)request.Action, (int)request.CredentialApplicationTypeId).Data;

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
        [Route("noterules")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.Application)]
        public HttpResponseMessage PostNoteFieldRules(CredentialRequestBulkActionRequest request)
        {
            return this.CreateResponse(() => _applicationWizardLogicService.GetNoteFieldRulesByApplicationTypeId(request.Action, request.CredentialApplicationTypeId));
        }

        [HttpPost]
        [Route("newapplicants")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.Application)]
        public HttpResponseMessage PostNewApplicants(CredentialRequestBulkActionRequest request)
        {
            try
            {
                var validationErrors = _testSessionService.ValidateNewTestSessionApplicants(request).Data.ToList();
                return this.CreateResponse(() => new { InvalidFields =  validationErrors });
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        [Route("checkOptionMessage")]
        public HttpResponseMessage GetConfirmationMessage()
        {
            return this.CreateResponse(() => _applicationService.GetCheckOptionMessage());
        }
    }
}
