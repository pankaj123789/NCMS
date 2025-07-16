
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180702_AddTaskLabelColumnToTestComponent
{
    [NaatiMigration(201807021340)]
    public class AddTaskLabelColumnToTestComponent : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("TaskLabel").OnTable("tblTestComponent").AsInt32().Nullable();
        }        
    }
}
