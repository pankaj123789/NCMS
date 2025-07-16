namespace F1Solutions.Naati.Common.Migrations.NAATI._20200316_AddCookiesTable
{
    [NaatiMigration(202003160801)]
    public class AddCookiesTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblNcmsInvalidCookie")
                .WithColumn("NcmsInvalidCookieId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Cookie").AsString(500).NotNullable()
                .WithColumn("ExpiryDate").AsDateTime().NotNullable();

            Create.Table("tblMyNaatiInvalidCookie")
                .WithColumn("MyNaatiInvalidCookieId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Cookie").AsString(500).NotNullable()
                .WithColumn("ExpiryDate").AsDateTime().NotNullable();
        }
    }
}
