using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerMarkAsRemovedAction : RolePlayerUpdateAction
    {
        protected override RolePlayerStatusTypeName[] RolePlayerEntryStates => new[] { RolePlayerStatusTypeName.Rejected };

        protected override RolePlayerStatusTypeName RolePlayerExitState => RolePlayerStatusTypeName.Attended;

        protected override void SetActionFlag()
        {
            RemoveRolePlayerTasks();
            RemoveRolePlayer();
        }

        protected override string GetPersonNote()
        {
            return String.Format(Naati.Resources.Shared.RolePlayerRemovedFromTestSession, ActionModel.TestSessionRolePlayer.TestSessionId);
        }
    }
}
