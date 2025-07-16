using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml;
using F1Solutions.DatabaseUpdater.Framework;
using F1Solutions.DatabaseUpdater.Framework.ConsoleRunner;
using F1Solutions.Naati.Common.Migrations;
using F1Solutions.Naati.Common.Migrations.Updater;
using F1Solutions.Naati.Common.Utils.EnvironmentVariables;

namespace F1Solutions.Naati.Common.Utils
{
    class Program
    {
        static string[] _args;
        private const string NaatiMigration = "NAATI";
        private const string ReportingMigration = "Reports";

        static void Main(string[] args)
        {
            _args = args;
            MigrateDatabase();
        }

        private static void GenerateEnvironmentVariables()
        {
            var generator = new Generator();
            generator.Generate();
        }

        private static void MigrateDatabase()
        {
            var newArgs = _args;
            if (!_args.Any(x => x?.ToUpper().Contains("TIMEOUT") ?? false))
            {
                newArgs = _args.Concat(new[] { "-timeout:500" }).ToArray();
            }

            var databaseMigrator = new DatabaseMigrationService( null);
            var naatiMigration = databaseMigrator.GetNcmsTargetDefinition(newArgs, false);

            var ncmsConnectionString = GetConnectionStringFromNcmsConfig("NcmsMigrationConnectionString");
            var naatiConnectionStringBuilder = new SqlConnectionStringBuilder(ncmsConnectionString);
            var reportTargetDefinition = databaseMigrator.GetNcmsReportTargetDefinition(newArgs, false, naatiConnectionStringBuilder.InitialCatalog);

            var consoleRunner = new ConsoleRunner();
            consoleRunner.MigrationTargetDefinitions.Add(naatiMigration);
            var ncmsArgs = SetConnectionStringArg(newArgs, "NcmsMigrationConnectionString", NaatiMigration);
            consoleRunner.Run(ncmsArgs, false);

            var reportingArgs = SetConnectionStringArg(newArgs, "ReportingMigrationConnectionString", ReportingMigration);
            consoleRunner.MigrationTargetDefinitions.Add(reportTargetDefinition);
            consoleRunner.Run(reportingArgs, true);
        }

        public static string[] SetConnectionStringArg(string[] args, string connectionStringConfigKey, string migrationName)
        {
            var arguments = args;
            arguments = arguments.Concat(new[] { $"-m={migrationName}" }).ToArray();

            var connectionString = GetConnectionStringFromNcmsConfig(connectionStringConfigKey);
        
            ConfigurationManager.AppSettings.Set(connectionStringConfigKey, connectionString);
            arguments = arguments.Concat(new[] { $"-c={connectionString}" }).ToArray();
            return arguments;
        }

        private static string GetConnectionStringFromNcmsConfig(string connectionStringKey)
        {
            var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            var ncmsDirectoryPath = currentDirectory.Parent.Parent.Parent.FullName;
            var userConfigPath = Path.Combine(ncmsDirectoryPath, "Ncms.Ui\\User.config");
            var doc = new XmlDocument();
            doc.Load(userConfigPath);

            var applicationSettingsNode = doc.SelectSingleNode("//appSettings");
            var connectionStringNode = applicationSettingsNode.SelectSingleNode($"//add[@key='{connectionStringKey}']");

            var value = connectionStringNode.Attributes["value"].Value;return value;
        }

    }
}
