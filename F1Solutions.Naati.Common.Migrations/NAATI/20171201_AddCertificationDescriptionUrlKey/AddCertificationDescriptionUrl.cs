namespace F1Solutions.Naati.Common.Migrations.NAATI._20171201_AddCertificationDescriptionUrlKey
{
    [NaatiMigration(201712011300)]
    public class AddCertificationDescriptionUrl : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.AddUrl);
        }
    }
}
