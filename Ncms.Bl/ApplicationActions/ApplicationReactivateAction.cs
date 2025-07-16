using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationReactivateAction : ApplicationStateAction
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.Completed };
        protected override CredentialApplicationStatusTypeName ApplicationExitState => CredentialApplicationStatusTypeName.InProgress;

        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Reactivate;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              CreateNote,
                                                              SetExitState
                                                          };
    }
}
