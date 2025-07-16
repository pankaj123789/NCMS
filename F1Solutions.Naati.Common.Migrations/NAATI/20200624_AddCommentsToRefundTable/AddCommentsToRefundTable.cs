namespace F1Solutions.Naati.Common.Migrations.NAATI._20200624_AddCommentsToRefundTable
{
    [NaatiMigration(202006241345)]
    public class AddCommentsToRefundTable : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblCredentialApplicationRefund").AddColumn("Comments").AsString(500).Nullable();
            Alter.Table("tblCredentialApplicationRefund").AddColumn("BankDetails").AsString(500).Nullable();
        }
    }
}
