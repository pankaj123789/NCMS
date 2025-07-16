
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180809_ModifyCertificationPeriodRecertifyKey
{
    [NaatiMigration(201808091401)]
    public class ModifyCertificationPeriodRecertifyKey: NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("UPDATE TBLSYSTEMVALUE SET VALUEKEY = 'CertificationPeriodRecertifyMonths' WHERE VALUEKEY = 'CertificationPeriodRecertify'");
         
        }
    }
}
