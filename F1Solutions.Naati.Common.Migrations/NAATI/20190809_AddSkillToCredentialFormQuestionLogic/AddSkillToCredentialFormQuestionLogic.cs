
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190809_AddSkillToCredentialFormQuestionLogic
{
    [NaatiMigration(201908091321)]
    public class AddSkillToCredentialFormQuestionLogic : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("SkillId").OnTable("tblCredentialApplicationFormQuestionLogic").AsInt32().Nullable();

            Create.ForeignKey("FK_CredentialApplicationFormQuestionLogic_Skill")
                .FromTable("tblCredentialApplicationFormQuestionLogic")
                .ForeignColumn("SkillId")
                .ToTable("tblSkill")
                .PrimaryColumn("SkillId");
        }
    }
}
