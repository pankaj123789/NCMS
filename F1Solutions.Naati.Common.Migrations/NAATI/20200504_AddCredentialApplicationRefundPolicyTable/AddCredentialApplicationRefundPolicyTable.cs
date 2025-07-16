using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20200504_AddCredentialApplicationRefundPolicyTable
{
    [NaatiMigration(202005041624)]
    public class AddCredentialApplicationRefundPolicyTable : NaatiMigration
    {
        public override void Up()
        {
            CreateRefundPolicyTable();
            PopulateRefunPolicyTable();
        }

        private void CreateRefundPolicyTable()
        {
            Create.Table("tblCredentialApplicationRefundPolicy")
                .WithColumn("CredentialApplicationRefundPolicyId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Description").AsString(int.MaxValue).NotNullable();
        }
        private void PopulateRefunPolicyTable()
        {
            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationRefundPolicy ON ");
            Insert.IntoTable("tblCredentialApplicationRefundPolicy")
                .Row(new { CredentialApplicationRefundPolicyId = 1, Description = "" })
                .Row(new { CredentialApplicationRefundPolicyId = 2, Description = "" })
                .Row(new { CredentialApplicationRefundPolicyId = 3, Description = "" });

            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationRefundPolicy OFF ");
        }
    }
}
