
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171123_AddVenueTable
{
   [NaatiMigration(201711231400)]
    public class AddVenueTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblVenue")
                .WithColumn("VenueId").AsInt32().Identity().PrimaryKey()
                .WithColumn("TestLocationId").AsInt32().NotNullable()
                .WithColumn("Address").AsString().Nullable()
                .WithColumn("Capacity").AsInt32().Nullable();

                Create.ForeignKey("FK_Venue_TestLocation")
                    .FromTable("tblVenue")
                    .ForeignColumn("TestLocationId")
                    .ToTable("tblTestLocation")
                    .PrimaryColumn("TestLocationId");
        }
    }
}
