namespace F1Solutions.Naati.Common.Migrations.NAATI._20171018_AddEmailExaminerCorrespondence
{
    [NaatiMigration(201710181436)]
    public class AddEmailExaminerCorrespondence : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("ExaminerCorrespondence").OnTable("tblEmail").AsBoolean().NotNullable().WithDefaultValue(0);
            Insert.IntoTable("tblSystemValue").Row(new
            {
                Valuekey = "ExaminerRoles",
                Value = "1,4,307,612,622"
            });
        }

    }
}
