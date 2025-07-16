
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180615_ExaminerToolsDownloadColumn
{
    [NaatiMigration(201806151200)]
    public class AddExaminerToolsDownloadColumn : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("ExaminerToolsDownload").OnTable("tblTestSpecificationAttachment").AsBoolean().NotNullable().WithDefaultValue(0);
        }
    }
}
