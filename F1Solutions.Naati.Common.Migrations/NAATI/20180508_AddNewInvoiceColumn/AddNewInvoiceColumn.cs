
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180508_AddNewInvoiceColumn
{
    [NaatiMigration(201805081200)]
    public class AddNewInvoiceColumn:NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Invoice").OnTable("tblCredentialWorkflowActionEmailTemplate").AsBoolean().Nullable();
            Execute.Sql("UPDATE tblCredentialWorkflowActionEmailTemplate SET Invoice = 0");
            Alter.Column("Invoice").OnTable("tblCredentialWorkflowActionEmailTemplate").AsBoolean().NotNullable();
        }
    }
}
