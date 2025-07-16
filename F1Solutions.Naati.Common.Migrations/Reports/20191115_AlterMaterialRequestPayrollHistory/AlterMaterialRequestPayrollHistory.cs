using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20191115_AlterMaterialRequestPayrollHistory
{
    [NaatiMigration(201911151053)]
    public class AlterMaterialRequestPayrollHistory: NaatiMigration
    {
        public override void Up()
        {
            Alter.Column("PaymentReference").OnTable("MaterialRequestPayrollHistory").AsString(255).Nullable();
        }
    }
}
