
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180726_AddComplaintPolicyUrlToSystemValue
{
    [NaatiMigration(201807261730)]
    public class AddComplaintPolicyUrlToSystemValue : NaatiMigration
    {
        public override void Up()
        {
           Execute.Sql(@"insert into tblSystemValue (valueKey,value) values ('ComplaintPolicyUrl','http://www.ausit.org')");
        }
    }
}
