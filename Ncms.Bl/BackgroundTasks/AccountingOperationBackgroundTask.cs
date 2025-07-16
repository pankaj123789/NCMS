using System;
using System.Collections.Generic;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl.BackgroundTasks
{
    public class AccountingOperationBackgroundTask : BaseBackgroundTask, IAccountingOperationBackgroundTask
    {
        private readonly IFinanceService _financeService;
        private readonly IUserService _userService;

        public AccountingOperationBackgroundTask(IFinanceService financeService,
            ISystemQueryService systemQueryService, 
            IBackgroundTaskLogger backgroundTaskLogger,
            IUserService userService,
            IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger,utilityQueryService)
        {
            _financeService = financeService;
            _userService = userService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            ProcessPendingAccountingOperations();
        }

        private void ProcessPendingAccountingOperations()
        {
            var userId = _userService.Get().Id;
            var maxBatchSize = Convert.ToInt32(GetSystemValue("AccountingOperationsMaxBatchSize"));
            var waitingTimeInMinutes = Convert.ToInt32(GetSystemValue("AccountingOperationsMinWaitingMinutes"));

            var response = _financeService.PerformBatchOperations(
                new PerformBatchOperationsRequest
                {
                    MaxBatchSize = maxBatchSize,
                    UserId = userId,
                    MaxRequestedDate = DateTime.Now.AddMinutes(-waitingTimeInMinutes)
                });

            TaskLogger.WriteError(response.ErrorMessage, string.Empty, string.Empty, true);
            TaskLogger.WriteError(response.StackTrace, string.Empty, string.Empty, false);
            TaskLogger.WriteWarning(response.WarningMessage, string.Empty, string.Empty, true);

            response.Invoices.ForEach(ValidateResult);
            response.Payments.ForEach(ValidateResult);
            response.CreditNotes.ForEach(ValidateResult);
        }

        private void ValidateResult(FinanceBatchPaymentOperationResult operationResult)
        {
            var result = IsValidOperationResult(
                operationResult,
                "Payment",
                $"{operationResult.PaymentReference}, Invoice:{operationResult.InvoiceNumber ?? operationResult.InvoiceReference}");
            if (result)
            {
                TaskLogger.AdProcessedPayment(operationResult.PaymentReference);
            }
        }

        private bool IsValidOperationResult<T>(T operationResult, string type, string reference)
            where T : FinanceBatchOperationResult
        {
            TaskLogger.WriteError(operationResult.ErrorMessage, type, reference, true);
            TaskLogger.WriteWarning(operationResult.WarningMessage, type, reference, true);

            return operationResult.Success;
        }

        private void ValidateResult(FinanceBatchInvoiceOperationResult operationResult)
        {
            var result = IsValidOperationResult(
                operationResult,
                "Invoice",
                operationResult.InvoiceNumber ?? operationResult.InvoiceReference);

            if (result)
            {
                TaskLogger.AdProcessedInvoice(operationResult.InvoiceNumber);
            }
        }

        private void ValidateResult(FinanceBatchCreditNoteOperationResult operationResult)
        {
            var result = IsValidOperationResult(
                operationResult,
                "Credit note",
                operationResult.CreditNoteNumber ?? operationResult.CreditNoteReference);

            if (result)
            {
                //TODO add to taskLogger
                //TaskLogger.AdProcessedInvoice(operationResult.InvoiceNumber);
            }
        }
    }
}