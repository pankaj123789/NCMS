namespace F1Solutions.Naati.Common.Migrations.NAATI._20210324_AddPaymentMethodTypeTable
{
    [NaatiMigration(202103261310)]
    public class AddAccountIdToGLCode : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblGLCode").AddColumn("ExternalReferenceAccountId").AsGuid().Nullable();
        }
    }
}
