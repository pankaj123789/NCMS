namespace F1Solutions.Naati.Common.Migrations.NAATI._20200422_AddRefundMethodTypeTable
{
    [NaatiMigration(202004221022)]
    public class AddRefundMethodTypeTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblRefundMethodType")
                .WithColumn("RefundMethodTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("DisplayName").AsString(50).NotNullable();
        }
    }
}
