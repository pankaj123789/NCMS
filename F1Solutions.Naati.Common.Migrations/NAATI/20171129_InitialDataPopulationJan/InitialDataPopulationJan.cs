
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171129_InitialDataPopulationJan
{
    [NaatiMigration(201711291800)]
    public class InitialDataPopulationJan : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.SkillType);
            Execute.Sql(Sql.Skill);
            Execute.Sql(Sql.CredentialType);
            Execute.Sql(Sql.LanguageGroup);
            Execute.Sql(Sql.Languages);
            Execute.Sql(Sql.DocumentTypeInsert);
            Execute.Sql(Sql.CredentialApplicationTypeCredentialTypeInsert);
            Execute.Sql(Sql.DataTypeInsert);
            Execute.Sql(Sql.CredentialApplicationFieldInsert);
            Execute.Sql(Sql.CredentialApplicationTypeDocumentTypeInsert);

            Execute.Sql(@"UPDATE cr set CredentialTypeId = (SELECT ct.CredentialTypeId FROM tblSkill s 
													inner join tblSkillType st on s.SkillTypeId = st.SkillTypeId
													inner join tblCredentialType ct on ct.SkillTypeId = st.SkillTypeId
													WHERE s.SkillId = cr.SkillId) from tblCredentialRequest cr");
          
        }
    }
}
