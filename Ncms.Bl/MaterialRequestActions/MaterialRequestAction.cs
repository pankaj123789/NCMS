using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Bl.SystemActions;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.MaterialRequest;
using LookupTypeModel = Ncms.Contracts.Models.Application.LookupTypeModel;


namespace Ncms.Bl.MaterialRequestActions
{
    public abstract class MaterialRequestAction : SystemAction<MaterialRequestActionModel, MaterialRequestWizardModel, MaterialRequestActionOutput, UpsertMaterialRequestResultModel, MaterialRequestEmailMessageModel>
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.MaterialRequest;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Update;

        protected virtual MaterialRequestStatusTypeName[] MaterialRequestEntryStates { get; }
        protected virtual MaterialRequestStatusTypeName MaterialRequestExitState { get; }
        protected virtual MaterialRequestPanelMembershipModel Coordinator { get; set; }
        protected virtual MaterialRequestStatusTypeName CurrentEntryState { get; private set; }

        protected IList<LookupTypeModel> MaterialRequestStatusTypes { get; private set; }
        protected IList<LookupTypeModel> MaterialRequestRoundStatusTypes { get; private set; }

        protected MaterialRequestAction(IServiceLocator serviceLocator = null) : base(serviceLocator)
        {
        }

        protected override void CreateEmailAttachmentsIfApplicable()
        {
        }

        protected void SetTestMaterialDomain()
        {
            if (WizardModel.TestMaterialDomainId > 0)
            {
                ActionModel.MaterialRequestInfo.OutputTestMaterial.TestMaterialDomainId = WizardModel.TestMaterialDomainId;
            }
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            TryInsertToken(tokenDictionary, TokenReplacementField.NaatiNo, () => Coordinator.NaatiNumber.ToString());
            TryInsertToken(tokenDictionary, TokenReplacementField.GivenName, () => Coordinator.GivenName);
            TryInsertToken(tokenDictionary, TokenReplacementField.TestMaterialTitle, () => ActionModel.MaterialRequestInfo.OutputTestMaterial.Title);
            TryInsertToken(tokenDictionary, TokenReplacementField.TestMaterialId, () => ActionModel.MaterialRequestInfo.OutputTestMaterial.Id.ToString());
            TryInsertToken(tokenDictionary, TokenReplacementField.CredentialType, () => ActionModel.MaterialRequestInfo.OutputTestMaterial.CredentialType);
            TryInsertToken(tokenDictionary, TokenReplacementField.TestTaskDescription, () => ActionModel.MaterialRequestInfo.OutputTestMaterial.TypeLabel);
            TryInsertToken(tokenDictionary, TokenReplacementField.Language, () => ActionModel.MaterialRequestInfo.OutputTestMaterial.SkillName ?? ActionModel.MaterialRequestInfo.OutputTestMaterial.Language);
            TryInsertToken(tokenDictionary, TokenReplacementField.MaterialCoordinator, () => Coordinator.GivenName);
            TryInsertToken(tokenDictionary, TokenReplacementField.CoordinatorEmail, () => Coordinator.PrimaryEmail);



            TryInsertToken(tokenDictionary, TokenReplacementField.MaterialRequestCost, () =>
            {
                var cost = ApplicationService.GetLookupType(LookupType.MaterialSpecificationCost.ToString()).Data.First(x => x.Id == ActionModel.MaterialRequestInfo.ProductSpecificationId);
                return cost.DisplayName;
            });
            TryInsertToken(tokenDictionary, TokenReplacementField.MaxBillableHours, () => ActionModel.MaterialRequestInfo.MaxBillableHours.ToString(CultureInfo.InvariantCulture));
            TryInsertToken(tokenDictionary, TokenReplacementField.ActionPublicNote, () => WizardModel.PublicNotes);

            var userId = ActionModel.MaterialRequestInfo.OwnedByUserId.GetValueOrDefault();
            if (userId > 0)
            {
                var user = UserService.GetUserDetailsById(userId).Data;


                TryInsertToken(tokenDictionary, TokenReplacementField.MaterialRequestOwner, () => user.FullName);
                TryInsertToken(tokenDictionary, TokenReplacementField.MaterialRequestOwnerEmail, () => user.Email);
            }
        }

        protected virtual void TryInsertToken(Dictionary<string, string> tokenDictionary, TokenReplacementField token, Func<string> valueFunc)
        {
            var tokenName = TokenReplacementService.GetTokenNameFor(token);
            if (!tokenDictionary.ContainsKey(tokenName))
            {
                tokenDictionary[tokenName] = valueFunc();
            }
        }

        public override void SaveChanges()
        {
            Output.UpsertResults = SaveActionData();

            CreateEmailAttachmentsIfApplicable();

            var pendingEmails = CreatePendingEmails().ToList();

            Output.PendingEmails = new List<MaterialRequestEmailMessageModel>();

            foreach (var emailModel in pendingEmails)
            {
                var emailCreateResponse = EmailMessageService.CreateEmailMessage(emailModel);

                if (!emailCreateResponse.Success)
                {
                    throw new Exception(emailCreateResponse.Errors.FirstOrDefault());
                }
                var email = emailCreateResponse.Data;
                Output.PendingEmails.Add(email);
            }

        }

        protected override GenericResponse<UpsertMaterialRequestResultModel> SaveActionData()
        {
            return MaterialRequestService.UpsertMaterialRequestActionData(ActionModel);
        }

        public override IList<EmailTemplateModel> GetEmailTemplates()
        {
            if (!CanSendEmail())
            {
                return new List<EmailTemplateModel>();
            }

            GetEmailTemplateResponse serviceReponse = null;
            serviceReponse = EmailMessageService.GetSystemEmailTemplate(new GetSystemEmailTemplateRequest
            {
                Actions = new[] { (SystemActionTypeName)WizardModel.ActionType },
                ActionEvents = ActionEvents.GroupBy(x => x.Level).OrderByDescending(y => y.Key).First().Select(x => x.Event)
            });

            var emailTemplates = serviceReponse.Data.Select(AutoMapperHelper.Mapper.Map<EmailTemplateModel>).ToList();

            return emailTemplates;
        }

        protected override IEnumerable<MaterialRequestEmailMessageModel> GetEmails(EmailTemplateModel template, MaterialRequestEmailMessageModel baseEmail)
        {
            var materialRequestEmails = new List<MaterialRequestEmailMessageModel>();

            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.SendToRequestOwner))
            {
                var requesterEmail = AutoMapperHelper.Mapper.Map<MaterialRequestEmailMessageModel>(baseEmail);
                requesterEmail.RecipientEmail = ActionModel.MaterialRequestInfo.OwnedByUserEmail;
                requesterEmail.RecipientUserId = ActionModel.MaterialRequestInfo.CreatedByUserId;

                materialRequestEmails.Add(requesterEmail);
            }

            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.SendToCoordinator))
            {
                var coordinatorEmail = AutoMapperHelper.Mapper.Map<MaterialRequestEmailMessageModel>(baseEmail);

                coordinatorEmail.RecipientEmail = Coordinator.PrimaryEmail;
                coordinatorEmail.RecipientEntityId = Coordinator.EntityId;

                materialRequestEmails.Add(coordinatorEmail);
            }

            return materialRequestEmails;
        }

        protected override MaterialRequestEmailMessageModel CreateEmail(EmailTemplateModel template, IDictionary<string, string> tokenDictionary)
        {
            IEnumerable<string> replacementErrros;
            var stringValue = string.Empty;
            void ReplaceString(string token, string value) => stringValue = stringValue.Replace(token, value ?? string.Empty);

            stringValue = template.Content;
            TokenReplacementService.ReplaceTemplateFieldValues(stringValue, ReplaceString, tokenDictionary, true, out replacementErrros);
            var content = stringValue;
            stringValue = template.Subject;
            TokenReplacementService.ReplaceTemplateFieldValues(stringValue, ReplaceString, tokenDictionary, true, out replacementErrros);
            var subject = stringValue;
            var fromAddress = string.IsNullOrWhiteSpace(template.FromAddress)
                ? SystemService.GetSystemValue("DefaultEmailSenderAddress")
                : template.FromAddress;

            var email = new MaterialRequestEmailMessageModel
            {
                Subject = subject,
                Body = content,
                From = fromAddress,
                CreatedDate = DateTime.Now,
                CreatedUserId = CurrentUser.Id,
                Attachments = new List<EmailMessageAttachmentModel>(),
                EmailSendStatusTypeId = (int)EmailSendStatusTypeName.Requested,
                MaterialRequestId = ActionModel.MaterialRequestInfo.MaterialRequestId,
                EmailTemplateId = template.Id
            };

            return email;
        }

        public static MaterialRequestAction CreateAction(SystemActionTypeName actionType, MaterialRequestActionModel actionModel, MaterialRequestWizardModel wizardModel)
        {
            var action = CreateSystemAction<MaterialRequestAction>(ActionDict, actionType, actionModel, wizardModel, new MaterialRequestActionOutput());

            return action;
        }

        protected override void ConfigureInstance(MaterialRequestActionModel actionModel, MaterialRequestWizardModel wizardModel, MaterialRequestActionOutput output)
        {
            base.ConfigureInstance(actionModel, wizardModel, output);

            MaterialRequestStatusTypes = ApplicationService.GetLookupType(nameof(LookupType.MaterialRequestStatus)).Data.ToList();
            MaterialRequestRoundStatusTypes = ApplicationService.GetLookupType(nameof(LookupType.MaterialRequestRoundStatus)).Data.ToList();


            if (wizardModel.MaterialRequestId > 0)
            {
                CurrentEntryState = (MaterialRequestStatusTypeName)ActionModel.MaterialRequestInfo.StatusTypeId;
                Coordinator = actionModel.MaterialRequestInfo.Members.First(
                    x => x.MemberTypeId == (int)MaterialRequestPanelMembershipTypeName.Coordinator);
            }
        }

        protected virtual void ValidateEntryState()
        {
            if (!MaterialRequestEntryStates.Contains((MaterialRequestStatusTypeName)ActionModel.MaterialRequestInfo.StatusTypeId))
            {
                var entryStateNames = MaterialRequestEntryStates.Select(x => MaterialRequestStatusTypes.SingleOrDefault(y => y.Id == (int)x)?.DisplayName);
                throw new UserFriendlySamException(String.Format(Naati.Resources.MaterialRequest.WrongMaterialRequestStatusErrorMessage,
                    string.Join(", ", entryStateNames)));
            }
        }

        protected virtual void SetExitState()
        {
            if (ActionModel.MaterialRequestInfo.StatusTypeId != (int)MaterialRequestExitState)
            {
                ActionModel.MaterialRequestInfo.StatusTypeId = (int)MaterialRequestExitState;
                ActionModel.MaterialRequestInfo.StatusChangeDate = DateTime.Now;
            }

            ActionModel.MaterialRequestInfo.StatusChangeUserId = CurrentUser.Id;

        }
        protected virtual void CreatePublicNote()
        {
            var noteModel = new MaterialRequestActionPublicNoteModel()
            {
                CreatedDate = DateTime.Now,
                Description = string.Empty,
                UserId = CurrentUser.Id,
                ReadOnly = true
            };
            noteModel.Description = GetPublicNote();

            if (!string.IsNullOrWhiteSpace(noteModel.Description))
            {
                ActionModel.PublicNotes.Add(noteModel);
            }
        }

        protected virtual string GetPublicNote()
        {
            if (!string.IsNullOrWhiteSpace(WizardModel.PublicNotes))
            {
                return WizardModel.PublicNotes;
            }

            return string.Empty;
        }

        protected virtual void CreateNote()
        {
            var noteModel = new MaterialRequestActionNoteModel()
            {
                CreatedDate = DateTime.Now,
                Description = string.Empty,
                UserId = CurrentUser.Id,
                ReadOnly = true
            };

            noteModel.Description = GetNote();
            if (!CanSendEmail())
            {
                noteModel.Description = $"{noteModel.Description ?? string.Empty} - {Naati.Resources.Application.EmailSkipped}";
            }
            if (!String.IsNullOrWhiteSpace(WizardModel.PublicNotes))
            {
                noteModel.Description = $"{noteModel.Description}\n\nPublic Comments: {WizardModel.PublicNotes}";
                noteModel.Highlight = true;
            }
            if (!String.IsNullOrWhiteSpace(WizardModel.PrivateNotes))
            {
                noteModel.Description = $"{noteModel.Description}\n\nPrivate Comments: {WizardModel.PrivateNotes}";
            }
            if (!String.IsNullOrWhiteSpace(noteModel.Description))
            {
                ActionModel.Notes.Add(noteModel);
            }
        }
        protected virtual string GetNote()
        {
            var entryStateName = MaterialRequestStatusTypes.Single(x => x.Id == ActionModel.MaterialRequestInfo.StatusTypeId).DisplayName;
            var exitStateName = MaterialRequestStatusTypes.Single(x => x.Id == (int)MaterialRequestExitState).DisplayName;
            if (entryStateName == exitStateName)
            {
                return string.Empty;
            }
            return String.Format(Naati.Resources.MaterialRequest.MaterialRquestStatusChangeNote, entryStateName, exitStateName);
        }

        protected override GenericResponse<MaterialRequestEmailMessageModel> SaveEmailMessage(MaterialRequestEmailMessageModel message)
        {
            return EmailMessageService.CreateEmailMessage(message);
        }

        protected override void LogAction()
        {
            LoggingHelper.LogInfo("Performing workflow action {Action} for MaterialRequest{MaterialRequestId}",
                (SystemActionTypeName)WizardModel.ActionType, WizardModel.MaterialRequestId);
        }

        protected virtual void SetOwner()
        {
            ActionModel.MaterialRequestInfo.OwnedByUserId = UserService.Get().Id;
        }

        protected virtual void ClearOwner()
        {
            ActionModel.MaterialRequestInfo.OwnedByUserId = null;
        }



        protected virtual void AddDocuments()
        {
            foreach (var existingDocument in WizardModel.ExistingDocuments)
            {
                if (string.IsNullOrWhiteSpace(existingDocument.DisplayName))
                {
                    throw new Exception("Document Name has not been specified");
                }
                ActionModel.Documents.Add(new OutputTestMaterialDocumentInfoModel()
                {
                    StoredFileId = existingDocument.Id,
                    Description = Path.GetFileNameWithoutExtension(existingDocument.DisplayName ?? string.Empty) + existingDocument.FileType,
                    ExaminersAvailable = existingDocument.ExaminersAvailable,
                    MergeDocument = existingDocument.MergeDocument

                });
            }

            var userId = UserService.Get().Id;
            var newDocuments = DocumentUploadWizardHelper.GetDocuments(new DirectoryInfo(ConfigurationManager.AppSettings["tempFilePath"]),
                WizardModel.NewDocumentsModel.WizardInstanceId, true);


            foreach (var newDocument in newDocuments)
            {
                ActionModel.Documents.Add(new OutputTestMaterialDocumentInfoModel()
                {
                    StoredFileId = 0,
                    Description = newDocument.Title,
                    DocumentTypeId = newDocument.DocumenTypeId,
                    FileName = newDocument.FileName,
                    FilePath = newDocument.FilePath,
                    UserId = userId,
                    ExaminersAvailable = newDocument.ExaminersAvailable,
                    MergeDocument = newDocument.MergeDocument
                });

            }
        }

        private static readonly Dictionary<SystemActionTypeName, Type> ActionDict =
            new Dictionary<SystemActionTypeName, Type>
            {
                {SystemActionTypeName.CreateMaterialRequest, typeof(MaterialRequestCreateAction)},
                {SystemActionTypeName.CloneMaterialRequest, typeof(MaterialRequestCloneAction)},
                {SystemActionTypeName.CreateMaterialRequestRound, typeof(MaterialRequestRoundCreateAction)},
                {SystemActionTypeName.SubmitRoundForApproval, typeof(MaterialRequestRoundSubmitForApprovalAction)},
                {SystemActionTypeName.ApproveMaterialRequestRound, typeof(MaterialRequestRoundApproveAction)},
                {SystemActionTypeName.RejectMaterialRequestRound, typeof(MaterialRequestRoundRejectAction)},
                {SystemActionTypeName.RevertMaterialRequestRound, typeof(MaterialRequestRoundRevertAction)},
                {SystemActionTypeName.RevertMaterialRequest, typeof(MaterialRequestRevertAction)},
                {SystemActionTypeName.UploadFinalMaterialDocuments, typeof(MaterialRequestUploadFinalDocumentsAction)},
                {SystemActionTypeName.UpdateMaterialRequest, typeof(MaterialRequestUpdateAction)},
                {SystemActionTypeName.UpdateMaterialRequestRound, typeof(MaterialRequestRoundUpdateAction)},
                {SystemActionTypeName.CancelMaterialRequest, typeof(MaterialRequestCancelAction)},
                {SystemActionTypeName.MarkMaterialRequestMemberAsPaid, typeof(MaterialRequestMarkMemberAsPaidAction)},
                {SystemActionTypeName.ApproveMaterialRequestPayment, typeof(MaterialRequestApprovePaymentAction)},
                {SystemActionTypeName.UnApproveMaterialRequestPayment, typeof(MaterialRequestUnApprovePaymentsAction)},
                {SystemActionTypeName.UpdateMaterialRequestMembers, typeof(MaterialRequestUpdateMembersAction)},
            };

        protected virtual void UpdateMaterialRequestTitle()
        {
            ActionModel.MaterialRequestInfo.OutputTestMaterial.Title = WizardModel.OutputTestMaterial.Title;
        }

        protected virtual void AddMembers()
        {
            var member = PanelService.GetPanelMembershipInfo(new[] { WizardModel.Coordinator.PanelMembershipId }).Data.Single();

            var memberDetails = new MaterialRequestPanelMembershipModel()
            {
                EntityId = member.EntityId,
                GivenName = member.GivenName,
                Tasks = new List<MaterialRequestTaskModel>(),
                MemberTypeId = (int)MaterialRequestPanelMembershipTypeName.Coordinator,
                NaatiNumber = member.NaatiNumber,
                PanelMemberShipId = member.PanelMemberShipId,
                PrimaryEmail = member.PrimaryEmail,
                Id = 0
            };

            ActionModel.MaterialRequestInfo.Members.Add(memberDetails);
            Coordinator = memberDetails;
        }

        protected virtual void ValidateMemberTasks()
        {
            var invalidTasks = WizardModel.Members.Any(x => x.Tasks.Any(t => t.HoursSpent < 0));
            if (invalidTasks)
            {
                throw new UserFriendlySamException(Naati.Resources.MaterialRequest.ParticipantNumberOfHoursCannotBeNegative);
            }
        }


        protected virtual bool CheckMembers()
        {
            var modified = false;
            var selectedMembersIds = WizardModel.Members.Select(x => x.PanelMembershipId).ToList();

            var membersData = PanelService.GetPanelMembershipInfo(selectedMembersIds).Data.ToDictionary(x => x.PanelMemberShipId, y => y);

            var deletedMembers = ActionModel.MaterialRequestInfo.Members.Where(m => selectedMembersIds.All(x => x != m.PanelMemberShipId)).ToList();

            foreach (var deletedMember in deletedMembers)
            {
                ActionModel.MaterialRequestInfo.Members.Remove(deletedMember);
                modified = true;
            }

            var existingMembers = ActionModel.MaterialRequestInfo.Members.ToDictionary(x => x.PanelMemberShipId, y => y);

            foreach (var selectedMember in WizardModel.Members)
            {
                MaterialRequestPanelMembershipModel member = null;
                if (!existingMembers.TryGetValue(selectedMember.PanelMembershipId, out member))
                {
                    var data = membersData[selectedMember.PanelMembershipId];
                    member = new MaterialRequestPanelMembershipModel()
                    {
                        EntityId = data.EntityId,
                        GivenName = data.GivenName,
                        Tasks = new List<MaterialRequestTaskModel>(),
                        MemberTypeId = selectedMember.MemberTypeId,
                        NaatiNumber = data.NaatiNumber,
                        PanelMemberShipId = selectedMember.PanelMembershipId,
                        PrimaryEmail = data.PrimaryEmail,
                        Id = 0,
                    };

                    modified = true;
                    ActionModel.MaterialRequestInfo.Members.Add(member);
                }

                var selectedTaskIds = selectedMember.Tasks.Select(x => x.MaterialRequestTaskTypeId).ToList();
                var deletedTaks = member.Tasks.Where(m => selectedTaskIds.All(x => x != m.MaterialRequestTaskTypeId)).ToList();

                foreach (var deletedTask in deletedTaks)
                {
                    modified = true;
                    member.Tasks.Remove(deletedTask);
                }

                foreach (var memberTask in selectedMember.Tasks)
                {
                    if (memberTask.HoursSpent < 0)
                    {
                        throw new UserFriendlySamException(Naati.Resources.MaterialRequest.ParticipantNumberOfHoursCannotBeNegative);
                    }

                    var task = member.Tasks.FirstOrDefault(
                        x => x.MaterialRequestTaskTypeId == memberTask.MaterialRequestTaskTypeId);
                    if (task == null)
                    {
                        task = new MaterialRequestTaskModel()
                        {
                            MaterialRequestTaskTypeId = memberTask.MaterialRequestTaskTypeId
                        };

                        modified = true;
                        member.Tasks.Add(task);
                    }

                    if (Math.Abs(task.HoursSpent - memberTask.HoursSpent) > 0)
                    {
                        modified = true;
                        task.HoursSpent = memberTask.HoursSpent;
                    }
                }
            }

            return modified;
        }
    }
}
