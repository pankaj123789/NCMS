using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20170901_ReportingDbChanges
{
    [NaatiMigration(201709081755)]
    public class ReportingDbChanges : NaatiMigration
    {
        public override void Up()
        {
            // Drop PersonHistory Index
            Execute.Sql(Sql.DropPersonHistoryIndex);

            // Person
            Delete.Column("EntityId").FromTable("PersonHistory");
            Delete.Column("Deceased").FromTable("PersonHistory");
            Delete.Column("HighestEducationLevel").FromTable("PersonHistory");
            Delete.Column("ReleaseDetails").FromTable("PersonHistory");
            if (Schema.Table("PersonHistory").Column("Citizen").Exists())
            {
                Delete.Column("Citizen").FromTable("PersonHistory");
            }
            Delete.Column("DoNotInviteToDirectory").FromTable("PersonHistory");
            Delete.Column("EnteredDate").FromTable("PersonHistory");
            Delete.Column("ExpertiseFreeText").FromTable("PersonHistory");
            Delete.Column("NameOnAccreditationProduct").FromTable("PersonHistory");
            Delete.Column("DoNotSendCorrespondence").FromTable("PersonHistory");
            Delete.Column("ScanRequired").FromTable("PersonHistory");
            Delete.Column("IsEportalActive").FromTable("PersonHistory");
            Delete.Column("PersonalDetailsLastUpdatedOnEportal").FromTable("PersonHistory");
            Delete.Column("WebAccountCreateDate").FromTable("PersonHistory");
            Delete.Column("AllowVerifyOnline").FromTable("PersonHistory");
            Delete.Column("ABN").FromTable("PersonHistory");
            Delete.Column("Note").FromTable("PersonHistory");
            Delete.Column("NAATINumber").FromTable("PersonHistory");
            Delete.Column("NAATINumberDisplay").FromTable("PersonHistory");
            Delete.Column("EntityType").FromTable("PersonHistory");
            Delete.Column("Postcode").FromTable("PersonHistory");
            Delete.Column("State").FromTable("PersonHistory");
            Delete.Column("MostRecentArchiveDate").FromTable("PersonHistory");
            Delete.Column("MostRecentInvoiceDate").FromTable("PersonHistory");
            Delete.Column("MostRecentApplicationDate").FromTable("PersonHistory");
            Delete.Column("FullName").FromTable("PersonHistory");
            Delete.Column("LatestPdListingRenewalFy").FromTable("PersonHistory");
            Delete.Column("LatestPdListingRenewalDate").FromTable("PersonHistory");
            Delete.Column("Mobile").FromTable("PersonHistory");
            Create.Column("SecondaryAddress").OnTable("PersonHistory").AsString(500).Nullable();
            Create.Column("SecondaryPhone").OnTable("PersonHistory").AsString(60).Nullable();
            Create.Column("SecondaryEmail").OnTable("PersonHistory").AsString(200).Nullable();
            Rename.Column("StreetDetails").OnTable("PersonHistory").To("PrimaryAddress");
            Rename.Column("BirthCountry").OnTable("PersonHistory").To("CountryOfBirth");
            Rename.Column("BirthDate").OnTable("PersonHistory").To("DateOfBirth");
            Rename.Column("Surname").OnTable("PersonHistory").To("FamilyName");
            Rename.Column("Phone").OnTable("PersonHistory").To("PrimaryPhone");
            Rename.Column("Email").OnTable("PersonHistory").To("PrimaryEmail");

            // Re-Create PersonHistory Index
            Execute.Sql(Sql.CreateNewPersonHistoryIndex);

            // Application
            Delete.Column("LanguageId").FromTable("ApplicationHistory");
            Delete.Column("LanguageName").FromTable("ApplicationHistory");
            Delete.Column("LanguageIndigenous").FromTable("ApplicationHistory");
            Delete.Column("ToLanguageId").FromTable("ApplicationHistory");
            Delete.Column("ToLanguageName").FromTable("ApplicationHistory");
            Delete.Column("ToEnglish").FromTable("ApplicationHistory");
            Delete.Column("AccreditationMethodDescription").FromTable("ApplicationHistory");
            Delete.Column("AccreditationLevel").FromTable("ApplicationHistory");
            Delete.Column("AccreditationCategoryDescription").FromTable("ApplicationHistory");
            Delete.Column("StatusReason").FromTable("ApplicationHistory");
            Delete.Column("EligibilityMeetsRequirements").FromTable("ApplicationHistory");
            Delete.Column("EligibilityJustification").FromTable("ApplicationHistory");
            Delete.Column("IncompleteApplication").FromTable("ApplicationHistory");
            Delete.Column("LanguageToBoth").FromTable("ApplicationHistory");
            Delete.Column("Direction").FromTable("ApplicationHistory");
            Delete.Column("InviteToTesting").FromTable("ApplicationHistory");
            Delete.Column("MaxOpportunities").FromTable("ApplicationHistory");
            Delete.Column("Prerequisite1").FromTable("ApplicationHistory");
            Delete.Column("Prerequisite2").FromTable("ApplicationHistory");
            Delete.Column("Prerequisite3").FromTable("ApplicationHistory");
            Delete.Column("ForMigration").FromTable("ApplicationHistory");
            Delete.Column("CountryOfTrainingName").FromTable("ApplicationHistory");
            Delete.Column("ProjectName").FromTable("ApplicationHistory");
            Delete.Column("EligibilityListedOnApplication").FromTable("ApplicationHistory");
            Delete.Column("ForCommunityLanguagePoints").FromTable("ApplicationHistory");
            Delete.Column("ForEducationalAndSkillEmployment").FromTable("ApplicationHistory");
            Delete.Column("SkillAssessmentResult").FromTable("ApplicationHistory");
            Delete.Column("CourseCompletedDateInWizard").FromTable("ApplicationHistory");
            Delete.Column("ResultDate").FromTable("ApplicationHistory");
            Delete.Column("FailureReason").FromTable("ApplicationHistory");
            Delete.Column("CandidateName").FromTable("ApplicationHistory");
            Delete.Column("PreferredTestCentreName").FromTable("ApplicationHistory");
            Create.Column("Title").OnTable("ApplicationHistory").AsString(50).Nullable();
            Create.Column("GivenName").OnTable("ApplicationHistory").AsString(100).Nullable();
            Create.Column("OtherNames").OnTable("ApplicationHistory").AsString(100).Nullable();
            Create.Column("FamilyName").OnTable("ApplicationHistory").AsString(100).Nullable();
            Create.Column("PrimaryAddress").OnTable("ApplicationHistory").AsString(500).Nullable();
            Create.Column("Country").OnTable("ApplicationHistory").AsString(50).Nullable();
            Create.Column("PrimaryPhone").OnTable("ApplicationHistory").AsString(60).Nullable();
            Create.Column("PrimaryEmail").OnTable("ApplicationHistory").AsString(200).Nullable();
            Create.Column("ApplicationType").OnTable("ApplicationHistory").AsString(50).Nullable();
            Create.Column("ApplicationReference").OnTable("ApplicationHistory").AsString(50).Nullable();
            Create.Column("ApplicationOwner").OnTable("ApplicationHistory").AsString(100).Nullable();
            Create.Column("EnteredUser").OnTable("ApplicationHistory").AsInt32().Nullable();
            Create.Column("ModifiedUser").OnTable("ApplicationHistory").AsString(100).Nullable();
            Rename.Column("Status").OnTable("ApplicationHistory").To("ApplicationStatus");
            Rename.Column("ReceivingOfficeName").OnTable("ApplicationHistory").To("EnteredOffice");
            Rename.Column("SponsorInstitutionName").OnTable("ApplicationHistory").To("Sponsor");
            Rename.Column("StatusDate").OnTable("ApplicationHistory").To("StatusChangeDate");

            // CredentialRequests
            Create.Table("CredentialRequestsHistory")
                .WithColumn("CredentialRequestId").AsInt32().PrimaryKey()
                .WithColumn("PersonId").AsInt32().NotNullable()
                .WithColumn("Title").AsString(50).Nullable()
                .WithColumn("GivenName").AsString(100).Nullable()
                .WithColumn("OtherNames").AsString(100).Nullable()
                .WithColumn("FamilyName").AsString(100).Nullable()
                .WithColumn("PrimaryAddress").AsString(500).Nullable()
                .WithColumn("Country").AsString(50).Nullable()
                .WithColumn("PrimaryEmail").AsString(200).Nullable()
                .WithColumn("PrimaryPhone").AsString(60).Nullable()
                .WithColumn("ApplicationId").AsInt32().NotNullable().ForeignKey()
                .WithColumn("ApplicationType").AsString(50)
                .WithColumn("ApplicationReference").AsString(50)
                .WithColumn("ApplicationOwner").AsString(100).Nullable()
                .WithColumn("CredentialTypeInternalName").AsString(50)
                .WithColumn("CredentialTypeExternalName").AsString(50)
                .WithColumn("Certification").AsBoolean()
                .WithColumn("Language1").AsString(50)
                .WithColumn("Language2").AsString(50)
                .WithColumn("DirectionDisplayName").AsString(50)
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable()
                .WithColumn("StatusChangedDate").AsDateTime().Nullable()
                .WithColumn("ModifiedUser").AsString(100)
                .WithColumn("Status").AsString(50);

            // Credentials
            Create.Table("CredentialsHistory")
                .WithColumn("CredentialId").AsInt32().PrimaryKey()
                .WithColumn("PersonId").AsInt32().NotNullable()
                .WithColumn("Title").AsString(50).Nullable()
                .WithColumn("GivenName").AsString(100).Nullable()
                .WithColumn("OtherNames").AsString(100).Nullable()
                .WithColumn("FamilyName").AsString(100).Nullable()
                .WithColumn("PrimaryAddress").AsString(500).Nullable()
                .WithColumn("Country").AsString(50).Nullable()
                .WithColumn("PrimaryPhone").AsString(60).Nullable()
                .WithColumn("PrimaryEmail").AsString(200).Nullable()
                .WithColumn("CredentialType").AsString(50)
                .WithColumn("Direction").AsString(50)
                .WithColumn("StartDate").AsDateTime()
                .WithColumn("ExpiryDate").AsDateTime()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable()
                .WithColumn("TerminationDate").AsDateTime().Nullable()
                .WithColumn("Status").AsString(50)
                .WithColumn("ShowInOnlineDirectory").AsBoolean();

            // Create CredentialRequests view
            Execute.Sql(Sql.CreateCredentialRequestsView);

            // Create CredentialRequests procedure
            Execute.Sql(Sql.CreateCredentialRequestsProcedure);

            // Create Credentials view
            Execute.Sql(Sql.CreateCredentialsView);

            // Create Credentials procedure
            Execute.Sql(Sql.CreateCredentialsProcedure);

            // Tables to be deleted
            Delete.Table("AccreditationHistory");
            Delete.Table("ExpressionOfInterestHistory");
            Delete.Table("InvoiceHistory");
            Delete.Table("PaymentHistory");
            Delete.Table("RevalidationHistory");

            // Delete procedures
            Execute.Sql(Sql.DropProcedures);

            // Delete views
            Execute.Sql(Sql.DropViews);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
