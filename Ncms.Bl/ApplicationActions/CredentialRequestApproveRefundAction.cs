using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestApproveRefundAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] {
            CredentialRequestStatusTypeName.RefundRequested
        };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.RefundRequestApproved;

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.CredentialRequest;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.ApproveRefund;

        protected override IList<Action> Preconditions => new List<Action>
                                                                        {
                                                                            ValidateEntryState,
                                                                            ValidateUserPermissions
                                                                        };

        protected override IList<Action> SystemActions => new List<Action>
                                                                        {
                                                                            ValidateRefundRequest,
                                                                            ApproveRefundRequest,
                                                                            CreateNote,
                                                                            SetExitState
                                                                        };

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RefundAmount), RefundRequest.RefundAmount.ToString());

            var workflowFee = CredentialRequestModel.CredentialWorkflowFees.Single(
                workflowFeeItem => workflowFeeItem.Id == RefundRequest.CredentialWorkflowFeeId);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RefundPolicy), $"{workflowFee.CredentialApplicationRefundPolicy.Description}");
        }

        private void ApproveRefundRequest()
        {
            var stepData = WizardModel.RefundStepData;
            if (stepData != null)
            {
                RefundRequest.RefundPercentage = stepData.RefundPercentage;
                RefundRequest.RefundAmount = Convert.ToDecimal(stepData.RefundAmount);
                RefundRequest.RefundMethodTypeId = stepData.RefundMethodTypeId;
                RefundRequest.Comments = stepData.Comments; 
                RefundRequest.BankDetails = stepData.BankDetails; 
            }
            else
            {
                RefundRequest.RefundPercentage = WizardModel.RefundPercentage;
                RefundRequest.RefundAmount = Convert.ToDecimal(WizardModel.RefundAmount);
                RefundRequest.RefundMethodTypeId = WizardModel.RefundMethodTypeId.GetValueOrDefault();
            }
            RefundRequest.ObjectStatusId = (int)ObjectStatusTypeName.Updated;
        }

        private void ValidateRefundRequest()
        {
            var step = WizardModel.RefundStepData;
            if (step != null)
            {
                ValidateRefundApprovalRequest(
                    step.RefundPercentage,
                    step.RefundMethodTypeId,
                    step.CredentialWorkflowFeeId,
                    step.RefundAmount);
            }
            else if(WizardModel.RefundPercentage.HasValue && WizardModel.RefundAmount.HasValue)
            {
                ValidateRefundApprovalRequest(
                    WizardModel.RefundPercentage,
                    WizardModel.RefundMethodTypeId,
                    WizardModel.CredentialWorkflowFeeId,
                    WizardModel.RefundAmount);
            }
            else
            {
                throw new Exception("Invalid refund request");
            }
        }
    }
}
