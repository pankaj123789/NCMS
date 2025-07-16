using System;
using System.Collections.Generic;
using F1Solutions.Global.Common.SystemLifecycle;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using Hangfire;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl.BackgroundTasks
{
    //public abstract class BaseBackgroundTask : IBackgroundTask
    //{
    //    private readonly ISystemService _systemService;

    //    protected BaseBackgroundTask(ISystemService systemService)
    //    {
    //        _systemService = systemService;
    //    }

    //    protected IBackgroundTaskLogger TaskLogger { get; private set; }

    //    protected IJobCancellationToken CancellationToken { get; private set; }

    //    protected int UserId { get; private set; }

    //    public virtual string ReportEmailConfigKey => "AccountingOperationsLogEmailsTo"; // default

    //    public virtual string LogEmailConfigKey => "AccountingOperationsLogEmailsTo"; // default

    //    public void Configure(IBackgroundTaskLogger logger, IJobCancellationToken cancellationToken, int userId)
    //    {
    //        TaskLogger = logger;
    //        CancellationToken = cancellationToken;
    //        UserId = userId;
    //    }

    //    public abstract void Execute();

    //    protected string GetSystemValue(string systemValueKey)
    //    {
    //        return _systemService.GetSystemValue(systemValueKey);
    //    }

    //    protected void ExecuteIfRunning<T>(IEnumerable<T> data, Action<T> action)
    //    {
    //        foreach (var item in data)
    //        {
    //            ThrowIfNotRunning();
    //            action(item);
    //        }
    //    }

    //    protected void ThrowIfNotRunning()
    //    {
    //        CancellationToken.ThrowIfCancellationRequested();
    //        SystemLifecycleHelper.ThrowIfNotRunning();
    //    }
    //}
}