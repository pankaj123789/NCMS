
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181010_AddTestSessionBookingRejectKey
{
    [NaatiMigration(201810101601)]
    public class AddTestSessionBookingRejectKey: NaatiMigration
    {
        public override void Up()
        {
            Insert.IntoTable("tblSystemValue").Row(new {ValueKey = "TestSessionBookingReject", Value = "3"});
            Update.Table("tblSystemValue").Set( new{ Value ="1"}).Where(new {ValueKey = "TestSessionBookingClosed" });
        }
    }
}
