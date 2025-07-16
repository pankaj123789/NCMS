using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.RolePlayerActions
{
    public class RolePlayerNotifyAllocationUpdateAction : RolePlayerAllocateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.RolePlayer;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Notify;
    }
}
