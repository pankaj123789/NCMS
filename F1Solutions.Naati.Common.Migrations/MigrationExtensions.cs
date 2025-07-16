using System;
using System.Collections.Generic;
using System.Linq;
using FluentMigrator;

namespace F1Solutions.Naati.Common.Migrations
{
    internal static class MigrationExtensions
    {
        /// <summary>
        /// Executes a SQL statement, by parsing and executing individual components separated by Go statements.
        /// </summary>
        /// <param name="migration"></param>
        /// <param name="sqlStatment"></param>
        public static void ExecuteSql(this Migration migration, string sqlStatment)
        {
            //  Split the sql on the Go statements.  Split is case sensitive so I included all possible combinations of Go.
            var sqlStatments = sqlStatment.Split(new [] { "\r\nGO\r\n", "\r\ngo\r\n", "\r\nGo\r\n", "\r\ngO\r\n", "\r\nGO ", "\r\ngo ", "\r\nGo ", "\r\ngO " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var sql in sqlStatments)
            {
                migration.Execute.Sql(sql);
            }
        }

        /// <summary>
        /// Intended for use with reporting db migrations that make changes to existing tables or create new tables, where the ObsolotedDate index needs to be (re)created
        /// </summary>
        public static void CreateObsoletedDateIndex(this Migration migration, string table, string tableSuffix = "History")
        {
            migration.Execute.WithConnection((connection, transaction) =>
            {
                var columnsNames = new List<string>();

                var getColumnNamesCommand = connection.CreateCommand();

                getColumnNamesCommand.Transaction = transaction;
                getColumnNamesCommand.CommandText = string.Format(@"
SELECT
	[COLUMN_NAME]
FROM [INFORMATION_SCHEMA].[COLUMNS]
WHERE [TABLE_NAME] = '{0}{1}'
AND [TABLE_SCHEMA] = 'dbo'
AND [COLUMN_NAME] != 'ModifiedDate'
AND [COLUMN_NAME] != 'DeletedDate'
AND [COLUMN_NAME] != 'ObsoletedDate'
", table, tableSuffix);

                var reader = getColumnNamesCommand.ExecuteReader();

                while (reader.Read())
                {
                    columnsNames.Add(Convert.ToString(reader[0]));
                }

                reader.Close();

                var columns = string.Join(@",
	", columnsNames.Select(x => string.Format("[{0}]", x)).ToArray());

                var createIndexCommand = connection.CreateCommand();

                createIndexCommand.Transaction = transaction;
                createIndexCommand.CommandText = string.Format(@"
CREATE NONCLUSTERED INDEX [IX_{0}_ObsoletedDate] ON [dbo].[{0}{1}]
(
    [ObsoletedDate] ASC
)
INCLUDE
(
    {2}
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]",
table, tableSuffix, columns);
            });
        }
    }
}
