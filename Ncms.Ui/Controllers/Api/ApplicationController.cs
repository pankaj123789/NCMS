using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Bl.Refund;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Common;
using Ncms.Contracts.Models.CredentialPrerequisite;
using Ncms.Contracts.Models.User;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;
using Newtonsoft.Json;
using SearchRequest = Ncms.Contracts.SearchRequest;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/application")]
    public class ApplicationController : BaseApiController
    {
        private readonly ISystemService _systemService;
        private readonly INoteService _noteService;
        private readonly IApplicationService _applicationService;
        private readonly IPersonService _personService;
        private readonly IEmailMessageService _emailMessageService;
        private readonly IUserService _userService;
        private readonly IApplicationWizardLogicService _wizardLogicService;
        private readonly ITestResultService _testResultService;
        private readonly IAccountingService _accountingService;
        private readonly ICredentialService _credentialService;
        private readonly IRefundCalculator _refundCalculator;
        private readonly ICredentialPrerequisiteService _credentialPrerequisiteService;

        public ApplicationController(INoteService noteService, IApplicationService applicationService, IPersonService personService,
            IEmailMessageService emailMessageService, IUserService userService, IApplicationWizardLogicService wizardLogicService,
            ISystemService systemService, ITestResultService testResultService, IAccountingService accountingService, ICredentialService credentialService,
            IRefundCalculator refundCalculator, ICredentialPrerequisiteService credentialPrerequisiteService)
        {
            _noteService = noteService;
            _applicationService = applicationService;
            _personService = personService;
            _emailMessageService = emailMessageService;
            _userService = userService;
            _wizardLogicService = wizardLogicService;
            _systemService = systemService;
            _testResultService = testResultService;
            _accountingService = accountingService;
            _credentialService = credentialService;
            _refundCalculator = refundCalculator;
            _credentialPrerequisiteService = credentialPrerequisiteService;
        }

        /// <summary>
        /// Get the Application details
        /// </summary>
        /// <param name="id">Application Id</param>
        /// <returns>Application Data</returns>
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage Get(int id)
        {
            return this.CreateResponse(() => _applicationService.GetApplication(id));
        }

        /// <summary>
        /// Search for Applications filtering by request parameters
        /// </summary>
        /// <param name="request">Filters</param>
        /// <returns>List of applicaitons in simple format</returns>
        [HttpPost]
        [Route("search")]
        [NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.Application)]
        public HttpResponseMessage Get(SearchRequest request)
        {
            return this.CreateResponse(() => _applicationService.Search(request));
        }

        /// <summary>
        /// Lists lookup that make up the Application structure. This action is used to build dropdown elements
        /// </summary>
        /// <param name="lookupType">List type (Credential Request Type, Credential Application Type, etc)</param>
        /// <returns>List of lookup types in simple format (Id, DisplayName)</returns>
        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.General)]
        [Route("LookupType/{lookupType}")]
        public HttpResponseMessage GetDynamicLookupType(string lookupType)
        {
            return this.CreateResponse(() => _applicationService.GetLookupType(lookupType));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        [Route("credentialapplicationtype")]
        public HttpResponseMessage GetDynamicLookupType([FromUri]IEnumerable<int> skillTypeIds)
        {
            return this.CreateResponse(() => _applicationService.GetCredentialApplicaionTypes(skillTypeIds));
        }

        /// <summary>
        /// Lists all email message of credential application id
        /// </summary>
        /// <param name="credentialApplicationId">Credential Application Id</param>
        /// <returns>List all email messages</returns>
        [HttpGet]
        [Route("listEmail/{credentialApplicationId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetEmailMessageByApplicationId(int credentialApplicationId)
        {
            return this.CreateResponse(() => _emailMessageService.GetEmailMessageByCredentialApplicationId(credentialApplicationId));
        }

        /// <summary>
        /// Get details of email message by id
        /// </summary>
        /// <param name="emailMessageId">Credential Application Id</param>
        /// <returns>details of email message</returns>
        [HttpGet]
        [Route("email/{emailMessageId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetEmailMessageById(int emailMessageId)
        {
            return this.CreateResponse(() => _emailMessageService.GetGenericEmailMessage(emailMessageId));
        }

        /// <summary>
        /// Check Duplicated Application
        /// </summary>
        /// <param name="request">Model on this format: 
        /// {
        ///     naatiNumber: { naatiNumber}
        ///     ApplicationTypeId: { ApplicationTypeId}
        /// }
        /// </param>
        /// <returns>true or false</returns>
        [HttpPost]
        [Route("duplicatedApplication")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage CheckDuplicatedApplication(dynamic request)
        {
            try
            {
                var naatiNumber = (int)request.NaatiNumber;
                var typeId = (int)request.ApplicationTypeId;
                return this.CreateResponse(() => _applicationService.CheckDuplicatedApplication(naatiNumber, typeId));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        /// <summary>
        /// Send Email Message
        /// </summary>
        /// <param name="request">Model on this format: 
        /// {
        ///     emailMessageId: { emailMessageId}
        /// }
        /// </param>
        /// <returns>Send Email Message</returns>
        [HttpPost]
        [Route("sendEmail")]
        [NcmsAuthorize(SecurityVerbName.Send, SecurityNounName.Email)]
        public HttpResponseMessage SendEmailMessage(dynamic request)
        {
            try
            {
                var emailMessageId = (int)request.emailMessageId;
                return this.CreateResponse(() => _emailMessageService.SendEmailMessage(emailMessageId));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpGet]
        [Route("emailDocuments/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Email)]
        public HttpResponseMessage EmailDocumentsGet([FromUri]int id)
        {
            return this.CreateSearchResponse(() => _emailMessageService.ListAttachments(id));
        }

        /// <summary>
        /// Get fields that make up an Application
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        /// <returns>Fields from that Application</returns>
        [HttpGet]
        [Route("{applicationId}/header")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetHeader(int applicationId)
        {
            return this.CreateResponse(() => _applicationService.GetApplicationFieldsData(applicationId));
        }

        /// <summary>
        /// List the credential requests added to an application
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        /// <returns>List of Credential Request. Each Credential Request should have own Fields and appliable Actions</returns>
        [HttpGet]
        [Route("{applicationId}/credentialrequests")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetCredentialRequests(int applicationId)
        {
            return this.CreateResponse(() => _applicationService.GetCredentialRequests(applicationId));
        }
        /// <summary>
        /// List the credential requests added to an application
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        /// <returns>List of Credential Request. Each Credential Request should have own Fields and appliable Actions</returns>
        [HttpGet]
        [Route("{applicationId}/credentialotherrequests")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetOtherCredentialRequests(int applicationId)
        {
            return this.CreateResponse(() => _applicationService.GetOtherCredentialRequests(applicationId));
        }

        [HttpGet]
        [Route("credentialSearch")]
        [NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.Credential)]
        public HttpResponseMessage GetCredentialSearch([FromUri] CredentialSearchRequest request)
        {
            return this.CreateResponse(() => _credentialService.Search(request));
        }

        [HttpGet]
        [Route("credentialPhotoExcel")]
        [NcmsAuthorize(SecurityVerbName.Download, SecurityNounName.Credential)]
        public HttpResponseMessage GetPhotosAndExcel([FromUri] CredentialSearchRequest request)
        {
            return this.CreateResponse(() => _credentialService.GetPhotosAndExcel(request));
        }

        /// <summary>
        /// Gets all credential applications for a specific credential
        /// </summary>
        /// <param name="credentialId">credentialId</param>
        /// <returns>List of Credential Applications</returns>
        [HttpGet]
        [Route("applicationsforcredential/{credentialId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage ApplicationsForCredential(int credentialId)
        {
            return this.CreateResponse(() => _applicationService.ApplicationsForCredential(credentialId));
        }

        /// <summary>
        /// List the notes from an Application
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        /// <returns>List of Notes from that Application</returns>
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        [Route("{applicationId}/notes")]
        public HttpResponseMessage GetNotes([FromUri]int applicationId)
        {
            return this.CreateResponse(() => _noteService.ListApplicationNotes(applicationId).OrderByDescending(x => x.CreatedDate));
        }

        /// <summary>
        /// Add or update a Note from an Application
        /// </summary>
        /// <param name="model">Note</param>
        /// <returns>Id from the added/updated Note</returns>
        [HttpPost]
        [Route("notes")]
        [NcmsAuthorize(new[] { SecurityVerbName.Create, SecurityVerbName.Update }, SecurityNounName.Notes)]
        public HttpResponseMessage PostNotes(ApplicationNoteModel model)
        {
            model.UserId = CurrentUser.Id;
            return this.CreateResponse(() => _noteService.CreateApplicationNote(model));
        }

        /// <summary>
        /// Create a new Application
        /// </summary>
        /// <param name="model">Application</param>
        /// <returns>Id from Application, Credential Request, Notes and Sections added/updated</returns>
        [HttpPost]
        [Route("application")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage CreateApplication(UpsertApplicationRequestModel model)
        {
            return this.CreateResponse(() => _applicationService.CreateApplication(model));
        }

        /// <summary>
        /// Update an Application 
        /// </summary>
        /// <param name="model">Application</param>
        /// <returns>Id from Application, Credential Request, Notes and Sections added/updated</returns>
        [HttpPost]
        [Route("update")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage UpdateApplication(UpsertApplicationRequestModel model)
        {
            return this.CreateResponse(() => _applicationService.UpsertApplication(model));
        }

        /// <summary>
        /// Delete a Note
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        /// <param name="id">Note Id</param>
        /// <returns>The removed Note</returns>
        [HttpDelete]
        [Route("{id}/{applicationId}/notes")]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Notes)]
        public HttpResponseMessage DeleteNotes([FromUri]int id, [FromUri]int applicationId)
        {
            return this.CreateResponse(() => _noteService.DeleteApplicationNote(applicationId, id));
        }

        /// <summary>
        /// Get the Applicant data
        /// </summary>
        /// <param name="naatiNumber">NaatiNumber</param>
        /// <returns>Applicant data</returns>
        [HttpGet]
        [Route("applicant/{naatiNumber}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetApplicant(int naatiNumber)
        {
            return this.CreateResponse(() => _personService.GetPersonBasic(naatiNumber));
        }

        /// <summary>
        /// Get the Credential Request data
        /// </summary>
        /// <param name="credentialRequestId">Credential Request Id</param>
        /// <returns>Credential Request data</returns>
        [HttpGet]
        [Route("credentialrequest/{credentialrequestId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetCredentialRequest(int credentialRequestId)
        {
            return this.CreateResponse(() => _applicationService.GetCredentialRequest(credentialRequestId));
        }

        [HttpPost]
        [Route("credentialrequest")]
        [NcmsAuthorize(SecurityVerbName.Configure, SecurityNounName.Credential)]
        public HttpResponseMessage UpdateCredential(CredentialModel credential)
        {
            return this.CreateResponse(() => _applicationService.UpdateCredential(credential));
        }

        /// <summary>
        /// Get available steps from an Application / Action pair
        /// </summary>
        /// <param name="request">{ ApplicationId, ActionId, CredentialRequestId }</param>
        /// <returns>Available Steps</returns>
        [HttpGet]
        [Route("steps")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application, SecurityNounName.TestSitting)]
        public HttpResponseMessage GetSteps([FromUri]int applicationStatusId, [FromUri]int actionId, [FromUri]int applicationId, [FromUri]int credentialTypeId, [FromUri]int applicationTypeId, [FromUri]int credentialRequestId)
        {
            return this.CreateResponse(() => _wizardLogicService.GetWizardSteps(applicationStatusId, actionId, applicationId, applicationTypeId, credentialTypeId, credentialRequestId));
        }

        [HttpGet]
        [Route("confirmationMessage")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application, SecurityNounName.TestSitting)]
        public HttpResponseMessage GetConfirmationMessage([FromUri]int applicationId, [FromUri]int stepId, [FromUri]int credentialRequestId)
        {
            return this.CreateResponse(() => _wizardLogicService.GetConfirmationMessage((ApplicationWizardSteps)stepId, applicationId, credentialRequestId));
        }

        [HttpGet]
        [Route("stepMessage")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application, SecurityNounName.TestSitting)]
        public HttpResponseMessage GetStepMessage([FromUri] int action)
        {
            return this.CreateResponse(() => _wizardLogicService.GetStepMessage((SystemActionTypeName)action));
        }

        /// <summary>
        /// Finalize an Wizard process from an Application
        /// </summary>
        /// <param name="request">Model on this format: 
        /// {
        ///     Application: { ApplicationId, ...other simple data... },
        ///     CredentialRequest: { CredentialRequestId, ...other simple data... },
        ///     Action: { Id, Name },
        ///     Steps: [{ Id, Data }]
        /// }
        /// </param>
        /// <returns>Caller isn't expecting any specific response. Just an generic error response on case of fail</returns>
        [HttpPost]
        [Route("wizard")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage PostWizard(dynamic request)
        {
            try
            {
                var model = new ApplicationActionWizardModel
                {
                    ApplicationId = (int)request.ApplicationId.Value,
                    CredentialRequestId = (int)(request.CredentialRequestId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };
                return this.CreateResponse(() => _applicationService.PerformAction(model));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }


        /// <summary>
        /// Download an attachment from the email generated on "Preview Email and Attachment" step
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        /// <param name="wizardAction">Action Id</param>
        /// <param name="fileName">File Name</param>
        /// <returns>A stream based on FileModel class</returns>
        [HttpGet]
        [Route("downloademailattachment/{applicationId}/{wizardAction}/{fileName}")]
        [NcmsAuthorize(SecurityVerbName.Download, SecurityNounName.Application)]
        public HttpResponseMessage GetDownloadEmailAttachment([FromUri]int applicationId, [FromUri]int wizardAction, [FromUri]string fileName)
        {
            return this.FileStreamResponse(() => new FileModel());// todo: novemeber (possibly october)
        }

        /// <summary>
        /// Get the available actions to a specific Application
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        /// <returns>List of Actions on this structure - [{ Id, Name, IsExceptionFlow }]</returns>
        [HttpGet]
        [Route("action/{applicationId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetActions([FromUri]int applicationId)
        {
            return this.CreateResponse(() => _wizardLogicService.GetValidActions(applicationId));
        }

        /// <summary>
        /// Validate a taken Action. Check if exists any pre-condition to that
        /// </summary>
        /// <param name="request">{ ApplicationId, ActionId, CredentialRequestId }</param>
        /// <returns>
        /// List of messages on this format - { Messages: [{ Message, Field }] }
        /// If message has a linked Field: It will be highlighted in red and the message is displayed next to it
        /// If not: The message is displayed on a toastr.errorsteps
        /// </returns>
        [HttpPost]
        [Route("action")]
        //base authorisation of applicaiton manage is put here even though permissions are wide and veried for each action
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application, SecurityNounName.TestSitting, SecurityNounName.CredentialRequest)]
        public HttpResponseMessage ValidateAction(dynamic request)
        {
            try
            {
                var model = new ApplicationActionWizardModel
                {
                    ApplicationId = (int)request.ApplicationId.Value,
                    CredentialRequestId = (int)(request.CredentialRequestId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };
                return this.CreateResponse(() => _applicationService.ValidateActionPreconditions(model));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("fieldMandatories")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application, SecurityNounName.CredentialRequest)]
        public HttpResponseMessage ValidateFieldMandatories(CreatePrerequisiteRequest createPrerequisiteRequest)
        {
            createPrerequisiteRequest.EnteredUserId = CurrentUser.Id;
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _credentialPrerequisiteService.ValidateMandatoryFields(createPrerequisiteRequest).Data;

                return this.CreateResponse(() => new { validationErrors });

            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("documentMandatories")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application, SecurityNounName.CredentialRequest)]
        public HttpResponseMessage ValidateDocumentMandatories(CreatePrerequisiteRequest createPrerequisiteRequest)
        {
            createPrerequisiteRequest.EnteredUserId = CurrentUser.Id;
            try
            {
                var sucessResponse = this.CreateResponse(() => new { InvalidFields = Enumerable.Empty<object>() });

                var validationErrors = _credentialPrerequisiteService.ValidateMandatoryDocuments(createPrerequisiteRequest).Data;

                return this.CreateResponse(() => new { validationErrors });

            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        /// Methods that validate the steps from the wizard. All methods was developed to 
        /// Receive: { ApplicationId, ActionId, CredentialRequestId?, ...other specific step data... }
        /// Response: { InvalidFields: [{ FieldName, Message }] }
        /// The validate method will clarify just the "other specific step data"
        /// Note 1: I could to build a single action to validate all steps but you couldn't use a Model as request, in other words, 
        /// you would need to make an explicit casting (or mapping) to the correct Model (as occurrs on the Save method)
        /// Note 2: These validations aren't be done on UI because some fields are mandatory to a specific Action. I did understand that it's a 
        /// business rule (not a simple CRUD validation), so it shouldn't be done on UI

        /// <summary>
        /// Validate the "Note" step
        /// </summary>
        /// <param name="request">{ PublicNote, PrivateNote }</param>
        [HttpPost]
        [Route("wizard/notes")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application, SecurityNounName.TestSitting)]
        public HttpResponseMessage ValidateWizardNotes(dynamic request)
        {
            var result = new
            {
                InvalidFields = new List<object>()
            };

            try
            {
                var rules = _wizardLogicService.GetNoteFieldRules((int)request.Action, (int)request.ApplicationId).Data;

                if (rules.ShowPublicNote && rules.RequirePublicNote && String.IsNullOrWhiteSpace(request.PublicNote?.Value))
                {
                    result.InvalidFields.Add(new
                    {
                        FieldName = "PublicNote",
                        Message = Naati.Resources.Shared.RequiredFieldValidationError
                    });
                }

                if (rules.ShowPrivateNote && rules.RequirePrivateNote && String.IsNullOrWhiteSpace(request.PrivateNote?.Value))
                {
                    result.InvalidFields.Add(new
                    {
                        FieldName = "PrivateNote",
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

        /// <summary>
        /// Validate the "Select Credential" step
        /// </summary>
        /// <param name="request">{ Category, CredentialTypeId, Language1, Language2, Direction }</param>
        /// <returns></returns>
        [HttpPost]
        [Route("wizard/selectcredential")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage ValidateSelectcredential(object request)
        {
            var result = new
            {
                InvalidFields = new List<object>()
            };

            try
            {
                var categoryIdPropertyName = "CategoryId";
                var credentialTypeIdPropertyName = "CredentialTypeId";
                var skillIdPropertyName = "SkillId";
                var applicationIdPropertyName = "ApplicationId";
                var properties = JsonConvert.DeserializeObject<IDictionary<string, string>>(request.ToString());

                foreach (var field in new[] { categoryIdPropertyName, credentialTypeIdPropertyName, skillIdPropertyName })
                {
                    if (!properties.ContainsKey(field) || string.IsNullOrEmpty(properties[field]))
                    {
                        result.InvalidFields.Add(
                            new
                            {
                                FieldName = field,
                                Message = Naati.Resources.Shared.RequiredFieldValidationError
                            });
                    }
                }

                if (!result.InvalidFields.Any())
                {
                    var requestInProgress = _applicationService.HasValidCredentialRequest(
                        int.Parse(properties[applicationIdPropertyName]),
                        int.Parse(properties[categoryIdPropertyName]),
                        int.Parse(properties[credentialTypeIdPropertyName]),
                        int.Parse(properties[skillIdPropertyName]));

                    if (requestInProgress)

                    {
                        result.InvalidFields.Add(
                            new
                            {
                                FieldName = skillIdPropertyName,
                                Message = Naati.Resources.Shared.CredentialReaquestAlreadyExists
                            });
                    }
                }

            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }

            return this.CreateResponse(() => result);
        }

        /// <summary>
        /// Validate the "Issue Credential" step
        /// </summary>
        /// <param name="request">{ PractitionerNumber, StartDate, ExpiryDate, ShowInOnlineDirectory }</param>
        /// <returns></returns>
        [HttpPost]
        [Route("wizard/issuecredential")]
        [NcmsAuthorize(SecurityVerbName.Issue, SecurityNounName.CredentialRequest)]
        public HttpResponseMessage ValidateIssueCredential(dynamic request)
        {
            var properties = JsonConvert.DeserializeObject<IDictionary<string, string>>(request.ToString());
            var result = new
            {
                InvalidFields = new List<object>()
            };

            var mandatoryFields = new List<string> { "StartDate" };

            //Todo: improve this
            if (properties.ContainsKey("Certification") && Convert.ToBoolean(properties["Certification"]))
            {
                if (!properties.ContainsKey("SelectedCertificationPeriodId") || string.IsNullOrEmpty(properties["SelectedCertificationPeriodId"]))
                {
                    result.InvalidFields.Add(
                        new
                        {
                            FieldName = "SelectedCertificationPeriodId",
                            Message = Naati.Resources.Shared.RequiredFieldValidationError
                        });
                }
                else if (Convert.ToInt32(properties["SelectedCertificationPeriodId"]) == 0)
                {
                    mandatoryFields.AddRange(new[] { "CertificationPeriodStart", "CertificationPeriodEnd" });

                    IList<ValidationResultModel> errors = _applicationService.ValidateNewCertificationPeriod(
                        (string)properties["PractitionerNumber"],
                        Convert.ToDateTime(properties["CertificationPeriodStart"]),
                        Convert.ToDateTime(properties["CertificationPeriodEnd"]));

                    errors.ForEach(x =>
                        result.InvalidFields.Add(new
                        {
                            FieldName = x.Field,
                            Message = x.Message
                        }));
                }
            }


            foreach (var field in mandatoryFields)
            {
                if (!properties.ContainsKey(field) || string.IsNullOrEmpty(properties[field]) || Convert.ToDateTime(properties[field]) < MinDate.Value)
                {
                    result.InvalidFields.Add(
                        new
                        {
                            FieldName = field,
                            Message = Naati.Resources.Shared.RequiredFieldValidationError
                        });
                }
            }

            return this.CreateResponse(() => result);
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        [Route("wizard/supplementarytest")]
        public HttpResponseMessage ValidateSupplementaryTest(dynamic request)
        {
            var result = new
            {
                InvalidFields = new List<object>()
            };

            if (request.Components == null || request.Components.Count == 0)
            {
                result.InvalidFields.Add(
                    new
                    {
                        FieldName = "Components",
                        Message = Naati.Resources.Shared.NoComponentsSelected
                    });
            }

            return this.CreateResponse(() => result);
        }

        /// Methods that load data from some specific steps. All methods was developed to 
        /// Receive: { ApplicationId, ActionId, CredentialRequestId }
        /// Response: { ...specific step data... }
        /// The load method will clarify just the "specific step data" response

        /// <summary>
        /// Get default data to the "Issue Credential" step
        /// </summary>
        /// <returns>{ PractitionerNumber, StartDate, ExpiryDate, ShowInOnlineDirectory }</returns>
        [HttpGet]
        [Route("wizard/issuecredential")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage GetWizardIssueCredential([FromUri]int applicationId, [FromUri]int credentialRequestId, [FromUri]int actionId)
        {
            return this.CreateResponse(() => _applicationService.GetWizardIssueCredentialData(applicationId, credentialRequestId, actionId));
        }

        [HttpGet]
        [Route("wizard/credentialpreview")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage GetWizardCredentialPreview([FromUri]DocumentsPreviewRequestModel model)
        {
            return this.CreateResponse(() => _applicationService.GetIssueCredentialPreview(model));
        }

        /// <summary>
        /// Get preview Email and Attachments to an specific action
        /// </summary>
        /// <param name="request"></param>
        /// <returns>{ From, To, Subject, EmailContent, Attachments: [{ FileName, DocumentType, FileSize }] }</returns>
        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.PreviewEmail, SecurityNounName.Application)]
        [Route("wizard/emailpreview")]
        public HttpResponseMessage GetWizardEmailPreview(dynamic request)
        {
            try
            {
                var model = new ApplicationActionWizardModel
                {
                    ApplicationId = (int)request.ApplicationId.Value,
                    CredentialRequestId = (int)(request.CredentialRequestId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };

                var emailModels = _applicationService.GetEmailPreview(model).Data;


                var emails = emailModels.Select(x => new
                {
                    From = x.From,
                    To = x.RecipientEmail,
                    Subject = x.Subject,
                    EmailContent = x.Body,
                    Attachments = x.Attachments
                });

                return this.CreateResponse(() => emails);
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.PreviewEmail, SecurityNounName.Application)]
        [Route("wizard/emailTemplates")]
        public HttpResponseMessage GetWizardEmailCompose(dynamic request)
        {
            try
            {
                var model = new ApplicationActionWizardModel
                {
                    ApplicationId = (int)request.ApplicationId.Value,
                    CredentialRequestId = (int)(request.CredentialRequestId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };

                var emailTemplates = _applicationService.GetEmailTemplates(model).Data;

                return this.CreateResponse(() => emailTemplates);
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        /// <summary>
        /// Get preview Invoice to an specific action
        /// </summary>
        /// <param name="request"></param>
        /// <returns>{ InvoiceTo, DueDate, Branding, InvoiceReference, ReferenceText, Lines: [{ Code, Description, GLCode, ExGst, Gst, Total }] }</returns>
        [HttpPost]
        [Route("wizard/invoice")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage GetWizardInvoicePreview(dynamic request)
        {
            try
            {
                var model = new ApplicationActionWizardModel
                {
                    ApplicationId = (int)request.ApplicationId.Value,
                    CredentialRequestId = (int)(request.CredentialRequestId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };

                var invoiceModel = _applicationService.GetInvoicePreview(model).Data;

                var invoice = new
                {
                    InvoiceTo = invoiceModel.InvoiceTo,
                    DueDate = invoiceModel.DueDate,
                    Branding = invoiceModel.BrandingTheme,
                    InvoiceReference = "",
                    ReferenceText = "",
                    invoiceModel.UserOfficeAbbreviation,
                    Lines = invoiceModel.LineItems.Select(x =>
                        new
                        {
                            Code = x.ProductCode,
                            Description = x.Description,
                            GLCode = x.GlCode,
                            ExGst = x.GstApplies ? x.IncGstCostPerUnit * (10m / 11) : x.IncGstCostPerUnit,
                            Gst = x.GstApplies ? x.IncGstCostPerUnit / 11 : 0,
                            Total = x.IncGstCostPerUnit,
                        }).ToList()
                };

                return this.CreateResponse(() => invoice);
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpGet]
        [Route("wizard/supplementarytest/{credentialRequestId}")]
        [NcmsAuthorize(new[] { SecurityVerbName.Create, SecurityVerbName.Issue }, new[] { SecurityNounName.SupplementaryTest, SecurityNounName.TestResult })]
        public HttpResponseMessage GetSupplementaryTest([FromUri]int credentialRequestId)
        {
            return this.CreateResponse(() => _testResultService.GetMarks(credentialRequestId));
        }

        [HttpGet]
        [Route("wizard/selectcredentialrecertification")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage GetWizardSelectCredentialRecertification([FromUri]int applicationId, [FromUri]int naatiNumber)
        {
            return this.CreateResponse(() => _applicationService.CredentialsForRecertification(applicationId, naatiNumber));
        }

        [HttpGet]
        [Route("wizard/refundDetails")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage GetRefundDetails([FromUri]int credentialRequestId)
        {
            return this.CreateResponse(() => _refundCalculator.CalculateRefund(credentialRequestId));
        }

        /// <summary>
        /// Get which note fields to show for a give application type and action
        /// </summary>
        [HttpGet]
        [Route("noterules")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        public HttpResponseMessage GetNoteFieldRules([FromUri]int actionId, [FromUri]int applicationId)
        {
            return this.CreateResponse(() => _wizardLogicService.GetNoteFieldRules(actionId, applicationId));
        }

        /// <summary>
        /// Get valid credential categories for an application
        /// </summary>
        /// <returns>List of Credential Categories in the format [{ Id, DisplayName }]</returns>
        [HttpGet]
        [Route("categories")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetCategories([FromUri]int applicationId)
        {
            return this.CreateResponse(() => _applicationService.GetCredentialCategoriesForApplicationType(applicationId));
        }

        /// <summary>
        /// Get Credential Types based on ApplicationId and CategoryId
        /// </summary>
        /// <returns>List of Credential Types in the format [{ Id, DisplayName }]</returns>
        [HttpGet]
        [Route("credentialtype")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetCredentialType([FromUri]int applicationId, [FromUri]int categoryId)
        {
            return this.CreateResponse(() => _applicationService.GetCredentialTypesForApplication(applicationId, categoryId));
        }

        /// <summary>
        /// Get a list of skills for a crential type ID
        /// </summary>
        /// <returns>List of Skills in the format [{ Id, DisplayName }]</returns>
        [HttpGet]
        [Route("skill")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetSkills([FromUri]int[] credentialTypeIds, [FromUri]int[] credentialApplicationTypeIds)
        {
            return this.CreateResponse(() => _applicationService.GetSkillsForCredentialType(credentialTypeIds, credentialApplicationTypeIds));
        }

        [HttpGet]
        [Route("testtask")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetTestTask([FromUri]int[] credentialTypeIds)
        {
            return this.CreateResponse(() => _applicationService.GetTestTask(credentialTypeIds));
        }

        [HttpGet]
        [Route("documents/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage DocumentsGet([FromUri]int id)
        {
            return this.CreateSearchResponse(() =>
            {
                return _applicationService.ListAttachments(id);
            });
        }

        [Route("documents/upload")]
        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.Application)]
        public async Task<HttpResponseMessage> Upload()
        {
            return await this.ProcessMultipartFileData((Func<string, MultipartFileData, MultipartFormDataStreamProvider, int>)((fileName, fileData, provider) =>
            {
                var applicationId = Convert.ToInt32(provider.FormData["applicationId"]);
                var id = Convert.ToInt32(provider.FormData["id"]);
                var type = provider.FormData["type"];
                var title = provider.FormData["title"];
                var storedFileId = Convert.ToInt32(provider.FormData["storedFileId"]);

                var user = _userService.Get() ?? new UserModel();
                var request = new CredentialApplicationAttachmentModel
                {
                    FileName = fileName,
                    Type = (StoredFileType) Enum.Parse(typeof(StoredFileType), type),
                    FilePath = fileData.LocalFileName,
                    UploadedByUserId = user.Id,
                    StoragePath = $@"{type}\{applicationId}\{fileName}",
                    StoredFileId = storedFileId,
                    CredentialApplicationId = applicationId,
                    CredentialApplicationAttachmentId = id,
                    Title = title
                };

                return _applicationService.CreateOrReplaceAttachment(request);
            }));
        }

        [HttpPost]
        [Route("documents")]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.Application)]
        public HttpResponseMessage DocumentsPost(CredentialApplicationAttachmentModel request)
        {
            var user = _userService.Get() ?? new UserModel();
            request.UploadedByUserId = user.Id;
            return this.CreateResponse(() => _applicationService.CreateOrReplaceAttachment(request));
        }

        [HttpDelete]
        [Route("documents/{id}")]
        //using upload here instead of delete
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Application)]
        public HttpResponseMessage DocumentsDelete([FromUri]int id)
        {
            return this.CreateResponse(() => _applicationService.DeleteAttachment(id));
        }

        [HttpGet]
        [Route("documentTypes/{applicationId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.General)]
        public HttpResponseMessage GetDocumentTypesByCredentialType([FromUri] int applicationId)
        {
            return this.CreateResponse(() => _applicationService.GetDocumentTypesForApplicationType(applicationId));
        }

        [HttpGet]
        [Route("exportApplications")]
        [NcmsAuthorize(SecurityVerbName.Download, SecurityNounName.Application)]
        public HttpResponseMessage ExportApplications([FromUri]ApplicationExport request)
        {
            return this.FileStreamResponse(() => _applicationService.ExportApplicationsExcel(request));
        }

        [HttpPost]
        [Route("outstandingInvoices")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Invoice)]
        public HttpResponseMessage UpdateOutstandingInvoices(dynamic request)
        {
            var model = new UpdateOutstandingInvoicesRequestModel
            {
                InvoiceNumber = request.InvoiceNumber?.Value
            };
            return this.CreateResponse(() => _applicationService.UpdateOutstandingInvoices(model));
        }


        [HttpGet]
        [Route("credentialTypeSkills")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage CredentialTypeSkills([FromUri]IEnumerable<int> credentialTypeIds)
        {
            return this.CreateResponse(() => _applicationService.GetCredentialTypeSkills(credentialTypeIds));
        }

        [HttpGet]
        [Route("credentialTypeDomains")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage CredentialTypeDomains([FromUri]IEnumerable<int> credentialTypeIds)
        {
            return this.CreateResponse(() => _applicationService.GetCredentialTypeDomains(credentialTypeIds));
        }

        [HttpGet]
        [Route("credentialTypeSkillsTestSession")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage CredentialTypeSkillsTestSession([FromUri]IEnumerable<int> credentialTypeIds)
        {
            return this.CreateResponse(() => _applicationService.GetCredentialTypeSkillsTestSession(credentialTypeIds));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Venue)]
        [Route("venue")]
        public HttpResponseMessage Venue([FromUri]IEnumerable<int> testLocation)
        {
            return this.CreateResponse(() => _applicationService.GetVenue(testLocation));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Venue)]
        [Route("venuesShowingInactive")]
        public HttpResponseMessage VenuesShowingInactive([FromUri] IEnumerable<int> testLocation)
        {
            return this.CreateResponse(() => _applicationService.GetVenuesShowingInactive(testLocation));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Configure, SecurityNounName.Credential)]
        [Route("movecredential")]
        public HttpResponseMessage MoveCredentialPost(MoveCredentialModel model)
        {
            return this.CreateResponse(() => _applicationService.MoveCredential(model));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Application)]
        [Route("checkOptionMessage")]
        public HttpResponseMessage GetConfirmationMessage()
        {
            return this.CreateResponse(() => _applicationService.GetCheckOptionMessage());
        }

        [HttpGet]
        [Route("incompletePrerequsiteCredentialCheckOptionMessage")]
        public HttpResponseMessage GetIncompletePrerequisiteConfirmationMessage()
        {
            return this.CreateResponse(() => _applicationService.GetIncompletePrerequisiteCheckOptionData());
        }

        [HttpGet]
        [Route("issueOnHoldCredentialCheckOptionData")]
        public HttpResponseMessage GetIssueOnHoldCredentialCheckOptionData()
        {
            return this.CreateResponse(() => _applicationService.GetIssueOnHoldCredentialsCheckOptionData());
        }

        [HttpPost]
        [Route("GetEndorsementQualificationLookup")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetEndorsementQualificationLookup(GetEndorsementQualificationLookupRequestModel request)
        {
            return this.CreateResponse(() => _applicationService.GetEndorsementQualificationLookup(request));
        }

        [HttpGet]
        [Route("GetEndorsementLocationLookup")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetEndorsementLocationLookup([FromUri] GetEndorsementLocationLookupRequestModel request)
        {
            return this.CreateResponse(() => _applicationService.GetEndorsementLocationLookup(request));
        }

        [AllowAnonymous]
        public HttpResponseMessage TestApi2()
        {
            LoggingHelper.LogWarning("TestAPI2 CALLED");

            return this.CreateResponse(() => new List<CredentialRequestModel>());
        }

        [HttpGet]
        [Route("refundRequestToApprove")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.CredentialRequest)]
        public HttpResponseMessage GetRefundRequestsToApprove()
        {
            return this.CreateResponse(() => _applicationService.GetApprovalPendingRefundRequests()); ;
        }

        [HttpPost]
        [Route("approverefundrequests")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.CredentialRequest)]
        public HttpResponseMessage ApprovePayment(IEnumerable<RefundRequestsGroupingModel> model)
        {
            return this.CreateResponse(() => _applicationService.ApproveRefundRequests(model));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        [Route("prerequisiteSummary")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.CredentialRequest)]
        public HttpResponseMessage PrerequisiteSummary([FromUri] int credentialRequestId)
        {
            return this.CreateResponse(() => _credentialPrerequisiteService.GetPrerequisiteSummary(credentialRequestId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        [Route("relatedApplications/{applicationId}")]
        public HttpResponseMessage GetPrerequisiteApplications(int applicationId)
        {
            return this.CreateResponse(() => _applicationService.GetPrerequisiteApplications(applicationId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        [Route("prerequisiteApplicationsNullableApplications/{credentialRequestId}")]
        public HttpResponseMessage GetPrerequisiteApplicationsNullableApplications(int credentialRequestId)
        {
            return this.CreateResponse(() => _applicationService.GetPrerequisiteApplicationsNullableApplications(credentialRequestId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.CredentialRequest)]
        [Route("prerequisiteExemptions/{credentialRequestId}")]
        public HttpResponseMessage GetPrerequisiteExemptions(int credentialRequestId)
        {
            return this.CreateResponse(() => _credentialPrerequisiteService.GetCredentialPrerequisiteExemptions(credentialRequestId));
        }

        /// <summary>
        /// Get the Exemption data
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <returns>Exemption data</returns>
        [HttpGet]
        [Route("exemptions")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Application)]
        public HttpResponseMessage GetExemptions(int naatiNumber)
        {
            return this.CreateResponse(() => _credentialPrerequisiteService.GetExemptions(naatiNumber));
        }

        [HttpGet]
        [Route("getOnHoldCredentialsToIssue/{credentialRequestId}")]
        public HttpResponseMessage GetOnHoldCredentialsToIssue(int credentialRequestId)
        {
            return this.CreateResponse(() => _credentialPrerequisiteService.GetRelatedCredentialIdsOnHold(credentialRequestId));
        }
    }
}
