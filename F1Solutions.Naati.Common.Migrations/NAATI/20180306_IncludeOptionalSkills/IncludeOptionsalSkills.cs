
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180306_IncludeOptionalSkills
{
    [NaatiMigration(201803061600)]
    public class IncludeOptionsalSkills : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("AllowSelfAssign").OnTable("tblTestSession").AsBoolean().NotNullable().WithDefaultValue(0);
            Create.Table("tblTestSessionSkill")
                .WithColumn("TestSessionSkillId").AsInt32().PrimaryKey().Identity()
                .WithColumn("TestSessionId").AsInt32().NotNullable()
                .WithColumn("SkillId").AsInt32().NotNullable()
                .WithColumn("Capacity").AsInt32().Nullable();

            Create.UniqueConstraint("UC_TestSession_Skill")
                .OnTable("tblTestSessionSkill")
                .Columns("TestSessionId", "SkillId");

            Create.ForeignKey("FK_TestSessionSkill_TestSession")
                .FromTable("tblTestSessionSkill")
                .ForeignColumn("TestSessionId")
                .ToTable("tblTestSession")
                .PrimaryColumn("TestSessionId");

            Create.ForeignKey("FK_TestSessionSkill_Skill")
                .FromTable("tblTestSessionSkill")
                .ForeignColumn("SkillId")
                .ToTable("tblSkill")
                .PrimaryColumn("SkillId");
        }
    }
}
