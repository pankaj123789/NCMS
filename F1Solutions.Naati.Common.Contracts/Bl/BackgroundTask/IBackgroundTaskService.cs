using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Hangfire.Server;

namespace F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask
{
    public interface IBackgroundTaskService<in TJobType>
    {
        void ExecuteTask(TJobType jobTypeName, PerformContext context, bool multiServer, bool allowDisable = true, IDictionary<string, string> parameters = null);
    }

}
