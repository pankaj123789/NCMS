using F1Solutions.Naati.Common.Contracts.Bl.Payment;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common;
using Ncms.Contracts.Models.Application;


namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestProcessRefundAction : CredentialRequestCreditNoteBatchedAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.CredentialRequest;

        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.ProcessRefund;

        protected override bool HasCreditCardPayment => RefundRequest.RefundMethodTypeId == (int)RefundMethodTypeName.CreditCard;
        protected override bool HasPayPalPayment => RefundRequest.RefundMethodTypeId == (int)RefundMethodTypeName.PayPal;

        protected override SystemActionTypeName CreditNoteProcessedAction => SystemActionTypeName.CreditNoteProcessed;

        protected override SystemActionTypeName CreditNotePaidAction => SystemActionTypeName.CreditNotePaid;

        protected override CredentialRequestStatusTypeName ProcessingCreditNoteStatus => GetProcessingCreditNoteStatus();

        protected override CredentialRequestStatusTypeName PendingPaymentStatus => CredentialRequestStatusTypeName.AwaitingCreditNotePayment;

        protected override CredentialRequestStatusTypeName CreditNotePaidStatus => CredentialRequestStatusTypeName.Withdrawn;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.RefundRequestApproved };

        protected string RefundErrorMessage { get; set; } 

        private string _OrderNumber;

        protected override IList<Action> Preconditions => new List<Action>
                                                            {
                                                                ValidateEntryState,
                                                                ValidateUserPermissions,
                                                                ValidateRefundFields
                                                            };

        protected override IList<Action> SystemActions => new List<Action>
                                                            {
                                                                RefundUsingSecurePayIfApplicable,
                                                                RefundUsingPayPalIfApplicable,
                                                                CreateRefundIfApplicable,
                                                                ProcessRefundRequest,
                                                                CreateNote,
                                                                SetExitState
                                                            };

        protected override void CreateRefundIfApplicable()
        {
            if (!string.IsNullOrWhiteSpace(RefundErrorMessage))
            {
                return;
            }

            base.CreateRefundIfApplicable();
        }

        protected virtual CredentialRequestStatusTypeName GetProcessingCreditNoteStatus()
        {
            if (!string.IsNullOrWhiteSpace(RefundErrorMessage))
            {
                return CredentialRequestStatusTypeName.RefundFailed;
            }

            return CredentialRequestStatusTypeName.ProcessingCreditNote;
        }

        protected override string GetNote()
        {
            var baseNote = base.GetNote();
            return $"{RefundErrorMessage}. {baseNote}";
        }

        private void RefundUsingPayPalIfApplicable()
        {
            if (HasPayPalPayment)
            {
                if (RefundRequest.RefundAmount.GetValueOrDefault() <= 0)
                {
                    throw new Exception("Invalid refund amount");
                }

                var workflowFee = CredentialRequestModel.CredentialWorkflowFees.Single(
                    workflowFeeItem => workflowFeeItem.Id == RefundRequest.CredentialWorkflowFeeId);

                _OrderNumber = workflowFee.OrderNumber;

                //call paypal
                RefundRequest.DisallowProcessing = true;
                ApplicationService.UpdateCredentialApplicationRefundRequest(RefundRequest);

                var apiContext = PayPalConfigurationHelper.GetAPIContext();
                var response = PayPalService.ExecuteRefund(PayPalConfigurationHelper.GetAPIContext(), RefundRequest.RefundAmount.GetValueOrDefault().ToString(),workflowFee.TransactionId);

                if (response.state == PayPalProcessStatus.Completed)
                {
                    RefundRequest.RefundedDate = DateTime.Now;
                    RefundRequest.RefundTransactionId = response.id;
                    RefundRequest.ObjectStatusId = (int)ObjectStatusTypeName.Updated;
                }
                else
                {
                    var notAvailable = "n/a";
                    RefundErrorMessage = string.Format(
                        Naati.Resources.Application.ErrorProcessingSecurePayRefund,
                        response.reason_code ?? notAvailable,
                        response.reason ?? notAvailable);

                    LoggingHelper.LogWarning(RefundErrorMessage);
                }
            }
        }

        private void RefundUsingSecurePayIfApplicable()
        {
            if (HasCreditCardPayment)
            {
                if (RefundRequest.RefundAmount.GetValueOrDefault() <= 0)
                {
                    throw new Exception("Invalid refund amount");
                }

                var workflowFee = CredentialRequestModel.CredentialWorkflowFees.Single(
                    workflowFeeItem => workflowFeeItem.Id == RefundRequest.CredentialWorkflowFeeId);

                var securePayMigrationDate = Convert.ToDateTime(SystemService.GetSystemValue("SecurePayMigrationDate"));

                _OrderNumber = workflowFee.OrderNumber;

                //call securepay
                RefundRequest.DisallowProcessing = true;
                ApplicationService.UpdateCredentialApplicationRefundRequest(RefundRequest);

                PaymentResponse paymentRefundResponse;
                if (workflowFee.PaymentActionProcessedDate < securePayMigrationDate)
                {
                    paymentRefundResponse = PaymentClient.MakeRefundPayment(new RefundPaymentRequest
                    {
                        Amount = RefundRequest.RefundAmount.GetValueOrDefault(),
                        TransactionId = workflowFee.TransactionId,
                        PurchaseOrderNo = workflowFee.OrderNumber
                    });
                }
                else
                {
                    paymentRefundResponse = PaymentClient.MakeRefundPaymentEmbedded(new RefundPaymentRequest
                    {
                        Amount = RefundRequest.RefundAmount.GetValueOrDefault(),
                        TransactionId = workflowFee.TransactionId,
                        PurchaseOrderNo = workflowFee.OrderNumber
                    });
                }

                if (paymentRefundResponse.PaymentApproved)
                {
                    RefundRequest.RefundedDate = DateTime.Now; 
                    RefundRequest.RefundTransactionId = paymentRefundResponse.AssignedTransactionId;
                    RefundRequest.ObjectStatusId = (int)ObjectStatusTypeName.Updated;
                }
                else
                {
                    var notAvailable = "n/a";
                    RefundErrorMessage = string.Format(
                        Naati.Resources.Application.ErrorProcessingSecurePayRefund,
                        paymentRefundResponse.PaymentErrorCode ?? notAvailable,
                        paymentRefundResponse.PaymentErrorDescription ?? notAvailable,
                        paymentRefundResponse.SystemErrorMessage ?? notAvailable);

                    LoggingHelper.LogWarning(RefundErrorMessage);
                }
            }
        }

        protected virtual void ValidateRefundFields()
        {
            if (RefundRequest == null ||
                RefundRequest.RefundAmount == null || 
                RefundRequest.RefundAmount <= 0 ||
                RefundRequest.InitialPaidAmount.GetValueOrDefault() < RefundRequest.RefundAmount.GetValueOrDefault())
            {
                throw new Exception("Invalid refund amount");
            }

            if (RefundRequest.DisallowProcessing)
            {
                throw new Exception("Refund processing disabled");
            }
        }

        protected override string GetOrderNumber()
        {
            return _OrderNumber;
        }

        protected override void ProcessRefundRequest()
        {
            if (!string.IsNullOrWhiteSpace(RefundErrorMessage))
            {
                return;
            }

            RefundRequest.DisallowProcessing = false;
            RefundRequest.ObjectStatusId = (int)ObjectStatusTypeName.Updated;
        }
    }

    public class PayPalProcessStatus
    {
        public const string Created = "created";
        public const string Completed = "completed";
        public const string Failed = "failed";
    }
}
