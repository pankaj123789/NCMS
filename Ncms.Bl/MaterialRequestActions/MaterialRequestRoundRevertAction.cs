using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestRoundRevertAction : MaterialRequestRoundAction
    {
        protected override MaterialRequestRoundStatusTypeName[] MaterialRequestRoundEntryStates => new[] { MaterialRequestRoundStatusTypeName.Rejected, MaterialRequestRoundStatusTypeName.Approved };

        protected override MaterialRequestRoundStatusTypeName MaterialRequestRoundExitState => MaterialRequestRoundStatusTypeName.AwaitingApproval;

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateMaterialRequestStatus,
                    ValidateUserPermissions,
                    ValidateRound,
                    ValidateEntryState,
                    ValidateMembersPayment
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
                    CreateNote,
                    CreatePublicNote,
                    SetMaterialRequestState,
                    SetOwner,
                    SetExitState
                };

                return actions;
            }
        }

        protected override string GetPublicNote()
        {
            return string.Format(Naati.Resources.MaterialRequest.RoundNote, RoundModel.RoundNumber, $"{Naati.Resources.MaterialRequest.RoundReverted}");
        }

        private void ValidateRound()
        {
            var maxRound = ActionModel.Rounds.Max(x => x.RoundNumber);
            if (RoundModel.RoundNumber != maxRound)
            {
                throw new UserFriendlySamException(Naati.Resources.MaterialRequest.OnlyLastestRoundCanBeReverted);
            }
        }

        private void ValidateMembersPayment()
        {
            if (ActionModel.MaterialRequestInfo.Members.Any(x => x.PayRoll?.ApprovedDate != null))
            {
                throw new UserFriendlySamException(Naati.Resources.MaterialRequest.PaymentHasBeenAlreadyApproved);
            }
        }

        private void ValidateMaterialRequestStatus()
        {
            if (!new[] { MaterialRequestStatusTypeName.AwaitingFinalisation, MaterialRequestStatusTypeName.InProgress }.Contains((MaterialRequestStatusTypeName)ActionModel.MaterialRequestInfo.StatusTypeId))
            {
                var entryStateNames = new[] { MaterialRequestStatusTypeName.AwaitingFinalisation }.Select(x => MaterialRequestStatusTypes.SingleOrDefault(y => y.Id == (int)x)?.DisplayName);
                throw new UserFriendlySamException(String.Format(Naati.Resources.MaterialRequest.WrongMaterialRequestStatusErrorMessage,
                    string.Join(", ", entryStateNames)));
            }
        }

        protected virtual void SetMaterialRequestState()
        {
            ActionModel.MaterialRequestInfo.StatusTypeId = (int)MaterialRequestStatusTypeName.InProgress;
            ActionModel.MaterialRequestInfo.StatusChangeUserId = CurrentUser.Id;
            ActionModel.MaterialRequestInfo.StatusChangeDate = DateTime.Now;
        }


        protected override string GetNote()
        {
            var entryStateName = MaterialRequestStatusTypes.Single(x => x.Id == ActionModel.MaterialRequestInfo.StatusTypeId).DisplayName;
            var exitStateName = MaterialRequestStatusTypes.Single(x => x.Id == (int)MaterialRequestStatusTypeName.InProgress).DisplayName;
            var notes = new List<string>();
            if (entryStateName != exitStateName)
            {
                notes.Add(String.Format(Naati.Resources.MaterialRequest.MaterialRquestStatusChangeNote, entryStateName, exitStateName));
                return string.Join(". ", notes);
            }
            notes.Insert(0, base.GetNote());
            return string.Join(". ", notes);
        }


    }
}
