using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestRejectRefundAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] {
            CredentialRequestStatusTypeName.RefundRequested,
            CredentialRequestStatusTypeName.RefundFailed,
        };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.TestAccepted;

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.CredentialRequest;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.RejectRefund;

        protected override IList<Action> Preconditions => new List<Action>
                                                                        {
                                                                            ValidateEntryState,
                                                                            ValidateUserPermissions
                                                                        };

        protected override IList<Action> SystemActions => new List<Action>
                                                                        {
                                                                            RejectRefundRequest,
                                                                            CreateNote,
                                                                            SetExitState
                                                                        };

        private void RejectRefundRequest()
        {
            RefundRequest.IsRejected = true;
            RefundRequest.ObjectStatusId = (int)ObjectStatusTypeName.Updated;
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            var workflowFee = CredentialRequestModel.CredentialWorkflowFees.Single(
                workflowFeeItem => workflowFeeItem.Id == RefundRequest.CredentialWorkflowFeeId);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RefundPolicy), $"{workflowFee.CredentialApplicationRefundPolicy.Description}");
        }
    }
}
