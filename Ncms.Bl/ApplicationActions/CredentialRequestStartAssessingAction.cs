using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestStartAssessingAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.ReadyForAssessment };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.BeingAssessed;


        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.CredentialRequest;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Assess;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateApplicationState,
                                                              ValidateMandatoryFields,
                                                              ValidateMandatoryDocuments,
                                                              ValidateMandatoryPersonFields,
                                                              ValidateCredentialRequestInvoices,
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              SetOwner,
                                                              CreateNote,
                                                              SetExitState,
                                                          };
    }
}