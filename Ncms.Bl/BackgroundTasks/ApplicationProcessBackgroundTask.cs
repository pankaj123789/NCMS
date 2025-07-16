using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.BackgroundTasks
{
    public class ApplicationProcessBackgroundTask : ApplicationBackgroundTask, IApplicationProcessBackgroundTask
    {
        private readonly IApplicationQueryService _applicationQueryService;

        public ApplicationProcessBackgroundTask(
            ISystemQueryService systemQueryService,
            IApplicationQueryService applicationQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {            
                _applicationQueryService = applicationQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            ProcessPendingApplications();
            ProcessPendingApplicationRefunds();
        }

        private void ProcessPendingApplications()
        {
            TaskLogger.WriteInfo("Getting Invoices and payments to process ...");
            var workflowFees = _applicationQueryService.GetInvoicesAndPaymentsToProcess();
            var invoicesToProcess =
                workflowFees.CredentialWorkflowFees.Where(
                    x => x.OnInvoiceActionType.HasValue && !x.InvoiceActionProcessedDate.HasValue &&
                         x.InvoiceId.HasValue).ToList();

            var paymentsToProcess =
                workflowFees.CredentialWorkflowFees.Where(
                    x => x.OnPaymentActionType.HasValue && !x.PaymentActionProcessedDate.HasValue &&
                         !string.IsNullOrEmpty(x.PaymentReference)).ToList();

            TaskLogger.WriteInfo("Processing Invoices...");
            ExecuteIfRunning(invoicesToProcess, ExecuteInvoiceAction);

            TaskLogger.WriteInfo("Processing Payments...");
            ExecuteIfRunning(paymentsToProcess, ExecutePaymentAction);

            TaskLogger.WriteInfo("Getting Applications that can go to Eligible for Testing ...");
            
            var newCredentialsThatCanProgressToEligibleForTestingResponse = _applicationQueryService.GetNewCredentialsThatCanprogressToEligibleForTesting();

            TaskLogger.WriteInfo("Processing Credentials to move to Eligible for Testing...");

            if(!newCredentialsThatCanProgressToEligibleForTestingResponse.Success)
            {
                TaskLogger.WriteWarning($"newCredentialsThatCanProgressToEligibleForTesting errors: {newCredentialsThatCanProgressToEligibleForTestingResponse.Errors.ToString()}");
            }

            ExecuteIfRunning(newCredentialsThatCanProgressToEligibleForTestingResponse.Data, ExecuteProgessNewCredentialsToEligibleForTestingAction);

            TaskLogger.WriteInfo("Getting Credential Requests on hold to be issued...");
            // get all credential requests with status on hold to be issued
            var credentialRequestsOnHoldToBeIssuedResponse = _applicationQueryService.GetCredentialRequestsOnHoldToBeIssued();

            if (!credentialRequestsOnHoldToBeIssuedResponse.Success)
            {
                TaskLogger.WriteWarning($"Error occured when getting credential requests on hold to be issued: {credentialRequestsOnHoldToBeIssuedResponse.Errors}");
            }

            TaskLogger.WriteInfo("Processing Credential Requests...");
            // Issue credential requests
            ExecuteIfRunning(credentialRequestsOnHoldToBeIssuedResponse.Data, ExecuteIssueOnHoldCredentialsAction);
        }

        private void ProcessPendingApplicationRefunds()
        {
            TaskLogger.WriteInfo("Getting credit notes and payments to process ...");
            var refunds = _applicationQueryService.GetCreditNotesAndPaymentsToProcess();
            var creditNotetoProcess =
                refunds.Results.Where(
                    x => x.OnCreditNoteCreatedSystemActionTypeId.HasValue && !x.CreditNoteProcessedDate.HasValue &&
                         x.CreditNoteId.HasValue).ToList();

            var paymentsToProcess =
                refunds.Results.Where(
                    x => x.OnPaymentCreatedSystemActionTypeId.HasValue && !x.CreditNotePaymentProcessedDate.HasValue &&
                         !string.IsNullOrEmpty(x.PaymentReference)).ToList();

            TaskLogger.WriteInfo("Processing Credit Notes...");
            ExecuteIfRunning(creditNotetoProcess, ExecuteCreditNoteAction);

            TaskLogger.WriteInfo("Processing Credit Note Payments...");
            ExecuteIfRunning(paymentsToProcess, ExecutePaymentAction);
        }

        private void ExecuteInvoiceAction(CredentialWorkflowFeeDto workflowFee)
        {
            ExecuteAction(
                workflowFee.OnInvoiceActionType.GetValueOrDefault(),
                workflowFee,
                "Invoice",
                workflowFee.InvoiceNumber);
        }

        private void ExecuteCreditNoteAction(RefundDto refund)
        {
            ExecuteAction(
                (SystemActionTypeName)refund.OnCreditNoteCreatedSystemActionTypeId.GetValueOrDefault(),
                refund,
                "CreditNote",
                refund.CreditNoteNumber);
        }

        private void ExecutePaymentAction(CredentialWorkflowFeeDto workflowFee)
        {
            ExecuteAction(
                workflowFee.OnPaymentActionType.GetValueOrDefault(),
                workflowFee,
                "Payment",
                $"{workflowFee.PaymentReference}, Invoice:{workflowFee.InvoiceNumber}");
        }

        private void ExecuteProgessNewCredentialsToEligibleForTestingAction(NewCredentialsThatCanProgressToEligibleForTesting credential)
        {
            ExecuteAction(
                SystemActionTypeName.ProgressCredentialToEligibleForTesting,
                credential);
        }

        private void ExecuteIssueOnHoldCredentialsAction(CredentialRequestDto credentialRequestDto)
        {
            ExecuteAction
                (SystemActionTypeName.IssueCredential, 
                credentialRequestDto);
        }

        private void ExecutePaymentAction(RefundDto refund)
        {
            ExecuteAction(
                (SystemActionTypeName)refund.OnPaymentCreatedSystemActionTypeId.GetValueOrDefault(),
                refund,
                "CreditNotePayment",
                $"{refund.PaymentReference}, Invoice:{refund.CreditNoteNumber}");
        }

        private bool ExecuteAction(
            SystemActionTypeName action,
            CredentialWorkflowFeeDto credentialWorkflowFee,
            string type,
            string reference)
        {
            TaskLogger.WriteInfo(
                "Executing action: {Action}, ApplicationId: {ApplicationId}",
                action,
                credentialWorkflowFee.CredentialApplicationId);

            var applicationId = credentialWorkflowFee.CredentialApplicationId;
            var model = new ApplicationActionWizardModel
            {
                ApplicationId = applicationId,
                CredentialRequestId = credentialWorkflowFee.CredentialRequestId.GetValueOrDefault(),
                ActionType = (int)action
            };
            model.SetPaymentReference(credentialWorkflowFee.PaymentReference, null, credentialWorkflowFee.TransactionId, credentialWorkflowFee.OrderNumber);
            model.SetBackGroundAction(true);
            try
            {
                var response = ServiceLocator.Resolve<IApplicationService>().PerformAction(model);
                LogResponse(response, type, applicationId);
                TaskLogger.AdProcessedApplication(
                    $"ApplicationId : {credentialWorkflowFee.CredentialApplicationId}, CredentialRequestId: {credentialWorkflowFee.CredentialRequestId.GetValueOrDefault()}");

                return true;
            }
            catch (WiiseRateExceededException ex)
            {
                TaskLogger.WriteError(
                    "Wiise Limit Exceeded. {ApplicationId}",
                    type,
                    reference,
                    true,
                    new[] { applicationId });
                TaskLogger.WriteApplicationError(ex, applicationId, type, reference, false);
                throw;
            }
            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, type, reference, true);
            }
            catch (Exception ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, type, reference, false);
            }

            return false;
        }

        private bool ExecuteAction(
         SystemActionTypeName action,
         RefundDto refund,
         string type,
         string reference)
        {
            TaskLogger.WriteInfo(
                "Executing action: {Action}, ApplicationId: {ApplicationId}",
                action,
                refund.CredentialApplicationId);

            var applicationId = refund.CredentialApplicationId;
            var model = new ApplicationActionWizardModel
            {
                ApplicationId = applicationId,
                CredentialRequestId = refund.CredentialRequestId,
                ActionType = (int)action
            };
            model.SetPaymentReference(refund.PaymentReference, null, refund.RefundTransactionId, refund.OrderNumber);
            model.SetBackGroundAction(true);
            try
            {
                var response = ServiceLocator.Resolve<IApplicationService>().PerformAction(model);
                LogResponse(response, type, applicationId);
                TaskLogger.AdProcessedApplication(
                    $"ApplicationId : {refund.CredentialApplicationId}, CredentialRequestId: {refund.CredentialRequestId}");

                return true;
            }
            catch (WiiseRateExceededException ex)
            {
                TaskLogger.WriteError(
                    "Wiise Limit Exceeded. {ApplicationId}",
                    type,
                    reference,
                    true,
                    new[] { applicationId });
                TaskLogger.WriteApplicationError(ex, applicationId, type, reference, false);
                throw;
            }
            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, type, reference, true);
            }
            catch (Exception ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, type, reference, false);
            }

            return false;
        }

        private bool ExecuteAction(SystemActionTypeName action,NewCredentialsThatCanProgressToEligibleForTesting application)
        {
            TaskLogger.WriteInfo(
                "Executing action: {Action}, ApplicationId: {ApplicationId}",
                action,
                application.CredentialApplicationId);

            var applicationId = application.CredentialApplicationId;
            var model = new ApplicationActionWizardModel
            {
                ApplicationId = applicationId,
                CredentialRequestId = application.CredentialRequestId,
                ActionType = (int)action,                
            };
            model.SetBackGroundAction(true);
            try
            {
                var response = ServiceLocator.Resolve<IApplicationService>().PerformAction(model);
                LogResponse(response, "ProgressToEligibleForTesting", applicationId);
                TaskLogger.AdProcessedApplication(
                    $"ApplicationId : {applicationId}");

                return true;
            }
            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "ProgressToEligibleForTesting", applicationId.ToString(), true);
            }
            catch (Exception ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "ProgressToEligibleForTesting", applicationId.ToString(), false);
            }

            return false;
        }

        private bool ExecuteAction(SystemActionTypeName action, CredentialRequestDto credentialRequestDto)
        {
            TaskLogger.WriteInfo(
                "Executing action: {Action}, ApplicationId: {ApplicationId}, CredentialRequestId: {credentialRequestId}",
                action,
                credentialRequestDto.CredentialApplicationId,
                credentialRequestDto.Id);

            var model = new ApplicationActionWizardModel
            {
                ApplicationId = credentialRequestDto.CredentialApplicationId,
                CredentialRequestId = credentialRequestDto.Id,
                ActionType = (int)action,
            };
            model.SetBackGroundAction(true);
            try
            {
                var response = ServiceLocator.Resolve<IApplicationService>().PerformAction(model);
                LogResponse(response, "IssueOnHoldCredentials", credentialRequestDto.CredentialApplicationId);
                TaskLogger.AdProcessedApplication(
                    $"ApplicationId : {credentialRequestDto.CredentialApplicationId}");

                return true;
            }
            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteApplicationError(ex, credentialRequestDto.CredentialApplicationId, "IssueOnHoldCredentials", credentialRequestDto.CredentialApplicationId.ToString(), true);
            }
            catch (Exception ex)
            {
                TaskLogger.WriteApplicationError(ex, credentialRequestDto.CredentialApplicationId, "IssueOnHoldCredentials", credentialRequestDto.CredentialApplicationId.ToString(), false);
            }

            return false;
        }
    }
}