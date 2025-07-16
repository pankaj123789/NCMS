using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.DatabaseUpdater.Framework;
using F1Solutions.Global.Common.Logging;
using FluentMigrator;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using NDesk.Options;
using NHibernate;

namespace F1Solutions.Naati.Common.Migrations.Updater
{
    public class NaatiMigrationRunner
    {
        public IList<MigrationTargetDefinition> MigrationTargetDefinitions { get; set; }
        private OptionSet _mOptionSet;
        private MigrationTargetDefinition _selectedMigrationTargetDefinition;
        private long? _maxVersionNumber;
        private int _mCommandTimeout = 30;
        private string ConnectionString { get; set; }
        public NaatiMigrationRunner()
        {
            MigrationTargetDefinitions = new List<MigrationTargetDefinition>();
            InitialiseOptionSet();
        }

        private void InitialiseOptionSet()
        {
            _mOptionSet = new OptionSet();
            _mOptionSet.Add("m=", "Choose which migration to use by its name.", SetMigrationTargetDefinition);
            _mOptionSet.Add("c|s=", "The connection string to use for communicating with the database", value => ConnectionString = value);
            _mOptionSet.Add("v=", "The highest migration version number to run.", arg => _maxVersionNumber = long.Parse(arg));
            _mOptionSet.Add("timeout:", "Overrides the default SqlCommand timeout of 30 seconds.", v => _mCommandTimeout = int.Parse(v));
        }

        private void SetMigrationTargetDefinition(string name)
        {
            _selectedMigrationTargetDefinition = MigrationTargetDefinitions.SingleOrDefault(definition => definition.Name == name);
        }
        public bool Run(string[] args, string migrationTargetDefinitionName)
        {
            SetMigrationTargetDefinition(migrationTargetDefinitionName);
            try
            {
                ParseArguments(args);
                RunMigrations();
                return true;
            }
            catch (Exception e)
            {
                LoggingHelper.LogException(e);
            }

            return false;
        }
        private void ParseArguments(string[] args)
        {
            _mOptionSet.Parse(args);
        }
        private void RunMigrations()
        {
            IList<MigrationTarget> targets = GetTargets();
            var conventions = new CustomMigrationConventions(_maxVersionNumber);
            RunTarget(targets.First(), conventions);
        }

        private void RunTarget(MigrationTarget target, MigrationConventions conventions)
        {
            var announcer = new NaatiMigratorAnnouncer() { ShowSql = true };
            var runnerContext = new RunnerContext(announcer)
            {
                Database = target.ProcessorType,
                Connection = target.ConnectionString,
                Targets = target.Assembly.Select(x => x.Location).ToArray(),
                Namespace = target.MigrationsNamespace,
                NestedNamespaces = true,
                PreviewOnly = false,
                Timeout = _mCommandTimeout,
                TransactionPerSession = true
            };

            LoggingHelper.LogInfo($"Executing Migration Target {target.GetType().Name}");
            target.RunPreMigrationSteps();
            var executor = new TaskExecutor(runnerContext)
            {
                Conventions = conventions
            };
            executor.Execute();
            target.RunPostMigrationSteps();
            LoggingHelper.LogInfo($"Executon fo Migration Target {target.GetType().Name} Finished");
        }

        private IList<MigrationTarget> GetTargets()
        {
            return new[] { _selectedMigrationTargetDefinition.CreateMigrationTarget(ConnectionString) };
        }
    }
}
