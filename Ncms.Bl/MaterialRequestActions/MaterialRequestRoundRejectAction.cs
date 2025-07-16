using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestRoundRejectAction : MaterialRequestRoundAction
    {
        protected override MaterialRequestRoundStatusTypeName[] MaterialRequestRoundEntryStates => new[] { MaterialRequestRoundStatusTypeName.SentForDevelopment, MaterialRequestRoundStatusTypeName.AwaitingApproval };

        protected override MaterialRequestRoundStatusTypeName MaterialRequestRoundExitState => MaterialRequestRoundStatusTypeName.Rejected;

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
                    SetExitState
                };

                return actions;
            }
        }
        
    }
}
