
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181127_AddCredentialApplicationIdToCertificationPeriod
{

    [NaatiMigration(201811271122)]
    public class AddCredentialApplicationIdToCertificationPeriod : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("CredentialApplicationId").OnTable("tblCertificationPeriod").AsInt32().Nullable();

            // Set CredentialapplicationId for recertification applications
            Execute.Sql(@"WITH dataToMigrate AS (select distinct(credentialApplication.CredentialApplicationId) as credentialApplicationId, credential.CertificationPeriodId as certificationPeriodId  from tblCredentialRequest credentialRequest
                        inner join tblCredentialApplication credentialApplication 
                        on credentialRequest.CredentialApplicationId = credentialApplication.CredentialApplicationId
                        inner join tblCredentialApplicationType credentialApplicationType
                        on credentialApplicationType.CredentialApplicationTypeId = credentialApplication.CredentialApplicationTypeId
                        inner join tblCredentialCredentialRequest credentialCredentialRequest
                        on credentialCredentialRequest.CredentialRequestId = credentialRequest.CredentialRequestId
                        inner join tblCredential credential on credential.CredentialId = credentialCredentialRequest.CredentialId
                        where 
                        credentialRequest.CredentialRequestStatusTypeId = 12  and-- Credential issued 
                        credentialApplicationType.CredentialApplicationTypeCategoryId = 3)--Recertfication

                        UPDATE tblCertificationPeriod 
                        SET 
                        CredentialApplicationId = cte.CredentialApplicationId
                        FROM
                        dataToMigrate cte
                        where tblCertificationPeriod.CertificationPeriodId = cte.CertificationPeriodId;");

            //Set CredentialApplicationId for transition applications
            Execute.Sql(@"WITH dataToMigrate AS (select distinct(credentialApplication.CredentialApplicationId) as credentialApplicationId, credential.CertificationPeriodId as certificationPeriodId  from tblCredentialRequest credentialRequest
                        inner join tblCredentialApplication credentialApplication 
                        on credentialRequest.CredentialApplicationId = credentialApplication.CredentialApplicationId
                        inner join tblCredentialApplicationType credentialApplicationType
                        on credentialApplicationType.CredentialApplicationTypeId = credentialApplication.CredentialApplicationTypeId
                        inner join tblCredentialCredentialRequest credentialCredentialRequest
                        on credentialCredentialRequest.CredentialRequestId = credentialRequest.CredentialRequestId
                        inner join tblCredential credential on credential.CredentialId = credentialCredentialRequest.CredentialId
                        inner join tblCertificationPeriod certificationPeriod on certificationPeriod.CertificationPeriodId = credential.CertificationPeriodId
                        where 
                        credentialRequest.CredentialRequestStatusTypeId = 12  and-- Credential issued and
                        credentialApplicationType.CredentialApplicationTypeCategoryId = 2 and--Transition
                        certificationPeriod.CredentialApplicationId is null) -- Doesnt have recertification applications

                        UPDATE tblCertificationPeriod 
                        SET 
                        CredentialApplicationId = cte.CredentialApplicationId
                        FROM
                        dataToMigrate cte
                        where tblCertificationPeriod.CertificationPeriodId = cte.CertificationPeriodId;");

            //Set CredentialApplicationId for certification applications
            Execute.Sql(@"WITH dataToMigrate AS (select distinct(credentialApplication.CredentialApplicationId) as credentialApplicationId, credential.CertificationPeriodId as certificationPeriodId  from tblCredentialRequest credentialRequest
                        inner join tblCredentialApplication credentialApplication 
                        on credentialRequest.CredentialApplicationId = credentialApplication.CredentialApplicationId
                        inner join tblCredentialApplicationType credentialApplicationType
                        on credentialApplicationType.CredentialApplicationTypeId = credentialApplication.CredentialApplicationTypeId
                        inner join tblCredentialCredentialRequest credentialCredentialRequest
                        on credentialCredentialRequest.CredentialRequestId = credentialRequest.CredentialRequestId
                        inner join tblCredential credential on credential.CredentialId = credentialCredentialRequest.CredentialId
                        inner join tblCertificationPeriod certificationPeriod on certificationPeriod.CertificationPeriodId = credential.CertificationPeriodId
                        inner join tblCredentialtype credentialType on credentialType.CredentialTypeId = credentialRequest.CredentialTypeId
                        where 
                        credentialRequest.CredentialRequestStatusTypeId = 12  and-- Credential issued and
                        credentialApplicationType.CredentialApplicationTypeCategoryId = 1 and-- None
                        certificationPeriod.CredentialApplicationId is null and -- Doesnt have recertification or transition applications
                        credentialType.certification = 1 )--Not really necessary, just to make sure migration is OK

                        UPDATE tblCertificationPeriod 
                        SET 
                        CredentialApplicationId = cte.CredentialApplicationId
                        FROM
                        dataToMigrate cte
                        where tblCertificationPeriod.CertificationPeriodId = cte.CertificationPeriodId;");

            // Process certification periods without credentials
            Execute.Sql(@"WITH dataToMigrate AS (select certificationPeriod.certificationPeriodId CertificationPeriodId, credentialApplication.CredentialApplicationId,  max(entity.NaatiNumber) as NaatiNumber, MIN(credentialRequest.StatusChangeDate)  as MinCredentialRequestStatusChangeDate 
						from tblCertificationPeriod certificationPeriod
						inner join tblPerson person on person.PersonId = certificationPeriod.PersonId						
                        inner join tblCredentialApplication credentialApplication 
                        on person.PersonId = credentialApplication.PersonId
						inner join tblCredentialRequest credentialRequest
						on credentialRequest.CredentialApplicationId = credentialApplication.CredentialApplicationId
                        inner join tblCredentialApplicationType credentialApplicationType
                        on credentialApplicationType.CredentialApplicationTypeId = credentialApplication.CredentialApplicationTypeId                                     
                        inner join tblCredentialtype credentialType on credentialType.CredentialTypeId = credentialRequest.CredentialTypeId
						inner join tblEntity entity on entity.EntityId = person.EntityId
                        where 
                        credentialRequest.CredentialRequestStatusTypeId = 12  and-- Credential issued and
                        credentialApplicationType.CredentialApplicationTypeCategoryId = 2 and-- transition
                        certificationPeriod.CredentialApplicationId is null and -- Doesnt have recertification or transition applications
                        credentialType.certification = 1
						group by certificationPeriod.certificationPeriodId , credentialApplication.CredentialApplicationId)
                        UPDATE tblCertificationPeriod 
                        SET 
                        CredentialApplicationId = cte.CredentialApplicationId
                        FROM
                        dataToMigrate cte
                        where tblCertificationPeriod.CertificationPeriodId = cte.CertificationPeriodId
                        and cte.MinCredentialRequestStatusChangeDate = (SELECT MIN(MinCredentialRequestStatusChangeDate) from dataToMigrate ct2 where  CertificationPeriodId = tblCertificationPeriod.CertificationPeriodId )");

            Alter.Column("CredentialApplicationId").OnTable("tblCertificationPeriod").AsInt32().NotNullable();
        }
    }
}
