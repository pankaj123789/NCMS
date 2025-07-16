using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestCreateReviewAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.AssessmentFailed };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.AssessmentPaidReview;

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
                                                              ClearOwner, // todo check with Pas (UC says check with Kevin)
                                                              CreateNote,
                                                              SetExitState,
                                                          };

        // may be needed in future?
        //protected override void ClearOwner()
        //{
        //    // clear the owner if none of the credential requests are in state [ReadyForAssessment, BeingAssessed]
        //    var keepOwnerStates = new[] { CredentialRequestStatusTypeName.ReadyForAssessment, CredentialRequestStatusTypeName.BeingAssessed };
        //    var allOtherCredentialRequests = ApplicationModel.CredentialRequests.Where(x => x.Id != CredentialRequestModel.Id);
        //    if (!allOtherCredentialRequests.Any(x => keepOwnerStates.Contains((CredentialRequestStatusTypeName)x.StatusTypeId)))
        //    {
        //        base.ClearOwner();
        //    }
        //}
    }
}