
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180409_AddSupplmentartTestColumn
{
    [NaatiMigration(201804091300)]
    public class AddSupplementarytTestColumn : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Supplementary").OnTable("TBLCREDENTIALREQUEST").AsBoolean().WithDefaultValue(0);
            Create.Column("Supplementary").OnTable("TBLTESTSITTING").AsBoolean().WithDefaultValue(0);

            Create.Table("tblTestComponentResultType").WithColumn("TestComponentResultTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);

            // Adding default value, this will be complted  in post migration script

            Execute.Sql("SET IDENTITY_INSERT[dbo].[tblTestComponentResultType] ON ");
            Insert.IntoTable("tblTestComponentResultType").Row(new { TestComponentResultTypeId = 1, Name = "Undefined", DisplayName = "Undefined" });
            Execute.Sql("SET IDENTITY_INSERT[dbo].[tblTestComponentResultType] OFF ");

            Create.Column("TestComponentResultTypeId").OnTable("tblTestComponentResult").AsInt32().WithDefaultValue(1);

            Create.ForeignKey("FK_TestComponentResult_TestComponentResultType]")
                .FromTable("tblTestComponentResult")
                .ForeignColumn("TestComponentResultTypeId")
                .ToTable("tblTestComponentResultType")
                .PrimaryColumn("TestComponentResultTypeId");
        }
    }
}
