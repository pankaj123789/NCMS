
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180125_TestMaterialMigration
{
    [NaatiMigration(201801251600)]
    public class TestMaterialMigration : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"DROP TABLE tblMaterial
                          ALTER TABLE [dbo].[tblWorkshopMaterial] DROP CONSTRAINT [FK_tblWorkshopMaterial_tblTestMaterial]
                          ALTER TABLE [dbo].[tblTestAttendance] DROP CONSTRAINT [FK_tblTestAttendance_tblTestMaterial]
                          DROP TABLE tblTestMaterial");


            Create.Table("tblTestMaterial")
              .WithColumn("TestMaterialId").AsInt32().NotNullable().PrimaryKey().Identity()
              .WithColumn("LanguageId").AsInt32().NotNullable()
              .WithColumn("CredentialTypeId").AsInt32().NotNullable()
              .WithColumn("Available").AsBoolean().NotNullable()
              .WithColumn("Title").AsString(255).NotNullable()
              .WithColumn("Notes").AsString(1000).Nullable();

            Create.ForeignKey("FK_TestMaterial_Language")
             .FromTable("tblTestMaterial")
             .ForeignColumn("LanguageId")
             .ToTable("tblLanguage")
             .PrimaryColumn("LanguageId");

            Create.ForeignKey("FK_TestMaterial_CredentialType")
            .FromTable("tblTestMaterial")
            .ForeignColumn("CredentialTypeId")
            .ToTable("tblCredentialType")
            .PrimaryColumn("CredentialTypeId");



            Create.Table("tblTestMaterialAttachment")
                .WithColumn("TestMaterialAttachmentId").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("StoredFileId").AsInt32().NotNullable()
                .WithColumn("TestMaterialId").AsInt32().NotNullable()
                .WithColumn("Title").AsString(255).Nullable()
                .WithColumn("Deleted").AsBoolean().NotNullable();

            Create.ForeignKey("FK_TestMaterialAttachment_StoredFile")
             .FromTable("tblTestMaterialAttachment")
             .ForeignColumn("StoredFileId")
             .ToTable("tblStoredFile")
             .PrimaryColumn("StoredFileId");

            Create.ForeignKey("FK_TestMaterialAttachment_TestMaterial")
            .FromTable("tblTestMaterialAttachment")
            .ForeignColumn("TestMaterialId")
            .ToTable("tblTestMaterial")
            .PrimaryColumn("TestMaterialId");


            Execute.Sql(@"ALTER TABLE [dbo].[tblWorkshopMaterial]  WITH CHECK ADD  CONSTRAINT [FK_tblWorkshopMaterial_tblTestMaterial] FOREIGN KEY([TestMaterialId])
                          REFERENCES [dbo].[tblTestMaterial] ([TestMaterialId])
                            
                          ALTER TABLE [dbo].[tblTestAttendance]  WITH NOCHECK ADD  CONSTRAINT [FK_tblTestAttendance_tblTestMaterial] FOREIGN KEY([TestMaterialId])
                          REFERENCES [dbo].[tblTestMaterial] ([TestMaterialId])
                          NOT FOR REPLICATION ");


            Execute.Sql(@"
                             SET IDENTITY_INSERT tblDocumentTypeCategory ON
                            insert into tblDocumentTypeCategory (DocumentTypeCategoryId,Name,DisplayName)                           
                            values (4,'TestMaterial','Test Material')
                            SET IDENTITY_INSERT tblDocumentTypeCategory OFF


                           SET IDENTITY_INSERT tluDocumentType ON
                          if not exists(select null from tluDocumentType where DocumentTypeId in (30,31))
                          begin
                            INSERT [dbo].[tluDocumentType] ([DocumentTypeId], [Name], [DisplayName], [ExaminerToolsDownload],[ExaminerToolsUpload], [DocumentTypeCategoryId]) 
                            VALUES (30, N'CandidateTestMaterial', N'Candidate Test Material',0,0,4),
                            (31, N'ExaminerTestMaterial', N'Examiner Test Material',0,0,4)
                          end
                          SET IDENTITY_INSERT tluDocumentType OFF
                        
                        ");
          
        }
    }
}
