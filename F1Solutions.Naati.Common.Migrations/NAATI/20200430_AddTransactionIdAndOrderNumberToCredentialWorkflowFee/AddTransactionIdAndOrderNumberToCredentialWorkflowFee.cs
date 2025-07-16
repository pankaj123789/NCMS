namespace F1Solutions.Naati.Common.Migrations.NAATI._20200430_AddTransactionIdAndOrderNumberToCredentialWorkflowFee
{
    [NaatiMigration(202004301616)]
    public class AddTransactionIdAndOrderNumberToCredentialWorkflowFee : NaatiMigration
    {
        public override void Up()
        {
            this.ExecuteSql(Sql.AddTransactionIdAndOrderNumberToCredentialWorkflowFee);
        }
    }
}
