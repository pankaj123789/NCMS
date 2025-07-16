
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180410_Office365
{
    [NaatiMigration(201804101700)]
    public class Office365 : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"alter table tblSystemValue alter column [value] nvarchar(max) not null");
            Insert.IntoTable("tblSystemValue").Row(new {ValueKey = "MicrosoftGraphAccessToken", Value = ""});
        }
    }
}
