
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180521_AddSystemValueKeysForHangFireConfig
{
    [NaatiMigration(201805211201)]
    public class AddSystemValueKeysForHangFireConfig : NaatiMigration
    {
        public override void Up()
        {
            Insert.IntoTable("tblSystemValue").Row(new { ValueKey = "ApplicationsCheckingInterval", Value = "10,30,50 * * * *" });
            Insert.IntoTable("tblSystemValue").Row(new { ValueKey = "XeroOperationsCheckingInterval", Value = "1,20,40 * * * *" });
            Insert.IntoTable("tblSystemValue").Row(new { ValueKey = "AccountingOperationsMaxBatchSize", Value = "30" });
            Insert.IntoTable("tblSystemValue").Row(new { ValueKey = "AccountingOperationsMinWaitingMinutes", Value = "2" });
            Insert.IntoTable("tblSystemValue").Row(new { ValueKey = "DisableBatchingFrom", Value = "01:55" });
            Insert.IntoTable("tblSystemValue").Row(new { ValueKey = "DisableBatchingTo", Value = "03:00" });

        }
    }
}
