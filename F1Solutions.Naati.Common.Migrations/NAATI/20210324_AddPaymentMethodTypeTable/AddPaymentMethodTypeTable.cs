namespace F1Solutions.Naati.Common.Migrations.NAATI._20210324_AddPaymentMethodTypeTable
{
    [NaatiMigration(202103241340)]
    public class AddPaymentMethodTypeTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblPaymentMethodType")
                .WithColumn("paymentMethodTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50)
                .WithColumn("ExternalReferenceId").AsGuid();
        }
    }
}
