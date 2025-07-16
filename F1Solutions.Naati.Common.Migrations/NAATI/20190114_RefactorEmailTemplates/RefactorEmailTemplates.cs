
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190114_RefactorEmailTemplates
{
   [NaatiMigration(201901141135)]
    public class RefactorEmailTemplates : NaatiMigration
    {
        public override void Up()
        {
            RemoveForeignKeys();
            RenameTables();
            AddForeignKeys();

        }

        private void RemoveForeignKeys()
        {
            Delete.ForeignKey("FK_CredentialWorkflowActionEmailTemplate_CredentialApplicationType")
                .OnTable("tblCredentialWorkflowActionEmailTemplate");
       

            Delete.UniqueConstraint("U_tblCredentialWorkflowActionEmailTemplate").FromTable("tblCredentialWorkflowActionEmailTemplate");

            Delete.UniqueConstraint("U_tblCredentialWorkflowActionEmailTemplateDetail").FromTable("tblCredentialWorkflowActionEmailTemplateDetail");

        }

        private void RenameTables()
        {
            // Rename workflow email template
            Rename.Table("tblCredentialWorkflowActionEmailTemplate").To("tblSystemActionEmailTemplate");
            Execute.Sql("sp_rename @objname = N'[tblSystemActionEmailTemplate].[PK_tblCredentialWorkflowActionEmailTemplate]', @newname = N'PK_SystemActionEmailTemplate'");
            Rename.Column("CredentialWorkflowActionEmailTemplateId").OnTable("tblSystemActionEmailTemplate").To("SystemActionEmailTemplateId");
            Rename.Column("CredentialWorkflowActionTypeId").OnTable("tblSystemActionEmailTemplate").To("SystemActionTypeId");
            Rename.Column("CredentialWorkFlowActionEventTypeId").OnTable("tblSystemActionEmailTemplate").To("SystemActionEventTypeId");
            Execute.Sql("EXEC  sp_rename @objname = N'[dbo].[FK_CredentialWorkflowActionEmailTemplate_CredentialWorkFlowActionEventType]', @newname = N'FK_SystemActionEmailTemplate_SystemActionEventType', @objtype = 'object'");
            Execute.Sql("EXEC sp_rename @objname = N'[dbo].[FK_CredentialWorkflowActionEmailTemplate_EmailTemplate]', @newname = N'FK_SystemActionEmailTemplate_EmailTemplate', @objtype = 'object'");
            Execute.Sql("EXEC sp_rename @objname = N'[dbo].[FK_CredentialWorkflowActionEmailTemplate_User]', @newname = N'FK_SystemActionEmailTemplate_User', @objtype = 'object'");
            Execute.Sql("EXEC sp_rename @objname = N'[dbo].[FK_CredentialWorkflowActionEmailTemplate_CredentialWorkflowActionType]', @newname = N'FK_SystemActionType', @objtype = 'object'");

            //Create new workflowActionEmailTemplate
            Create.Table("tblCredentialWorkflowActionEmailTemplate")
                .WithColumn("CredentialWorkflowActionEmailTemplateId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationTypeId").AsInt32()
                .WithColumn("SystemActionEmailTemplateId").AsInt32();
 

            // Remove colums from tblSystemActionEmailTemplate
            Delete.Column("CredentialApplicationTypeId").FromTable("tblSystemActionEmailTemplate");
         
            

            // Rename tblCredentialWorkflowActionType
            Rename.Table("tblCredentialWorkflowActionType").To("tblSystemActionType");
            Rename.Column("CredentialWorkflowActionTypeId").OnTable("tblSystemActionType").To("SystemActionTypeId");
            Execute.Sql("sp_rename @objname = N'[tblSystemActionType].[PK_tblCredentialWorkflowActionType]', @newname = N'PK_tblSystemActionType'");
            Execute.Sql("EXEC sp_rename @objname = N'[DBO].[FK_CredentialWorkflowActionType_User]', @newname = N'FK_SystemActionType_User'");

            //Rename Columns on tblCredentialWorkflowFee
            Rename.Column("OnPaymentCreatedCredentialWorkflowActionTypeId").OnTable("tblCredentialWorkflowFee").To("OnPaymentCreatedSystemActionTypeId");
            Rename.Column("OnInvoiceCreatedCredentialWorkflowActionTypeId").OnTable("tblCredentialWorkflowFee").To("OnInvoiceCreatedSystemActionTypeId");


            // Rename EmailTemplateDetails

            Rename.Table("tblCredentialWorkflowActionEmailTemplateDetail").To("tblSystemActionEmailTemplateDetail");
            Rename.Column("CredentialWorkflowActionEmailTemplateDetailId").OnTable("tblSystemActionEmailTemplateDetail").To("SystemActionEmailTemplateDetailId");
            Rename.Column("CredentialWorkflowActionEmailTemplateId").OnTable("tblSystemActionEmailTemplateDetail").To("SystemActionEmailTemplateId");
            Execute.Sql("sp_rename @objname = N'[tblSystemActionEmailTemplateDetail].[PK_tblCredentialWorkflowActionEmailTemplateDetail]', @newname = N'PK_tblSystemActionEmailTemplateDetail'");
            Execute.Sql("EXEC sp_rename @objname = N'[dbo].[FK_CredentialWorkflowActionEmailTemplateDetail_CredentialWorkflowActionEmailTemplate]', @newname = N'FK_SystemActionEmailTemplateDetail_SystemActionActionEmailTemplate', @objtype = 'object'");
            Execute.Sql("EXEC sp_rename @objname = N'[dbo].[FK_CredentialWorkflowActionEmailTemplateDetail_EmailTemplateDetailType]', @newname = N'FK_SystemActionEmailTemplateDetail_EmailTemplateDetailType', @objtype = 'object'");
            Execute.Sql("EXEC sp_rename @objname = N'[dbo].[FK_CredentialWorkflowActionEmailTemplateDetail_User]', @newname = N'FK_SystemActionEmailTemplateDetail_User', @objtype = 'object'");


            // Rename tblCredentialWorkFlowActionEventType

            Rename.Table("tblCredentialWorkFlowActionEventType").To("tblSystemActionEventType");
            Rename.Column("CredentialWorkFlowActionEventTypeId").OnTable("tblSystemActionEventType").To("SystemActionEventTypeId");
            Execute.Sql("sp_rename @objname = N'[tblSystemActionEventType].[PK_tblCredentialWorkFlowActionEventType]', @newname = N'PK_tblSystemActionEventType'");
        }

        private void AddForeignKeys()
        {
            Create.ForeignKey("FK_CredentialWorkflowActionEmailTemplate_CredentialApplicationType")
                .FromTable("tblCredentialWorkflowActionEmailTemplate")
                .ForeignColumn("CredentialApplicationTypeId")
                .ToTable("tblCredentialApplicationType")
                .PrimaryColumn("CredentialApplicationTypeId");


            Create.ForeignKey("FK_CredentialWorkflowActionEmailTemplate_SystemActionEmailTemplate")
                .FromTable("tblCredentialWorkflowActionEmailTemplate")
                .ForeignColumn("SystemActionEmailTemplateId")
                .ToTable("tblSystemActionEmailTemplate")
                .PrimaryColumn("SystemActionEmailTemplateId");


            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialWorkflowActionEmailTemplate] 
                ADD  CONSTRAINT [U_tblCredentialWorkflowActionEmailTemplate]
                UNIQUE NONCLUSTERED ([CredentialApplicationTypeId] , [SystemActionEmailTemplateId])");
            

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblSystemActionEmailTemplateDetail] 
                ADD  CONSTRAINT [U_tblSystemActionEmailTemplateDetail]
                UNIQUE NONCLUSTERED ([SystemActionEmailTemplateId] , [EmailTemplateDetailTypeId])");

        }
    }
}
