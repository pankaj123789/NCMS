
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180625_AddMarkingResultTypeToRubric
{
    [NaatiMigration(201806021101)]
    public class AddMarkingResultTypeToRubric:NaatiMigration
    {
        public override void Up()
        {
            RenameTestComponentResultType();
            RenameTestComponentResultTypeOnTestComponentResult();
            AddMarkingResultTypeToRubricResult();
        }

        void RenameTestComponentResultType()
        {
            Execute.Sql("ALTER TABLE tblTestComponentResult DROP CONSTRAINT [FK_TestComponentResult_TestComponentResultType]]]");
            Rename.Table("tblTestComponentResultType").InSchema("dbo").To("tblMarkingResultType");
            Rename.Column("TestComponentResultTypeId").OnTable("tblMarkingResultType").To("MarkingResultTypeId");
        }

        void RenameTestComponentResultTypeOnTestComponentResult()
        {
            Rename.Column("TestComponentResultTypeId").OnTable("tblTestComponentResult").To("MarkingResultTypeId");

            Create.ForeignKey("FK_TestComponentResult_MarkingResultType]")
                .FromTable("tblTestComponentResult")
                .ForeignColumn("MarkingResultTypeId")
                .ToTable("tblMarkingResultType")
                .PrimaryColumn("MarkingResultTypeId");
        }

        void AddMarkingResultTypeToRubricResult()
        {
            Alter.Table("tblRubricTestComponentResult").AddColumn("MarkingResultTypeId").AsInt32().Nullable();
            Update.Table("tblRubricTestComponentResult").InSchema("dbo").Set(new { MarkingResultTypeId = 1}).AllRows();
            Alter.Column("MarkingResultTypeId").OnTable("tblRubricTestComponentResult").AsInt32().NotNullable();

            Create.ForeignKey("FK_RubricTestComponentResult_MarkingResultType]")
                .FromTable("tblRubricTestComponentResult")
                .ForeignColumn("MarkingResultTypeId")
                .ToTable("tblMarkingResultType")
                .PrimaryColumn("MarkingResultTypeId");

        }
    }
}
