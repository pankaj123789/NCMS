
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171208_AddCredentialWorkflowFeeTable
{
    [NaatiMigration(201712081000)]
    public class AddCredentialWorkflowFeeTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblCredentialWorkflowFee")
                .WithColumn("CredentialWorkflowFeeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationId").AsInt32().NotNullable()
                .WithColumn("CredentialRequestId ").AsInt32().Nullable()
                .WithColumn("InvoiceNumber").AsAnsiString(20).NotNullable()
                .WithColumn("ProductSpecificationId").AsInt32().NotNullable()
                .WithColumn("OnPaymentCredentialWorkflowActionTypeId").AsInt32().Nullable()
                .WithColumn("ProcessedDate").AsDateTime().Nullable();

            Create.ForeignKey("FK_CredentialWorkflowFee_CredentialApplication")
                .FromTable("tblCredentialWorkflowFee")
                .ForeignColumn("CredentialApplicationId")
                .ToTable("tblCredentialApplication")
                .PrimaryColumn("CredentialApplicationId");

            Create.ForeignKey("FK_CredentialWorkflowFee_CredentialRequest")
                .FromTable("tblCredentialWorkflowFee")
                .ForeignColumn("CredentialRequestId")
                .ToTable("tblCredentialRequest")
                .PrimaryColumn("CredentialRequestId");

            Create.ForeignKey("FK_CredentialWorkflowFee_ProductSpecification")
                .FromTable("tblCredentialWorkflowFee")
                .ForeignColumn("ProductSpecificationId")
                .ToTable("tblProductSpecification")
                .PrimaryColumn("ProductSpecificationId");

            Create.ForeignKey("FK_CredentialWorkflowFee_CredentialWorkflowActionType")
                .FromTable("tblCredentialWorkflowFee")
                .ForeignColumn("OnPaymentCredentialWorkflowActionTypeId")
                .ToTable("tblCredentialWorkflowActionType")
                .PrimaryColumn("CredentialWorkflowActionTypeId");
        }
    }
}
