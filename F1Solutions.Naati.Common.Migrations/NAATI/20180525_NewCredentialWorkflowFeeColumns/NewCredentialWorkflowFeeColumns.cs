
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180525_NewCredentialWorkflowFeeColumns
{
    [NaatiMigration(201805251200)]
    public class NewCredentialWorkflowFeeColumns : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("OnInvoiceCreatedCredentialWorkflowActionTypeId")
                .OnTable("tblCredentialWorkflowFee")
                .AsInt32()
                .Nullable();

            Create.Column("InvoiceActionProcessedDate")
                .OnTable("tblCredentialWorkflowFee")
                .AsDateTime()
                .Nullable();

            Create.Column("InvoiceId")
                .OnTable("tblCredentialWorkflowFee")
                .AsGuid()
                .Nullable();

            Create.Column("PaymentReference")
                .OnTable("tblCredentialWorkflowFee")
                .AsString(40)
                .Nullable();

            Delete.ForeignKey("FK_CredentialWorkflowFee_CredentialWorkflowActionType")
                .OnTable("tblCredentialWorkflowFee");

            Rename.Column("OnPaymentCredentialWorkflowActionTypeId")
                .OnTable("tblCredentialWorkflowFee")
                .To("OnPaymentCreatedCredentialWorkflowActionTypeId");

            Rename.Column("ProcessedDate")
                .OnTable("tblCredentialWorkflowFee")
                .To("PaymentActionProcessedDate");

            Create.ForeignKey("FK_CredentialWorkflowFee_CredentialWorkflowActionType_1")
                .FromTable("tblCredentialWorkflowFee")
                .ForeignColumn("OnPaymentCreatedCredentialWorkflowActionTypeId")
                .ToTable("tblCredentialWorkflowActionType")
                .PrimaryColumn("CredentialWorkflowActionTypeId");

            Create.ForeignKey("FK_CredentialWorkflowFee_CredentialWorkflowActionType_2")
                .FromTable("tblCredentialWorkflowFee")
                .ForeignColumn("OnInvoiceCreatedCredentialWorkflowActionTypeId")
                .ToTable("tblCredentialWorkflowActionType")
                .PrimaryColumn("CredentialWorkflowActionTypeId");

            Execute.Sql("ALTER TABLE tblCredentialWorkflowFee ALTER COLUMN InvoiceNumber nvarchar(20) NULL");
        }
    }
}
