namespace F1Solutions.Naati.Common.Migrations.NAATI._20200602_AddRefundPolicyParameterTable
{
    [NaatiMigration(202006021014)]
    public class AddRefundPolicyParameterTable : NaatiMigration
    {
        public override void Up()
        {
            AlterPolicyTable();
            CreateRefundPolicyParameterTable();
        }

        private void AlterPolicyTable()
        {
            Alter.Table("tblCredentialApplicationRefundPolicy").AddColumn("Name").AsString().Nullable();
            Update.Table("tblCredentialApplicationRefundPolicy").Set(new { Name = "temp" }).AllRows();
            Alter.Table("tblCredentialApplicationRefundPolicy").AlterColumn("Name").AsString().NotNullable();
        }

        private void CreateRefundPolicyParameterTable()
        {
            Create.Table("tblRefundPolicyParameter")
               .WithColumn("RefundPolicyParameterId").AsInt32().Identity().PrimaryKey()
               .WithColumn("CredentialApplicationRefundPolicyId").AsInt32().NotNullable()
                    .ForeignKey("FK_RefundPolicyParameter_CredentialApplicationRefundPolicy", "tblCredentialApplicationRefundPolicy",
                    "CredentialApplicationRefundPolicyId")
               .WithColumn("Name").AsString(50).NotNullable()
               .WithColumn("Value").AsString(50).NotNullable();
        }
    }
}
