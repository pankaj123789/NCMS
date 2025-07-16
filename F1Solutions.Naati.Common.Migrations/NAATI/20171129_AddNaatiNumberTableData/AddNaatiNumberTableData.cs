
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171129_AddNaatiNumberTableData
{
    [NaatiMigration(201711291850)]
    public class AddNaatiNumberTableData : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.AddNaatiNumber);
        }
    }
}
