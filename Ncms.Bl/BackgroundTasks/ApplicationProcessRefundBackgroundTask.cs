using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using Ncms.Contracts.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncms.Bl.BackgroundTasks
{
    public class ApplicationProcessRefundBackgroundTask : ApplicationBackgroundTask, IApplicationProcessRefundBackgroundTask
    {
        private readonly IApplicationQueryService _applicationQueryService;

        public ApplicationProcessRefundBackgroundTask(
            ISystemQueryService systemQueryService,
            IBackgroundTaskLogger backgroundTaskLogger,
            IUtilityQueryService utilityQueryService,
            IApplicationQueryService applicationQueryService,
            IFinanceService financeService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _applicationQueryService = applicationQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            ProcessRefundRequests();
        }

        private void ProcessRefundRequests()
        {
            TaskLogger.WriteInfo("Getting refund requests to process...");
            var refundRequests = _applicationQueryService.GetCredentialRequestsWithPendingRefundRequests();

            if (refundRequests.Any())
            {
                TaskLogger.WriteInfo("Processing refund requests...");
                ExecuteIfRunning(refundRequests, ExecuteRefundAction);
            }
            else
            {
                TaskLogger.WriteInfo("No refund requests to process...");
            }
        }

        private void ExecuteRefundAction(CredentialRequestWithPendingRefundRequest refundRequest)
        {
           
            var applicationId = refundRequest.CredentialApplicationId;
            var actionType = (int)SystemActionTypeName.ProcessRefund;
            var initialPaidAmount = refundRequest.InitialPaidAmount;
            TaskLogger.WriteInfo("Executing action: {Action}, ApplicationId: {ApplicationId}. InvoiceNumber: {invoice}, Invoice Paid Amount:{InitialPaidAmount}", actionType, applicationId, refundRequest.InvoiceNumber, initialPaidAmount);

            var model = new ApplicationActionWizardModel
            {
                ApplicationId = applicationId,
                CredentialRequestId = refundRequest.CredentialRequestId,
                ActionType = actionType
            };
            model.SetBackGroundAction(true);

            try
            {
                var response = ServiceLocator.Resolve<IApplicationService>().PerformAction(model);
            }
            catch (WiiseRateExceededException ex)
            {
                TaskLogger.WriteError(
                    "Wiise Limit Exceeded. {ApplicationId}",
                    "refund",
                    refundRequest.InvoiceNumber,
                    true,
                    new[] { applicationId });
                TaskLogger.WriteApplicationError(ex, applicationId, "refund", refundRequest.InvoiceNumber, false);
                throw;
            }
            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "refund", refundRequest.InvoiceNumber, true);
            }
            catch (Exception ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "refund", refundRequest.InvoiceNumber, false);
            }
        }
    }
}
