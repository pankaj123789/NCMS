
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171215_UpdateEntityType
{
    [NaatiMigration(201712151700)]
    public class UpdateEntityType : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("UPDATE TBLENTITY SET ENTITYTYPEID = 1 WHERE NAATINUMBER >=200000 AND NAATINUMBER <900000 ");
            Execute.Sql("UPDATE TBLENTITY SET ENTITYTYPEID = 3 WHERE NAATINUMBER >=950000");
        }
    }
}
