using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerMarkAsPendingAction : RolePlayerUpdateAction
    {
        protected override RolePlayerStatusTypeName[] RolePlayerEntryStates => new[] { RolePlayerStatusTypeName.Accepted };

        protected override RolePlayerStatusTypeName RolePlayerExitState => RolePlayerStatusTypeName.Pending;

    }
}
