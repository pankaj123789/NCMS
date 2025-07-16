using System;
using F1Solutions.Global.Common.Logging;
using FluentMigrator.Runner.Announcers;

namespace F1Solutions.Naati.Common.Migrations.Updater
{
    public class NaatiMigratorAnnouncer : Announcer
    {
        public override void Write(string message, bool escaped)
        {
           LoggingHelper.LogInfo($"Fluent Migrator: {message}");
        }

        public override void Error(Exception exception)
        {
            if (exception != null)
            {
                LoggingHelper.LogException(exception);
            }
           
            base.Error(exception);
        }

        public override void Error(string message)
        {
            LoggingHelper.LogError(message ?? string.Empty);
            base.Error(message);
        }
    }
}
