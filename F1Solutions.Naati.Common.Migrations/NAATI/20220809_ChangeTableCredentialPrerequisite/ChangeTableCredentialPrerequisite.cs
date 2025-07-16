namespace F1Solutions.Naati.Common.Migrations.NAATI._20220809_ChangeTableCredentialPrerequisite
{
    [NaatiMigration(202208091541)]
    public class ChangeTableCredentialPrerequisite : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.ChangeTableCredentialPrerequisite);
        }
    }
}
