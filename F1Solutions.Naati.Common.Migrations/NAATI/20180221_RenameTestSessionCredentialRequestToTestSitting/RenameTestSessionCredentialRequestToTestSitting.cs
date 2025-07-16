
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180221_RenameTestSessionCredentialRequestToTestSitting
{
    [NaatiMigration(201802211200)]
    public class RenameTestSessionCredentialRequestToTestSitting : NaatiMigration
    {
        public override void Up()
        {
            // this script renames 3 tables and several columns and constraints
            Execute.Sql(Sql.RenameScript);

            // also add a couple of missing constraints
            Create.ForeignKey("FK_TestSittingTestMaterial_TestSitting")
                .FromTable("tblTestSittingTestMaterial")
                .ForeignColumn("TestSittingId")
                .ToTable("tblTestSitting")
                .PrimaryColumn("TestSittingId");

            Create.ForeignKey("FK_TestSittingTestMaterial_TestMaterial")
                .FromTable("tblTestSittingTestMaterial")
                .ForeignColumn("TestMaterialId")
                .ToTable("tblTestMaterial")
                .PrimaryColumn("TestMaterialId");
        }
    }
}
