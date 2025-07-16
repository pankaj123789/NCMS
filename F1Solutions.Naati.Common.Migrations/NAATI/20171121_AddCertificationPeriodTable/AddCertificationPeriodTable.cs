
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171121_AddCertificationPeriodTable
{
    [NaatiMigration(201711210800)]
    public class AddCertificationPeriodTable: NaatiMigration
    {
        public override void Up()
        {
            CreateCertificationPeriodTable();
            AddCertificationPeriodColumnToCredential();
            ModifyExpiryDateColumnOnCredentialTable();
            PopulateCertficationPeriodTable();
        }

        void CreateCertificationPeriodTable()
        {
            Create.Table("tblCertificationPeriod")
                .WithColumn("CertificationPeriodId").AsInt32().Identity().PrimaryKey()
                .WithColumn("PersonId").AsInt32().NotNullable()
                .WithColumn("StartDate").AsDate().NotNullable()
                .WithColumn("EndDate").AsDate().NotNullable()
                .WithColumn("OriginalEndDate").AsDate().NotNullable();

            Create.ForeignKey("FK_CertificationPeriod_Person")
                .FromTable("tblCertificationPeriod")
                .ForeignColumn("PersonId")
                .ToTable("tblPerson")
                .PrimaryColumn("PersonId");
        }

        void AddCertificationPeriodColumnToCredential()
        {
            Create.Column("CertificationPeriodId").OnTable("tblCredential").AsInt32().Nullable();

            Create.ForeignKey("FK_Credential_CertificationPeriod")
                .FromTable("tblCredential")
                .ForeignColumn("CertificationPeriodId")
                .ToTable("tblCertificationPeriod")
                .PrimaryColumn("CertificationPeriodId");
        }

        void PopulateCertficationPeriodTable()
        {
           Execute.Sql(@"Declare @CredentialsInfo Table(CredentialsInfoId Int NOT NULL IDENTITY(1,1) PRIMARY KEY,
						CredentialId Int NOT NULL,
						PersonId Int NOT NULL,
						StartDate DATETIME not null, 						
						EndDate DATETIME not null )

                        Insert into @CredentialsInfo
                        SELECT C.CredentialId, CA.PersonId, C.StartDate , C.ExpiryDate AS EndDate FROM tblCredential C
                        INNER JOIN tblCredentialCredentialRequest ccr
                        on ccr.CredentialId = c.CredentialId
                        inner join tblCredentialRequest cr
                        on cr.CredentialRequestId = ccr.CredentialRequestId
                        inner join tblCredentialApplication ca
                        on ca.CredentialApplicationId = cr.CredentialApplicationId
				        inner join tblCredentialType ct on cr.CredentialTypeId = ct.CredentialTypeId
                        where ct.Certification = 1

                        INSERT INTO tblCertificationPeriod
                        SELECT PersonId, MIN(StartDate) AS StartDate , MAX(EndDate) AS EndDate ,MAX(EndDate) AS OriginalEndDate FROM @CredentialsInfo
                        GROUP BY PersonId

                        UPDATE C SET CertificationPeriodId = cp.CertificationPeriodId, ExpiryDate = NULL
                        FROM tblCredential C
                        inner join @CredentialsInfo ci 
                        on ci.CredentialId = c.CredentialId
                        inner join tblCertificationPeriod cp
                        on cp.PersonId = ci.PersonId");
        }

        void ModifyExpiryDateColumnOnCredentialTable()
        {
            Alter.Column("ExpiryDate").OnTable("tblCredential").AsDate().Nullable();
        }
    }
}
