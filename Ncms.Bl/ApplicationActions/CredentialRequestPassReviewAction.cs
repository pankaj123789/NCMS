using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestPassReviewAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.AssessmentPaidReview };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.AssessmentComplete;


        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.CredentialRequest;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Assess;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateMandatoryFields,
                                                              ValidateMandatoryDocuments,
                                                              ValidateMandatoryPersonFields,
                                                              ValidateCredentialRequestInvoices
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              ClearOwner,
                                                              CreateNote,
                                                              SetExitState,
                                                              CreateTest,
                                                          };

        private void CreateTest()
        {
            // todo for Jan/Febish - UC5000 BR14
        }
    }
}