namespace F1Solutions.Naati.Common.Migrations.NAATI._20200422_AddCredentialApplicationRefundTable
{
    [NaatiMigration(202004221031)]
    public class AddCredentialApplicationRefundTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblCredentialApplicationRefund")
                .WithColumn("CredentialApplicationRefundId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialWorkflowFeeId").AsInt32().NotNullable()
                    .ForeignKey("FK_CredentialApplicationRefund_CredentialWorkflowFee", "tblCredentialWorkflowFee",
                    "CredentialWorkflowFeeId")
                .WithColumn("CreditNoteNumber").AsString().Nullable()
                .WithColumn("CreditNoteId").AsGuid().Nullable()
                .WithColumn("PaymentReference").AsString().Nullable()
                .WithColumn("InitialPaidAmount").AsCurrency().Nullable()
                .WithColumn("RefundAmount").AsCurrency().Nullable()
                .WithColumn("RefundPercentage").AsDouble().Nullable()
                .WithColumn("RefundTransactionId").AsString(50).Nullable()
                .WithColumn("RefundMethodTypeId").AsInt32().NotNullable()
                    .ForeignKey("FK_CredentialApplicationRefund_RefundMethodType", "tblRefundMethodType",
                    "RefundMethodTypeId")
                .WithColumn("UserId").AsInt32().NotNullable()
                    .ForeignKey("FK_CredentialApplicationRefund_User", "tblUser", "UserId")
                .WithColumn("CreatedDate").AsDateTime().NotNullable()
                .WithColumn("RefundedDate").AsDateTime().Nullable()
                .WithColumn("CreditNoteProcessedDate").AsDateTime().Nullable()
                .WithColumn("CreditNotePaymentProcessedDate").AsDateTime().Nullable()
                .WithColumn("OnCreditNoteCreatedSystemActionTypeId").AsInt32().Nullable()
                    .ForeignKey("FK_CredentialApplicationRefund_SystemActionTypeOnCreditNote", "tblSystemActionType",
                    "SystemActionTypeId")
                .WithColumn("OnPaymentCreatedSystemActionTypeId").AsInt32().Nullable()
                    .ForeignKey("FK_CredentialApplicationRefund_SystemActionTypeOnPaymentCreated", "tblSystemActionType",
                    "SystemActionTypeId")
                .WithColumn("DisallowProcessing").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("IsRejected").AsBoolean().NotNullable();
        }
    }
}
