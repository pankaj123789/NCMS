using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20221213_AddRMACIdToTestResultExaminerMarkingTbl
{
    [NaatiMigration(202212131115)]
    public class AddRMACIdToTestResultExaminerMarkingTbl : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddRMACIdToTestResultExaminerMarkingTbl);
        }
    }
}
