
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190225_AddMatchDirectionColumn
{
    [NaatiMigration(201902251004)]
    public class AddMatchDirectionColumn:NaatiMigration
    {
        public override void Up()
        {
            Create.Column("MatchDirection").OnTable("tblCredentialTypeUpgradePath").AsBoolean().Nullable();

            Update.Table("tblCredentialTypeUpgradePath").Set(new { MatchDirection = 1}).AllRows();
            Alter.Column("MatchDirection").OnTable("tblCredentialTypeUpgradePath").AsBoolean().NotNullable();
        }
    }
}
