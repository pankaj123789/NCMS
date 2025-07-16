using System;
using FluentMigrator;

namespace F1Solutions.Naati.Common.Migrations
{
    /// <summary>
    /// Intended for use with reporting db migrations that make column changes to existing tables, where the ObsolotedDate index needs to be recreated.
    /// Drops the ObsoletedDate's index for the table when constructed, and then recreates it when disposed
    /// </summary>
    internal class ReportMigrationContainer : IDisposable
    {
        private readonly Migration _migration;
        private readonly string _table;
        private readonly string _tableSuffix;

        public ReportMigrationContainer(Migration migration, string table, string tableSuffix = "History")
        {
            _migration = migration;
            _table = table;
            _tableSuffix = tableSuffix;

            _migration.ExecuteSql(string.Format("DROP INDEX IX_{0}_ObsoletedDate ON {0}{1}", _table, _tableSuffix));
        }

        public void Dispose()
        {
            _migration.CreateObsoletedDateIndex(_table, _tableSuffix);
        }
    }
}
