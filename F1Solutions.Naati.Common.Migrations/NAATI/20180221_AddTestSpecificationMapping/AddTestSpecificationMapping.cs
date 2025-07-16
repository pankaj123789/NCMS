
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180221_AddTestSpecificationMapping
{
    [NaatiMigration(201802211100)]
    public class AddTestSpecificationMapping: NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblTestSpecificationMapping")
                .WithColumn("TestSpecificationMappingId").AsInt32().Identity().PrimaryKey()
                .WithColumn("TestSpecificationId").AsInt32()
                .WithColumn("CredentialTypeId").AsInt32()
                .WithColumn("SkillId").AsInt32()
                .WithColumn("IsActive").AsBoolean();

            Create.ForeignKey("FK_TestSpecificationMapping_TestSpecification")
                .FromTable("tblTestSpecificationMapping")
                .ForeignColumn("TestSpecificationId")
                .ToTable("tblTestSpecification")
                .PrimaryColumn("TestSpecificationId");

            Create.ForeignKey("FK_TestSpecificationMapping_CredentialType")
                .FromTable("tblTestSpecificationMapping")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Create.ForeignKey("FK_TestSpecificationMapping_Skill")
                .FromTable("tblTestSpecificationMapping")
                .ForeignColumn("SkillId")
                .ToTable("tblSkill")
                .PrimaryColumn("SkillId");

            Create.Column("TestSpecificationMappingId").OnTable("tblTestResult").AsInt32().NotNullable();

            Create.ForeignKey("FK_TestResult_TestSpecificationMapping")
                .FromTable("tblTestResult")
                .ForeignColumn("TestSpecificationMappingId")
                .ToTable("tblTestSpecificationMapping")
                .PrimaryColumn("TestSpecificationMappingId");


        }
    }
}
