
namespace F1Solutions.Naati.Common.Migrations.NAATI._202301231431_AddIndexesForTestMaterialWizard
{
    [NaatiMigration(202301231431)]
    public class AddIndexesForTestMaterialWizard : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddIndexesForTestMaterialWizard);
        }
    }
}
