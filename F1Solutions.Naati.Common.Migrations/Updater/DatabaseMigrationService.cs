using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using F1Solutions.DatabaseUpdater.Framework;
using F1Solutions.DatabaseUpdater.Framework.ConsoleRunner;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal.NHibernate.DataAccess;
using F1Solutions.Naati.Common.Migrations.NAATI;
using F1Solutions.Naati.Common.Migrations.Reports;
using FluentMigrator;

namespace F1Solutions.Naati.Common.Migrations.Updater
{
    public class DatabaseMigrationService : IDatabaseMigrationService
    {
        private const string RunMigrationsValueKey = "RunMigrations";
        private readonly ISecretsCacheQueryService _secretsProvider;

        private const string NaatiMigration = "NAATI";
        private const string ReportingMigration = "Reports";
        public DatabaseMigrationService(ISecretsCacheQueryService secretsProvider)
        {
            _secretsProvider = secretsProvider;
        }

        public (bool migrated, bool error) MigrateDb()
        {
            LoggingHelper.LogInfo("Starting MigrateDb...");
            if (JobManager.RequestJobToken(RunMigrationsValueKey))
            {
                try
                {
                    LoggingHelper.LogInfo("Running Migrations...");

                    var ncmsConnectionString = _secretsProvider.Get("NcmsMigrationConnectionString");
                    var scriptTimeoutSeconds = ConfigurationManager.AppSettings["ScriptsTimeoutSeconds"];

                    var naatiRunner = new NaatiMigrationRunner();

                    var ncmsArgs = new[] { $"-m={NaatiMigration}", $"-c={ncmsConnectionString}", $"-timeout:{scriptTimeoutSeconds}" };
                    var ncmsMigration = GetNcmsTargetDefinition(ncmsArgs, true);
                    naatiRunner.MigrationTargetDefinitions.Add(ncmsMigration);
                    var migrated = naatiRunner.Run(ncmsArgs, ncmsMigration.Name);

                    if (!migrated)
                    {
                        return (false, true);
                    }

                    LoggingHelper.LogInfo("Running Reporting Migrations...");

                    //var naatiConnectionStringBuilder = new SqlConnectionStringBuilder(ncmsConnectionString);

                    //var reportingDbConnectionString = _secretsProvider.Get("ReportingMigrationConnectionString");
                    //var reportArgs = new[] { $"-m={ReportingMigration}", $"-c={reportingDbConnectionString}", $"-timeout:{scriptTimeoutSeconds}" };
                    //var reportingDbMigration = GetNcmsReportTargetDefinition(reportArgs, true, naatiConnectionStringBuilder.InitialCatalog);
                    //naatiRunner.MigrationTargetDefinitions.Add(reportingDbMigration);
                    //migrated = naatiRunner.Run(reportArgs, reportingDbMigration.Name);
                    //LoggingHelper.LogInfo("DB Migration Finished...");
                    return (migrated, !migrated);
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(ex);
                    return (false, true);
                }
                finally
                {
                    JobManager.ReleaseJobToken(RunMigrationsValueKey);
                }
            }
            else
            {
                LoggingHelper.LogInfo("RequestJobToken failed...");
            }
            return (false, false);
        }

        private void ValidateMigrations(MigrationTargetDefinition targetDefinition)
        {
            foreach (var dummyType in targetDefinition.DummyType)
            {
                var migrationsToFix = dummyType.Assembly.GetTypes()
                    .Where(x => typeof(Migration).IsAssignableFrom(x) && !(typeof(NaatiMigration).IsAssignableFrom(x))).ToList();

                if (migrationsToFix.Any())
                {
                    throw new Exception($"Following classes need to inherit from  {typeof(NaatiMigration).Name} class :{ string.Join(",", migrationsToFix.Select(x => x.Name))}");
                }
            }
        }

        private void SetCommonMigrationArguments(string[] args, bool checkBuildVersion)
        {
            MigrationArguments.Reset();

            MigrationArguments.AddCommandLineArg("checkBuildVersion", checkBuildVersion.ToString());

            var buildVersion = GetAssemblyFileVersion();
            MigrationArguments.AddCommandLineArg("buildVersion", buildVersion);
            MigrationArguments.AcceptCommandLineArgs(args);
        }
        public MigrationTargetDefinition GetNcmsReportTargetDefinition(string[] args, bool checkBuildVersion, string naatiDbName)
        {
            SetCommonMigrationArguments(args, checkBuildVersion);
            MigrationArguments.AddCommandLineArg("NaatiDbName", naatiDbName);
            var reportsMigrationDefinition = new MigrationTargetDefinition(new[] { typeof(DummyReportsMigration) }, ReportingMigration, typeof(ReportsMigrationTarget));
            reportsMigrationDefinition.ConfigLocations.Add(@"..\..\..\Ncms.Ui\web.config");
            reportsMigrationDefinition.ConnectionStringConfigKey = "ReportingMigrationConnectionString";
            reportsMigrationDefinition.ProcessorType = "SqlServer2008";

            ValidateMigrations(reportsMigrationDefinition);
            return reportsMigrationDefinition;
        }

        public MigrationTargetDefinition GetNcmsTargetDefinition(string[] args, bool checkBuildVersion)
        {
            SetCommonMigrationArguments(args, checkBuildVersion);

            var naatiMigrationDefinition = new MigrationTargetDefinition(new[] { typeof(NaatiMigration) }, NaatiMigration, typeof(NaatiMigrationTarget));
            naatiMigrationDefinition.ConfigLocations.Add(@"..\..\..\Ncms.Ui\web.config");
            naatiMigrationDefinition.ConfigLocations.Add("web.config");
            naatiMigrationDefinition.ConfigLocations.Add(@"..\web.config");
            naatiMigrationDefinition.ConnectionStringConfigKey = "NcmsMigrationConnectionString";
            naatiMigrationDefinition.ProcessorType = "SqlServer2008";

            ValidateMigrations(naatiMigrationDefinition);
            return naatiMigrationDefinition;
        }
        public string GetAssemblyFileVersion()
        {
            var attribute = Assembly.GetAssembly(this.GetType())
                .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)
                .Cast<AssemblyFileVersionAttribute>()
                .FirstOrDefault();

            return attribute != null ? attribute.Version : string.Empty;
        }
    }
}
