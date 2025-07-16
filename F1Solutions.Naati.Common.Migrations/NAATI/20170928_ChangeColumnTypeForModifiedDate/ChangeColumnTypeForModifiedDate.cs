
namespace F1Solutions.Naati.Common.Migrations.NAATI._20170928_ChangeColumnTypeForModifiedDate
{
    [NaatiMigration(201709281741)]
    public class ChangeColumnTypeForModifiedDate : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"ALTER TABLE tblNote ALTER COLUMN ModifiedDate DateTime null");
        }
    }
}
