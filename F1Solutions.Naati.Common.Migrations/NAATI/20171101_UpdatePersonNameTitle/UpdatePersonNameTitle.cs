
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171101_UpdatePersonNameTitle
{
    [NaatiMigration(201711011525)]
    public class UpdatePersonNameTitle : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.UpdatePersonNameTitle);
        }
    }
}
