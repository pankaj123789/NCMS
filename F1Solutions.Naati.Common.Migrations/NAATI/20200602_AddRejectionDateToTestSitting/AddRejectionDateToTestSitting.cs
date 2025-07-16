namespace F1Solutions.Naati.Common.Migrations.NAATI._20200602_AddRejectionDateToTestSitting
{
    [NaatiMigration(202006021032)]
    public class AddRejectionDateToTestSitting : NaatiMigration
    {
        public override void Up()
        {
            this.ExecuteSql(Sql.AddRejectionDateToTestSitting);
        }
    }
}
