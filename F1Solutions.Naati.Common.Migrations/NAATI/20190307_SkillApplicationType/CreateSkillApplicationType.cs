
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190307_SkillApplicationType
{
    [NaatiMigration(201903071637)]
    public class CreateSkillApplicationType : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblSkillApplicationType")
                .WithColumn("SkillApplicationTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("SkillId").AsInt32().NotNullable().ForeignKey("tblSkill", "SkillId")
                .WithColumn("CredentialApplicationTypeId").AsInt32().NotNullable().ForeignKey("tblCredentialApplicationType", "CredentialApplicationTypeId");
        }       
    }
}
