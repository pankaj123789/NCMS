using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20170907_UpdateDocumentTypeTable
{
    [NaatiMigration(201709071200)]
    public class UpdateDocumentTypeTable : NaatiMigration
    {
        public override void Up()
        {
            UpdateDocumentTypeSchema();
            CreateDocumentTypeCategoryTable();
            PopulateDocumentTypeCategory();
            AddDocumentTypeCategoryColumnToDocumentType();
            UpdateDocumentTypeData();
            UpdateDocumentTypeCategoryColumn();
            CreateCredentialAttachment();
            CreateCredentialTypeTemplate();
            CreateCredentialApplicationTypeDocumentType();
        }

        void CreateDocumentTypeCategoryTable()
        {
            Create.Table("tblDocumentTypeCategory")
                .WithColumn("DocumentTypeCategoryId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50).NotNullable()
                .WithColumn("DisplayName").AsAnsiString(50).NotNullable();
        }

        void PopulateDocumentTypeCategory()
        {
            Execute.Sql("Set IDENTITY_INSERT tblDocumentTypeCategory ON ");
            Insert.IntoTable("tblDocumentTypeCategory")
                .Row(new { DocumentTypeCategoryId = 1, Name = "Tests", DisplayName = "Tests" })
                .Row(new { DocumentTypeCategoryId = 2, Name = "Applications", DisplayName = "Applications" })
                .Row(new { DocumentTypeCategoryId = 3, Name = "General", DisplayName = "General" });

            Execute.Sql("Set IDENTITY_INSERT tblDocumentTypeCategory OFF ");

        }

        void AddDocumentTypeCategoryColumnToDocumentType()
        {
            Create.Column("DocumentTypeCategoryId").OnTable("tluDocumentType").AsInt32().Nullable();

            Create.ForeignKey("FK_DocumentType_DocumentTypeCategory")
                .FromTable("tluDocumentType")
                .ForeignColumn("DocumentTypeCategoryId")
                .ToTable("tblDocumentTypeCategory")
                .PrimaryColumn("DocumentTypeCategoryId");
        }

        void UpdateDocumentTypeSchema()
        {
            Alter.Column("Name").OnTable("tluDocumentType").AsString(50).NotNullable();
            Alter.Column("DisplayName").OnTable("tluDocumentType").AsString(50).NotNullable();
        }

        void UpdateDocumentTypeData()
        {
            Execute.Sql("Update tluDocumentType set DocumentTypeCategoryId= 1 where Name='Undefined'");
            Execute.Sql("Update tluDocumentType set DocumentTypeCategoryId= 1 where Name='UnmarkedTestAsset'");
            Execute.Sql("Update tluDocumentType set DocumentTypeCategoryId= 1 where Name='MarkedTestAsset'");
            Execute.Sql("Update tluDocumentType set DocumentTypeCategoryId= 1 where Name='EnglishMarking'");
            Execute.Sql("Update tluDocumentType set DocumentTypeCategoryId= 1 where Name='TestMaterial'");
            Execute.Sql("Update tluDocumentType set DocumentTypeCategoryId= 1 where Name='ReviewReport'");
            Execute.Sql("Update tluDocumentType set DocumentTypeCategoryId= 1 where Name='GeneralTestDocument'");

            Execute.Sql("Update tluDocumentType set DocumentTypeCategoryId=3 where name='General'");

            Execute.Sql("Set IDENTITY_INSERT tluDocumentType ON ");
            Insert.IntoTable("tluDocumentType")
                .Row(new { DocumentTypeId = 9, Name = "Identification", DisplayName = "Identification", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 10, Name = "WorkPracticeEvidence", DisplayName = "Work Practice Evidence", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 11, Name = "Transcript", DisplayName = "Transcript", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 12, Name = "TrainingEvidence", DisplayName = "Training Evidence", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 13, Name = "AuslanProficiencyEvidence", DisplayName = "Auslan Proficiency Evidence", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 14, Name = "EnglishProficiencyEvidence", DisplayName = "English Proficiency Evidence", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 15, Name = "EthicalCompetencyEvidence", DisplayName = "Ethical Competency Evidence", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 16, Name = "InterculturalCompetencyEvidence", DisplayName = "Intercultural Competency Evidence", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 17, Name = "Other", DisplayName = "Other", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 18, Name = "ChuchotageEvidence", DisplayName = "Chuchotage Evidence", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 19, Name = "CertificateTemplate", DisplayName = "Certificate Template", DocumentTypeCategoryId = 3, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 20, Name = "Certificate", DisplayName = "Certificate", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 21, Name = "ApplicationForm", DisplayName = "Application Form", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 22, Name = "CredentialLetterTemplate", DisplayName = "Credential Letter Template", DocumentTypeCategoryId = 3, ExaminerToolsDownload = false, ExaminerToolsUpload = false })
                .Row(new { DocumentTypeId = 23, Name = "CredentialLetter", DisplayName = "Credential Letter", DocumentTypeCategoryId = 2, ExaminerToolsDownload = false, ExaminerToolsUpload = false });
            Execute.Sql("Set IDENTITY_INSERT tluDocumentType OFF ");
        }

        void UpdateDocumentTypeCategoryColumn()
        {
            Alter.Column("DocumentTypeCategoryId").OnTable("tluDocumentType").AsInt32().NotNullable();
        }

        void CreateCredentialAttachment()
        {
            Create.Table("tblCredentialAttachment")
                .WithColumn("CredentialAttachmentId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialId").AsInt32().NotNullable()
                .WithColumn("StoredFileId").AsInt32().NotNullable()
                .WithColumn("DocumentNumber").AsString(20).Nullable();

            Create.ForeignKey("FK_CredentialAttachment_Credential")
                .FromTable("tblCredentialAttachment")
                .ForeignColumn("CredentialId")
                .ToTable("tblCredential")
                .PrimaryColumn("CredentialId");

            Create.ForeignKey("FK_CredentialAttachment_StoredFile")
                .FromTable("tblCredentialAttachment")
                .ForeignColumn("StoredFileId")
                .ToTable("tblStoredFile")
                .PrimaryColumn("StoredFileId");
        }

        void CreateCredentialTypeTemplate()
        {
            Create.Table("tblCredentialTypeTemplate")
                .WithColumn("CredentialTypeTemplateId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialTypeId").AsInt32().NotNullable()
                .WithColumn("StoredFileId").AsInt32().NotNullable()
                .WithColumn("DocumentNameTemplate").AsString(500).NotNullable();

            Create.ForeignKey("FK_CredentialTypeTemplate_CredentialType")
                .FromTable("tblCredentialTypeTemplate")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Create.ForeignKey("FK_CredentialTypeTemplate_StoredFile")
                .FromTable("tblCredentialTypeTemplate")
                .ForeignColumn("StoredFileId")
                .ToTable("tblStoredFile")
                .PrimaryColumn("StoredFileId");
        }

        void CreateCredentialApplicationTypeDocumentType()
        {
            Create.Table("tblCredentialApplicationTypeDocumentType")
                .WithColumn("CredentialApplicationTypeDocumentTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationTypeId").AsInt32().NotNullable()
                .WithColumn("DocumentTypeId").AsInt32().NotNullable()
                .WithColumn("Mandatory").AsBoolean().NotNullable();

            Create.ForeignKey("FK_CredentialApplicationTypeDocumentType_CredentialApplicationType")
                .FromTable("tblCredentialApplicationTypeDocumentType")
                .ForeignColumn("CredentialApplicationTypeId")
                .ToTable("tblCredentialApplicationType")
                .PrimaryColumn("CredentialApplicationTypeId");

            Create.ForeignKey("FK_CredentialApplicationTypeDocumentType_DocumentType")
                .FromTable("tblCredentialApplicationTypeDocumentType")
                .ForeignColumn("DocumentTypeId")
                .ToTable("tluDocumentType")
                .PrimaryColumn("DocumentTypeId");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
