using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160907_AdditionalChangesForETL
{
    [NaatiMigration(201609071542)]
    public class ChangeTableNamesAndSchema : NaatiMigration
    {
        public override void Up()
        {
            foreach (var table in TableInfo.Tables)
            {
                Execute.Sql(string.Format("EXEC sp_rename 'Internal.{0}', '{0}History'", table.TableName));
                Execute.Sql(string.Format("ALTER SCHEMA dbo TRANSFER Internal.{0}History;", table.TableName));
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
