
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181121_AddMinCommentLengthColumns
{
   [NaatiMigration(201811211107)]
    public class AddMinCommentLengthColumns:NaatiMigration
    {
        public override void Up()
        {
            Create.Column("MinNaatiCommentLength").OnTable("tblTestComponentType").AsInt32().WithDefaultValue(0);
            Create.Column("MinExaminerCommentLength").OnTable("tblTestComponentType").AsInt32().WithDefaultValue(0);
        }
    }
}
