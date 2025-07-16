namespace F1Solutions.Naati.Common.Migrations.NAATI._20171218_AddCredentialRoleScreenPermission
{
    [NaatiMigration(201712181430)]
    public class AddCredentialRoleScreenPermission : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.AddCredentialRoleScreenPermission);
        }
    }
}
