
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171127_ApplicationWorkflow
{
    [NaatiMigration(201711281600)]
    public class ApplicationWorkflow : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblCredentialWorkflowActionType")
                .WithColumn("CredentialWorkflowActionTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);

            Create.Table("tblCredentialWorkflowActionEmailTemplate")
                .WithColumn("CredentialWorkflowActionEmailTemplateId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationTypeId").AsInt32()
                .WithColumn("CredentialWorkflowActionTypeId").AsInt32()
                .WithColumn("EmailTemplateId").AsInt32();

            Create.ForeignKey("FK_CredentialWorkflowActionEmailTemplate_CredentialApplicationType")
                .FromTable("tblCredentialWorkflowActionEmailTemplate")
                .ForeignColumn("CredentialApplicationTypeId")
                .ToTable("tblCredentialApplicationType")
                .PrimaryColumn("CredentialApplicationTypeId");

            Create.ForeignKey("FK_CredentialWorkflowActionEmailTemplate_CredentialWorkflowActionType")
                .FromTable("tblCredentialWorkflowActionEmailTemplate")
                .ForeignColumn("CredentialWorkflowActionTypeId")
                .ToTable("tblCredentialWorkflowActionType")
                .PrimaryColumn("CredentialWorkflowActionTypeId");

            Create.ForeignKey("FK_CredentialWorkflowActionEmailTemplate_EmailTemplate")
                .FromTable("tblCredentialWorkflowActionEmailTemplate")
                .ForeignColumn("EmailTemplateId")
                .ToTable("tblEmailTemplate")
                .PrimaryColumn("EmailTemplateId");

            Execute.Sql(@"
                ALTER TABLE tblCredentialWorkflowActionEmailTemplate 
                ADD  CONSTRAINT U_tblCredentialWorkflowActionEmailTemplate
                UNIQUE NONCLUSTERED (CredentialApplicationTypeId, CredentialWorkflowActionTypeId, EmailTemplateId)");

            Execute.Sql(@"           
                SET IDENTITY_INSERT tblCredentialApplicationStatusType ON 
                INSERT INTO tblCredentialApplicationStatusType (CredentialApplicationStatusTypeId,[Name],DisplayName) 
                    VALUES( 8,'AwaitingPayment',N'Awaiting Payment' ) 
                SET IDENTITY_INSERT tblCredentialApplicationStatusType OFF 

                SET IDENTITY_INSERT tblCredentialRequestStatusType ON 
	            INSERT INTO tblCredentialRequestStatusType (CredentialRequestStatusTypeId, [Name],DisplayName) 
                    VALUES ( 15,'AwaitingTestPayment',N'Awaiting Test Payment' ), 
                           (16, 'Withdrawn',N'Withdrawn' ), 
                           (17, 'TestAccepted',N'Test Accepted' ) 
                SET IDENTITY_INSERT tblCredentialRequestStatusType OFF");
        }
    }
}
