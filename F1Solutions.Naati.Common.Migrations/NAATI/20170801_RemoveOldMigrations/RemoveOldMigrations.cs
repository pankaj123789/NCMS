
namespace F1Solutions.Naati.Common.Migrations.NAATI._20170801_RemoveOldMigrations
{
    [NaatiMigration(201708010000)]
    public class RemoveOldMigrations : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("DELETE FROM VersionInfo WHERE Version < 201707200000");
        }
    }
}
