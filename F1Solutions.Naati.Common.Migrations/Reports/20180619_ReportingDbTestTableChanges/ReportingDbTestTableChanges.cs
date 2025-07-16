using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20180619_ReportingDbTestTableChanges
{
    [NaatiMigration(201806191045)]
    public class ReportingDbTestTableChanges : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("CredentialTypeInternalName").OnTable("TestHistory").AsString(50).Nullable();
            Create.Column("CredentialTypeExternalName").OnTable("TestHistory").AsString(50).Nullable();
            Create.Column("Skill").OnTable("TestHistory").AsString(100).Nullable();
            Create.Column("ApplicationType").OnTable("TestHistory").AsString(50).Nullable();
            Create.Column("SupplementaryTest").OnTable("TestHistory").AsBoolean().NotNullable().WithDefaultValue(0);
        }
    }
}
