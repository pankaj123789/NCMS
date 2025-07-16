namespace F1Solutions.Naati.Common.Migrations.NAATI._20210602_UpdateCredentialTypeTemplate
{
    [NaatiMigration(202106021142)]
    public class UpdateCredentialTypeTemplate : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.UpdateCredentialTypeTemplate);
        }
    }
}
