using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F1Solutions.Global.Common.SystemLifecycle;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Hangfire;

namespace F1Solutions.Naati.Common.Bl.BackgroundTask
{
    public abstract class BaseBackgroundTask : IBackgroundTask
    {
        private readonly ISystemQueryService _systemQueryService;
        private readonly IUtilityQueryService _utilityQueryService;

        protected BaseBackgroundTask(ISystemQueryService systemQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService)
        {
            _systemQueryService = systemQueryService;
            _utilityQueryService = utilityQueryService;

            TaskLogger = backgroundTaskLogger;
        }

        protected IBackgroundTaskLogger TaskLogger { get; private set; }

        protected IJobCancellationToken CancellationToken { get; private set; }



        public virtual string ReportEmailConfigKey => "AccountingOperationsLogEmailsTo"; // default

        public virtual string LogEmailConfigKey => "AccountingOperationsLogEmailsTo"; // default

        public void Configure(IBackgroundTaskLogger logger, IJobCancellationToken cancellationToken)
        {
            TaskLogger = logger;
            CancellationToken = cancellationToken;
        }

        public abstract void Execute(IDictionary<string, string> parameters);

        protected string GetSystemValue(string systemValueKey)
        {
            return _systemQueryService.GetSystemValue(new GetSystemValueRequest() { ValueKey = systemValueKey, ForceRefresh = true }).Value;
        }

        protected void ExecuteIfRunning<T>(IEnumerable<T> data, Action<T> action)
        {
            foreach (var item in data)
            {
                ThrowIfNotRunning();
                action(item);
            }
        }

        protected void ExecuteRemoteServersAction(Func<string, Task<BusinessServiceResponse>> action)
        {
            var serverNames = GetRemoteHangFireServers();

            var tasks = new List<Task<BusinessServiceResponse>>();
            foreach (var serverName in serverNames)
            {
                try
                {
                    TaskLogger.WriteInfo("Execution action on server {serverName}", serverName);
                    var task = action(serverName);
                    tasks.Add(task);
                }
                catch (Exception ex)
                {
                    TaskLogger.WriteError(ex, "undefined", "undefined", false);
                }
            }

            try
            {
                Task.WhenAll(tasks).Wait();
                foreach (var task in tasks)
                {
                    if (task.Exception != null)
                    {
                        TaskLogger.WriteError(task.Exception.Message, "undefined", "undefined", false);
                    }
                }
            }
            catch (Exception ex)
            {
                TaskLogger.WriteError(ex, "undefined", "undefined", false);
            }

        }


        public IEnumerable<string> GetRemoteHangFireServers()
        {
            var localIp = _utilityQueryService.GetSystemIp();
            var api = JobStorage.Current.GetMonitoringApi();
            var serverNames = api.Servers().Select(x => x.Name.Substring(0, x.Name.IndexOf(':'))).ToList();
            return serverNames.Where(y => y != localIp).ToList();
        }

        protected void ThrowIfNotRunning()
        {
            CancellationToken?.ThrowIfCancellationRequested();
            SystemLifecycleHelper.ThrowIfNotRunning();
        }

        protected bool ValidateParameter(string parameterName, IDictionary<string, string> parameters)
        {
            var typeName = this.GetType().Name;
            if (!parameters.ContainsKey(parameterName))
            {
                TaskLogger.WriteError(
                    $"{parameterName} has not been defined.",
                    typeName,
                    typeName,
                    false);
                return false;
            }

            return true;
        }
    }
}
