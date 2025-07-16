using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestRoundApproveAction: MaterialRequestRoundAction
    {
        protected override MaterialRequestRoundStatusTypeName[] MaterialRequestRoundEntryStates => new[] { MaterialRequestRoundStatusTypeName.AwaitingApproval };

        protected override MaterialRequestRoundStatusTypeName MaterialRequestRoundExitState => MaterialRequestRoundStatusTypeName.Approved;

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateUserPermissions,
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
                    SetOwner,
                    CreateNote,
                    CreatePublicNote,
                    SetMaterialRequestState,
                    SetExitState
                };

                return actions;
            }
        }

        protected virtual void SetMaterialRequestState()
        {
            ActionModel.MaterialRequestInfo.StatusTypeId = (int)MaterialRequestStatusTypeName.AwaitingFinalisation;
            ActionModel.MaterialRequestInfo.StatusChangeUserId = CurrentUser.Id;
            ActionModel.MaterialRequestInfo.StatusChangeDate = DateTime.Now;
        }


        protected override string GetNote()
        {
            var entryStateName = MaterialRequestStatusTypes.Single(x => x.Id == ActionModel.MaterialRequestInfo.StatusTypeId).DisplayName;
            var exitStateName = MaterialRequestStatusTypes.Single(x => x.Id == (int)MaterialRequestStatusTypeName.AwaitingFinalisation).DisplayName;
            if (entryStateName == exitStateName)
            {
                return string.Empty;
            }
            var note = String.Format(Naati.Resources.MaterialRequest.MaterialRquestStatusChangeNote, entryStateName, exitStateName);
            return string.Join(". ", new string[] {  base.GetNote(),note });
        }

    }
}
