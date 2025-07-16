using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using F1Solutions.Global.Common.SystemLifecycle;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;

namespace F1Solutions.Global.Common.Logging
{
    public class LoggingHelper
    {
        public static void ConfigureLogging(string system, string product, string seqBufferbaseName, string logFileName, string loggingDiagnosticsLogFileName, string database)
        {
            if (!String.IsNullOrEmpty(loggingDiagnosticsLogFileName))
            {
                // this logfile is for debugging issues with logging, how meta
                var file = File.CreateText(loggingDiagnosticsLogFileName);
                Serilog.Debugging.SelfLog.Enable(TextWriter.Synchronized(file));
            }

            var minLevelFromConfig = ConfigurationManager.AppSettings["logging:MinimumLevel"];
            var minLevel = String.IsNullOrEmpty(minLevelFromConfig) ? LogEventLevel.Debug : minLevelFromConfig.ParseEnum<LogEventLevel>();
            var logLevelSwitch = new LoggingLevelSwitch(minLevel);

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("System", system)
                .Enrich.WithProperty("Product", product)
                .Enrich.WithProperty("Version", GetVersion(new StackFrame(1).GetMethod()?.DeclaringType?.Assembly))
                .Enrich.WithProperty("Database", database)
                .Enrich.WithProperty("Environment", ConfigurationManager.AppSettings["EnvironmentType"])
                .Enrich.WithProperty("EnvironmentName", ConfigurationManager.AppSettings["EnvironmentName"])
                .Enrich.WithProperty("MachineName", System.Net.Dns.GetHostEntry("").HostName)
                .Enrich.WithDynamicProperty("UserName", () => Thread.CurrentPrincipal?.Identity?.Name ?? Environment.UserName)
                .Enrich.WithExceptionDetails()
                .Enrich.FromLogContext()
                .WriteTo.Seq(ConfigurationManager.AppSettings["logging:SeqUrl"], batchPostingLimit : 10, bufferBaseFilename : seqBufferbaseName,
                    bufferSizeLimitBytes : 1000000, compact : true)
                .Filter.ByExcluding(x => x.Level == LogEventLevel.Debug && x.Properties.ContainsKey("Name") && x.Properties["Name"].ToString().StartsWith("\"Hangfire"))
                .WriteTo.File(logFileName, LogEventLevel.Debug, fileSizeLimitBytes : 1000000, rollingInterval : RollingInterval.Day,
                    rollOnFileSizeLimit : true, retainedFileCountLimit : 28)
                .MinimumLevel.ControlledBy(logLevelSwitch)
                .CreateLogger();

            LogInfo("Application initialising on {MachineName}: {System}:{Product} {Version} {EnvironmentName} with " + minLevel + " logging");
            SystemLifecycleHelper.UpdateLifecycleStatus(SystemLifecycleStatus.Starting);
        }

        private static string GetVersion(Assembly versionAssembly)
        {
            if (versionAssembly == null)
            {
                return "unknown";
            }

            object[] attributes = versionAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            return attributes.Length != 0 ? versionAssembly.GetName().Version.ToString() : "";
        }

        public static void LogVerbose(string messageTemplate, params object[] properties)
        {
            LogVerbose(messageTemplate, new StackFrame(1).GetMethod()?.DeclaringType?.Name, properties);
        }

        public static void LogDebug(string messageTemplate, params object[] properties)
        {
            LogDebug(messageTemplate, new StackFrame(1).GetMethod()?.DeclaringType?.Name, properties);
        }

        public static void LogInfo(string messageTemplate, params object[] properties)
        {
            LogInfo(messageTemplate, new StackFrame(1).GetMethod()?.DeclaringType?.Name, properties);
        }

        public static void LogWarning(string messageTemplate, params object[] properties)
        {
            LogWarning(messageTemplate, new StackFrame(1).GetMethod()?.DeclaringType?.Name, properties);
        }

        public static void LogWarning(Exception exception, string messageTemplate, params object[] properties)
        {
            LogWarning(exception, messageTemplate, new StackFrame(1).GetMethod()?.DeclaringType?.Name, properties);
        }

        public static void LogError(string messageTemplate, params object[] properties)
        {
            LogError(messageTemplate, new StackFrame(1).GetMethod()?.DeclaringType?.Name, properties);
        }

        public static void LogException(Exception exception, string messageTemplate, params object[] properties)
        {
            LogException(exception, messageTemplate, new StackFrame(1).GetMethod()?.DeclaringType?.Name, properties);
        }

        public static void LogException(Exception exception)
        {
            LogException(exception, exception.Message,
                String.IsNullOrEmpty(exception.Source)
                    ? new StackFrame(1).GetMethod()?.DeclaringType?.Name
                    : exception.Source);

            if (exception?.InnerException != null)
            {
                LogException(exception.InnerException, exception.InnerException.Message,
                    String.IsNullOrEmpty(exception.InnerException.Source)
                        ? new StackFrame(1).GetMethod()?.DeclaringType?.Name
                        : exception.InnerException.Source);
            }
        }

        public static void LogExceptionWithHttpContext(Exception exception, HttpContext context)
        {
            using (LogContext.PushProperty("RequestType", context?.Request?.RequestType))
            using (LogContext.PushProperty("URL", context?.Request?.Url?.ToString()))
            {
                LogException(exception, exception.Message,
                    String.IsNullOrEmpty(exception.Source)
                        ? new StackFrame(1).GetMethod()?.DeclaringType?.Name
                        : exception.Source);
            }
        }

        public static void LogVerbose(string messageTemplate, string source, object[] properties)
        {
            LogEvent(LogEventLevel.Verbose, source, null, messageTemplate, properties);
        }

        public static void LogDebug(string messageTemplate, string source, object[] properties)
        {
            LogEvent(LogEventLevel.Debug, source, null, messageTemplate, properties);
        }

        public static void LogInfo(string messageTemplate, string source, object[] properties)
        {
            LogEvent(LogEventLevel.Information, source, null, messageTemplate, properties);
        }

        public static void LogWarning(string messageTemplate, string source, object[] properties)
        {
            LogEvent(LogEventLevel.Warning, source, null, messageTemplate, properties);
        }

        public static void LogWarning(Exception exception, string messageTemplate, string source, object[] properties)
        {
            LogEvent(LogEventLevel.Warning, source, exception, messageTemplate, properties);
        }

        public static void LogError(string messageTemplate, string source, object[] properties)
        {
            LogEvent(LogEventLevel.Error, source, null, messageTemplate, properties);
        }

        public static void LogException(Exception exception, string messageTemplate, string source, object[] properties)
        {
            LogEvent(LogEventLevel.Error, source, exception, messageTemplate, properties);
        }

        private static void LogEvent(LogEventLevel level, string source, Exception exception, string messageTemplate, params object[] properties)
        {
            try
            {
                using (LogContext.PushProperty("Source", source))
                {

                    Log.Logger.Write(level, exception, messageTemplate, properties);
                }
            }
            catch
            {
                // todo: log the logging error somehow?
            }
        }
    }
}