using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerMarkAsRejectedAction : RolePlayerUpdateAction
    {
        protected override RolePlayerStatusTypeName[] RolePlayerEntryStates => new[] { RolePlayerStatusTypeName.Pending, RolePlayerStatusTypeName.Accepted };
        protected override RolePlayerStatusTypeName RolePlayerExitState => RolePlayerStatusTypeName.Rejected;

        protected override void SetActionFlag()
        {

            ActionModel.TestSessionRolePlayer.Rejected = true;
        }
    }
}
