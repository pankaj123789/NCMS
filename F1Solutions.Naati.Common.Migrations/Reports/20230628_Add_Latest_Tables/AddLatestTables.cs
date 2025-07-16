using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20230628_Add_Latest_Tables
{
    [NaatiMigration(202306281000)]
    public class AddLatestTables : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddLatestTables);
        }
    }
}
