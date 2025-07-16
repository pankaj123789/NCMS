
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180219_AddTestSittingTestMaterailTable
{
    [NaatiMigration(201802191430)]
    public class AddTestSittingTestMaterial : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblTestSittingTestMaterial")
                .WithColumn("TestSittingTestMaterialId").AsInt32().Identity().PrimaryKey()
                .WithColumn("TestSittingId").AsInt32()
                .WithColumn("TestMaterialId").AsInt32();
        }
    }
}
