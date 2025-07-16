
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190705_AddSessionBookingToCredentialType
{
    [NaatiMigration(201907051015)]
    public class AddSessionBookingToCredentialType : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("TestSessionBookingAvailabilityWeeks").OnTable("tblCredentialType").AsInt32().Nullable();
            Create.Column("TestSessionBookingClosedWeeks").OnTable("tblCredentialType").AsInt32().Nullable();
            Create.Column("TestSessionBookingRejectWeeks").OnTable("tblCredentialType").AsInt32().Nullable();

            Update.Table("tblCredentialType").Set(new { TestSessionBookingAvailabilityWeeks = 1, TestSessionBookingClosedWeeks = 1, TestSessionBookingRejectWeeks =1 }).AllRows();

            Alter.Column("TestSessionBookingAvailabilityWeeks").OnTable("tblCredentialType").AsInt32().NotNullable();
            Alter.Column("TestSessionBookingClosedWeeks").OnTable("tblCredentialType").AsInt32().NotNullable();
            Alter.Column("TestSessionBookingRejectWeeks").OnTable("tblCredentialType").AsInt32().NotNullable();
        }
    }
}
