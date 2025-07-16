
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171003_ConfigureCountriesAndLanguages
{
    [NaatiMigration(201710031700)]
    public class ConfigureCountriesAndLanguages : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.ConfigureCountries);
            Execute.Sql(Sql.ConfigureLanguages);
        }
    }
}
