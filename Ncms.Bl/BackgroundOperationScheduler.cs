using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.NotificationScheduler;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using Hangfire;
using Hangfire.Server;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl
{
    public class BackgroundOperationScheduler : IBackgroundOperationScheduler, INotificationScheduler
    {
        private readonly IUserService _userService;
        private readonly INcmsRefreshPendingUsersTask _pendingUsersTask;
        private readonly ILifecycleService _lifecycleService;

        public BackgroundOperationScheduler(IUserService userService, INcmsRefreshPendingUsersTask pendingUsersTask, ILifecycleService lifecycleService)
        {
            _userService = userService;
            _pendingUsersTask = pendingUsersTask;
            _lifecycleService = lifecycleService;
        }
        public BusinessServiceResponse Enqueue(NcmsBackgoundOperationTypeName jobName, IDictionary<string, string> parameters)
        {
            var userName = _userService.Get().Name;
            parameters[BackgroundOperationParameters.CreatedUserName] = userName;

            Enqueue(() => ExecuteOperation(jobName, null, parameters));

            return new BusinessServiceResponse()
            {
                Messages = new List<string>() { Naati.Resources.Shared.OperationCreated }
            };
        }

        public void EnqueueNotification(NotificationDto notification)
        {
            var jobDelaySeconds = _pendingUsersTask.RegisterHubNotification(notification.ToUserName, notification.Id);
            if (jobDelaySeconds > 0)
            { // if delay is returned, then job need to be scheduled
                var serverName = _lifecycleService.GetSystemIp();
                var parameters = new Dictionary<string, string>() { { BackgroundTasksParameters.ServerName, serverName } };
                Schedule(() => ExecuteJob(NcmsJobTypeName.RefreshNcmsPendingUsers, null, parameters), TimeSpan.FromSeconds(jobDelaySeconds));
            }
        }

        public static void ExecuteJob(NcmsJobTypeName jobTypeName, PerformContext context, IDictionary<string, string> parameters)
        {
            SetContext();
            ServiceLocator.Resolve<IBackgroundTaskService<NcmsJobTypeName>>().ExecuteTask(jobTypeName, context, true, false, parameters);
        }

        public static void ExecuteOperation(NcmsBackgoundOperationTypeName jobTypeName, PerformContext context, IDictionary<string, string> parameters)
        {
            SetContext();
            ServiceLocator.Resolve<IBackgroundTaskService<NcmsBackgoundOperationTypeName>>().ExecuteTask(jobTypeName, context, true, false, parameters);
        }

        private static void SetContext()
        {
            var defaultUrl = ConfigurationManager.AppSettings["DefaultAppLocalUrl"];
            HttpContext.Current = new HttpContext(new HttpRequest(string.Empty, defaultUrl, string.Empty), new HttpResponse(new StringWriter()));
        }

        private static void Enqueue(Expression<Action> methodCall)
        {
            BackgroundJob.Enqueue(methodCall);
        }

        private static void Schedule(Expression<Action> methodCall, TimeSpan timeSpan)
        {
            BackgroundJob.Schedule(methodCall, timeSpan);
        }
    }
}
