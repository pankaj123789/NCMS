using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationRejectAction : ApplicationStateAction
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.BeingChecked, CredentialApplicationStatusTypeName.AwaitingApplicationPayment, CredentialApplicationStatusTypeName.AwaitingAssessmentPayment } ;
        protected override CredentialApplicationStatusTypeName ApplicationExitState => CredentialApplicationStatusTypeName.Rejected;

        protected override SecurityNounName? RequiredSecurityNoun =>
            (CredentialApplicationStatusTypeName)ApplicationModel.ApplicationStatus.Id == CredentialApplicationStatusTypeName.BeingChecked
                ? SecurityNounName.Application
                : SecurityNounName.FinanceOther;

        protected override SecurityVerbName? RequiredSecurityVerb =>
            (CredentialApplicationStatusTypeName)ApplicationModel.ApplicationStatus.Id == CredentialApplicationStatusTypeName.BeingChecked
                ? SecurityVerbName.Reject
                : SecurityVerbName.Manage;

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateEntryState,
                    ValidateUserPermissions,
                };

                if ((CredentialApplicationStatusTypeName)ApplicationModel.ApplicationStatus.Id == CredentialApplicationStatusTypeName.BeingChecked)
                {
                    actions.Add(ValidateMinimumCredentialRequests);
                    actions.Add(ValidateMandatoryFields);
                }

                return actions;
            }
        }

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              ClearOwner,
                                                              CreateNote,
                                                              SetExitState,
                                                              UpdateCredentialRequestStatus,
                                                          };

        protected override void UpdateCredentialRequestStatus()
        {
            foreach (var credentialRequest in ApplicationModel.CredentialRequests)
            {
                credentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.Rejected;
                credentialRequest.StatusChangeUserId = CurrentUser.Id;
                credentialRequest.StatusChangeDate = DateTime.Now;
            }
        }
    }
}