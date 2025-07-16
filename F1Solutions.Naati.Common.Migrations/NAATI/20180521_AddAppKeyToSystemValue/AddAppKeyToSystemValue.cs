
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180521_AddAppKeyToSystemValue
{
    [NaatiMigration(201805211200)]
    public class AddAppKeyToSystemValue : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"insert into tblSystemValue (valueKey,value) values ('MicrosoftGraphAppKey','')");
        }
    }
}
