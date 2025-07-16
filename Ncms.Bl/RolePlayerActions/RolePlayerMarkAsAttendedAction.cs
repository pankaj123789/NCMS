using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerMarkAsAttendedAction : RolePlayerUpdateAction
    {
        protected override RolePlayerStatusTypeName[] RolePlayerEntryStates => new[] { RolePlayerStatusTypeName.Rehearsed, RolePlayerStatusTypeName.NoShow };

        protected override RolePlayerStatusTypeName RolePlayerExitState => RolePlayerStatusTypeName.Attended;

        protected override void SetActionFlag()
        {
            ActionModel.TestSessionRolePlayer.Attended = true;

        }
    }
}
