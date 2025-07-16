using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20181216_TestSessionCapacity
{
	[NaatiMigration(201812161130)]
	public class TestSessionCapacity : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Capacity")
                .OnTable("TestSessionsHistory")
                .AsInt32()
                .Nullable();

            Create.Column("VenueCapacityOverridden")
                .OnTable("TestSessionsHistory")
                .AsBoolean()
                .Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
