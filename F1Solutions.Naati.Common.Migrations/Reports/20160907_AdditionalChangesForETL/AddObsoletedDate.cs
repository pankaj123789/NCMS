using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160907_AdditionalChangesForETL
{
    [NaatiMigration(201609071510)]
    public class AddObsoletedDate : NaatiMigration
    {
        public override void Up()
        {
            foreach (var table in TableInfo.Tables)
            {
                Execute.Sql(string.Format("ALTER TABLE [Internal].[{0}] ADD [ObsoletedDate] DATETIME NULL", table.TableName));
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
