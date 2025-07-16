using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160816_UpdatesForNewETL
{
    [NaatiMigration(201608160920)]
    public class AddModifiedAndDeletedDateColumns : NaatiMigration
    {
        public override void Up()
        {
            var tableNames = new []
            {
                "Accreditation",
                "Application",
                "Invoice",
                "Person",
                "Revalidation",
                "Test"
            };

            foreach (var tableName in tableNames)
            {
                Execute.Sql(string.Format("sp_rename '{0}.DateRecorded', 'ModifiedDate', 'COLUMN'", tableName));
                Execute.Sql(string.Format("ALTER TABLE [{0}] ADD [DeletedDate] DATETIME NULL", tableName));
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
