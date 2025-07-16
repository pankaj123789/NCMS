using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerMarkAsNoShowAction : RolePlayerUpdateAction
    {
        protected override RolePlayerStatusTypeName[] RolePlayerEntryStates => new[] { RolePlayerStatusTypeName.Accepted, RolePlayerStatusTypeName.Rehearsed };

        protected override RolePlayerStatusTypeName RolePlayerExitState => RolePlayerStatusTypeName.NoShow;
       
    }
}
