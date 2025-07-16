using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts.BackgroundTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Bl.BackgroundTasks
{
    public class NcmsDeleteBankDetailsTask : BaseBackgroundTask, INcmsDeleteBankDetailsTask
    {
        private readonly IApplicationQueryService _applicationQueryService;

        public NcmsDeleteBankDetailsTask(ISystemQueryService systemQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService, IApplicationQueryService applicationQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _applicationQueryService = applicationQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            DeleteBankDetails();
        }

        private void DeleteBankDetails()
        {
            var batchSizeText = GetSystemValue("DeleteNcmsBankDetailsBatchSize");
            var batchSize = Convert.ToInt32(batchSizeText);
            var maxProcessedDaysText = GetSystemValue("RefundBankDetailsFlushingDays");           
            var maxProcessed = DateTime.Now.AddDays(-Convert.ToInt32(maxProcessedDaysText));
            List<RefundDto> refunds;
            do
            {
                var request = new NotFlushedRefundRequest
                {
                    MaxProcessedDate = maxProcessed,
                    Take = batchSize,
                };

                TaskLogger.WriteInfo("Getting refunds to flush...");
                refunds = _applicationQueryService.GetNotFlushedProcessedRefunds(request).Results.ToList();

                FlushAllBankDetails(refunds);
            }
            while (refunds.Count() > 0);
        }

        private void FlushAllBankDetails(IEnumerable<RefundDto> refunds)
        {          
            ThrowIfNotRunning();
            var applicationsData = string.Join(";", refunds.Select(x=> $"applicationId:{x.CredentialApplicationId} , refundId:{x.Id}"));
            TaskLogger.WriteInfo("Flushing bank details for : {applications}", applicationsData);
            var refundIds = refunds.Select(x => x.Id).Concat(new[] { 0 }).ToList();
            _applicationQueryService.FlushRefundBankDetails(refundIds);
        }
    }
}
