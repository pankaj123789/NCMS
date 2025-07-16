namespace F1Solutions.Naati.Common.Migrations.NAATI._20200225_AddIndigenousCertificationToSkills
{
    [NaatiMigration(202002251440)]
    public class AddIndigenousCertificationToSkills :NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
                      insert into tblSkillApplicationType
                      (SkillId, CredentialApplicationTypeId, ModifiedByNaati, ModifiedDate, ModifiedUser)
                      select distinct skillId, 10, 0, GetDate(), 40
                      from tblSkillApplicationType
                      where skillid in (Select skillId from [tblSkill]
                      where SkillTypeId in (25,28))
                      and SkillId not in (
                      select SkillId
                      from tblSkillApplicationType as sat
                      where skillid in (Select skillId from [tblSkill]
                      where SkillTypeId in (25,28))
                      and CredentialApplicationTypeId = 10
                      )
                      order by skillid
                ");
        }
    }
}
