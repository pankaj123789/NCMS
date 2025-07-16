using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationInvoicePaidAction : ApplicationStateAction
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.AwaitingApplicationPayment };
        protected override CredentialApplicationStatusTypeName ApplicationExitState => CredentialApplicationStatusTypeName.Entered;

        protected override IList<Action> Preconditions => new List<Action>
                                                                        {
                                                                            ValidateEntryState
                                                                        };

        protected override IList<Action> SystemActions => new List<Action>
                                                                        {
                                                                            ClearOwner,
                                                                            CreateNote,
                                                                            SetExitState,
                                                                            UpdateCredentialRequestStatus,
                                                                            ProcessFee,
                                                                        };


        protected override void UpdateCredentialRequestStatus()
        {
            foreach (var credentialRequest in ApplicationModel.CredentialRequests)
            {
                if (ApplicationType.RequiresChecking)
                {
                    credentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.RequestEntered;
                }
                else
                {
                    var credentialApplicationTypeCredentialType = credentialRequest.CredentialType.CredentialApplicationTypeCredentialTypes.FirstOrDefault(c => c.CredentialApplicationTypeId == ApplicationModel.ApplicationType.Id);

                    if (credentialApplicationTypeCredentialType != null && credentialApplicationTypeCredentialType.HasTest && credentialApplicationTypeCredentialType.HasTestFee)
                    {
                        credentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.EligibleForTesting;
                    }
                    else if (credentialApplicationTypeCredentialType != null && credentialApplicationTypeCredentialType.HasTest && !credentialApplicationTypeCredentialType.HasTestFee)
                    {
                        credentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.TestAccepted;
                    }
                    else
                    {
                        credentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.AssessmentComplete;
                    }
                }

                credentialRequest.StatusChangeUserId = CurrentUser.Id;
                credentialRequest.StatusChangeDate = DateTime.Now;
            }
        }

        protected override CredentialWorkflowFeeModel GetWorkflowFee()
        {
            var fees = ApplicationModel.CredentialWorkflowFees
                .Where(x => x.OnPaymentActionType == SystemActionTypeName.ApplicationInvoicePaid
                            && x.PaymentActionProcessedDate == null).ToList();

            if (fees.Count > 1)
            {
                throw new UserFriendlySamException(
                    $"{fees.Count} application fees found for APP{ApplicationModel.ApplicationInfo.ApplicationId}. Expecting only 1.");
            }
            return fees.SingleOrDefault();
        }

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.Invoice;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Manage;

        protected override void ProcessFee()
        {
            base.ProcessFee();
            var fee = GetWorkflowFee();
            if (fee != null)
            {
                Output.InvoiceNumber = fee.InvoiceNumber;
            }

            if (fee?.InvoiceId != null)
            {
                Output.InvoiceId = fee.InvoiceId.GetValueOrDefault();
            }
        }
    }
}
