namespace F1Solutions.Naati.Common.Migrations.NAATI._20200507_ExternalAccountingOperationTypeColmunsSize
{
    [NaatiMigration(202005071426)]
    public class ExternalAccountingOperationTypeColmunsSize : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tluExternalAccountingOperationType")
                .AlterColumn("Name").AsString(100).NotNullable()
                .AlterColumn("DisplayName").AsString(100).NotNullable();
        }
    }
}
