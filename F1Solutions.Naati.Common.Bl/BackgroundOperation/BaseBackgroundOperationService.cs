using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundOperation;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace F1Solutions.Naati.Common.Bl.BackgroundOperation
{
    public abstract class BaseBackgroundOperationService<TJobType> : BaseBackgroundService<TJobType, IBackgroundOperation>
    {
        protected BaseBackgroundOperationService(IUtilityQueryService utilityQueryService, IEmailMessageQueryService emailMessageQueryService, IBackgroundTaskLogger backgroundTaskLogger) : base(utilityQueryService, emailMessageQueryService, backgroundTaskLogger)
        {
        }
    }
}
