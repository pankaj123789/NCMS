using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using F1Solutions.DatabaseUpdater.Framework;
using F1Solutions.Global.Common.Logging;

namespace F1Solutions.Naati.Common.Migrations.Updater
{
    public class NaatiMigrationTarget : MigrationTarget
    {
        public NaatiMigrationTarget(Type[] dummyMigration, string connectionString)
            : base(dummyMigration, connectionString) { }

        public NaatiMigrationTarget(Type[] dummyMigration, IEnumerable<string> pathsToCheck, string connectionStringKey)
            : base(dummyMigration, pathsToCheck, connectionStringKey) { }

        public override void RunPreMigrationSteps()
        {
            LoggingHelper.LogInfo("Checking pre-migration scripts...");
            var buildVersion = MigrationArguments.GetArgument("buildVersion");
            LoggingHelper.LogInfo($"Build Version: {buildVersion}");
            var checkVersion = bool.Parse(MigrationArguments.GetArgument("checkBuildVersion"));
            using (var scriptRunner = GetMigrationRunner())
            {
                var dbBuildVersion = scriptRunner.GetLastRunVersion();
                LoggingHelper.LogInfo($"Database Build Version: {dbBuildVersion}");
                if (checkVersion && buildVersion == dbBuildVersion)
                {
                    LoggingHelper.LogInfo("No new build version found.");
                    return;
                }

                LoggingHelper.LogInfo("Running pre-migration scripts...");
                scriptRunner.RunPreMigrationScripts();
            }
        }

        public override void RunPostMigrationSteps()
        {
            LoggingHelper.LogInfo("Checking post-migration scripts...");
            var buildVersion = MigrationArguments.GetArgument("buildVersion");
            var checkVersion = bool.Parse(MigrationArguments.GetArgument("checkBuildVersion"));
            using (var scriptRunner = GetMigrationRunner())
            {
                LoggingHelper.LogInfo($"Build Version: {buildVersion}");
                var dbBuildVersion = scriptRunner.GetLastRunVersion();
                LoggingHelper.LogInfo($"Database Build Version: {dbBuildVersion}");

                if (checkVersion && buildVersion == scriptRunner.GetLastRunVersion())
                {
                    LoggingHelper.LogInfo("No new build version found.");
                    return;
                }

                LoggingHelper.LogInfo("Running post-migration scripts....");
                 
                scriptRunner.RunPostMigrationScripts();
                LoggingHelper.LogInfo($"Updating build version to {buildVersion}");
                scriptRunner.UpdateDbVersion(buildVersion);
                LoggingHelper.LogInfo("$Updating build version updated.");
            }
        }
        
        private NaatiScriptRunner GetMigrationRunner()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
            var ncmsDbName = connectionStringBuilder.InitialCatalog;
            var dbNameTokenMappings = new Dictionary<string, string>
            {
                {"naati_db", ncmsDbName},
                {"sam4_db", GetSamDatabaseName()}

            };
            var checkVersion = bool.Parse(MigrationArguments.GetArgument("checkBuildVersion"));
            return new NaatiScriptRunner(ConnectionString, dbNameTokenMappings, !checkVersion);
        }

        private string GetSamDatabaseName()
        {
            var scalar = ExecuteScalar("select Value from tblSystemValue where ValueKey = 'SamDatabaseName'");
            return scalar?.ToString() ?? "NAATI";
        }

        
        private void ExecuteNonQuery(string sql)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }

        private object ExecuteScalar(string sql)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    return command.ExecuteScalar();
                }
            }
        }
    }
}
