
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180501_AddCredentialTypeCrossSkill
{
    [NaatiMigration(201805011400)]
    public class AddCredentialTypeCrossSkill:NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblCredentialTypeCrossSkill")
                .WithColumn("CredentialTypeCrossSkillId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialTypeFromId").AsInt32().Nullable()
                .WithColumn("CredentialTypeToId").AsInt32().NotNullable();
            
            Create.ForeignKey("FK_CredentialTypeCrossSkillFrom_CredentialType")
                .FromTable("tblCredentialTypeCrossSkill")
                .ForeignColumn("CredentialTypeFromId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Create.ForeignKey("FK_CredentialTypeCrossSkillFromTo_CredentialType")
                .FromTable("tblCredentialTypeCrossSkill")
                .ForeignColumn("CredentialTypeToId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");
        }
    }
}
