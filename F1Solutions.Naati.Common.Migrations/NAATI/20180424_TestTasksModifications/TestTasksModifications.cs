
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180424_TestTasksModifications
{
    [NaatiMigration(201804240800)]
    public class TestTasksModifications : NaatiMigration
    {
        public override void Up()
        {
            ModifyTestComponentType();
            ModifyTestSpecification();
            ModifyTestMarkingScheme();
            
            CreateTestSpecStandardMarkingScheme();
            ModiyTestSitting();
            ModifyTestResult();
            ModifyTestSpecificationMapping();
            ModifyTestSittingTestMaterial();
        }

        void ModifyTestComponentType()
        {
            Execute.Sql("ALTER TABLE tblTestComponentType DROP  CONSTRAINT FK_TestComponentType_TestMarkingScheme");
            Delete.Column("TestMarkingSchemeId").FromTable("tblTestComponentType");
            
            Execute.Sql("ALTER TABLE tblTestComponentType DROP CONSTRAINT FK_TestComponentType_CredentialType");
            Delete.Column("CredentialTypeId").FromTable("tblTestComponentType");
            Create.Column("[Label]").OnTable("tblTestComponentType").AsString(50).Nullable();
            Create.Column("TestSpecificationId").OnTable("tblTestComponentType").AsInt32().Nullable();

            Create.ForeignKey("FK_TestComponentType_TestSpecification")
                .FromTable("tblTestComponentType")
                .ForeignColumn("TestSpecificationId")
                .ToTable("tblTestSpecification")
                .PrimaryColumn("TestSpecificationId");

            Execute.Sql("UPDATE tblTestComponentType SET TestSpecificationId=1 WHERE TestComponentTypeId = 1");
            Execute.Sql("UPDATE tblTestComponentType SET TestSpecificationId=2 WHERE TestComponentTypeId = 2");
            Execute.Sql("UPDATE tblTestComponentType SET TestSpecificationId=2 WHERE TestComponentTypeId = 3");

            Alter.Column("TestSpecificationId").OnTable("tblTestComponentType").AsInt32().NotNullable();

        }

        void ModifyTestMarkingScheme()
        {
            
            Rename.Table("tblTestMarkingScheme").InSchema("dbo").To("tblTestComponentTypeStandardMarkingScheme");
            Rename.Column("TestMarkingSchemeId")
                .OnTable("tblTestComponentTypeStandardMarkingScheme")
                .To("TestComponentTypeStandardMarkingSchemeId");

            Delete.Column("DisplayName").FromTable("tblTestComponentTypeStandardMarkingScheme");

            Create.Column("TestComponentTypeId").OnTable("tblTestComponentTypeStandardMarkingScheme").AsInt32().Nullable();

            Create.ForeignKey("FK_TestComponentTypeStandardMarkingScheme_TestComponentType")
                .FromTable("tblTestComponentTypeStandardMarkingScheme")
                .ForeignColumn("TestComponentTypeId")
                .ToTable("tblTestComponentType")
                .PrimaryColumn("TestComponentTypeId");
        }

        void ModifyTestSpecification()
        {
            Execute.Sql("ALTER TABLE tblTestSpecification DROP CONSTRAINT FK_TestSpecification_TestMarkingScheme");
            Delete.Column("OverallPassMark").FromTable("tblTestSpecification");
            Delete.Column("TestMarkingSchemeId").FromTable("tblTestSpecification");
            Create.Column("CredentialTypeId").OnTable("tblTestSpecification").AsInt32().Nullable();
            Create.Column("Active").OnTable("tblTestSpecification").AsBoolean().WithDefaultValue(1);

            Create.ForeignKey("FK_TestSpecification_CredentialType")
                .FromTable("tblTestSpecification")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Execute.Sql("SET IDENTITY_INSERT [dbo].TBLTESTSPECIFICATION ON");
            Execute.Sql("UPDATE TBLTESTSPECIFICATION SET CREDENTIALTYPEID=14 WHERE TESTSPECIFICATIONID = 1");
            Execute.Sql("UPDATE TBLTESTSPECIFICATION SET CREDENTIALTYPEID=2 WHERE TESTSPECIFICATIONID = 2");
            Execute.Sql("INSERT INTO  TBLTESTSPECIFICATION(TESTSPECIFICATIONID,Description, CredentialTypeId ) VALUES(3,'',16)");
            Execute.Sql("INSERT INTO  TBLTESTSPECIFICATION(TESTSPECIFICATIONID,Description, CredentialTypeId ) VALUES(100,'',2)");
            Execute.Sql("SET IDENTITY_INSERT [dbo].TBLTESTSPECIFICATION OFF");

            Alter.Column("CredentialTypeId").OnTable("tblTestSpecification").AsInt32().NotNullable();
        }

        void CreateTestSpecStandardMarkingScheme()
        {
            Create.Table("tblTestSpecificationStandardMarkingScheme")
                .WithColumn("TestSpecificationStandardMarkingSchemeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("TestSpecificationId").AsInt32()
                .WithColumn("OverallPassMark").AsInt32();


            Create.ForeignKey("FK_TestSpecificationStandardMarkingScheme_TestSpecification")
                .FromTable("tblTestSpecificationStandardMarkingScheme")
                .ForeignColumn("TestSpecificationId")
                .ToTable("tblTestSpecification")
                .PrimaryColumn("TestSpecificationId");
        }

        void ModiyTestSitting()
        {
            Create.Column("TestSpecificationId").OnTable("tblTestSitting").AsInt32().Nullable();

            Create.ForeignKey("FK_TestSitting_TestSpecification")
                .FromTable("tblTestSitting")
                .ForeignColumn("TestSpecificationId")
                .ToTable("tblTestSpecification")
                .PrimaryColumn("TestSpecificationId");

            Execute.Sql(@"UPDATE TS 
            SET TS.TESTSPECIFICATIONID = 100
            FROM TBLTESTSITTING AS TS");
            Alter.Column("TestSpecificationId").OnTable("tblTestSitting").AsInt32().NotNullable();

            Execute.Sql(@"UPDATE TS 
            SET TS.TESTSPECIFICATIONID = 1
            FROM TBLTESTSITTING AS TS
            INNER JOIN TBLCREDENTIALREQUEST  AS CR 
            ON CR.CREDENTIALREQUESTID = TS.CREDENTIALREQUESTID          
            WHERE CR.CREDENTIALTYPEID = 14");

            Execute.Sql(@"UPDATE TS 
            SET TS.TESTSPECIFICATIONID = 2
            FROM TBLTESTSITTING AS TS
            INNER JOIN TBLCREDENTIALREQUEST  AS CR 
            ON CR.CREDENTIALREQUESTID = TS.CREDENTIALREQUESTID         
            WHERE CR.CREDENTIALTYPEID = 2");

            Execute.Sql(@"UPDATE TS 
            SET TS.TESTSPECIFICATIONID = 3
            FROM TBLTESTSITTING AS TS
            INNER JOIN TBLCREDENTIALREQUEST  AS CR 
            ON CR.CREDENTIALREQUESTID = TS.CREDENTIALREQUESTID         
            WHERE CR.CREDENTIALTYPEID = 16");
            

        }

        void ModifyTestResult()
        {
            Delete.Column("TestSpecificationMappingId").FromTable("tblTestResult");
        }

        void ModifyTestSpecificationMapping()
        {
            Delete.Table("tblTestSpecificationMapping");
        }

        void ModifyTestSittingTestMaterial()
        {
            Create.Column("TestComponentTypeId").OnTable("tblTestSittingTestMaterial").AsInt32().Nullable();

            Create.ForeignKey("FK_TestSittingTestMaterial_TestComponentType")
                .FromTable("tblTestSittingTestMaterial")
                .ForeignColumn("TestComponentTypeId")
                .ToTable("tblTestComponentType")
                .PrimaryColumn("TestComponentTypeId");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblTestSittingTestMaterial] 
                ADD  CONSTRAINT [U_tblTestSittingTestMaterial]
                UNIQUE NONCLUSTERED ([TestSittingId] , [TestMaterialId], [TestComponentTypeId])");
        }

      
    }
    //Use UC-BackOffice-4000.1 Test Specification.xlsx as a reference for Config Data.


    //tblTestComponentType
    //• Added Label.
    //• Removed field TestMarkingSchemeId (was originally introduced on v1.6, and is not in Prod).

    //tblTestCompTypeStandardMarkingScheme[Renamed from tblTestMarkingScheme]
    //• Rename table.
    //• Remove field DisplayName.

    //tblTestSpecification
    //• Remove field OverallPassMark
    //• Remove field TestMarkingSchemeId

    //tblTestSpecStandardMarkingScheme [new table]


    //tblTestSpecificationMapping
    //• Remove SkillId
    //• Migration needed to set the new values.Might need to delete all records, modify the schema, then insert the new values.

    //tblTestSittingTestMaterial
    //• Added field TestComponentTypeId(fk)
    //• New Constraint(TestSittingId, TestMaterialId and TestComponentTypeId must all be unique)

    //tblTestSpecificationTestComponentType
    //• Comment out this table.We will not have the time in this release to implement the required functionality.  But we will be able to use this table for v1.7 release.

    //tblTestSitting:
    //• Add field TestSpecificationId (FK to Test Specification). 
}