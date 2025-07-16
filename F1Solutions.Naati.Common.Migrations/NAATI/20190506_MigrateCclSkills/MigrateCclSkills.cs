
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190506_MigrateCclSkills
{
    [NaatiMigration(201905061002)]
    public class MigrateCclSkills : NaatiMigration
    {
        public override void Up()
        {

            Execute.Sql(@" SET IDENTITY_INSERT [dbo].tblCredentialApplicationType ON
                            INSERT INTO tblCredentialApplicationType (CredentialApplicationTypeId, Name, DisplayName, Online, RequiresChecking, RequiresAssessment, BackOffice,PendingAllowed, AssessmentReviewAllowed, DisplayBills,CredentialApplicationTypeCategoryId,ModifiedByNaati,ModifiedDate,ModifiedUser, AllowMultiple)
                            VALUES  (13,'CCL V3','Credentialed Community Language Test','1','1','0','1','0','0','0','1','0','2019-05-03 17:09:17.893','40','0')
                            SET IDENTITY_INSERT [dbo].tblCredentialApplicationType OFF");
            Execute.Sql(@"INSERT INTO TBLSKILLAPPLICATIONTYPE (SKILLID, CREDENTIALAPPLICATIONTYPEID, MODIFIEDBYNAATI, MODIFIEDDATE, MODIFIEDUSER)
                          SELECT SKILLID, 13,MODIFIEDBYNAATI,GETDATE(),  MODIFIEDUSER FROM TBLSKILLAPPLICATIONTYPE WHERE CREDENTIALAPPLICATIONTYPEID = 7");

            Execute.Sql("UPDATE TBLCREDENTIALREQUEST SET CREDENTIALREQUESTSTATUSTYPEID=19 WHERE CREDENTIALREQUESTSTATUSTYPEID =18 ");
        }
    }
}
