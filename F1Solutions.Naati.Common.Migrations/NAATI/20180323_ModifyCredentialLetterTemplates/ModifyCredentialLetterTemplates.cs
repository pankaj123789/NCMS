
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180323_ModifyCredentialLetterTemplates
{
    [NaatiMigration(201803231100)]
    public class ModifyCredentialLetterTemplates: NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.ModifyCredentialLetterTemplates);
        }
    }
}
