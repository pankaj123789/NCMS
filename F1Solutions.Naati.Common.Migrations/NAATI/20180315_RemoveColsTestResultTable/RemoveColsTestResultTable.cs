
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180315_RemoveColsTestResultTable
{
    [NaatiMigration(201803151800)]
    public class RemoveColsTestResultTable : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"ALTER TABLE tblTestResult DROP COLUMN Comments1
                          ALTER TABLE tblTestResult DROP COLUMN Comments2
                          ALTER TABLE tblTestResult DROP COLUMN CommentsEthics");
        }
    }
}
