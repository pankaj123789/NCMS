
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181204_DatabaseCleanUp
{
    [NaatiMigration(201812041000)]
    public class DatabaseCleanUp : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.DropObsoleteDatabaseObjects);
        }
    }
}
