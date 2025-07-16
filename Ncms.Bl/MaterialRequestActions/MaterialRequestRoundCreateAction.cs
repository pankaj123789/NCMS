using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.MaterialRequest;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestRoundCreateAction : MaterialRequestRoundAction
    {

        protected override MaterialRequestRoundStatusTypeName[] MaterialRequestRoundEntryStates => new[] { MaterialRequestRoundStatusTypeName.None };

        protected override MaterialRequestRoundStatusTypeName MaterialRequestRoundExitState => MaterialRequestRoundStatusTypeName.SentForDevelopment;

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateUserPermissions,
                    CreateRound,
                    ValidatePreviousRounds,
                    ValidateEntryState,
                };
                
                return actions;
            }
        }
        
        protected override IList<Action> SystemActions
        {
            get
            {
                var actions = new List<Action>
                {
                    PopulateRoundDetails,
                    AddNewRound,
                    AddDocuments,
                    AddRoundLinks,
                    SetRequestedDate,
                    CreateNote,
                    CreatePublicNote,
                    SetExitState
                };

                return actions;
            }
        }
        
        protected virtual void ValidatePreviousRounds()
        {
            if (ActionModel.Rounds.Any(x => x.StatusTypeId != (int)MaterialRequestRoundStatusTypeName.Rejected))
            {
                throw new UserFriendlySamException(Naati.Resources.MaterialRequest.RoundsaMustBeFinalised);
            }
        }

        protected override void SetExitState()
        {
            RoundModel.StatusTypeId = (int)MaterialRequestRoundExitState;
            RoundModel.StatusChangeDate = DateTime.Now;
            RoundModel.ModifiedUserId = UserService.Get().Id;
        }
        
        protected virtual void AddNewRound()
        {
            ActionModel.Rounds.Add(RoundModel);
        }

        protected virtual void CreateRound()
        {
            RoundModel = new MaterialRequestRoundModel()
            {
                Documents = new List<MaterialRequestDocumentInfoModel>(),
                Links = new List<MaterialRequestRoundLinkModel>(),
            };
        }

        protected virtual void PopulateRoundDetails()
        {
            if (WizardModel.MaterialRequestRoundId != 0)
            {
                throw new ArgumentException(nameof(WizardModel.MaterialRequestRoundId));
            }
            if (WizardModel.RoundDetails == null)
            {
                throw new ArgumentException(nameof(WizardModel.RoundDetails));
            }

            if (WizardModel.RoundDetails.RoundNumber <= 0)
            {
                throw new ArgumentException(nameof(WizardModel.RoundDetails.RoundNumber));
            }

            if (WizardModel.RoundDetails.DueDate <= DateTime.Now)
            {
                throw new ArgumentException(nameof(WizardModel.RoundDetails.DueDate));
            }

            RoundModel.RoundNumber = WizardModel.RoundDetails.RoundNumber;
            RoundModel.DueDate = WizardModel.RoundDetails.DueDate.Date;
        }

        protected virtual void SetRequestedDate()
        {
            RoundModel.RequestedDate = DateTime.Now;
        }
        protected override string GetNote()
        {
            return string.Format(Naati.Resources.MaterialRequest.MaterialRequestRoundCreatedNote, RoundModel.RoundNumber);
        }

        protected override void LogAction()
        {
            LoggingHelper.LogInfo("Performing workflow action {Action} for MaterialRequestRound{RoundId}",
                (SystemActionTypeName)WizardModel.ActionType, RoundModel.MaterialRoundId);
        }


        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            TryInsertToken(tokenDictionary, TokenReplacementField.RoundDueDate, () => WizardModel.RoundDetails.DueDate.ToShortDateString());
            TryInsertToken(tokenDictionary, TokenReplacementField.RoundNumber, () => WizardModel.RoundDetails.RoundNumber.ToString());
            TryInsertToken(tokenDictionary, TokenReplacementField.SubmittedDate, () => string.Empty);
            base.GetEmailTokens(tokenDictionary);
        }


        protected override GenericResponse<UpsertMaterialRequestResultModel> SaveActionData()
        {
            var result = base.SaveActionData();
            WizardModel.MaterialRequestRoundId = result.Data.MaterialRequestRoundIds.Max();
            RoundModel.MaterialRoundId = result.Data.MaterialRequestRoundIds.Max();
            return result;
        }

        protected override void CreateEmailAttachmentsIfApplicable()
        {
            if (Output.UpsertResults.Success)
            {
                foreach (var storeFileId in Output.UpsertResults.Data.StoredFileIds)
                {
                    Attachments.Add(new EmailMessageAttachmentModel
                    {
                        StoredFileId = storeFileId,
                        AttachmentType = AttachmentType.RoundRequestDocuments
                    });
                }
            }
        }
    }
}
