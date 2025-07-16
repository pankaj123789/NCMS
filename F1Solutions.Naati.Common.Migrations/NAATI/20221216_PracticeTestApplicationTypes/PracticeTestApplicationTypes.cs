namespace F1Solutions.Naati.Common.Migrations.NAATI._20221216_PracticeTestApplicationTypes
{
    [NaatiMigration(202212160921)]
    public class PracticeTestApplicationTypes : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.PracticeTestApplicationTypes);
        }
    }
}
