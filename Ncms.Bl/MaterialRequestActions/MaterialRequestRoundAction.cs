using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.MaterialRequest;

namespace Ncms.Bl.MaterialRequestActions
{
    public abstract class MaterialRequestRoundAction : MaterialRequestAction
    {
        protected virtual MaterialRequestRoundStatusTypeName[] MaterialRequestRoundEntryStates { get; }
        protected virtual MaterialRequestRoundStatusTypeName MaterialRequestRoundExitState { get; }


        protected virtual MaterialRequestRoundStatusTypeName CurrentRoundEntryState { get; private set; }
        protected MaterialRequestRoundModel RoundModel { get; set; }

        protected override void ConfigureInstance(MaterialRequestActionModel actionModel, MaterialRequestWizardModel wizardModel, MaterialRequestActionOutput output)
        {
            base.ConfigureInstance(actionModel, wizardModel, output);

            if (wizardModel.MaterialRequestRoundId > 0)
            {
                RoundModel = actionModel.Rounds.SingleOrDefault(x => x.MaterialRoundId == wizardModel.MaterialRequestRoundId);
                if (RoundModel == null)
                {
                    throw new Exception($"Round {wizardModel.MaterialRequestRoundId} not found on Material Request model.");
                }

                CurrentRoundEntryState = (MaterialRequestRoundStatusTypeName)RoundModel.StatusTypeId;
            }
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

            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.AttachRoundRequestDocuments))
            {
                SetAttachments(materialRequestEmails, AttachmentType.RoundRequestDocuments);
            }
            if (template.EmailTemplateDetails.Contains(EmailTemplateDetailTypeName.AttachRoundResponseDocuments))
            {
                SetAttachments(materialRequestEmails, AttachmentType.RoundResponseDocuments);
            }

            return materialRequestEmails;
        }

        protected override void ValidateEntryState()
        {
            if (!MaterialRequestRoundEntryStates.Contains((MaterialRequestRoundStatusTypeName)RoundModel.StatusTypeId))
            {
                var entryStateNames = MaterialRequestRoundEntryStates.Select(x => MaterialRequestRoundStatusTypes.SingleOrDefault(y => y.Id == (int)x)?.DisplayName);
                throw new UserFriendlySamException(String.Format(Naati.Resources.MaterialRequest.WrongMaterialRequestRoundStatusErrorMessage,
                    string.Join(", ", entryStateNames)));
            }
        }

        protected override void SetExitState()
        {
            if (RoundModel.StatusTypeId != (int)MaterialRequestRoundExitState)
            {
                RoundModel.StatusTypeId = (int)MaterialRequestRoundExitState;
                RoundModel.StatusChangeDate = DateTime.Now;
            }
            RoundModel.ModifiedUserId = UserService.Get().Id;


        }
        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            TryInsertToken(tokenDictionary, TokenReplacementField.RoundDueDate, () => RoundModel.DueDate.ToShortDateString());
            TryInsertToken(tokenDictionary, TokenReplacementField.RoundNumber, () => RoundModel.RoundNumber.ToString());
            TryInsertToken(tokenDictionary, TokenReplacementField.SubmittedDate, () => RoundModel.SubmittedDate?.ToString("dd/MM/yyyy") ?? string.Empty);
            base.GetEmailTokens(tokenDictionary);
        }

        protected override string GetNote()
        {
            var entryStateName = MaterialRequestRoundStatusTypes.Single(x => x.Id == RoundModel.StatusTypeId).DisplayName;
            var exitStateName = MaterialRequestRoundStatusTypes.Single(x => x.Id == (int)MaterialRequestRoundExitState).DisplayName;
            if (entryStateName == exitStateName)
            {
                return string.Empty;
            }
            return String.Format(Naati.Resources.MaterialRequest.MaterialRquestRoundStatusChangeNote, RoundModel.RoundNumber, entryStateName, exitStateName);
        }

        protected override string GetPublicNote()
        {
            if (!string.IsNullOrWhiteSpace(WizardModel.PublicNotes))
            {
                return string.Format(Naati.Resources.MaterialRequest.RoundNote, RoundModel.RoundNumber, WizardModel.PublicNotes);
            }

            return string.Empty;
        }

        protected virtual void AddRoundLinks()
        {
            var userId = UserService.Get().Id;
            foreach (var link in WizardModel.RoundLinks)
            {
                if (string.IsNullOrWhiteSpace(link))
                {
                    continue;
                }

                RoundModel.Links.Add(new MaterialRequestRoundLinkModel() { Link = link, UserId = userId, NcmsAvailable = true });
            }
        }

        protected override void AddDocuments()
        {
            foreach (var existingDocument in WizardModel.ExistingDocuments)
            {
                if (string.IsNullOrWhiteSpace(existingDocument.DisplayName))
                {
                    throw new Exception("Document Name has not been specified");
                }
                RoundModel.Documents.Add(new MaterialRequestDocumentInfoModel()
                {
                    StoredFileId = existingDocument.Id,
                    Description = Path.GetFileNameWithoutExtension(existingDocument.DisplayName ?? string.Empty) + existingDocument.FileType,
                    ExaminersAvailable = existingDocument.ExaminersAvailable,
                    NcmsAvailable = true,
                });
            }

            var userId = UserService.Get().Id;
            var newDocuments = DocumentUploadWizardHelper.GetDocuments(new DirectoryInfo(ConfigurationManager.AppSettings["tempFilePath"]),
                WizardModel.NewDocumentsModel.WizardInstanceId, true);
            
            foreach (var newDocument in newDocuments)
            {
                RoundModel.Documents.Add(new MaterialRequestDocumentInfoModel()
                {
                    StoredFileId = 0,
                    Description = newDocument.Title,
                    DocumentTypeId = newDocument.DocumenTypeId,
                    FileName = newDocument.FileName,
                    FilePath = newDocument.FilePath,
                    UserId = userId,
                    ExaminersAvailable = newDocument.ExaminersAvailable,
                    NcmsAvailable = true
                });

            }
        }

    }
}
