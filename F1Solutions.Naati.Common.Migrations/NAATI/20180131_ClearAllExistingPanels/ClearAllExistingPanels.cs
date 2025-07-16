namespace F1Solutions.Naati.Common.Migrations.NAATI._20180131_ClearAllExistingPanels
{
    [NaatiMigration(201801310111)]
    public class ClearAllExistingPanels : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.ClearAllPanels);
        }
    }
}
