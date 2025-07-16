using System.Collections.Generic;
using Hangfire;

namespace F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask
{
    public interface IBackgroundTask
    {
        string ReportEmailConfigKey { get; }
        string LogEmailConfigKey { get; }
        void Configure(IBackgroundTaskLogger logger, IJobCancellationToken cancellationToken);
        void Execute(IDictionary<string, string> parameters);
    }
}
