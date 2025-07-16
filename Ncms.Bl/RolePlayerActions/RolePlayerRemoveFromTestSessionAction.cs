using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerRemoveFromTestSessionAction : AssignRolePlayersWizardRolePlayerAction
    {
        protected override RolePlayerStatusTypeName[] RolePlayerEntryStates => new[] { RolePlayerStatusTypeName.Pending, RolePlayerStatusTypeName.Accepted };
        protected override RolePlayerStatusTypeName RolePlayerExitState => RolePlayerStatusTypeName.None;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
        };

       

        protected override IList<Action> SystemActions => new List<Action>
        {
            RemoveRolePlayerTasks,
            RemoveRolePlayer,
            CreatePersonNote,
            SetExitState
        };
        protected override string GetPersonNote()
        {
            return String.Format(Naati.Resources.Shared.RolePlayerRemovedFromTestSession, WizardModel.TestSessionId);
        }

    }
}
