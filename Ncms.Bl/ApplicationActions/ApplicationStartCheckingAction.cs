using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationStartCheckingAction : ApplicationStateAction
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.Entered };
        protected override CredentialApplicationStatusTypeName ApplicationExitState => CredentialApplicationStatusTypeName.BeingChecked;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Validate;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateMinimumCredentialRequests,
                                                              ValidateMandatoryFields,
                                                              ValidateMandatoryPersonFields,
                                                              ValidateMandatoryDocuments,
                                                              ValidateNotTrustedApplicationInvoices
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              SetOwner,
                                                              CreateNote,
                                                              SetExitState,
                                                          };
    }
}