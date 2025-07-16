
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171130_IssueCredentialPeriods
{
    [NaatiMigration(201711301451)]
    public class IssueCredentialPeriods : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("DefaultExpiry").OnTable("tblCredentialType").AsInt32().Nullable();

            Execute.Sql("SET IDENTITY_INSERT tblSystemValue ON");
            Insert.IntoTable("tblSystemValue").Row(new
            {
                SystemValueId = 47,
                ValueKey = "CertificationPeriodNextPeriod",
                Value = 12
            });

            Insert.IntoTable("tblSystemValue").Row(new
            {
                SystemValueId = 48,
                ValueKey = "CertificationPeriodDefaultDuration",
                Value = 3
            });

            Insert.IntoTable("tblSystemValue").Row(new
            {
                SystemValueId = 49,
                ValueKey = "CertificationPeriodRecertify",
                Value = 3
            });

            Execute.Sql("SET IDENTITY_INSERT tblSystemValue OFF");
        }
    }
}
