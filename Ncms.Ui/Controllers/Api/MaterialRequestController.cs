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
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.MaterialRequest;
using Ncms.Contracts.Models.MaterialRequest.Wizard;
using Ncms.Contracts.Models.System;
using Ncms.Contracts.Models.User;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;
using SearchRequest = Ncms.Contracts.SearchRequest;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/materialRequest")]
    public class MaterialRequestController : BaseApiController
    {
        private readonly IMaterialRequestWizardLogicService _materialRequestWizardLogicService;
        private readonly IMaterialRequestService _materialRequestService;
        private readonly IPanelService _panelService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly IEmailMessageService _emailMessageService;
        private readonly INoteService _noteService;

        public MaterialRequestController(IMaterialRequestWizardLogicService materialRequestWizardLogicService,
            IMaterialRequestService materialRequestService, IPanelService panelService, IUserService userService, IFileService fileService, IEmailMessageService emailMessageService, INoteService noteService)
        {
            _materialRequestWizardLogicService = materialRequestWizardLogicService;
            _materialRequestService = materialRequestService;
            _panelService = panelService;
            _userService = userService;
            _fileService = fileService;
            _emailMessageService = emailMessageService;
            _noteService = noteService;
        }

        [HttpGet]
        [Route("steps/{materialRequestId}/{actionId}/{panelId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetMaterialRequestSteps([FromUri] int actionId, [FromUri] int materialRequestId, int panelId)
        {
            return this.CreateResponse(
                () => _materialRequestWizardLogicService.GetMaterialRequestSteps(actionId, materialRequestId, panelId));
        }

        [HttpGet]
        [Route("roundSteps/{materialRequestRoundId}/{actionId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetMaterialRequestRoundSteps([FromUri] int actionId, [FromUri] int materialRequestRoundId)
        {
            return this.CreateResponse(() => _materialRequestWizardLogicService.GetMaterialRequestRoundSteps(actionId, materialRequestRoundId));
        }

        [HttpGet]
        [Route("action/{materialRequestStatusTypeId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetMaterialRequestActions([FromUri] int materialRequestStatusTypeId)
        {
            return this.CreateResponse(
                () => _materialRequestWizardLogicService.GetValidMaterialRequestActions(materialRequestStatusTypeId));
        }

        [HttpGet]
        [Route("roundAction/{materialRequestRoundStatusTypeId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetMaterialRequestRoundActions([FromUri] int materialRequestRoundStatusTypeId)
        {
            return this.CreateResponse(
                () => _materialRequestWizardLogicService.GetValidMaterialRequestRoundActions(
                    materialRequestRoundStatusTypeId));
        }

        [HttpPost]
        [Route("wizard/validateNotes")]
        [NcmsAuthorize(SecurityVerbName.Validate, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage ValidateMaterialRequestNotes(dynamic request)
        {
            var result = new
            {
                InvalidFields = new List<object>()
            };

            try
            {
                var rules = _materialRequestWizardLogicService.GetSystemNoteFieldRules((int)request.Action).Data;

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

        [HttpGet]
        [Route("wizard/testmaterialsource/{searchFilter?}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetTestmaterialSource([FromUri]string searchFilter = null)
        {
            return this.CreateResponse(() => _materialRequestService.SearchTestMaterials(searchFilter, true));
        }

        [HttpGet]
        [Route("wizard/roundlinks/{materialRequestId}/{actionId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetRoundLinks([FromUri]int? materialRequestId = null, [FromUri]int? actionId = null)
        {
            return this.CreateResponse(() => new RoundLinksStepModel { Links = new string[] { } });
        }

        [HttpGet]
        [Route("wizard/rounddetails/{materialRequestId}/{actionId}/{taskTypeId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetRoundDetails([FromUri]int materialRequestId, [FromUri] int actionId, [FromUri] int taskTypeId)
        {
            return this.CreateResponse(() => _materialRequestService.GetNewRoundDetails(materialRequestId, actionId, taskTypeId));
        }

        [HttpGet]
        [Route("wizard/documentsupload/{wizardInstanceId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetDocumentsUpload([FromUri]string wizardInstanceId)
        {
            return this.CreateResponse(() =>
            {
                var rootDirectory = new DirectoryInfo(ConfigurationManager.AppSettings["tempFilePath"]);
                var documents = DocumentUploadWizardHelper.GetDocuments(rootDirectory, wizardInstanceId, false);
                return documents;
            });
        }


        [HttpGet]
        [Route("skill")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetSkills([FromUri]int credentialTypeId, [FromUri]int panelId)
        {
            return this.CreateResponse(() => _materialRequestService.GetSkillsForCredentialType(credentialTypeId, panelId));
        }


        [HttpPost]
        [Route("wizard/documentsupload/upload")]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.MaterialRequest)]
        public async Task<HttpResponseMessage> SaveTempooraryFile()
        {
            var rootDirectory = new DirectoryInfo(ConfigurationManager.AppSettings["tempFilePath"]);
            return await this.ProcessMultipartFileData((Func<string, MultipartFileData, MultipartFormDataStreamProvider, int>)((fileName, fileData, provider) =>
            {
                var wizardInstanceId = provider.FormData["wizardInstanceId"];
                var type = provider.FormData["type"];
                var title = provider.FormData["title"];

                if (string.IsNullOrWhiteSpace(title))
                {
                    throw new Exception("Document Title has not been specified");
                }
                var tempFileId = Convert.ToInt32(provider.FormData["tempFileId"]);
                var examinersAvailable = Convert.ToBoolean(provider.FormData["eportalDownload"]);
                var mergeDocument = Convert.ToBoolean(provider.FormData["mergeDocument"]);

                if (mergeDocument)
                {
                    var documents = DocumentUploadWizardHelper.GetDocuments(rootDirectory, wizardInstanceId, true);
                    var documentsToUpdate = documents.Where(x => x.MergeDocument);
                    foreach (var documentToUpdate in documentsToUpdate)
                    {
                        var newFilePath = Path.Combine(rootDirectory.FullName, $"{Guid.NewGuid().ToString()}{Path.GetExtension(documentToUpdate.FilePath)}");
                        File.Copy(documentToUpdate.FilePath, newFilePath);
                        DocumentUploadWizardHelper.StoreTempFile(rootDirectory, wizardInstanceId, newFilePath, documentToUpdate.FileName, documentToUpdate.Title, ((StoredFileType)documentToUpdate.DocumenTypeId).ToString(), documentToUpdate.TempFileId, documentToUpdate.ExaminersAvailable, false);
                    }
                }
                //var deleted = bool.Parse(provider.FormData["deleted"]);
                var fileId = DocumentUploadWizardHelper.StoreTempFile(rootDirectory, wizardInstanceId, fileData.LocalFileName, fileName, title, type, tempFileId, examinersAvailable, mergeDocument);
                return fileId;
            }));
        }

        [HttpPost]
        [Route("wizard/documentsupload")]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage PostDocumentsUpload(dynamic data)
        {

            return this.CreateResponse(() => { });

        }

        [HttpGet]
        [Route("wizard/testMaterial/{baseMaterialRequestIdId}")]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetPreselectedData([FromUri] int baseMaterialRequestIdId)
        {

            return this.CreateResponse(() =>
            {
                var data = new TestMaterialStepModel();

                if (baseMaterialRequestIdId > 0)
                {
                    var materialRequest = _materialRequestService.GetMaterialRequest(baseMaterialRequestIdId).Data;
                    data.Title = materialRequest.OutputTestMaterial.Title;
                    data.LanguageId = materialRequest.OutputTestMaterial.LanguageId;
                    data.SkillId = materialRequest.OutputTestMaterial.SkillId;
                    data.PanelId = materialRequest.PanelId;
                    data.TaskTypeId = materialRequest.OutputTestMaterial.TypeId;
                    data.TestMaterialTypeId = materialRequest.OutputTestMaterial.TestMaterialTypeId;
                    data.TestMaterialDomainId = materialRequest.OutputTestMaterial.TestMaterialDomainId;
                }

                return data;
            });

        }

        [HttpGet]
        [Route("wizard/members/{panelId}/{credentialTypeId}/{materialRequestId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetMembers([FromUri] int panelId, int credentialTypeId, int materialRequestId)
        {
            return this.CreateResponse(() =>
            {
                var models = _panelService.GetPanelMembershipLookUp(
                    new GetPanelMemberLookupRequestModel()
                    {
                        PanelIds = new[] { panelId },
                        ActiveMembersOnly = true,
                        CredentialTypeId = credentialTypeId
                    }).Data.ToList();

                if (materialRequestId > 0)
                {

                    var existingMembers = _materialRequestService.GetMaterialRequest(materialRequestId)
                        .Data.Members.ToDictionary(x => x.PanelMemberShipId, y => y);


                    foreach (var model in models)
                    {
                        if (existingMembers.ContainsKey(model.Id))
                        {
                            model.PreSelected = true;
                            model.IsCoordinator = existingMembers[model.Id].MemberTypeId == (int)MaterialRequestPanelMembershipTypeName.Coordinator;
                            model.Tasks = existingMembers[model.Id].Tasks;
                        }
                    }
                }

                return models;

            });
        }

        [HttpGet]
        [Route("wizard/coordinator/{panelId}/{credentialTypeId}/{materialRequestId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetMembersForCoordinator([FromUri] int panelId, int credentialTypeId, int materialRequestId)
        {

            return this.CreateResponse(() =>
            {
                var models = _panelService.GetPanelMembershipLookUp(
                    new GetPanelMemberLookupRequestModel()
                    {
                        PanelIds = new[] { panelId },
                        ActiveMembersOnly = true,
                        CredentialTypeId = credentialTypeId
                    });

                if (materialRequestId > 0)
                {
                    var currentCoordianator = _materialRequestService.GetMaterialRequest(materialRequestId)
                        .Data.Members.First(x => x.MemberTypeId ==
                                                 (int)MaterialRequestPanelMembershipTypeName.Coordinator);

                    var coordinator = models.Data.FirstOrDefault(x => x.Id == currentCoordianator.PanelMemberShipId);
                    if (coordinator != null)
                    {
                        coordinator.PreSelected = true;
                        coordinator.IsCoordinator = true;
                    }
                }

                return models;

            });

        }

        [HttpPost]
        [Route("wizard/members")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage PostMembers([FromUri]dynamic request)
        {
            return this.CreateResponse(() => { });
        }

        [HttpGet]
        [Route("wizard/notes/{actionId}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetNotes([FromUri]int actionId)
        {
            return this.CreateResponse(() => _materialRequestWizardLogicService.GetSystemNoteFieldRules(actionId));
        }

        [HttpPost]
        [Route("wizard/notes")]
        [NcmsAuthorize(new[] { SecurityVerbName.Create, SecurityVerbName.Update }, SecurityNounName.Notes)]
        public HttpResponseMessage PostNotes([FromUri]dynamic request)
        {
            var result = new
            {
                InvalidFields = new List<object>()
            };
            return this.CreateResponse(() => result);
        }

        [HttpGet]
        [Route("wizard/send-email-check-option")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetSendEmailCheckOption()
        {
            return this.CreateResponse(() => new
            {
                PreventToSendEmailChecked = true,
                PreventToSendEmailMessage = Naati.Resources.MaterialRequest.SendNotificationEmails,
                ReadOnly = !CurrentPrincipal.HasPermission(SecurityNounName.Email, SecurityVerbName.Override)
            });
        }

        [HttpPost]
        [Route("wizard/email-preview")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage PostEmailPreview(dynamic request)
        {
            try
            {
                var model = new MaterialRequestWizardModel()
                {
                    MaterialRequestId = (int)request.MaterialRequestId.Value,
                    MaterialRequestRoundId = (int)(request.MaterialRequestRoundId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };

                var emailModels = _materialRequestService.GetEmailPreview(model).Data;

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
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.MaterialRequest)]
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
      


        [HttpPost]
        [Route("wizard")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage PostWizard(dynamic request)
        {
            try
            {
                var model = new MaterialRequestWizardModel
                {
                    MaterialRequestId = (int)request.MaterialRequestId.Value,
                    MaterialRequestRoundId = (int)(request.MaterialRequestRoundId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };

                return this.CreateResponse(() => _materialRequestService.PerformAction(model));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpGet]
        [Route("{materialRequestId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage Get([FromUri]int materialRequestId)
        {
            return this.CreateResponse(() => _materialRequestService.GetMaterialRequest(materialRequestId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage Get([FromUri] SearchRequest request)
        {
            return this.CreateResponse(() => _materialRequestService.SearchTestMaterialRequests(request));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        [Route("relations/{materialId}")]
        public HttpResponseMessage GetRelations([FromUri]int materialId)
        {
            return this.CreateResponse(() => _materialRequestService.GetTestMaterialRelations(materialId));
        }

        [HttpGet]
        [Route("panelRequests/{panelId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Panel)]
        public HttpResponseMessage GetPanelRequests([FromUri]int panelId)
        {
            return this.CreateResponse(() => _materialRequestService.GetPanelMaterialRequests(panelId));
        }

        [HttpGet]
        [Route("activeCoordinatorRequests/{coordinatorNaatiNumber}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetActiveCoordinatorRequests([FromUri]int coordinatorNaatiNumber)
        {
            return this.CreateResponse(() => _materialRequestService.GetActiveCoordinatorRequests(coordinatorNaatiNumber));
        }

        [HttpGet]
        [Route("wizard/roundDocumentTypes/{actionId}/{testmaterialTypeId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Document)]
        public HttpResponseMessage GetDocumentTypes([FromUri]int actionId, [FromUri]int testmaterialTypeId)
        {
            return this.CreateResponse(() => _materialRequestService.GetAvailableDocumentTypes(actionId, testmaterialTypeId));
        }

        [HttpGet]
        [Route("wizard/testMaterial/isTestMaterialReadOnly/{actionId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage IsTestMaterialReadOnly(int actionId)
        {
            return this.CreateResponse(() => actionId != (int)SystemActionTypeName.CloneMaterialRequest && actionId != (int)SystemActionTypeName.CreateMaterialRequest);
        }

        [HttpGet]
        [Route("wizard/testMaterial/isRequestCostReadOnly/{actionId}/{materialRequestId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage IsRequestCostReadOnly(int materialRequestId, int actionId)
        {
            return this.CreateResponse(() =>
            {
                if (materialRequestId <= 0)
                {
                    return false;
                }

                if (actionId == (int)SystemActionTypeName.CloneMaterialRequest)
                {
                    return false;
                }
                var rounds = _materialRequestService.GetRoundLookup(materialRequestId).Data.ToList();

                if (rounds.Count() > 1)
                {
                    return true;
                }
                var latestRoundId = rounds.First().Id;

                var latestRoundStatus = _materialRequestService.GetMaterialRequestRound(latestRoundId)
                    .Data.StatusTypeId;

                if (latestRoundStatus != (int)MaterialRequestRoundStatusTypeName.SentForDevelopment)
                {
                    return true;
                }

                return false;
            });
        }

        [HttpGet]
        [Route("wizard/sourceTestMaterial/show/{actionId}/{testMaterialTypeId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage ShowSourceTestMaterial([FromUri]int actionId, [FromUri]int testMaterialTypeId)
        {
            return this.CreateResponse(() => testMaterialTypeId == (int)TestMaterialTypeName.Test);
        }

        [HttpGet]
        [Route("wizard/existingdocuments/{materialRequestId}/{sourceTestMaterialId}/{actionId}/{testMaterialTypeId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetRoundDocumentTypes([FromUri]int materialRequestId, [FromUri]int sourceTestMaterialId, [FromUri]int actionId, [FromUri]int testMaterialTypeId)
        {
            return this.CreateResponse(() => _materialRequestService.GetExistingDocuments(materialRequestId, sourceTestMaterialId, actionId, testMaterialTypeId));
        }

        [HttpPost]
        [Route("wizard/documentsupload/delete")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage RemoveDocument(dynamic request)
        {

            return this.CreateResponse(() =>
            {
                var instanceId = request.WizardInstanceId.Value.ToString();
                var tempFileId = request.TempFileId.Value.ToString();
                var rootDirectory = new DirectoryInfo(ConfigurationManager.AppSettings["tempFilePath"]);
                DocumentUploadWizardHelper.RemoveDocument(rootDirectory, instanceId, tempFileId);
            });
        }

        [HttpGet]
        [Route("wizard/download/{tempFileId}/{wilzardInstanceId}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage Download(int tempFileId, string wilzardInstanceId)
        {
            return this.FileStreamResponse(
                () => DocumentUploadWizardHelper.GetFile(ConfigurationManager.AppSettings["tempFilePath"], tempFileId,
                    wilzardInstanceId));

        }

        [HttpGet]
        [Route("listEmail/{materialRequestId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetEmailMessageByApplicationId(int materialRequestId)
        {
            return this.CreateResponse(() => _emailMessageService.GetEmailMessageByMaterialRequestId(materialRequestId));
        }

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
		[Route("{materialRequestId}/notes")]
		public HttpResponseMessage GetMaterialRequestNotes([FromUri]int materialRequestId)
		{
			return this.CreateResponse(() => _noteService.ListMaterialRequestNotes(materialRequestId).OrderByDescending(x => x.CreatedDate));
		}

        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        [Route("{materialRequestId}/publicNotes")]
        public HttpResponseMessage GetMaterialRequestPublicNotes([FromUri]int materialRequestId)
        {
            return this.CreateResponse(() => _noteService.ListMaterialRequestPublicNotes(materialRequestId).OrderByDescending(x => x.CreatedDate));
        }

        [HttpPost]
        [Route("notes")]
        [NcmsAuthorize(new[] { SecurityVerbName.Create, SecurityVerbName.Update }, SecurityNounName.Notes)]
        public HttpResponseMessage PostNotes(MaterialRequestNoteModel model)
        {
            model.UserId = CurrentUser.Id;
            return this.CreateResponse(() => _noteService.CreateMaterialRequestNote(model));
        }

        [HttpPost]
        [Route("link/{materialRequestRoundId}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage PostLinks([FromUri]int materialRequestRoundId, MaterialRequestRoundLinkModel link)
        {
            return this.CreateResponse(() => _materialRequestService.AddLink(materialRequestRoundId, link));
        }


        [HttpDelete]
        [Route("link/{materialRequestRoundLinkId}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage PostLinks([FromUri]int materialRequestRoundLinkId)
        {
            return this.CreateResponse(() => _materialRequestService.DeleteLink(materialRequestRoundLinkId));
        }

        [HttpPost]
        [Route("publicNotes")]
        [NcmsAuthorize(new[] { SecurityVerbName.Create, SecurityVerbName.Update }, SecurityNounName.Notes)]
        public HttpResponseMessage PostPublicNotes(MaterialRequestPublicNoteModel model)
        {
            model.UserId = CurrentUser.Id;
            return this.CreateResponse(() => _noteService.CreateMaterialRequestPublicNote(model));
        }

        [HttpDelete]
        [Route("{id}/{materialRequestId}/notes")]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Notes)]
        public HttpResponseMessage DeleteNotes([FromUri]int id, [FromUri]int materialRequestId)
        {
            return this.CreateResponse(() => _noteService.DeleteMaterialRequestNote(materialRequestId, id));
        }

        [HttpDelete]
        [Route("{id}/{materialRequestId}/publicNotes")]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Notes)]
        public HttpResponseMessage DeletePublicNotes([FromUri]int id, [FromUri]int materialRequestId)
        {
            return this.CreateResponse(() => _noteService.DeleteMaterialRequestPublicNote(materialRequestId, id));
        }

        [HttpGet]
        [Route("documents/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage DocumentsGet([FromUri]int id)
        {
            return this.CreateSearchResponse(() =>
            {
                return _materialRequestService.ListAttachments(new ListAttachmentsRequestModel() { MaterialRequestRoundId = id, NcmsAvailable = true });
            });
        }

        [Route("documents/upload")]
        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.MaterialRequest)]
        public async Task<HttpResponseMessage> Upload()
        {
            return await this.ProcessMultipartFileData((Func<string, MultipartFileData, MultipartFormDataStreamProvider, int>)((fileName, fileData, provider) =>
            {
                var materialRequestRoundId = Convert.ToInt32(provider.FormData["materialRequestRoundId"]);
                var id = Convert.ToInt32(provider.FormData["id"]);
                var type = provider.FormData["type"];
                var title = provider.FormData["title"];
                var storedFileId = Convert.ToInt32(provider.FormData["storedFileId"]);
                var examinersAvailable = Convert.ToBoolean(provider.FormData["eportalDownload"]);

                var user = _userService.Get() ?? new UserModel();
                var request = new MaterialRequestRoundAttachmentModel()
                {
                    FileName = fileName,
                    Type = type,
                    FilePath = fileData.LocalFileName,
                    UploadedByUserId = user.Id,
                    StoragePath = $@"{type}\{materialRequestRoundId}\{fileName}",
                    StoredFileId = storedFileId,
                    MaterialRequestRoundId = materialRequestRoundId,
                    MaterialRequestRoundAttachmentId = id,
                    Title = title,
                    EportalDownload = examinersAvailable,
                    NcmsAvailable = true
                };

                return _materialRequestService.CreateOrReplaceAttachment(request);
            }));
        }

        [HttpPost]
        [Route("documents")]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage DocumentsPost(MaterialRequestRoundAttachmentModel request)
        {
            var user = _userService.Get() ?? new UserModel();
            request.UploadedByUserId = user.Id;
            request.NcmsAvailable = true;
            return this.CreateResponse(() => _materialRequestService.CreateOrReplaceAttachment(request));
        }

        [HttpDelete]
        [Route("documents/{id}")]
        [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage DocumentsDelete([FromUri]int id)
        {
            return this.CreateResponse(() => _materialRequestService.DeleteAttachment(id));
        }

        [HttpGet]
        [Route("documentTypes/{materialRequestRoundId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Document)]
        public HttpResponseMessage GetDocumentTypesByCredentialType([FromUri] int materialRequestRoundId)
        {
            return this.CreateResponse(() => _materialRequestService.GetAvailableDocumentTypes(0, 0)
                .Data.Select(x => x.Name)
                .OrderBy(y => y));
        }

        [HttpGet]
        [Route("round/{materialRequestRoundId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetMaterialRequestRound([FromUri] int materialRequestRoundId)
        {
            return this.CreateResponse(() => _materialRequestService.GetMaterialRequestRound(materialRequestRoundId));
        }

        [HttpGet]
        [Route("rounds/{materialRequestId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetRounds([FromUri] int materialRequestId)
        {
            return this.CreateResponse(() => _materialRequestService.GetRoundLookup(materialRequestId));
        }

        [HttpGet]
        [Route("testMaterial/allowEdit/{testMaterialId}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage CanEditTestMaterial([FromUri] int testMaterialId)
        {
            return this.CreateResponse(() => !_materialRequestService.HasActiveMaterialRequest(testMaterialId).Data);
        }

        [HttpPost]
        [Route("action")]
        [NcmsAuthorize(SecurityVerbName.Validate, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage ValidateAction(dynamic request)
        {
            try
            {
                var model = new MaterialRequestWizardModel()
                {
                    MaterialRequestId = (int)request.MaterialRequestId.Value,
                    MaterialRequestRoundId = (int)(request.MaterialRoundId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };
                return this.CreateResponse(() => _materialRequestService.ValidateActionPreconditions(model));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("roundAction")]
        [NcmsAuthorize(SecurityVerbName.Validate, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage ValidateRoundAction(dynamic request)
        {
            try
            {
                var model = new MaterialRequestWizardModel()
                {
                    MaterialRequestId = (int)request.MaterialRequestId.Value,
                    MaterialRequestRoundId = (int)(request.MaterialRequestsRoundId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };
                return this.CreateResponse(() => _materialRequestService.ValidateActionPreconditions(model));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("approvePayment")]
        [NcmsAuthorize(SecurityVerbName.Approve, SecurityNounName.PayRun)]
        public HttpResponseMessage ApprovePayment(IEnumerable<MaterialRequestPayrollUserGroupingModel> model)
        {
            return this.CreateResponse(() => _materialRequestService.ApproveRequests(model));
        }

        [HttpPost]
        [Route("MarkAsPaid")] 
        [NcmsAuthorize(SecurityVerbName.MarkAsPaid, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage MarkAsPaid(IEnumerable<MaterialRequestMemberGroupingModel> model)
        {
            return this.CreateResponse(() => _materialRequestService.PayMaterialRequestMembers(model));
        }

        [HttpPost]
        [Route("UnApprove")]
        [NcmsAuthorize(SecurityVerbName.Approve, SecurityNounName.Bill)]
        public HttpResponseMessage Unapprove(IEnumerable<MaterialRequestMemberGroupingModel> model)
        {
            return this.CreateResponse(() => _materialRequestService.UnApproveRequests(model));
        }

        [HttpGet]
        [Route("materialRequestToApprove")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetMaterialRequestsToApprove()
        {
            return this.CreateResponse(() => _materialRequestService.GetPendingItemsToApprove());
        }

        [HttpGet]
        [Route("membersToPay")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetMembersToPay()
        {
            return this.CreateResponse(() => _materialRequestService.GetPendingItemsToPay());
        }

        [HttpGet]
        [Route("allAvailableDocumentTypes")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage GetAllAvailableDocumentTypes()
        {
            return this.CreateResponse(() => _materialRequestService.GetAllAvailableDocumentTypes());
        }

        [HttpGet]
        [Route("showTotalCost/{materialRequestId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage CanShowTotalCost(int materialRequestId)
        {
            return this.CreateResponse(() => _materialRequestService.CanShowTotalCost(materialRequestId));
        }

        [HttpGet]
        [Route("wizard/showMergeOption/{actionId}/{testMaterialTypeId}")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage ShowMergeOption(int actionId, int testMaterialTypeId)
        {
            return this.CreateResponse(() => actionId == (int)SystemActionTypeName.UploadFinalMaterialDocuments && testMaterialTypeId == (int)TestMaterialTypeName.Test);
        }

        [HttpGet]
        [Route("wizard/domains/{credentialTypeId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage AvailableDomains(int credentialTypeId)
        {
            return this.CreateResponse(() => _materialRequestService.GetTestMaterialDomains(credentialTypeId));
        }

        [HttpGet]
        [Route("wizard/materialDomainRequired/{materialRequestId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.MaterialRequest)]
        public HttpResponseMessage ShowTestMaterialDomain(int materialRequestId)
        {
            return this.CreateResponse(() => _materialRequestService.IsMaterialDomainRequired(materialRequestId));
        }

        [HttpGet]
        [Route("roundDocument/download/{materialRoundId}/{storedFileId}")]
        [NcmsAuthorize(SecurityVerbName.Download, SecurityNounName.Document)]
        public HttpResponseMessage DownloadRoundDocument(int materialRoundId, int storedFileId)
        {
            return this.FileStreamResponse(() => _fileService.GetMaterialRoundFile(materialRoundId, storedFileId));
        }
    }
}
