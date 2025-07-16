namespace F1Solutions.Naati.Common.Migrations.NAATI._20220726_UpdateTblTestResultAutomaticIssuingExaminer
{
    [NaatiMigration(202207261104)]
    public class UpdateTblTestResultAutomaticIssuingExaminerWhereResultTypeIdEqualsOne : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.UpdateTblTestResultAutomaticIssuingExaminer);
        }
    }
}