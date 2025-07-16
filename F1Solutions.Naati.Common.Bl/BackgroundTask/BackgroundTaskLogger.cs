using System;
using System.Collections.Generic;
using System.Text;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using Hangfire.Console;
using Hangfire.Console.Progress;
using Hangfire.Server;

namespace F1Solutions.Naati.Common.Bl.BackgroundTask
{
    public class BackgroundTaskLogger : IBackgroundTaskLogger
    {
        private StringBuilder _info;
        private StringBuilder _errors;
        private StringBuilder _warnings;
        private StringBuilder _detailedLog;

        private List<string> _processedPayments;
        private List<string> _processedInvoices;
        private List<string> _processedApplications;
        private PerformContext _dashBoardContext;
        private const string MessageTemplate = "Type: {Operation}, Reference: {Reference}, {Message}";
        private const string ApplicationMessageTemplate = "ApplicationId:{ApplicationId} Type: {Operation}, Reference: {Reference}, {Message}";
        
        public BackgroundTaskLogger()
        {
            Reset();
        }

        private void Reset()
        {
            _info = new StringBuilder();
            _errors = new StringBuilder();
            _warnings = new StringBuilder();
            _detailedLog = new StringBuilder();
            _processedPayments = new List<string>();
            _processedInvoices = new List<string>();
            _processedApplications = new List<string>();
        }

        public void SetContext(PerformContext context)
        {
            _dashBoardContext = context;
        }
        public void UnsetContext()
        {
            _dashBoardContext = null;
            Reset();
        }

        public void WriteDebug(string text, params object[] properties)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            _dashBoardContext?.WriteLine(text.FormatTemplate(properties));
            LoggingHelper.LogDebug(text, properties);
        }

        public void WriteInfo(string text, params object[] properties)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            _dashBoardContext?.WriteLine(text.FormatTemplate(properties));
            _info.Append($"<li>{text}</li>");
            LoggingHelper.LogInfo(text, properties);
        }

        public void WriteWarning(string text, params object[] properties)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            _dashBoardContext?.SetTextColor(ConsoleTextColor.Yellow);
            _dashBoardContext?.WriteLine(text.FormatTemplate(properties));
            _dashBoardContext?.ResetTextColor();
            LoggingHelper.LogWarning(text, properties);
        }

        public void WriteWarning(string warning, string type, string reference, bool isUserFriendlyMessage)
        {
            if (string.IsNullOrWhiteSpace(warning))
            {
                return;
            }

            _dashBoardContext?.SetTextColor(ConsoleTextColor.Yellow);
            _dashBoardContext?.WriteLine(MessageTemplate.FormatTemplate(type, reference, warning));
            _dashBoardContext?.ResetTextColor();
            LoggingHelper.LogWarning(MessageTemplate, type, reference, warning);
            _warnings.AppendLine($"<li>{DateTime.Now}  {ProcessReportMessage(warning, type, reference, isUserFriendlyMessage)}</li>");
            _detailedLog.AppendLine($"<p>WARNING: {DateTime.Now}  {warning} </p>");
        }


        public void WriteError(Exception exception, string type, string reference, bool isUserFriendlyMessage)
        {
            if (string.IsNullOrWhiteSpace(exception.Message))
            {
                return;
            }
            LoggingHelper.LogException(exception, MessageTemplate, type, reference, exception.Message);
            _dashBoardContext?.SetTextColor(ConsoleTextColor.Red);
            _dashBoardContext?.WriteLine(MessageTemplate.FormatTemplate(type, reference, exception.Message));
            _dashBoardContext?.ResetTextColor();

            _errors.AppendLine($"<li>{DateTime.Now}  {ProcessReportMessage(exception.Message, type, reference, isUserFriendlyMessage)}</li>");
            _detailedLog.AppendLine($"<p> ERROR: {DateTime.Now}  {exception.Message}</p>");
        }

        public void WriteApplicationWarning(string warning, int applicationId, string type, string reference, bool isUserFriendlyMessage)
        {
            if (string.IsNullOrWhiteSpace(warning))
            {
                return;
            }

            _dashBoardContext?.SetTextColor(ConsoleTextColor.Yellow);
            _dashBoardContext?.WriteLine(MessageTemplate.FormatTemplate(type, reference, warning));
            _dashBoardContext?.ResetTextColor();
            LoggingHelper.LogWarning(ApplicationMessageTemplate, applicationId, type, reference, warning);
            _warnings.AppendLine($"<li>{DateTime.Now}  {ProcessReportMessage(warning, type, reference, isUserFriendlyMessage, applicationId)}</li>");
            _detailedLog.AppendLine($"<p>WARNING: {DateTime.Now} ApplicationId: {applicationId}  {warning} </p>");
        }

        public void WriteApplicationError(Exception exception, int applicationId, string type, string reference, bool isUserFriendlyMessage)
        {
            if (string.IsNullOrWhiteSpace(exception.Message))
            {
                return;
            }
            _dashBoardContext?.SetTextColor(ConsoleTextColor.Red);
            _dashBoardContext?.WriteLine(MessageTemplate.FormatTemplate(type, reference, exception.Message));
            _dashBoardContext?.ResetTextColor();
            LoggingHelper.LogException(exception, ApplicationMessageTemplate, applicationId, type, reference, exception.Message);

            _errors.AppendLine($"<li>{DateTime.Now}  {ProcessReportMessage(exception.Message, type, reference, isUserFriendlyMessage, applicationId)}</li>");
            _detailedLog.AppendLine($"<p> ERROR: {DateTime.Now}  ApplicationId: {applicationId}  {exception.Message} </p>");
        }

        public void WriteError(string error, string type, string reference, bool isUserFriendlyMessage, params object[] properties)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                return;
            }

            _dashBoardContext?.SetTextColor(ConsoleTextColor.Red);
            _dashBoardContext?.WriteLine(MessageTemplate.FormatTemplate(type, reference, error));
            _dashBoardContext?.ResetTextColor();
            LoggingHelper.LogError(MessageTemplate, type, reference, error, properties);

            _errors.AppendLine($"<li>{DateTime.Now}  {ProcessReportMessage(error, type, reference, isUserFriendlyMessage)}</li>");
            _detailedLog.AppendLine($"<p> ERROR: {DateTime.Now}  {error}</p>");
        }

        internal IProgressBar CreateProgressBar()
        {
            return _dashBoardContext?.WriteProgressBar();
        }

        public void AdProcessedPayment(string reference)
        {
            _processedPayments.Add(reference);
        }

        public void AdProcessedInvoice(string reference)
        {
            _processedInvoices.Add(reference);
        }

        public void AdProcessedApplication(string reference)
        {
            _processedApplications.Add(reference);
        }

        private string ProcessReportMessage(string message, string type, string reference, bool isUserFriendlyMessage)
        {
            if (isUserFriendlyMessage)
            {
                return $"Type: {type} Reference:{reference}, {message}";
            }

            return $"Type: {type} Reference:{reference}, General Error Ocurred.";
        }
        private string ProcessReportMessage(string message, string type, string reference, bool isUserFriendlyMessage, int applicationId)
        {
            if (isUserFriendlyMessage)
            {
                return $"Type: {type} Reference:{reference}, ApplicationId:{applicationId}. {message}";
            }

            return $"Type: {type} Reference:{reference}, ApplicationId:{applicationId}. General Error Ocurred.";
        }

        public string GetReportContent<TJobType>(TJobType taskName)
        {
            // if file deletion then send email report if info has been logged.
            if (taskName.ToString() == "ProcessFileDeletesPastExpiryDate")
            {
                if (_info.Length == 0
                && _errors.Length == 0
                && _warnings.Length == 0 &&
                _processedInvoices.Count == 0
                && _processedPayments.Count == 0
                && _processedApplications.Count == 0)
                {
                    return string.Empty;
                }
                return BuildReport(taskName);
            }

            if (_errors.Length == 0
                && _warnings.Length == 0 &&
                _processedInvoices.Count == 0
                && _processedPayments.Count == 0
                && _processedApplications.Count == 0)
            {
                return string.Empty;
            }

            return BuildReport(taskName);
        }

        public string GetDetailedLogContent()
        {
            if (_detailedLog.Length == 0)
            {
                return string.Empty;
            }

            return BuildDetailedLogReport();
        }


        private string BuildReport<TJobType>(TJobType taskName)
        {
            var info = _info.Length > 0 ? $"<p><b>Task Information Report: </b></p><ul> {_info} </ul>" : string.Empty;
            var errors = _errors.Length > 0 ? $"<p><b>The following errors were encountered: </b></p> <ul> {_errors} </ul>" : string.Empty;
            var warnings = _warnings.Length > 0 ? $"<p><b>The following warnings were encountered: </b></p> <ul> {_warnings}</ul>" : string.Empty;
            return $"<!DOCTYPE html>" +
                   $" <html>" +
                   $"<head>" +
                   $"</head> " +
                   $"<body>" +
                   $"<p><b>Batch {taskName} Report</b></p>" +
                   $"{DateTime.Now}" +
                   $"<br/>" +
                   (_processedInvoices.Count > 0 ? $"<p>{_processedInvoices.Count} invoices successfully created in Wiise.</p>" : string.Empty) +
                   (_processedPayments.Count > 0 ? $"<p>{_processedPayments.Count} payments successfully created in Wiise.</p>" : string.Empty) +
                   (_processedApplications.Count > 0 ? $"<p>{_processedApplications.Count} applications successfully processed.</p>" : string.Empty) +
                   $"<br/>" +
                   $"{info}" +
                   $"{errors}" +
                   $"{warnings}" +
                   $" </body> " +
                   $"</html>";
        }

        private string BuildDetailedLogReport()
        {
            return $"<!DOCTYPE html> <html><head></head> <body> {_detailedLog} </body> </html>";
        }
    }
}
