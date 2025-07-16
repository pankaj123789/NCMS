using System;
using System.Configuration;
using F1Solutions.Global.Common.Logging;

namespace F1Solutions.Global.Common.SystemLifecycle
{
    public static class SystemLifecycleHelper
    {
        public static SystemLifecycleStatus SystemStatus { get; private set; }

        public static SystemLifecycleStatus UpdateLifecycleStatus(SystemLifecycleStatus status)
        {
            var previousStatus = SystemStatus;
            SystemStatus = status;
            LoggingHelper.LogWarning($"System status changed from {previousStatus} to {SystemStatus}");
            return SystemStatus;
        }

        public static void ValidateSystemTimeZone()
        {
            string desiredTimeZone = ConfigurationManager.AppSettings["DesiredTimeZone"];
            var currentTimeZone = TimeZone.CurrentTimeZone.StandardName;
            if (currentTimeZone.ToUpper() != desiredTimeZone.ToUpper())
            {
                LoggingHelper.LogError($"NAATI System is expecting a Environment in '{desiredTimeZone}' Time Zone. Current Time Zone: '{currentTimeZone}'");
                throw new Exception($"NAATI System is expecting a Environment in '{desiredTimeZone}' Time Zone. Current Time Zone: '{currentTimeZone}'");
            }
        }

        public static void ThrowIfNotRunning()
        {
            if (SystemLifecycleHelper.SystemStatus != SystemLifecycleStatus.Running)
            {
                throw new OperationCanceledException($"NaatiSystem status is  {SystemLifecycleHelper.SystemStatus}");
            }
        }
    }

    public enum SystemLifecycleStatus
    {
        None = 0,
        Starting = 1,
        Running = 2,
        Stopping = 3,
        Failed = 4,
    }
}
