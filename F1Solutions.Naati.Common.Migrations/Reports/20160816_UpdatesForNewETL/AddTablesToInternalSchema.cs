using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160816_UpdatesForNewETL
{
    [NaatiMigration(201608160921)]
    public class AddTablesToInternalSchema : NaatiMigration
    {
        public override void Up()
        {
            var tableNames = new[]
            {
                "Accreditation",
                "Application",
                "Invoice",
                "Person",
                "Revalidation",
                "Test"
            };

            Execute.Sql("CREATE SCHEMA Internal");

            foreach (var tableName in tableNames)
            {
                Execute.Sql(string.Format("ALTER SCHEMA Internal TRANSFER dbo.{0};", tableName));
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
