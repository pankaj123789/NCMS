
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180702_UpdateTaskLabelColumnToLabel
{
    [NaatiMigration(201807031710)]
    public class UpdateTaskLabelColumnToLabel : NaatiMigration
    {
        public override void Up()
        {            
            Delete.Column("TaskLabel").FromTable("tblTestComponent");
            Create.Column("Label").OnTable("tblTestComponent").AsString(50).Nullable();            
        }
    }
}
