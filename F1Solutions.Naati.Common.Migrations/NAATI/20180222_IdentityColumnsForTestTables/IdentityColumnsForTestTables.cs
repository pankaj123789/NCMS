
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180222_IdentityColumnsForTestTables
{
    [NaatiMigration(201802221500)]
    public class IdentityColumnsForTestTables : NaatiMigration
    {
        public override void Up()
        {
            /* updates the PK column to Identity(1,1) and removes the RowVersion column on the following tables:
             * tblJob
             * tblJobExaminer
             * tblTestResult
             * tblTestComponent              
             * tblTestComponentResult
             * tblExaminerMarking
             * tblExaminerTestComponentResult
             * tblTestSpecification
             */
            Execute.Sql(Sql.IdentityColumnsForTestTables);
        }
    }
}
