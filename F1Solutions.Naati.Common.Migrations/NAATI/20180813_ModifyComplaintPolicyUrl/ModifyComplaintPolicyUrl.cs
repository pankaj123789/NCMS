
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180813_ModifyComplaintPolicyUrl
{
    [NaatiMigration(201808131501)]
    public class ModifyComplaintPolicyUrl : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("Update tblsystemvalue set value ='https://www.naati.com.au/policies/' where ValueKey = 'ComplaintPolicyUrl' ");
        }
    }
}
