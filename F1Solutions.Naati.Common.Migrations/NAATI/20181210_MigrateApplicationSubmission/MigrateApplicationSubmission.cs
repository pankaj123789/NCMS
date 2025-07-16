
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181210_MigrateApplicationSubmission
{
    [NaatiMigration(201812101404)]
    public class MigrateApplicationSubmission : NaatiMigration
    {
        public override void Up()
        {
            Update.Table("tblCredentialWorkflowFee").Set(new { OnPaymentCreatedCredentialWorkflowActionTypeId =12 }).Where(new { OnPaymentCreatedCredentialWorkflowActionTypeId = 13 });
        }
    }
}
