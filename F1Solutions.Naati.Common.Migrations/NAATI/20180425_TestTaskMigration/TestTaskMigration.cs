namespace F1Solutions.Naati.Common.Migrations.NAATI._20180425_TestTaskMigration
{
    [NaatiMigration(201804250900)]
    public class TestTaskMigration: NaatiMigration
    {
        public override void Up()
        {
            MigrateTestMaterial();
            MigrateTestSittingTestMaterial();
        }

        void MigrateTestMaterial()
        {
            Execute.Sql(@"UPDATE TM 
            SET TM.TESTCOMPONENTTYPEID = 1
            FROM tbltestmaterial AS TM
            INNER JOIN tblTestSittingTestMaterial  AS TSTM 
            ON TM.TESTMATERIALID = TSTM.TESTMATERIALID   
            INNER JOIN TBLTESTSITTING  AS TS 
            ON TS.TESTSITTINGID = TSTM.TESTSITTINGID   
            INNER JOIN TBLCREDENTIALREQUEST  AS CR 
            ON CR.CREDENTIALREQUESTID = TS.CREDENTIALREQUESTID         
            WHERE CR.CREDENTIALTYPEID = 14");

            Execute.Sql(@"UPDATE TM 
            SET TM.TESTCOMPONENTTYPEID = 2
            FROM tbltestmaterial AS TM
            INNER JOIN tblTestSittingTestMaterial  AS TSTM 
            ON TM.TESTMATERIALID = TSTM.TESTMATERIALID   
            INNER JOIN TBLTESTSITTING  AS TS 
            ON TS.TESTSITTINGID = TSTM.TESTSITTINGID   
            INNER JOIN TBLCREDENTIALREQUEST  AS CR 
            ON CR.CREDENTIALREQUESTID = TS.CREDENTIALREQUESTID         
            WHERE CR.CREDENTIALTYPEID = 2");
        }

        void MigrateTestSittingTestMaterial()
        {
            Execute.Sql("ALTER TABLE tblTestSittingTestMaterial DROP CONSTRAINT FK_TestSittingTestMaterial_TestComponentType");
            Execute.Sql("ALTER TABLE tblTestSittingTestMaterial DROP CONSTRAINT U_tblTestSittingTestMaterial");
            Delete.Column("TestComponentTypeId").FromTable("tblTestSittingTestMaterial");
            Create.Column("TestComponentId").OnTable("tblTestSittingTestMaterial").AsInt32().Nullable();

            Create.ForeignKey("FK_TestSittingTestMaterial_TestComponent")
                .FromTable("tblTestSittingTestMaterial")
                .ForeignColumn("TestComponentId")
                .ToTable("tblTestComponent")
                .PrimaryColumn("TestComponentId");
           

            Execute.Sql(@"UPDATE TSTM 
            SET TSTM.TESTCOMPONENTID = 1
            FROM tblTestSittingTestMaterial  AS TSTM             
            INNER JOIN TBLTESTSITTING  AS TS 
            ON TS.TESTSITTINGID = TSTM.TESTSITTINGID   
            INNER JOIN TBLCREDENTIALREQUEST  AS CR 
            ON CR.CREDENTIALREQUESTID = TS.CREDENTIALREQUESTID   
            INNER JOIN TBLCREDENTIALAPPLICATION CA
            ON CA.CREDENTIALAPPLICATIONID =   CR.CREDENTIALAPPLICATIONID   
            WHERE CA.CredentialApplicationTypeId = 3 OR  CA.CredentialApplicationTypeId = 7"); 

            Execute.Sql(@"UPDATE TSTM 
            SET TSTM.TESTCOMPONENTID = 3
            FROM tblTestSittingTestMaterial  AS TSTM             
            INNER JOIN TBLTESTSITTING  AS TS 
            ON TS.TESTSITTINGID = TSTM.TESTSITTINGID   
            INNER JOIN TBLCREDENTIALREQUEST  AS CR 
            ON CR.CREDENTIALREQUESTID = TS.CREDENTIALREQUESTID         
            INNER JOIN TBLCREDENTIALAPPLICATION CA
            ON CA.CREDENTIALAPPLICATIONID =   CR.CREDENTIALAPPLICATIONID   
            WHERE CA.CredentialApplicationTypeId = 2 OR  CA.CredentialApplicationTypeId = 6");

           Alter.Column("TestComponentId").OnTable("tblTestSittingTestMaterial").AsInt32().NotNullable();
            Execute.Sql(@"
                ALTER TABLE [dbo].[tblTestSittingTestMaterial] 
                ADD  CONSTRAINT [U_tblTestSittingTestMaterial]
                UNIQUE NONCLUSTERED ([TestSittingId] , [TestMaterialId], [TestComponentId])");

        }
    }
}
