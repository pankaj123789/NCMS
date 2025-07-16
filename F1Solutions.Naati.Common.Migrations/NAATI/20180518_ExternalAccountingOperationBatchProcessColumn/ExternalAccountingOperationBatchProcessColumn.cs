
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180518_ExternalAccountingOperationBatchProcessColumn
{
    [NaatiMigration(201805180900)]
    public class ExternalAccountingOperationBatchProcessColumn : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("BatchProcess").OnTable("tblExternalAccountingOperation").AsBoolean().NotNullable().WithDefaultValue(0);
        }
    }
}
