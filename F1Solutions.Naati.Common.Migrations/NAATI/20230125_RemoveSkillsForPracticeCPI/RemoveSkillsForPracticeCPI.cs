
namespace F1Solutions.Naati.Common.Migrations.NAATI._20230125_RemoveSkillsForPracticeCPI
{
    [NaatiMigration(202301251516)]
    public class RemoveSkillsForPracticeCPI : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("update tblCredentialRequest set skillId = 5150 where skillId in (select skillid from tblSkill where skilltypeid = 40)");

            Execute.Sql("update tblTestSessionSkill set skillId = 5150 where skillId in (select skillid from tblSkill where skilltypeid = 40)");

            Execute.Sql("delete from tblSkillApplicationType where CredentialApplicationTypeId=17");
        }
    }
}
