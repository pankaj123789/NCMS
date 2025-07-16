using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using F1Solutions.DatabaseUpdater.Framework;
using F1Solutions.Global.Common.Logging;

namespace F1Solutions.Naati.Common.Migrations.Updater
{
    public class ReportsMigrationTarget : MigrationTarget
    {
        public string NaatiDbName { get; set; }

        public ReportsMigrationTarget(Type[] dummyMigration, string connectionString)
            : base(dummyMigration, connectionString)
        {
            NaatiDbName = MigrationArguments.GetArgument("NaatiDbName");
        }

        public ReportsMigrationTarget(Type[] dummyMigration, IEnumerable<string> pathsToCheck, string connectionStringKey)
            : base(dummyMigration, pathsToCheck, connectionStringKey)
        {
            NaatiDbName = MigrationArguments.GetArgument("NaatiDbName");
        }

        public override void RunPostMigrationSteps()
        {
            LoggingHelper.LogInfo("Checking pre-migration scripts...");
            var buildVersion = MigrationArguments.GetArgument("buildVersion");
            var checkVersion = bool.Parse(MigrationArguments.GetArgument("checkBuildVersion"));
            using (var scriptRunner = GetMigrationRunner())
            {
                LoggingHelper.LogInfo($"Build Version: {buildVersion}");
                var dbBuildVersion = scriptRunner.GetLastRunVersion();
                LoggingHelper.LogInfo($"Database Build Version: {dbBuildVersion}");
                if (checkVersion && buildVersion == dbBuildVersion)
                {
                    LoggingHelper.LogInfo("No new build version found.");
                    return;
                }
                LoggingHelper.LogInfo($"Running post-migration scripts...");
    
                scriptRunner.RunPostMigrationScripts();
                LoggingHelper.LogInfo($"Updating Build version to {buildVersion}.");
                scriptRunner.UpdateDbVersion(buildVersion);
                LoggingHelper.LogInfo("Build Version updated");
            }

            LoggingHelper.LogInfo("Post migration scripts run.");
        }

        public override void RunPreMigrationSteps()
        {
           
        }

        private ReportsScriptRunner GetMigrationRunner()
        {
            var reportsConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
            var reportsDbName = reportsConnectionStringBuilder.InitialCatalog;

            var dbNameTokenMappings = new Dictionary<string, string>
            {
                { "naati_db", NaatiDbName },
                { "reports_db", reportsDbName }
            };

            var checkVersion = bool.Parse(MigrationArguments.GetArgument("checkBuildVersion"));
            return new ReportsScriptRunner(ConnectionString, dbNameTokenMappings, !checkVersion);
        }
    }
}
