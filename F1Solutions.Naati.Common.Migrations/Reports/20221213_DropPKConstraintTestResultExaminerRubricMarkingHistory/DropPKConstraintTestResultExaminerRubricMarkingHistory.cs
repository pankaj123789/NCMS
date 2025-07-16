using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20221213_DropPKConstraintTestResultExaminerRubricMarkingHistory
{
    [NaatiMigration(202212131300)]
    public class DropPKConstraintTestResultExaminerRubricMarkingHistory : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.DropPKConstraintTestResultExaminerRubricMarkingHistory);
        }
    }
}
