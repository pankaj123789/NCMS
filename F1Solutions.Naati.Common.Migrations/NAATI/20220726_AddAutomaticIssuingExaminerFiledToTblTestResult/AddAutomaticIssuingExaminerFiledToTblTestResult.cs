namespace F1Solutions.Naati.Common.Migrations.NAATI._20220726_AddAutomaticIssuingExaminerFiledToTblTestResult
{
    [NaatiMigration(202207261050)]
    public class AddAutomaticIssuingExaminerFiledToTblTestResult : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.AddAutomaticIssuingExaminerFiledToTblTestResult);
        }
    }
}
