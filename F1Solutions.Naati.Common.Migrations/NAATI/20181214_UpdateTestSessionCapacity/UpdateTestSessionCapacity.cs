
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181214_UpdateTestSessionCapacity
{
    [NaatiMigration(201812141130)]
    public class UpdateTestSessionCapacity : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.UpdateTestSessioCapacity);
        }
    }
}
