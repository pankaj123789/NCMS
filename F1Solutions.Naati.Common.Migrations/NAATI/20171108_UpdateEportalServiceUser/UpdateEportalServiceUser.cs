
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171108_UpdateEportalServiceUser
{

    [NaatiMigration(201711081600)]
    public class UpdateEportalServiceUser : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.UpdateEportalServiceUser);
        }
    }
}
