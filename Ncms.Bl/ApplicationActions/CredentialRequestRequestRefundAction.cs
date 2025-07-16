using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.Refund;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.System;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestRequestRefundAction : CredentialRequestStateAction
    {
        protected ICredentialApplicationRefundService CredentialApplicationRefundService => ServiceLocatorInstance.Resolve<ICredentialApplicationRefundService>();

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] {
            CredentialRequestStatusTypeName.TestAccepted
        };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => GetExitState();

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.CredentialRequest;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.RequestRefund;

        protected override IList<Action> Preconditions => new List<Action>
                                                                        {
                                                                            ValidateEntryState,
                                                                            ValidatePayPalPayment,
                                                                            ValidateUserPermissions
                                                                        };

        protected override IList<Action> SystemActions => new List<Action>
                                                                        {
                                                                            ValidateRefundRequest,
                                                                            CreateRefund,
                                                                            CreateNote,
                                                                            SetExitState
                                                                        };

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            var stepData = WizardModel.RefundStepData;
            var workflowFeeId = stepData?.CredentialWorkflowFeeId ?? RefundRequest.CredentialWorkflowFeeId;
            var workflowFee = CredentialRequestModel.CredentialWorkflowFees.Single(
                workflowFeeItem => workflowFeeItem.Id == workflowFeeId);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.RefundPolicy), $"{workflowFee.CredentialApplicationRefundPolicy.Description}");
        }

        protected override void ValidateEntryState()
        {
            base.ValidateEntryState();

            var workflowFee = CredentialRequestModel.CredentialWorkflowFees
                .Where(workflowFeeItem => workflowFeeItem.PaymentActionProcessedDate != null)
                .OrderByDescending(workflowFeeItem => workflowFeeItem.PaymentActionProcessedDate)
                .FirstOrDefault();

            var cutoffDate = SystemService.GetSystemValue("AccountingCutOffDate");

            if (workflowFee != null && workflowFee.PaymentActionProcessedDate < Convert.ToDateTime(cutoffDate))
            {
                throw new UserFriendlySamException(Naati.Resources.Application.InvalidRefundRequestDate);
            }
        }

        private void ValidatePayPalPayment()
        {
            var refundTypeId = 0;

            var workflowFee = CredentialRequestModel.CredentialWorkflowFees
                .Where(workflowFeeItem => workflowFeeItem.PaymentActionProcessedDate != null)
                .OrderByDescending(workflowFeeItem => workflowFeeItem.PaymentActionProcessedDate)
                .FirstOrDefault();

            // Get Refund Type from Order Number
            var orderNumber = workflowFee.OrderNumber;
            if (!string.IsNullOrEmpty(orderNumber) && orderNumber.StartsWith("PAYID"))
            {
                refundTypeId = (int)RefundMethodTypeName.PayPal;
            }

            if (refundTypeId == (int)RefundMethodTypeName.PayPal)
            {
                var isValid = CredentialApplicationRefundService.ValidatePayPalDateForRefund(CredentialRequestModel.Id);

                if (isValid)
                {
                    return;
                }

                throw new UserFriendlySamException("The PayPal payment has exceeded 180 days. PayPal's policy does not allow refund to proceed. Please contact finance@naati.com.au.");
            }

            return;
        }

        private void ValidateRefundRequest()
        {
            if (WizardModel.Source == SystemActionSource.MyNaati)
            {
                ValidateRefundRequest(
                    WizardModel.RefundPercentage,
                    WizardModel.RefundMethodTypeId,
                    WizardModel.CredentialWorkflowFeeId);
                
            }
            else if (WizardModel.RefundStepData != null)
            {
                var wizardRefundModel = WizardModel.RefundStepData;
                ValidateRefundApprovalRequest(
                    wizardRefundModel.RefundPercentage,
                    wizardRefundModel.RefundMethodTypeId,
                    wizardRefundModel.CredentialWorkflowFeeId,
                    wizardRefundModel.RefundAmount);
            }
            else
            {
                throw new Exception("Invalid refund request");
            }

            if (WizardModel.RefundMethodTypeId == (int)RefundMethodTypeName.DirectDeposit
                && string.IsNullOrWhiteSpace(WizardModel.RefundBankDetails))
            {
                throw new Exception("Invalid refund bank account details");
            }
        }

        protected void CreateRefund()
        {
            RefundModel refundModel;
            if (WizardModel.Source == SystemActionSource.Ncms)
            {
                var stepData = WizardModel.RefundStepData;
                refundModel = new RefundModel
                {
                    RefundPercentage = stepData.RefundPercentage,
                    RefundAmount = Convert.ToDecimal(stepData.RefundAmount),
                    RefundMethodTypeId = stepData.RefundMethodTypeId,
                    CredentialWorkflowFeeId = stepData.CredentialWorkflowFeeId,
                    UserId = UserService.Get().Id,
                    ObjectStatusId = (int)ObjectStatusTypeName.Created,
                    IsRejected = false,
                    Comments = stepData.Comments,
                    BankDetails = stepData.BankDetails
                };
            }
            else
            {
                refundModel = new RefundModel
                {
                    RefundPercentage = WizardModel.RefundPercentage,
                    RefundMethodTypeId = WizardModel.RefundMethodTypeId.GetValueOrDefault(),
                    CredentialWorkflowFeeId = WizardModel.CredentialWorkflowFeeId.GetValueOrDefault(),
                    UserId = UserService.Get().Id,
                    ObjectStatusId = (int)ObjectStatusTypeName.Created,
                    IsRejected = false,
                    Comments = WizardModel.RefundComments,
                    BankDetails = WizardModel.RefundBankDetails
                };
            }

            CredentialRequestModel.RefundRequests.Add(refundModel);
        }

        private CredentialRequestStatusTypeName GetExitState()
        {
            return CredentialRequestStatusTypeName.RefundRequested;
        }

        protected override void CreateNote()
        {
            base.CreateNote();
            var refundNote = ApplicationModel.Notes[ApplicationModel.Notes.Count - 1];
            refundNote.Note = $"{refundNote.Note}\n\nRefund Comments: {WizardModel.RefundComments}";
        }
    }
}