using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20191203_AddJobExecutionEntity
{
    [NaatiMigration(201912031617)]
    public class AddJobExecutionEntity : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblJobExecutionLogType")
                .WithColumn("JobExecutionLogTypeId").AsInt32().PrimaryKey()
                .WithColumn("DisplayName").AsString(50);

            Insert.IntoTable("tblJobExecutionLogType").Row(new
            {
                JobExecutionLogTypeId = 1,
                DisplayName = "Information"
            });
            Insert.IntoTable("tblJobExecutionLogType").Row(new
            {
                JobExecutionLogTypeId = 2,
                DisplayName = "Warning"
            });
            Insert.IntoTable("tblJobExecutionLogType").Row(new
            {
                JobExecutionLogTypeId = 3,
                DisplayName = "Error"
            });

            Create.Table("tblJobExecutionLog")
                .WithColumn("JobExecutionLogId").AsInt32().PrimaryKey().Identity()
                .WithColumn("LogDate").AsDateTime()
                .WithColumn("EntityName").AsString(200)
                .WithColumn("Message").AsString(int.MaxValue)
                .WithColumn("SyncDate").AsDateTime().Nullable()
                .WithColumn("JobExecutionLogTypeId").AsInt32().ForeignKey("FK_JobExecutionLog_JobExecutionLogType", "tblJobExecutionLogType", "JobExecutionLogTypeId");
        }
    }
}
