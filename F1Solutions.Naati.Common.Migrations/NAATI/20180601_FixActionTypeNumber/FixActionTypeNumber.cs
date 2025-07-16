
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180601_FixActionTypeNumber
{
    [NaatiMigration(201806011002)]
    public class FixActionTypeNumber:NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(FixAction.FixActionNumberQuery);
        }
    }
}
