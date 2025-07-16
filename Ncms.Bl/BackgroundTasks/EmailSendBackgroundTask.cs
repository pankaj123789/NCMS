using System;
using System.Collections.Generic;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using IPersonService = Ncms.Contracts.IPersonService;

namespace Ncms.Bl.BackgroundTasks
{
    public class EmailSendBackgroundTask : BaseBackgroundTask, IEmailSendBackgroundTask
    {
        private readonly IEmailMessageQueryService _emailMessageQueryService;
        private readonly IPersonService _personService;
        private int? _emailRetryLimitHours;

        public EmailSendBackgroundTask(
            ISystemQueryService systemQueryService, IPersonService personService,
            IEmailMessageQueryService emailMessageQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _personService = personService;
            _emailMessageQueryService = emailMessageQueryService;
        }

        internal int EmailRetryLimitHours
        {
            get
            {
                if (_emailRetryLimitHours == null)
                {
                    _emailRetryLimitHours =
                        int.TryParse(GetSystemValue("EmailRetryLimitHours"), out var result)
                            ? result
                            : 24;
                }

                return _emailRetryLimitHours.Value;
            }
        }

        public override string ReportEmailConfigKey => "EmailBatchingReportEmailsTo";

        public override string LogEmailConfigKey => "EmailBatchingLogEmailsTo";

        public override void Execute(IDictionary<string, string> parameters)
        {
            ProcessPendingEmails();
        }

        private void ProcessPendingEmails()
        {
            var emailBatchingSize = Convert.ToInt32(GetSystemValue("EmailBatchingSize"));
            TaskLogger.WriteInfo("Getting Pendings Emails to send...");

            var foundEmails = _emailMessageQueryService.SearchEmail(
                new GetEmailSearchRequest
                {
                    Filters = new[]
                    {
                        new EmailSearchCriteria
                        {
                            Filter = EmailFilterType.EmailStatusIntList,
                            Values = new[]
                            {
                                ((int)EmailSendStatusTypeName.Requested).ToString(),
                                ((int)EmailSendStatusTypeName.Retry).ToString()
                            }
                        }
                    },
                    Take = emailBatchingSize,
                    SortingOptions = new[]
                    {
                        new EmailMessageSortingOption
                            { SortType = EmailSortType.CreatedDate, SortDirection = SortDirection.Ascending }
                    }
                }).Data;

            TaskLogger.WriteInfo("Processing pending emails...");
            ExecuteIfRunning(foundEmails, ProcessPendingEmail);
        }

        private void ProcessPendingEmail(EmailMessageResultDto message)
        {
            var sendStatus = (EmailSendStatusTypeName)message.EmailSendStatusTypeId;
            var expired = message.CreatedDate < DateTime.Now.AddHours(-EmailRetryLimitHours);

            if ((sendStatus == EmailSendStatusTypeName.Retry) && expired)
            {
                // we failed to send this email and it has now expired. notify people.
                TaskLogger.WriteWarning(
                    $"Email #{message.EmailMessageId} failed to send within allowed time. Last result: \"{message.LastSendResult.TakeWords(5)}\"",
                    "Email",
                    message.EmailMessageId.ToString(),
                    true);

                // mark as permanently failed, so we don't keep trying
                _emailMessageQueryService.UpdateEmailMessageSendStatus(
                    message.EmailMessageId,
                    EmailSendStatusTypeName.Failed);
            }
            else
            {
                var messageDto = _emailMessageQueryService.GetEmailMessage(message.EmailMessageId).Data;
                var isDeceased = _personService.GetPersonDetailsByEntityId(messageDto.RecipientEntityId)?.PersonDetails?.Deceased;
                if (isDeceased == true)
                {
                    TaskLogger.WriteWarning(
                        $"Skipping email #{messageDto.EmailMessageId} because recipient is deceased.",
                        "Email",
                        messageDto.EmailMessageId.ToString(),
                        true);

                    _emailMessageQueryService.UpdateEmailMessageSendStatus(
                        message.EmailMessageId,
                        EmailSendStatusTypeName.Failed);
                    return; //move to the next one. 
                }
                var emailRequest = new GenericEmailMessageRequest
                {
                    EmailMessage = messageDto,
                    FailureStatus = (sendStatus == EmailSendStatusTypeName.Requested) ||
                                (sendStatus == EmailSendStatusTypeName.Retry)
                    ? EmailSendStatusTypeName.Retry
                    : EmailSendStatusTypeName.Failed
                };

                ExecuteProcessPendingEmail(emailRequest);
            }
        }

        private bool ExecuteProcessPendingEmail(GenericEmailMessageRequest emailRequest)
        {
            TaskLogger.WriteInfo(
                "Processing email #{EmailMessageId} from {EmailSender}",
                emailRequest.EmailMessage.EmailMessageId,
                emailRequest.EmailMessage.From);

            var emailMessageId = emailRequest.EmailMessage.EmailMessageId;

            try
            {
                var response = _emailMessageQueryService.ProcessPendingEmail(emailRequest);

                // only report error on the first attempt. we don't want to spam people or they'll just ignore it.
                if (emailRequest.EmailMessage.EmailSendStatusTypeId == (int)EmailSendStatusTypeName.Requested)
                {
                    var expiryTime = emailRequest.EmailMessage.CreatedDate.Value.AddHours(EmailRetryLimitHours);
                    var timeUntilExpry = expiryTime - DateTime.Now;

                    // the reasoning here is that the EmailQueryService uses warnings when we get a non-200 code back from the email server. we don't mind showing that message
                    // even if it's not very user-friendly, because it might be something minor that can easily be fixed. but if EmailQueryService sets an Error, an exception
                    // occurred, which could be a defect, and the message is probably not for user eyes.
                    if (!string.IsNullOrEmpty(response.WarningMessage))
                    {
                        // don't say we're going to retry if the email is about to expire. normally this won't happen, but abnormal things do happen.
                        var retryPart =
                            timeUntilExpry.Hours >= 1
                                ? $" We'll keep trying for {timeUntilExpry.Hours + (timeUntilExpry.Minutes > 30 ? 1 : 0)} hour(s)."
                                : string.Empty;

                        TaskLogger.WriteWarning(
                            $"Email #{emailRequest.EmailMessage.EmailMessageId} failed to send. The error was: \"{response.WarningMessage.TakeWords(5)}\".{retryPart}",
                            "Email",
                            emailMessageId.ToString(),
                            true);
                    }
                    else if (response.Error)
                    {
                        LoggingHelper.LogError(response.ErrorMessage);
                        TaskLogger.WriteWarning(
                            $"Email #{emailRequest.EmailMessage.EmailMessageId} failed to send. An error was logged. We'll keep trying for {EmailRetryLimitHours} hours.",
                            "Email",
                            emailMessageId.ToString(),
                            true);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                TaskLogger.WriteApplicationError(
                    ex,
                    emailMessageId,
                    "ProcessPendingEmail",
                    string.Empty,
                    ex is UserFriendlySamException);
            }

            return false;
        }
    }
}