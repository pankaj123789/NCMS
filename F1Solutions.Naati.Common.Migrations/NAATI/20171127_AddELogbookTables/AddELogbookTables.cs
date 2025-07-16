
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171127_AddELogbookTables
{
   [NaatiMigration(201711271400)]
    public class AddELogbookTables : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblProfessionalDevelopmentRequirement")
                .WithColumn("ProfessionalDevelopmentRequirementId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50).NotNullable()
                .WithColumn("DisplayName").AsString(200).NotNullable();

            Create.Table("tblProfessionalDevelopmentSection")
                .WithColumn("ProfessionalDevelopmentSectionId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("Description").AsString(200).NotNullable();

            Create.Table("tblProfessionalDevelopmentCategory")
                .WithColumn("ProfessionalDevelopmentCategoryId").AsInt32().Identity().PrimaryKey()
                .WithColumn("ProfessionalDevelopmentSectionId").AsInt32().NotNullable()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("Description").AsString(200).NotNullable();

            Create.ForeignKey("FK_ProfessionalDevelopmentCategory_ProfessionalDevelopmentSection")
                .FromTable("tblProfessionalDevelopmentCategory")
                .ForeignColumn("ProfessionalDevelopmentSectionId")
                .ToTable("tblProfessionalDevelopmentSection")
                .PrimaryColumn("ProfessionalDevelopmentSectionId");

            Create.Table("tblProfessionalDevelopmentCategoryRequirement")
                .WithColumn("ProfessionalDevelopmentCategoryRequirementId").AsInt32().Identity().PrimaryKey()
                .WithColumn("ProfessionalDevelopmentCategoryId").AsInt32().NotNullable()
                .WithColumn("ProfessionalDevelopmentRequirementId").AsInt32().NotNullable()
                .WithColumn("Points").AsInt32().NotNullable();

            Execute.Sql(@"ALTER TABLE [tblProfessionalDevelopmentCategoryRequirement] 
                          ADD CONSTRAINT UC_ProfessionalDevelopmentCategoryId_ProfessionalDevelopmentRequirementId UNIQUE (ProfessionalDevelopmentCategoryId, ProfessionalDevelopmentRequirementId)");

            Create.ForeignKey("FK_ProfessionalDevelopmentCategoryRequirement_ProfessionalDevelopmentCategory")
                .FromTable("tblProfessionalDevelopmentCategoryRequirement")
                .ForeignColumn("ProfessionalDevelopmentCategoryId")
                .ToTable("tblProfessionalDevelopmentCategory")
                .PrimaryColumn("ProfessionalDevelopmentCategoryId");

            Create.ForeignKey("FK_ProfessionalDevelopmentCategoryRequirement_ProfessionalDevelopmentRequirement")
                .FromTable("tblProfessionalDevelopmentCategoryRequirement")
                .ForeignColumn("ProfessionalDevelopmentRequirementId")
                .ToTable("tblProfessionalDevelopmentRequirement")
                .PrimaryColumn("ProfessionalDevelopmentRequirementId");

            Create.Table("tblProfessionalDevelopmentActivity")
                .WithColumn("ProfessionalDevelopmentActivityId").AsInt32().Identity().PrimaryKey()
                .WithColumn("ProfessionalDevelopmentCategoryId").AsInt32().NotNullable()
                .WithColumn("ProfessionalDevelopmentRequirementId").AsInt32().NotNullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable()
                .WithColumn("Description").AsString(200).NotNullable()
                .WithColumn("DateCompleted").AsDate().NotNullable();

            Create.ForeignKey("FK_ProfessionalDevelopmentActivity_ProfessionalDevelopmentCategory")
                .FromTable("tblProfessionalDevelopmentActivity")
                .ForeignColumn("ProfessionalDevelopmentCategoryId")
                .ToTable("tblProfessionalDevelopmentCategory")
                .PrimaryColumn("ProfessionalDevelopmentCategoryId");

            Create.ForeignKey("FK_ProfessionalDevelopmentActivity_ProfessionalDevelopmentRequirement")
                .FromTable("tblProfessionalDevelopmentActivity")
                .ForeignColumn("ProfessionalDevelopmentRequirementId")
                .ToTable("tblProfessionalDevelopmentRequirement")
                .PrimaryColumn("ProfessionalDevelopmentRequirementId");

            Create.Table("tblProfessionalDevelopmentActivityAttachment")
                .WithColumn("ProfessionalDevelopmentActivityAttachmentId").AsInt32().Identity().PrimaryKey()
                .WithColumn("ProfessionalDevelopmentActivityId").AsInt32().NotNullable()
                .WithColumn("StoredFileId").AsInt32().NotNullable()
                .WithColumn("Description").AsString(100).Nullable();

            Create.ForeignKey("FK_ProfessionalDevelopmentActivityAttachment_ProfessionalDevelopmentActivity")
                .FromTable("tblProfessionalDevelopmentActivityAttachment")
                .ForeignColumn("ProfessionalDevelopmentActivityId")
                .ToTable("tblProfessionalDevelopmentActivity")
                .PrimaryColumn("ProfessionalDevelopmentActivityId");

            Create.ForeignKey("FK_ProfessionalDevelopmentActivityAttachment_StoredFile")
                .FromTable("tblProfessionalDevelopmentActivityAttachment")
                .ForeignColumn("StoredFileId")
                .ToTable("tblStoredFile")
                .PrimaryColumn("StoredFileId");

            Create.Table("tblWorkPractice")
                .WithColumn("WorkPracticeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialId").AsInt32().NotNullable()
                .WithColumn("Description").AsString(200).NotNullable()
                .WithColumn("Points").AsDecimal(9, 1).NotNullable()
                .WithColumn("Date").AsDate().NotNullable();

            Create.ForeignKey("FK_WorkPractice_Credential")
                .FromTable("tblWorkPractice")
                .ForeignColumn("CredentialId")
                .ToTable("tblCredential")
                .PrimaryColumn("CredentialId");

            Create.Table("tblWorkPracticeAttachment")
                .WithColumn("WorkPracticeAttachmentId").AsInt32().Identity().PrimaryKey()
                .WithColumn("WorkPracticeId ").AsInt32().NotNullable()
                .WithColumn("StoredFileId").AsInt32().NotNullable()
                .WithColumn("Description").AsString(100).Nullable();

            Create.ForeignKey("FK_WorkPracticeAttachment_WorkPractice")
                .FromTable("tblWorkPracticeAttachment")
                .ForeignColumn("WorkPracticeId")
                .ToTable("tblWorkPractice")
                .PrimaryColumn("WorkPracticeId");

            Create.ForeignKey("FK_WorkPracticeAttachment_StoredFile")
                .FromTable("tblWorkPracticeAttachment")
                .ForeignColumn("StoredFileId")
                .ToTable("tblStoredFile")
                .PrimaryColumn("StoredFileId");
        }
    }
}
