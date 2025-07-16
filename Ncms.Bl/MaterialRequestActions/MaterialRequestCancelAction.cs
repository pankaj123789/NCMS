using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestCancelAction : MaterialRequestAction
    {
        protected override MaterialRequestStatusTypeName[] MaterialRequestEntryStates => new[] { MaterialRequestStatusTypeName.InProgress, MaterialRequestStatusTypeName.AwaitingFinalisation };

        protected override MaterialRequestStatusTypeName MaterialRequestExitState => MaterialRequestStatusTypeName.Cancelled;

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateUserPermissions,
                    ValidateEntryState,
                    CancelRound

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
                    ClearOwner,
                    SetExitState
                };

                return actions;
            }
        }

        private void CancelRound()
        {
            foreach (var round in ActionModel.Rounds)
            {
                if (round.StatusTypeId != (int)MaterialRequestRoundStatusTypeName.Rejected && round.StatusTypeId != (int)MaterialRequestRoundStatusTypeName.Rejected)
                {
                    round.StatusTypeId = (int) MaterialRequestRoundStatusTypeName.Cancelled;
                }
            }
        }
    }
}
