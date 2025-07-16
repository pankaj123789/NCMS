
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180212_AddExaminerPanelsToSystemValue
{
    [NaatiMigration(201802121400)]
    public class AddExaminerPanelsToSystemValue : NaatiMigration
    {
        public override void Up()
        {
            Insert.IntoTable("tblSystemValue").Row(new
            {
                ValueKey = "ExaminerPanelTypes",
                Value = 1
            });
        }
    }
}
