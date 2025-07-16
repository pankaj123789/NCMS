using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestPendAssessmentAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.BeingAssessed };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.Pending;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateApplicationState,
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
                                                          };

        protected override void ClearOwner()
        {
            // clear the owner if none of the credential requests are in state [ReadyForAssessment, BeingAssessed]
            var keepOwnerStates = new[] { CredentialRequestStatusTypeName.ReadyForAssessment, CredentialRequestStatusTypeName.BeingAssessed };
            var allOtherCredentialRequests = ApplicationModel.CredentialRequests.Where(x => x.Id != CredentialRequestModel.Id);
            if (!allOtherCredentialRequests.Any(x => keepOwnerStates.Contains((CredentialRequestStatusTypeName)x.StatusTypeId)))
            {
                base.ClearOwner();
            }
        }
    }
}