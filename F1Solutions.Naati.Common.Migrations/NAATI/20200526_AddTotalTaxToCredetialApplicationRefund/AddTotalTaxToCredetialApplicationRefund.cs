namespace F1Solutions.Naati.Common.Migrations.NAATI._20200526_AddTotalTaxToCredetialApplicationRefund
{
    [NaatiMigration(202005261014)]
    public class AddTotalTaxToCredetialApplicationRefund : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblCredentialApplicationRefund").AddColumn("InitialPaidTax").AsCurrency().Nullable();
        }
    }
}
