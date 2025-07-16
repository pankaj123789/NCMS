using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerMarkAsRehearsedAction : RolePlayerUpdateAction
    {
        protected override RolePlayerStatusTypeName[] RolePlayerEntryStates => new[] { RolePlayerStatusTypeName.NoShow, RolePlayerStatusTypeName.Accepted, RolePlayerStatusTypeName.Attended };

        protected override RolePlayerStatusTypeName RolePlayerExitState => RolePlayerStatusTypeName.Rehearsed;

        protected override void SetActionFlag()
        {
            ActionModel.TestSessionRolePlayer.Attended = false;
            ActionModel.TestSessionRolePlayer.Rehearsed = true;
        }
    }
}
