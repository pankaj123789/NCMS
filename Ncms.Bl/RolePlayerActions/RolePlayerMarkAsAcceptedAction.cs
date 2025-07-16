using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerMarkAsAcceptedAction : RolePlayerUpdateAction
    {

        protected override RolePlayerStatusTypeName[] RolePlayerEntryStates => new[] { RolePlayerStatusTypeName.Rehearsed , RolePlayerStatusTypeName.Pending, RolePlayerStatusTypeName.NoShow };
        protected override RolePlayerStatusTypeName RolePlayerExitState => RolePlayerStatusTypeName.Accepted;
        protected override void SetActionFlag()
        {
            ActionModel.TestSessionRolePlayer.Rehearsed = false;
            ActionModel.TestSessionRolePlayer.Attended = false;
        }
    }
}
