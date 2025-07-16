using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.Finance;
using F1Solutions.Naati.Common.Dal.Finance.Wiise;
using Ncms.Contracts.BackgroundTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Bl.BackgroundTasks
{
    public class UtilityBackGroundTask : BaseBackgroundTask, IUtilityBackgroundTask
    {
        public UtilityBackGroundTask(ISystemQueryService systemQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            //new WiiseProcessingService(new ExternalAccountingQueueService()).ListInvoices();
        }
    }
}
