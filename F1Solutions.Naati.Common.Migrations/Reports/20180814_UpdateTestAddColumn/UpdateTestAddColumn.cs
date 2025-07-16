using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20180814_UpdateTestAddColumn
{
    [NaatiMigration(201808141030)]
    public class UpdateTestAddColumn : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("CredentialTypeInternalName").OnTable("TestResultHistory").AsString(100).Nullable();
            Create.Column("CredentialTypeExternalName").OnTable("TestResultHistory").AsString(100).Nullable();
        }
    }
}
