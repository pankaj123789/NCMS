
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180413_TestTasksFixing
{
    [NaatiMigration(201804131607)]
    public class TestTasksFixing : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("sp_rename 'tluTestComponentType', 'tblTestComponentType'");

            Alter.Column("LanguageId").OnTable("tblTestMaterial").AsInt32().Nullable();

            Delete.Column("BasedOn").FromTable("tblTestComponentType");

            Create.Table("tblTestComponentBaseType")
                .WithColumn("TestComponentBaseTypeId").AsInt32().Identity().PrimaryKey("PK_TestComponentBaseType")
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("DisplayName").AsString().NotNullable();

            Execute.Sql("Set IDENTITY_INSERT tblTestComponentBaseType ON ");
            Insert.IntoTable("tblTestComponentBaseType").Row(new { TestComponentBaseTypeId = "1", Name = "Default", DisplayName = "Default" });
            Execute.Sql("Set IDENTITY_INSERT tblTestComponentBaseType OFF ");

            Alter.Table("tblTestComponentType")
                .AddColumn("TestComponentBaseTypeId").AsInt32().NotNullable().WithDefaultValue(1).ForeignKey("tblTestComponentBaseType", "TestComponentBaseTypeId");
        }
    }
}
