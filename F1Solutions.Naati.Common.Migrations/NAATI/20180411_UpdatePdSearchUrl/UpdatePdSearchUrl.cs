
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180411_UpdatePdSearchUrl
{
    [NaatiMigration(201804111300)]
    public class UpdatePdSearchUrl: NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("update tblSystemValue set value = 'https://www.naati.com.au/certification/the-certification-system/' where ValueKey ='CertificationDescriptorUrl'");
        }
    }
}
