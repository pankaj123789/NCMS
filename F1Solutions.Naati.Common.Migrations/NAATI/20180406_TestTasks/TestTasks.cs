
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180406_TestTasks
{
    [NaatiMigration(201804061000)]
    public class TestTasks : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("CredentialTypeId").OnTable("tluTestComponentType").AsInt32().NotNullable().WithDefaultValue(1);
            Create.Column("TestMarkingSchemeId").OnTable("tluTestComponentType").AsInt32().NotNullable().WithDefaultValue(1);
            Create.Column("BasedOn").OnTable("tluTestComponentType").AsString().NotNullable().WithDefaultValue(' ');

            Create.Table("tblTestMarkingScheme")
                .WithColumn("TestMarkingSchemeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("DisplayName").AsString().NotNullable()
                .WithColumn("TotalMarks").AsInt32().NotNullable()
                .WithColumn("PassMark").AsFloat().NotNullable();

            Execute.Sql(@"ALTER TABLE tblTestComponent drop column TotalMarks
                          ALTER TABLE tblTestComponent drop column PassMark
                          ALTER TABLE tblTestMaterial DROP CONSTRAINT FK_TestMaterial_CredentialType
                          ALTER TABLE tblTestSpecification alter column OverallPassMark int null");

            Insert.IntoTable("tblTestMarkingScheme")
                .Row(new { DisplayName = "Standard", TotalMarks = 45, PassMark = 29.0 });

            Create.ForeignKey("FK_TestComponentType_CredentialType")
                .FromTable("tluTestComponentType")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Create.ForeignKey("FK_TestComponentType_TestMarkingScheme")
                .FromTable("tluTestComponentType")
                .ForeignColumn("TestMarkingSchemeId")
                .ToTable("tblTestMarkingScheme")
                .PrimaryColumn("TestMarkingSchemeId");


            Create.Column("TestMarkingSchemeId").OnTable("tblTestSpecification").AsInt32().NotNullable().WithDefaultValue(1);
            Create.Column("TestComponentTypeId").OnTable("tblTestMaterial").AsInt32().NotNullable().WithDefaultValue(1);
            Create.Column("SkillId").OnTable("tblTestMaterial").AsInt32().Nullable();
            Delete.Column("CredentialTypeId").FromTable("tblTestMaterial");

            Create.ForeignKey("FK_TestSpecification_TestMarkingScheme")
                .FromTable("tblTestSpecification")
                .ForeignColumn("TestMarkingSchemeId")
                .ToTable("tblTestMarkingScheme")
                .PrimaryColumn("TestMarkingSchemeId");

            Create.ForeignKey("FK_TestMaterial_TestComponentType")
                .FromTable("tblTestMaterial")
                .ForeignColumn("TestComponentTypeId")
                .ToTable("tluTestComponentType")
                .PrimaryColumn("TestComponentTypeId");

            Create.ForeignKey("FK_TestMaterial_Skill")
                .FromTable("tblTestMaterial")
                .ForeignColumn("SkillId")
                .ToTable("tblSkill")
                .PrimaryColumn("SkillId");

            Execute.Sql(@"Update tblTestMaterial set TestComponentTypeId = 1
                          ALTER TABLE tblTestMaterial WITH CHECK ADD 
                          CONSTRAINT LanguageId_SkillId_OnlyValue CHECK (LanguageId IS NOT NULL OR SkillId IS NOT NULL) ");

            Create.Table("tblTestSpecificationTestComponentType")
                .WithColumn("TestSpecificationTestComponentTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("TestSpecificationId").AsInt32().NotNullable()
                .WithColumn("TestComponentTypeId").AsInt32().NotNullable()
                .WithColumn("NumberRequired").AsInt32().NotNullable();

            Create.ForeignKey("FK_TestSpecificationTestComponentType_TestSpecification")
               .FromTable("tblTestSpecificationTestComponentType")
               .ForeignColumn("TestSpecificationId")
               .ToTable("tblTestSpecification")
               .PrimaryColumn("TestSpecificationId");

            Create.ForeignKey("FK_TestSpecificationTestComponentType_TestComponentType")
                .FromTable("tblTestSpecificationTestComponentType")
                .ForeignColumn("TestComponentTypeId")
                .ToTable("tluTestComponentType")
                .PrimaryColumn("TestComponentTypeId");


            Create.Table("tblTestSpecificationAttachment")
                .WithColumn("TestSpecificationAttachmentId").AsInt32().Identity().PrimaryKey()
                .WithColumn("StoredFileId").AsInt32().NotNullable()
                .WithColumn("TestSpecificationId").AsInt32().NotNullable()
                .WithColumn("Title").AsString(250).NotNullable()
                .WithColumn("Deleted").AsBoolean().WithDefaultValue(0);

            Create.ForeignKey("FK_TestSpecificationAttachment_StoredFile")
             .FromTable("tblTestSpecificationAttachment")
             .ForeignColumn("StoredFileId")
             .ToTable("tblStoredFile")
             .PrimaryColumn("StoredFileId");

            Create.ForeignKey("FK_TestSpecificationAttachment_TestSpecification")
                .FromTable("tblTestSpecificationAttachment")
                .ForeignColumn("TestSpecificationId")
                .ToTable("tblTestSpecification")
                .PrimaryColumn("TestSpecificationId");

            Create.Column("ExaminerToolsDownload").OnTable("tblTestMaterialAttachment").AsBoolean().WithDefaultValue(0);
        }
    }
}
