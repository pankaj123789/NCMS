using F1Solutions.Global.Common;
using F1Solutions.Global.Common.SystemLifecycle;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Hangfire;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Threading;

namespace F1Solutions.Naati.Common.Bl.BackgroundTask
{
    public abstract class BaseBackgroundService<TJobType, ITBackgroundTaskType> : IBackgroundTaskService<TJobType> where ITBackgroundTaskType : IBackgroundTask
    {
        protected abstract string BackgroundServiceName { get; }
        private readonly IUtilityQueryService _utilityQueryService;
        private readonly IEmailMessageQueryService _emailMessageQueryService;
        private IBackgroundTaskLogger _taskLogger;

        public BaseBackgroundService(
            IUtilityQueryService utilityQueryService,
            IEmailMessageQueryService emailMessageQueryService,
            IBackgroundTaskLogger backgroundTaskLogger)
        {
            _utilityQueryService = utilityQueryService;
            _emailMessageQueryService = emailMessageQueryService;
            _taskLogger = backgroundTaskLogger;
        }
        protected abstract IReadOnlyDictionary<TJobType, Func<ITBackgroundTaskType>> GetTaskFactory();
        public void ExecuteTask(TJobType jobTypeName, PerformContext context, bool multiServer, bool allowDisable, IDictionary<string, string> parameters)
        {
            _taskLogger.SetContext(context);

            var taskFactory = GetTaskFactory();
            if (!taskFactory.ContainsKey(jobTypeName))
            {
                throw new Exception($"Task {jobTypeName} is not supported");
            }

            var task = taskFactory[jobTypeName]();
            ExecuteTask(jobTypeName, task, multiServer, allowDisable, parameters, context?.CancellationToken);
        }

        protected abstract ITBackgroundTaskType CreateTask<T>() where T : ITBackgroundTaskType;
      


        private void ExecuteTask(
            TJobType tokenTypeName,
            ITBackgroundTaskType task,
            bool multiServer,
            bool allowDisable,
            IDictionary<string, string> parameters,
            IJobCancellationToken cancellationToken)
        {
            ThrowIfNotRunning(cancellationToken);

            var batchingDisableTimeFrom = DateTime.ParseExact(
                GetSystemValue("DisableBatchingFrom"),
                "HH:mm",
                CultureInfo.InvariantCulture);
            var batchingDisabledTo = DateTime.ParseExact(
                GetSystemValue("DisableBatchingTo"),
                "HH:mm",
                CultureInfo.InvariantCulture);

            if (allowDisable && (DateTime.Now.TimeOfDay > batchingDisableTimeFrom.TimeOfDay) &&
                (DateTime.Now.TimeOfDay < batchingDisabledTo.TimeOfDay))
            {
                _taskLogger.WriteWarning("Batching processing is temporary disabled.");
                return;
            }

            if (!multiServer && !_utilityQueryService.RequestJobToken(tokenTypeName))
            {
                _taskLogger.WriteWarning("Task {Task} is being processed or is disabled", tokenTypeName);
                return;
            }
           
            try
            {
                SetPrincipal();

                SetCulture();

                _taskLogger.WriteInfo("Configuring task {Task}", tokenTypeName);
                task.Configure(_taskLogger, cancellationToken);
                _taskLogger.WriteInfo("Starting task {Task}", tokenTypeName);

                ThrowIfNotRunning(cancellationToken);
                task.Execute(parameters ?? new Dictionary<string, string>());
                _taskLogger.WriteInfo("Background task {Task} Finished", tokenTypeName);
                SendReportEmail(tokenTypeName, task);
            }
            catch (OperationCanceledException ex)
            {
                _taskLogger.WriteWarning($"Background task cancelled. {ex.Message}", "Undefined", "Undefined", false);
                SendReportEmail(tokenTypeName, task);
                throw;
            }
            catch (WiiseRateExceededException)
            {
                SendReportEmail(tokenTypeName, task);
            }
            catch (Exception ex)
            {
                _taskLogger.WriteError(ex, "Undefined", "Undefined", false);
                SendReportEmail(tokenTypeName, task);
                throw;
            }
            finally
            {
                _taskLogger.UnsetContext();
                _utilityQueryService.ReleaseJobToken(tokenTypeName);
            }
        }

        private void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(GetSystemValue("Culture"));
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(GetSystemValue("UICulture"));
        }

        protected abstract void SetPrincipal();

        private void SendReportEmail(TJobType taskName, ITBackgroundTaskType task)
        {
            var reportContent = _taskLogger.GetReportContent(taskName);
            var detailedLog = _taskLogger.GetDetailedLogContent();
            if (string.IsNullOrWhiteSpace(reportContent) && string.IsNullOrWhiteSpace(detailedLog))
            {
                _taskLogger.WriteInfo($"No Execution Report required for task {taskName}.");
                return;
            }

            var reportEmails = ConfigurationManager.AppSettings[task.ReportEmailConfigKey].Split(';');
            var logEmails = ConfigurationManager.AppSettings[task.LogEmailConfigKey].Split(';');
            var fromAddress = ConfigurationManager.AppSettings["AccountingOperationsReportEmailsFrom"];

            var sendSuccessfulBatchReport = GetSystemValue("SendSuccessfulBatchReport");
            var sendInfoReportsEnabled = false;
            var sendSuccessfulBatchReportInt = 0;

            // in case value is a bit and not a string of true or false
            switch (sendSuccessfulBatchReport)
            {
                case ("0"):
                    sendSuccessfulBatchReportInt = int.Parse(sendSuccessfulBatchReport);
                    sendInfoReportsEnabled = Convert.ToBoolean(sendSuccessfulBatchReportInt);
                    break;
                case ("1"):
                    sendSuccessfulBatchReportInt = int.Parse(sendSuccessfulBatchReport);
                    sendInfoReportsEnabled = Convert.ToBoolean(sendSuccessfulBatchReportInt);
                    break;
                default:
                    sendInfoReportsEnabled = Convert.ToBoolean(sendSuccessfulBatchReport);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(reportContent) && sendInfoReportsEnabled)
            {
                _taskLogger.WriteInfo("Sending info Execution report to {@RecipientEmails}", reportEmails);
                var reportEmail = new EmailMessageDto
                {
                    Subject = $"{BackgroundServiceName} Batch {taskName} Report",
                    Body = reportContent,
                    From = fromAddress,
                    RecipientEntityId = 0,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = 0,
                    Attachments = new EmailMessageAttachmentDto[0]
                };

                reportEmails.ForEach(x => SendEmail(reportEmail, x));
            }

            if (!string.IsNullOrWhiteSpace(detailedLog))
            {
                _taskLogger.WriteInfo("Sending detailed Execution report to {@RecipientEmails}", logEmails);
                var detailedLogEmail = new EmailMessageDto
                {
                    Subject = $"Detailed {BackgroundServiceName} Batch Processing Report({taskName})",
                    Body = detailedLog,
                    From = fromAddress,
                    RecipientEntityId = 0,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = 0,
                    Attachments = new EmailMessageAttachmentDto[0]
                };

                logEmails.ForEach(x => SendEmail(detailedLogEmail, x));
            }

            _taskLogger.WriteInfo("Action finished");
        }

        protected virtual void SendEmail(EmailMessageDto email, string recipient)
        {
            email.RecipientEmail = recipient;
            var emailResponse = _emailMessageQueryService.SendAndForgetMail(
                new GenericEmailMessageRequest { EmailMessage = email, FailureStatus = EmailSendStatusTypeName.Retry });
            _taskLogger.WriteError(emailResponse.ErrorMessage, string.Empty, string.Empty, true);
            _taskLogger.WriteWarning(emailResponse.ErrorMessage, string.Empty, string.Empty, true);
        }

        protected abstract string GetSystemValue(string systemValueKey);

        public static void ThrowIfNotRunning(IJobCancellationToken cancellationToken)
        {
            cancellationToken?.ThrowIfCancellationRequested();
            SystemLifecycleHelper.ThrowIfNotRunning();
        }
    }
}
