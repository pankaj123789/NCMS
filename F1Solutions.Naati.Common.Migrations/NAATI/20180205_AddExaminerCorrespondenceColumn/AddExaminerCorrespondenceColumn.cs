
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180205_AddExaminerCorrespondenceColumn
{
    [NaatiMigration(201802051200)]
    public class AddExaminerColumn : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("ExaminerCorrespondence").OnTable("tblPhone").AsBoolean().NotNullable().WithDefaultValue(0);
            Create.Column("ExaminerCorrespondence").OnTable("tblAddress").AsBoolean().NotNullable().WithDefaultValue(0);
        }
    }
}
