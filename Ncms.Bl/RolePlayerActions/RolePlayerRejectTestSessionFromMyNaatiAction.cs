using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerRejectTestSessionFromMyNaatiAction : RolePlayerMarkAsRejectedAction
    {
        protected override RolePlayerStatusTypeName[] RolePlayerEntryStates => new[] { RolePlayerStatusTypeName.Pending };
    }
}
