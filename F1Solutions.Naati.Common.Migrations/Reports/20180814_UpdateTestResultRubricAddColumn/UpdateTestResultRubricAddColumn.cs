using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20180814_UpdateTestResultRubricAddColumn
{
    [NaatiMigration(201808141400)]
    public class UpdateTestResultRubricAddColumn : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("CredentialTypeInternalName").OnTable("TestResultRubricHistory").AsString(100).Nullable();
            Create.Column("CredentialTypeExternalName").OnTable("TestResultRubricHistory").AsString(100).Nullable();
        }
    }
}
