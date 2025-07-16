
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181211_TestSessionCapacity
{
    [NaatiMigration(201812111435)]
    public class TestSessionCapacity : NaatiMigration
    {
        public override void Up()
        {
			Create.Column("OverrideVenueCapacity").OnTable("tblTestSession").AsBoolean().NotNullable().WithDefaultValue(false);
			Create.Column("Capacity").OnTable("tblTestSession").AsInt32().Nullable();
		}
    }
}
