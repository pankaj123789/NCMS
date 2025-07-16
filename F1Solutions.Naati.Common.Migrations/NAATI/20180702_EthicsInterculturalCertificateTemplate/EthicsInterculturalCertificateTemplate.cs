
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180702_EthicsInterculturalCertificateTemplate
{
    [NaatiMigration(201807021800)]
    public class EthicsInterculturalCertificateTemplate : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.EthicsInterculturalCertificateTemplate);
        }
    }
}
