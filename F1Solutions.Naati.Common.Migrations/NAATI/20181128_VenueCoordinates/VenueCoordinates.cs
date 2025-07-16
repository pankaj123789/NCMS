
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181128_VenueCoordinates
{
   [NaatiMigration(201811280945)]
    public class VenueCoordinates : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Coordinates").OnTable("tblVenue").AsAnsiString(22).Nullable();
        }
    }
}
