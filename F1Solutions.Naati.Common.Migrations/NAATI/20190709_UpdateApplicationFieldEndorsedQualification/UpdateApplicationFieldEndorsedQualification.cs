
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190709_UpdateApplicationFieldEndorsedQualification
{
    [NaatiMigration(201907090710)]
    public class UpdateApplicationFieldEndorsedQualification : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Disabled").OnTable("tblCredentialApplicationField").AsBoolean().Nullable();
            Execute.Sql("update tblCredentialApplicationField set Disabled = 0");
            Alter.Column("Disabled").OnTable("tblCredentialApplicationField").AsBoolean().NotNullable();
        }
    }
}
