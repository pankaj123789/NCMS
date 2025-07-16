
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180516_AddHangFireFlag
{
    [NaatiMigration(201805161200)]
    public class AddHangFireFlag:NaatiMigration
    {
        public override void Up()
        {
            Insert.IntoTable("tblSystemValue").Row(new {ValueKey = "ProcessingPendingAccountingOperations", Value = "0"});
            Insert.IntoTable("tblSystemValue").Row(new {ValueKey = "ProcessingPendingApplications", Value = "0"});
        }
    }
}
