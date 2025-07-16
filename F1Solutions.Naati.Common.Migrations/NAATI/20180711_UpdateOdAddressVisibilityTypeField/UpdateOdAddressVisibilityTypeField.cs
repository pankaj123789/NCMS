
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180711_UpdateOdAddressVisibilityTypeField
{
    [NaatiMigration(201807111340)]
    public class UpdateOdAddressVisibilityTypeField : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("UPDATE tblAddress SET ODAddressVisibilityTypeId = 1 WHERE IncludeInPD = 0");
            Execute.Sql("UPDATE tblAddress SET ODAddressVisibilityTypeId = 3 WHERE IncludeInPD = 1");            
        }
    }
}
