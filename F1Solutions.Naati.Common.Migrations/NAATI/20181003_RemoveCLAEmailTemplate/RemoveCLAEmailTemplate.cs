
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181003_RemoveCLAEmailTemplate
{
    [NaatiMigration(201803101130)]
    public class RemoveCLAEmailTemplate : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("DELETE FROM tblCredentialWorkflowActionEmailTemplate where EmailTemplateId in (169, 170 ,171, 180,181, 182)");
            Execute.Sql("DELETE FROM tblEmailTemplate where EmailTemplateId in (169, 170 ,171, 180,181, 182)");
        }
    }
}

