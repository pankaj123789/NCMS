namespace F1Solutions.Naati.Common.Migrations.NAATI._20180323_AddTestSessionBookingSystemValue
{
    [NaatiMigration(201803230900)]
    public class AddTestSessionBookingSystemValue : NaatiMigration
    {
        public override void Up()
        {
            Insert.IntoTable("tblSystemValue").Row(new
            {
                ValueKey = "TestSessionBookingAvailability",
                Value = 26
            });
            Insert.IntoTable("tblSystemValue").Row(new
            {
                ValueKey = "TestSessionBookingClosed",
                Value = 3
            });
        }
    }

}
