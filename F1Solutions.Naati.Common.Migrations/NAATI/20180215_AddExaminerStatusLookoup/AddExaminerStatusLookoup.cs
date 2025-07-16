
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180215_AddExaminerStatusLookoup
{
    [NaatiMigration(201802151200)]
    public class AddExaminerStatusLookoup : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblExaminerStatusType")
                .WithColumn("ExaminerStatusTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);
        }
    }
}
