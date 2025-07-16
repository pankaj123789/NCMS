using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20170928_AddCredentialApplicationFormTables
{
   [NaatiMigration(201709281400)]
    public class AddCredentialApplicationFormTables: NaatiMigration
    {
        public override void Up()
        {
            CreateCredentialApplicationFormTable();
            CreateCredentialApplicationFormSectionTable();
            CreateCredentialApplicationFormAnswerTypeTable();
            CreateCredentialApplicationFormQuestionTypeTable();
            CreateCredentialApplicationFormQuestionTable();
            CreateCredentialApplicationFormActionTypeTable();
            CreateCredentialApplicationFormAnswerOptionTable();
            CreateCredentialApplicationFormAnswerOptionActionTypeTable();
            CreateCredentialApplicationFieldOptionTable();
            CreateCredentialApplicationFieldOptionOptionTable();
            CreateCredentialApplicationFormQuestionAnswerOptionTable();
            CreateCredentialApplicationFormAnswerOptionDocumentTypeTable();
            CreateCredentialApplicationFormQuestionLogicTable();
            CreateFielOptionColumntoCredentialApplicationFieldDataTable();
        }


        void CreateFielOptionColumntoCredentialApplicationFieldDataTable()
        {
            Create.Column("CredentialApplicationFieldOptionOptionId")
                .OnTable("tblCredentialApplicationFieldData")
                .AsInt32()
                .Nullable();

            Create.ForeignKey("FK_CredentialApplicationFieldData_CredentialApplicationFieldOptionOption")
                .FromTable("tblCredentialApplicationFieldData")
                .ForeignColumn("CredentialApplicationFieldOptionOptionId")
                .ToTable("tblCredentialApplicationFieldOptionOption")
                .PrimaryColumn("CredentialApplicationFieldOptionOptionId");

        }

        public override void Down()
        {
            throw new NotImplementedException();
        }

        void CreateCredentialApplicationFormTable()
        {
            Create.Table("tblCredentialApplicationForm")
                .WithColumn("CredentialApplicationFormId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationTypeId").AsInt32()
                .WithColumn("Name").AsString(100)
                .WithColumn("Description").AsString(int.MaxValue)
                .WithColumn("LoginRequired").AsBoolean();

            Create.ForeignKey("FK_CredentialApplicationForm_CredentialApplicationType")
                .FromTable("tblCredentialApplicationForm")
                .ForeignColumn("CredentialApplicationTypeId")
                .ToTable("tblCredentialApplicationType")
                .PrimaryColumn("CredentialApplicationTypeId");
        }

        void CreateCredentialApplicationFormSectionTable()
        {
            Create.Table("tblCredentialApplicationFormSection")
                .WithColumn("CredentialApplicationFormSectionId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationFormId").AsInt32()
                .WithColumn("Name").AsString(100)
                .WithColumn("DisplayOrder").AsInt32().Nullable()
                .WithColumn("Description").AsString(int.MaxValue);

            Create.ForeignKey("FK_CredentialApplicationFormSection_CredentialApplicationForm")
                .FromTable("tblCredentialApplicationFormSection")
                .ForeignColumn("CredentialApplicationFormId")
                .ToTable("tblCredentialApplicationForm")
                .PrimaryColumn("CredentialApplicationFormId");
        }

        void CreateCredentialApplicationFormAnswerTypeTable()
        {
            Create.Table("tblCredentialApplicationFormAnswerType")
                .WithColumn("CredentialApplicationFormAnswerTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50)
                .WithColumn("DisplayName").AsString(100);
        }

        void CreateCredentialApplicationFormQuestionTypeTable()
        {
            Create.Table("tblCredentialApplicationFormQuestionType")
                .WithColumn("CredentialApplicationFormQuestionTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Text").AsString(int.MaxValue)
                .WithColumn("CredentialApplicationFormAnswerTypeId").AsInt32()
                .WithColumn("Description").AsString(int.MaxValue);

            Create.ForeignKey("FK_CredentialApplicationFormQuestionType_CredentialApplicationFormAnswerType")
                .FromTable("tblCredentialApplicationFormQuestionType")
                .ForeignColumn("CredentialApplicationFormAnswerTypeId")
                .ToTable("tblCredentialApplicationFormAnswerType")
                .PrimaryColumn("CredentialApplicationFormAnswerTypeId");
        }

        void CreateCredentialApplicationFormQuestionTable()
        {
            Create.Table("tblCredentialApplicationFormQuestion")
                .WithColumn("CredentialApplicationFormQuestionId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationFormSectionId").AsInt32()
                .WithColumn("CredentialApplicationFormQuestionTypeId").AsInt32()
                .WithColumn("CredentialApplicationFieldId").AsInt32().Nullable()
                .WithColumn("DisplayOrder").AsInt32().Nullable();

            Create.ForeignKey("FK_CredentialApplicationFormQuestion_CredentialApplicationFormSection")
                .FromTable("tblCredentialApplicationFormQuestion")
                .ForeignColumn("CredentialApplicationFormSectionId")
                .ToTable("tblCredentialApplicationFormSection")
                .PrimaryColumn("CredentialApplicationFormSectionId");

            Create.ForeignKey("FK_CredentialApplicationFormQuestion_CredentialApplicationFormQuestionType")
                .FromTable("tblCredentialApplicationFormQuestion")
                .ForeignColumn("CredentialApplicationFormQuestionTypeId")
                .ToTable("tblCredentialApplicationFormQuestionType")
                .PrimaryColumn("CredentialApplicationFormQuestionTypeId");

            Create.ForeignKey("FK_CredentialApplicationFormQuestion_CredentialApplicationField")
                .FromTable("tblCredentialApplicationFormQuestion")
                .ForeignColumn("CredentialApplicationFieldId")
                .ToTable("tblCredentialApplicationField")
                .PrimaryColumn("CredentialApplicationFieldId");
        }

        void CreateCredentialApplicationFormActionTypeTable()
        {
            Create.Table("tblCredentialApplicationFormActionType")
                .WithColumn("CredentialApplicationFormActionTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50)
                .WithColumn("DisplayName").AsString(50);
        }

        void CreateCredentialApplicationFormAnswerOptionTable()
        {
            Create.Table("tblCredentialApplicationFormAnswerOption")
                .WithColumn("CredentialApplicationFormAnswerOptionId").AsInt32().Identity().PrimaryKey()
                .WithColumn("[Option]").AsString(int.MaxValue)
                .WithColumn("CredentialApplicationFormQuestionTypeId").AsInt32()
                .WithColumn("Description").AsString(int.MaxValue);

            Create.ForeignKey("FK_CredentialApplicationFormAnswerOption_CredentialApplicationFormQuestionType")
                .FromTable("tblCredentialApplicationFormAnswerOption")
                .ForeignColumn("CredentialApplicationFormQuestionTypeId")
                .ToTable("tblCredentialApplicationFormQuestionType")
                .PrimaryColumn("CredentialApplicationFormQuestionTypeId");
        }

        void CreateCredentialApplicationFormAnswerOptionActionTypeTable()
        {
            Create.Table("tblCredentialApplicationFormAnswerOptionActionType")
                .WithColumn("CredentialApplicationFormAnswerOptionActionTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationFormAnswerOptionId").AsInt32()
                .WithColumn("CredentialApplicationFormActionTypeId").AsInt32()
                .WithColumn("Parameter").AsString().Nullable()
                .WithColumn("[Order]").AsInt32().Nullable();

            Create.ForeignKey("FK_tblCredentialApplicationFormAnswerOptionActionType_CredentialApplicationFormAnswerOption")
                .FromTable("tblCredentialApplicationFormAnswerOptionActionType")
                .ForeignColumn("CredentialApplicationFormAnswerOptionId")
                .ToTable("tblCredentialApplicationFormAnswerOption")
                .PrimaryColumn("CredentialApplicationFormAnswerOptionId");

            Create.ForeignKey("FK_tblCredentialApplicationFormAnswerOptionActionType_CredentialApplicationFormActionType")
                .FromTable("tblCredentialApplicationFormAnswerOptionActionType")
                .ForeignColumn("CredentialApplicationFormActionTypeId")
                .ToTable("tblCredentialApplicationFormActionType")
                .PrimaryColumn("CredentialApplicationFormActionTypeId");
        }

        void CreateCredentialApplicationFormQuestionAnswerOptionTable()
        {
            Create.Table("tblCredentialApplicationFormQuestionAnswerOption")
                .WithColumn("CredentialApplicationFormQuestionAnswerOptionId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationFormQuestionId").AsInt32()
                .WithColumn("CredentialApplicationFormAnswerOptionId").AsInt32()
                .WithColumn("DefaultAnswer").AsBoolean()
                .WithColumn("CredentialApplicationFieldId").AsInt32().Nullable()
                .WithColumn("DisplayOrder").AsInt32().NotNullable()
                .WithColumn("CredentialApplicationFieldOptionOptionId").AsInt32().Nullable()
                .WithColumn("FieldData").AsString().Nullable();

            Create.ForeignKey("FK_CredentialApplicationFormQuestionAnswerOption_CredentialApplicationFormQuestion")
                .FromTable("tblCredentialApplicationFormQuestionAnswerOption")
                .ForeignColumn("CredentialApplicationFormQuestionId")
                .ToTable("tblCredentialApplicationFormQuestion")
                .PrimaryColumn("CredentialApplicationFormQuestionId");
            
            Create.ForeignKey("FK_CredentialApplicationFormQuestionAnswerOption_CredentialApplicationFormAnswerOption")
                .FromTable("tblCredentialApplicationFormQuestionAnswerOption")
                .ForeignColumn("CredentialApplicationFormAnswerOptionId")
                .ToTable("tblCredentialApplicationFormAnswerOption")
                .PrimaryColumn("CredentialApplicationFormAnswerOptionId");

            Create.ForeignKey("FK_CredentialApplicationFormQuestionAnswerOption_CredentialApplicationField")
                .FromTable("tblCredentialApplicationFormQuestionAnswerOption")
                .ForeignColumn("CredentialApplicationFieldId")
                .ToTable("tblCredentialApplicationField")
                .PrimaryColumn("CredentialApplicationFieldId");

            Create.ForeignKey("FK_CredentialApplicationFormQuestionAnswerOption_CredentialApplicationFieldOptionOption")
                .FromTable("tblCredentialApplicationFormQuestionAnswerOption")
                .ForeignColumn("CredentialApplicationFieldOptionOptionId")
                .ToTable("tblCredentialApplicationFieldOptionOption")
                .PrimaryColumn("CredentialApplicationFieldOptionOptionId");
        }

        void CreateCredentialApplicationFormAnswerOptionDocumentTypeTable()
        {
            Create.Table("tblCredentialApplicationFormAnswerOptionDocumentType")
                .WithColumn("CredentialApplicationFormAnswerOptionDocumentTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationFormAnswerOptionId").AsInt32()
                .WithColumn("DocumentTypeId").AsInt32();
            
            Create.ForeignKey("FK_CredentialApplicationFormAnswerOptionDocumentType_CredentialApplicationFormAnswerOption")
                .FromTable("tblCredentialApplicationFormAnswerOptionDocumentType")
                .ForeignColumn("CredentialApplicationFormAnswerOptionId")
                .ToTable("tblCredentialApplicationFormAnswerOption")
                .PrimaryColumn("CredentialApplicationFormAnswerOptionId");

            Create.ForeignKey("FK_CredentialApplicationFormAnswerOptionDocumentType_DocumentType")
                .FromTable("tblCredentialApplicationFormAnswerOptionDocumentType")
                .ForeignColumn("DocumentTypeId")
                .ToTable("tluDocumentType")
                .PrimaryColumn("DocumentTypeId");
        }

        void CreateCredentialApplicationFieldOptionTable()
        {
            Create.Table("tblCredentialApplicationFieldOption")
                .WithColumn("CredentialApplicationFieldOptionId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50)
                .WithColumn("DisplayName").AsString(255);
        }

        void CreateCredentialApplicationFieldOptionOptionTable()
        {
            Create.Table("tblCredentialApplicationFieldOptionOption")
                .WithColumn("CredentialApplicationFieldOptionOptionId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationFieldId").AsInt32()
                .WithColumn("CredentialApplicationFieldOptionId").AsInt32();

            Create.ForeignKey("FK_CredentialApplicationFieldOptionOption_CredentialApplicationField")
                .FromTable("tblCredentialApplicationFieldOptionOption")
                .ForeignColumn("CredentialApplicationFieldId")
                .ToTable("tblCredentialApplicationField")
                .PrimaryColumn("CredentialApplicationFieldId");

            Create.ForeignKey("FK_CredentialApplicationFieldOptionOption_CredentialApplicationFieldOption")
                .FromTable("tblCredentialApplicationFieldOptionOption")
                .ForeignColumn("CredentialApplicationFieldOptionId")
                .ToTable("tblCredentialApplicationFieldOption")
                .PrimaryColumn("CredentialApplicationFieldOptionId");
        }

        void CreateCredentialApplicationFormQuestionLogicTable()
        {
            Create.Table("tblCredentialApplicationFormQuestionLogic")
                .WithColumn("CredentialApplicationFormQuestionLogicId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationFormQuestionId").AsInt32()
                .WithColumn("CredentialApplicationFormQuestionAnswerOptionId").AsInt32()
                .WithColumn("[Not]").AsBoolean()
                .WithColumn("[And]").AsBoolean();

            Create.ForeignKey("FK_CredentialApplicationFormQuestionLogic_CredentialApplicationFormQuestion")
                .FromTable("tblCredentialApplicationFormQuestionLogic")
                .ForeignColumn("CredentialApplicationFormQuestionId")
                .ToTable("tblCredentialApplicationFormQuestion")
                .PrimaryColumn("CredentialApplicationFormQuestionId");

            Create.ForeignKey("FK_CredentialApplicationFormQuestionLogic_CredentialApplicationFormQuestionAnswerOption")
                .FromTable("tblCredentialApplicationFormQuestionLogic")
                .ForeignColumn("CredentialApplicationFormQuestionAnswerOptionId")
                .ToTable("tblCredentialApplicationFormQuestionAnswerOption")
                .PrimaryColumn("CredentialApplicationFormQuestionAnswerOptionId");
        }
    }
}
