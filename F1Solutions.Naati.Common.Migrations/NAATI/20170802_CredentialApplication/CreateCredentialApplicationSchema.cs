namespace F1Solutions.Naati.Common.Migrations.NAATI._20170802_CredentialApplication
{
    [NaatiMigration(201708020000)]
    public class CreateCredentialApplicationSchema : NaatiMigration
    {
        public override void Up()
        {
            CreateSkillTables();
            CreateCredentialApplicationTables();
            CreateCredentialApplicationFieldTables();
            CreateCredentialApplicationNoteTable();
            CreateCrentialApplicationAttachmentTables();
            CreateCredentialRequestTables();
            CreateCredentialRequestFieldTables();
            CreateCredentialTables();
            CreateNoteTables();
            InsertNoteDocumentType();
            CreatePractitionerNumberColumn();

            Execute.Sql("DBCC CHECKIDENT ('tblCredentialApplication', RESEED, 1000)");
        }

        void CreateSkillTables()
        {
            Create.Table("tblDirectionType")
                .WithColumn("DirectionTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);

            Create.Table("tblSkillType")
                .WithColumn("SkillTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);

            Create.Table("tblSkill")
                .WithColumn("SkillId").AsInt32().Identity().PrimaryKey()
                .WithColumn("SkillTypeId").AsInt32()
                .WithColumn("Language1Id").AsInt32()
                .WithColumn("Language2Id").AsInt32()
                .WithColumn("DirectionTypeId").AsInt32();

            Create.ForeignKey("FK_Skill_SkillType")
                .FromTable("tblSkill")
                .ForeignColumn("SkillTypeId")
                .ToTable("tblSkillType")
                .PrimaryColumn("SkillTypeId");

            Create.ForeignKey("FK_Skill_Language1")
                .FromTable("tblSkill")
                .ForeignColumn("Language1Id")
                .ToTable("tblLanguage")
                .PrimaryColumn("LanguageId");

            Create.ForeignKey("FK_Skill_Language2")
                .FromTable("tblSkill")
                .ForeignColumn("Language2Id")
                .ToTable("tblLanguage")
                .PrimaryColumn("LanguageId");

            Create.ForeignKey("FK_Skill_DirectionType")
                .FromTable("tblSkill")
                .ForeignColumn("DirectionTypeId")
                .ToTable("tblDirectionType")
                .PrimaryColumn("DirectionTypeId");
        }

        void CreateCredentialApplicationTables()
        {
            Create.Table("tblCredentialApplicationType")
                .WithColumn("CredentialApplicationTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50)
                .WithColumn("Online").AsBoolean()
                .WithColumn("RequiresChecking").AsBoolean()
                .WithColumn("RequiresAssessment").AsBoolean()
                .WithColumn("BackOffice").AsBoolean()
                .WithColumn("PendingAllowed").AsBoolean()
                .WithColumn("AssessmentReviewAllowed").AsBoolean();

            Create.Table("tblCredentialApplicationStatusType")
                .WithColumn("CredentialApplicationStatusTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);

            Create.Table("tblCredentialApplication")
                .WithColumn("CredentialApplicationId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationTypeId").AsInt32()
                .WithColumn("CredentialApplicationStatusTypeId").AsInt32()
                .WithColumn("EnteredDate").AsDateTime()
                .WithColumn("PersonId").AsInt32()
                .WithColumn("SponsorInstitutionId").AsInt32().Nullable()
                .WithColumn("EnteredUserId").AsInt32()
                .WithColumn("ReceivingOfficeId").AsInt32()
                .WithColumn("StatusChangeDate").AsDateTime()
                .WithColumn("StatusChangeUserId").AsInt32()
                .WithColumn("OwnedByUserId").AsInt32().Nullable()
                .WithColumn("OwnedByApplicant").AsBoolean();

            Create.ForeignKey("FK_CredentialApplication_CredentialApplicationType")
                .FromTable("tblCredentialApplication")
                .ForeignColumn("CredentialApplicationTypeId")
                .ToTable("tblCredentialApplicationType")
                .PrimaryColumn("CredentialApplicationTypeId");

            Create.ForeignKey("FK_CredentialApplication_CredentialApplicationStatusType")
                .FromTable("tblCredentialApplication")
                .ForeignColumn("CredentialApplicationStatusTypeId")
                .ToTable("tblCredentialApplicationStatusType")
                .PrimaryColumn("CredentialApplicationStatusTypeId");

            Create.ForeignKey("FK_CredentialApplication_Person")
                .FromTable("tblCredentialApplication")
                .ForeignColumn("PersonId")
                .ToTable("tblPerson")
                .PrimaryColumn("PersonId");

            Create.ForeignKey("FK_CredentialApplication_Institution")
                .FromTable("tblCredentialApplication")
                .ForeignColumn("SponsorInstitutionId")
                .ToTable("tblInstitution")
                .PrimaryColumn("InstitutionId");

            Create.ForeignKey("FK_CredentialApplication_User")
                .FromTable("tblCredentialApplication")
                .ForeignColumn("EnteredUserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");

            Create.ForeignKey("FK_CredentialApplication_Office")
                .FromTable("tblCredentialApplication")
                .ForeignColumn("ReceivingOfficeId")
                .ToTable("tblOffice")
                .PrimaryColumn("OfficeId");

            Create.ForeignKey("FK_CredentialApplication_User2")
                .FromTable("tblCredentialApplication")
                .ForeignColumn("StatusChangeUserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");

            Create.ForeignKey("FK_CredentialApplication_User3")
                .FromTable("tblCredentialApplication")
                .ForeignColumn("OwnedByUserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");
        }

        void CreateCredentialApplicationFieldTables()
        {
            Create.Table("tblDataType")
                .WithColumn("DataTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);

            Create.Table("tblCredentialApplicationField")
                .WithColumn("CredentialApplicationFieldId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationTypeId").AsInt32()
                .WithColumn("Reference").AsGuid().Unique().Nullable()
                .WithColumn("Name").AsString(50)
                .WithColumn("Section").AsString(100).Nullable()
                .WithColumn("DataTypeId").AsInt32()
                .WithColumn("DefaultValue").AsString(int.MaxValue).Nullable()
                .WithColumn("PerCredentialRequest").AsBoolean()
                .WithColumn("Description").AsString(int.MaxValue).Nullable()
                .WithColumn("Mandatory").AsBoolean()
                .WithColumn("DisplayOrder").AsInt32().Nullable();

            Create.Index("IX_CredentialApplicationField_Reference")
                .OnTable("tblCredentialApplicationField")
                .OnColumn("Reference")
                .Ascending()
                .WithOptions().NonClustered();
            
            Create.ForeignKey("FK_CredentialApplicationField_CredentialApplicationType")
                .FromTable("tblCredentialApplicationField")
                .ForeignColumn("CredentialApplicationTypeId")
                .ToTable("tblCredentialApplicationType")
                .PrimaryColumn("CredentialApplicationTypeId");

            Create.ForeignKey("FK_CredentialApplicationField_DataType")
                .FromTable("tblCredentialApplicationField")
                .ForeignColumn("DataTypeId")
                .ToTable("tblDataType")
                .PrimaryColumn("DataTypeId");

            Create.Table("tblCredentialApplicationFieldData")
                .WithColumn("CredentialApplicationFieldDataId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationId").AsInt32()
                .WithColumn("CredentialApplicationFieldId").AsInt32()
                .WithColumn("Value").AsString(int.MaxValue).Nullable();

            Create.ForeignKey("FK_CredentialApplicationFieldData_CredentialApplication")
                .FromTable("tblCredentialApplicationFieldData")
                .ForeignColumn("CredentialApplicationId")
                .ToTable("tblCredentialApplication")
                .PrimaryColumn("CredentialApplicationId");

            Create.ForeignKey("FK_CredentialApplicationFieldData_CredentialApplicationField")
                .FromTable("tblCredentialApplicationFieldData")
                .ForeignColumn("CredentialApplicationFieldId")
                .ToTable("tblCredentialApplicationField")
                .PrimaryColumn("CredentialApplicationFieldId");

            Create.Index("IX_CredentialApplicationFieldData_CredentialApplicationId")
                .OnTable("tblCredentialApplicationFieldData")
                .OnColumn("CredentialApplicationId")
                .Ascending()
                .WithOptions().NonClustered();

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialApplicationFieldData] 
                ADD  CONSTRAINT [U_tblCredentialApplicationFieldData]
                UNIQUE NONCLUSTERED ([CredentialApplicationId] , [CredentialApplicationFieldId])");
        }

        void CreateCredentialApplicationNoteTable()
        {
            Create.Table("tblCredentialApplicationNote")
                .WithColumn("CredentialApplicationNoteId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationId").AsInt32()
                .WithColumn("NoteId").AsInt32();

            Create.ForeignKey("FK_CredentialApplicationNote_CredentialApplication")
                .FromTable("tblCredentialApplicationNote")
                .ForeignColumn("CredentialApplicationId")
                .ToTable("tblCredentialApplication")
                .PrimaryColumn("CredentialApplicationId");

            Create.ForeignKey("FK_CredentialApplicationNote_Note")
                .FromTable("tblCredentialApplicationNote")
                .ForeignColumn("NoteId")
                .ToTable("tblNote")
                .PrimaryColumn("NoteId");
        }

        void CreateCrentialApplicationAttachmentTables()
        {
            Create.Table("tblCredentialApplicationAttachment")
                .WithColumn("CredentialApplicationAttachmentId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationId").AsInt32()
                .WithColumn("StoredFileId").AsInt32()
                .WithColumn("Description").AsString(100).Nullable();

            Create.ForeignKey("FK_CredentialApplicationAttachment_CredentialApplication")
                .FromTable("tblCredentialApplicationAttachment")
                .ForeignColumn("CredentialApplicationId")
                .ToTable("tblCredentialApplication")
                .PrimaryColumn("CredentialApplicationId");

            Create.ForeignKey("FK_CredentialApplicationAttachment_StoredFile")
                .FromTable("tblCredentialApplicationAttachment")
                .ForeignColumn("StoredFileId")
                .ToTable("tblStoredFile")
                .PrimaryColumn("StoredFileId");
        }

        void CreateCredentialRequestTables()
        {
            Create.Table("tblCredentialCategory")
                .WithColumn("CredentialCategoryId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);

            Create.Table("tblCredentialType")
                .WithColumn("CredentialTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialCategoryId").AsInt32()
                .WithColumn("InternalName").AsString(50)
                .WithColumn("ExternalName").AsString(50)
                .WithColumn("UpgradePath").AsInt32()
                .WithColumn("Simultaneous").AsBoolean()
                .WithColumn("SkillTypeId").AsInt32()
                .WithColumn("Certification").AsBoolean();

            Create.ForeignKey("FK_CredentialType_CredentialCategory")
                .FromTable("tblCredentialType")
                .ForeignColumn("CredentialCategoryId")
                .ToTable("tblCredentialCategory")
                .PrimaryColumn("CredentialCategoryId");

            Create.ForeignKey("FK_CredentialType_SkillType")
                .FromTable("tblCredentialType")
                .ForeignColumn("SkillTypeId")
                .ToTable("tblSkillType")
                .PrimaryColumn("SkillTypeId");

            Create.Table("tblCredentialRequestStatusType")
                .WithColumn("CredentialRequestStatusTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);

            Create.Table("tblCredentialRequest")
                .WithColumn("CredentialRequestId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationId").AsInt32()
                .WithColumn("CredentialTypeId").AsInt32()
                .WithColumn("SkillId").AsInt32()
                .WithColumn("CredentialRequestStatusTypeId").AsInt32()
                .WithColumn("StatusChangeDate").AsDateTime()
                .WithColumn("StatusChangeUserId").AsInt32();

            Create.ForeignKey("FK_CredentialRequest_CredentialApplication")
                .FromTable("tblCredentialRequest")
                .ForeignColumn("CredentialApplicationId")
                .ToTable("tblCredentialApplication")
                .PrimaryColumn("CredentialApplicationId");

            Create.ForeignKey("FK_CredentialRequest_CredentialType")
                .FromTable("tblCredentialRequest")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Create.ForeignKey("FK_CredentialRequest_Skill")
                .FromTable("tblCredentialRequest")
                .ForeignColumn("SkillId")
                .ToTable("tblSkill")
                .PrimaryColumn("SkillId");

            Create.ForeignKey("FK_CredentialRequest_CredentialRequestStatusType")
                .FromTable("tblCredentialRequest")
                .ForeignColumn("CredentialRequestStatusTypeId")
                .ToTable("tblCredentialRequestStatusType")
                .PrimaryColumn("CredentialRequestStatusTypeId");

            Create.Table("tblCredentialApplicationTypeCredentialType")
                .WithColumn("CredentialApplicationTypeCredentialTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationTypeId").AsInt32()
                .WithColumn("CredentialTypeId").AsInt32()
                .WithColumn("HasTest").AsBoolean();

            Create.ForeignKey("FK_CredentialApplicationTypeCredentialType_CredentialApplicationType")
                .FromTable("tblCredentialApplicationTypeCredentialType")
                .ForeignColumn("CredentialApplicationTypeId")
                .ToTable("tblCredentialApplicationType")
                .PrimaryColumn("CredentialApplicationTypeId");

            Create.ForeignKey("FK_CredentialApplicationTypeCredentialType_CredentialType")
                .FromTable("tblCredentialApplicationTypeCredentialType")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Create.ForeignKey("FK_CredentialRequest_User")
                .FromTable("tblCredentialRequest")
                .ForeignColumn("StatusChangeUserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");
        }

        void CreateCredentialRequestFieldTables()
        {
            Create.Table("tblCredentialRequestFieldData")
                .WithColumn("CredentialRequestFieldDataId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialRequestId").AsInt32()
                .WithColumn("CredentialApplicationFieldId").AsInt32()
                .WithColumn("Value").AsString(int.MaxValue).Nullable();

            Create.ForeignKey("FK_CredentialRequestFieldData_CredentialRequest")
                .FromTable("tblCredentialRequestFieldData")
                .ForeignColumn("CredentialRequestId")
                .ToTable("tblCredentialRequest")
                .PrimaryColumn("CredentialRequestId");

            Create.ForeignKey("FK_CredentialRequestFieldData_CredentialApplicationField")
                .FromTable("tblCredentialRequestFieldData")
                .ForeignColumn("CredentialApplicationFieldId")
                .ToTable("tblCredentialApplicationField")
                .PrimaryColumn("CredentialApplicationFieldId");
        }

        void CreateCredentialTables()
        {
            Create.Table("tblCredential")
                .WithColumn("CredentialId").AsInt32().Identity().PrimaryKey()
                .WithColumn("StartDate").AsDate()
                .WithColumn("ExpiryDate").AsDate()
                .WithColumn("TerminationDate").AsDate().Nullable()
                .WithColumn("ShowInOnlineDirectory").AsBoolean();

            Create.Table("tblCredentialCredentialRequest")
                .WithColumn("CredentialCredentialRequestId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialId").AsInt32()
                .WithColumn("CredentialRequestId").AsInt32();

            Create.ForeignKey("FK_CredentialCredentialRequest_Credential")
                .FromTable("tblCredentialCredentialRequest")
                .ForeignColumn("CredentialId")
                .ToTable("tblCredential")
                .PrimaryColumn("CredentialId");

            Create.ForeignKey("FK_CredentialCredentialRequest_CredentialRequest")
                .FromTable("tblCredentialCredentialRequest")
                .ForeignColumn("CredentialRequestId")
                .ToTable("tblCredentialRequest")
                .PrimaryColumn("CredentialRequestId");
        }

        void CreateNoteTables()
        {
            Create.Table("tblEntityNote")
                .WithColumn("EntityNoteId").AsInt32().Identity().PrimaryKey()
                .WithColumn("EntityId").AsInt32()
                .WithColumn("NoteId").AsInt32();

            Create.ForeignKey("FK_EntityNote_Entity")
                .FromTable("tblEntityNote")
                .ForeignColumn("EntityId")
                .ToTable("tblEntity")
                .PrimaryColumn("EntityId");

            Create.ForeignKey("FK_EntityNote_Note")
                .FromTable("tblEntityNote")
                .ForeignColumn("NoteId")
                .ToTable("tblNote")
                .PrimaryColumn("NoteId");

            Create.Table("tblNoteAttachment")
                .WithColumn("NoteAttachmentId").AsInt32().Identity().PrimaryKey()
                .WithColumn("NoteId").AsInt32()
                .WithColumn("StoredFileId").AsInt32()
                .WithColumn("Description").AsString(100);

            Create.ForeignKey("FK_NoteAttachment_Note")
                .FromTable("tblNoteAttachment")
                .ForeignColumn("NoteId")
                .ToTable("tblNote")
                .PrimaryColumn("NoteId");

            Create.ForeignKey("FK_NoteAttachment_StoredFile")
                .FromTable("tblNoteAttachment")
                .ForeignColumn("StoredFileId")
                .ToTable("tblStoredFile")
                .PrimaryColumn("StoredFileId");

            Create.Column("ModifiedDate")
                .OnTable("tblNote")
                .AsDateTime().Nullable();
        }

        void InsertNoteDocumentType()
        {
            Insert.IntoTable("tluDocumentType").Row(new {
                Name = "General",
                DisplayName = "General document",
                ExaminerToolsDownload = false,
                ExaminerToolsUpload  = false
            });
        }

        void CreatePractitionerNumberColumn()
        {
            Create.Column("PractitionerNumber")
                .OnTable("tblPerson")
                .AsAnsiString(50)
                .Nullable();
        }
    }
}
