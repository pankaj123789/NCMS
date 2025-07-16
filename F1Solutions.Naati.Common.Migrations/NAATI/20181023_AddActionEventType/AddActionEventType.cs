
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181023_AddActionEventType
{
    [NaatiMigration(201810231702)]
    public class AddActionEventType: NaatiMigration
    {
        public override void Up()
        {
            CreateActionEventType();
            CreateActionEventTypeColumn();

            CreateEmailTemplateDetailType();
            CreateCredentialWorkflowActionEmailTemplateDetail();

            RemoveInvoiceColumn();
            RemoveApplicantSponsorAndInvoiceColumns();
            CreateCredentialWorkflowActionEmailTemplateRecords();
        }

        private void CreateActionEventType()
        {
            Create.Table("tblCredentialWorkFlowActionEventType")
                .WithColumn("CredentialWorkFlowActionEventTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50)
                .WithColumn("DisplayName").AsString(50);

            Insert.IntoTable("tblCredentialWorkFlowActionEventType").Row(new {Name = "None", DisplayName = "None"});


        }

        private void CreateActionEventTypeColumn()
        {
            Alter.Table("tblCredentialWorkflowActionEmailTemplate").AddColumn("CredentialWorkFlowActionEventTypeId").AsInt32().Nullable();
            Update.Table("tblCredentialWorkflowActionEmailTemplate").Set(new { CredentialWorkFlowActionEventTypeId = "1"}).AllRows();
            Alter.Column("CredentialWorkFlowActionEventTypeId").OnTable("tblCredentialWorkflowActionEmailTemplate").AsInt32().NotNullable();

            Alter.Table("tblCredentialWorkflowActionEmailTemplate").AddColumn("Active").AsBoolean().Nullable();
            Update.Table("tblCredentialWorkflowActionEmailTemplate").Set(new { Active = "1" }).AllRows();
            Alter.Column("Active").OnTable("tblCredentialWorkflowActionEmailTemplate").AsBoolean().NotNullable();


            Create.ForeignKey("FK_CredentialWorkflowActionEmailTemplate_CredentialWorkFlowActionEventType")
                    .FromTable("tblCredentialWorkflowActionEmailTemplate")
                    .ForeignColumn("CredentialWorkFlowActionEventTypeId")
                    .ToTable("tblCredentialWorkFlowActionEventType")
                    .PrimaryColumn("CredentialWorkFlowActionEventTypeId");

            Delete.UniqueConstraint("U_tblCredentialWorkflowActionEmailTemplate")
                .FromTable("tblCredentialWorkflowActionEmailTemplate");

            Execute.Sql(@"
               ALTER TABLE [dbo].[tblCredentialWorkflowActionEmailTemplate] 
               ADD  CONSTRAINT [U_tblCredentialWorkflowActionEmailTemplate]
                UNIQUE NONCLUSTERED ([CredentialApplicationTypeId] , [CredentialWorkflowActionTypeId], [EmailTemplateId], [CredentialWorkFlowActionEventTypeId])");

        }
       

        private void RemoveInvoiceColumn()
        {
            Delete.Column("Invoice").FromTable("tblCredentialWorkflowActionEmailTemplate");
        }

        private void CreateEmailTemplateDetailType()
        {
            Create.Table("tblEmailTemplateDetailType")
                .WithColumn("EmailTemplateDetailTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50)
                .WithColumn("DisplayName").AsString(50);
        }

        public void CreateCredentialWorkflowActionEmailTemplateDetail()
        {
            Create.Table("tblCredentialWorkflowActionEmailTemplateDetail")
                .WithColumn("CredentialWorkflowActionEmailTemplateDetailId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialWorkflowActionEmailTemplateId").AsInt32()
                .WithColumn("EmailTemplateDetailTypeId").AsInt32();

            Create.ForeignKey("FK_CredentialWorkflowActionEmailTemplateDetail_CredentialWorkflowActionEmailTemplate")
                .FromTable("tblCredentialWorkflowActionEmailTemplateDetail")
                .ForeignColumn("CredentialWorkflowActionEmailTemplateId")
                .ToTable("tblCredentialWorkflowActionEmailTemplate")
                .PrimaryColumn("CredentialWorkflowActionEmailTemplateId");

            Create.ForeignKey("FK_CredentialWorkflowActionEmailTemplateDetail_EmailTemplateDetailType")
                .FromTable("tblCredentialWorkflowActionEmailTemplateDetail")
                .ForeignColumn("EmailTemplateDetailTypeId")
                .ToTable("tblEmailTemplateDetailType")
                .PrimaryColumn("EmailTemplateDetailTypeId");

            
            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialWorkflowActionEmailTemplateDetail] 
                ADD  CONSTRAINT [U_tblCredentialWorkflowActionEmailTemplateDetail]
                UNIQUE NONCLUSTERED ([CredentialWorkflowActionEmailTemplateId] , [EmailTemplateDetailTypeId])");
        }

        private void RemoveApplicantSponsorAndInvoiceColumns()
        {
            Delete.Column("Applicant").FromTable("tblEmailTemplate");
            Delete.Column("Sponsor").FromTable("tblEmailTemplate");
            Delete.Column("Invoice").FromTable("tblEmailTemplate");
        }

        private void CreateCredentialWorkflowActionEmailTemplateRecords()
        {
            Delete.FromTable("tblCredentialWorkflowActionEmailTemplate").AllRows();
        }
    }
}
