using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20161024_MoveResultToApplication
{
    [NaatiMigration(201610241645)]
    public class MoveResultToApplication : NaatiMigration
    {
        public override void Up()
        {
            // drop the index
            this.ExecuteSql(@"DROP INDEX [dbo].[ApplicationHistory].[IX_Application_ObsoletedDate]");
            // create the column
            Create.Column("FailureReason").OnTable("ApplicationHistory").AsString(500).Nullable();

            // re-create the index
            this.ExecuteSql(@"
CREATE NONCLUSTERED INDEX [IX_Application_ObsoletedDate] ON [dbo].[ApplicationHistory]
(
	[ObsoletedDate] ASC
)
INCLUDE ( 	[ApplicationId],
	[PersonId],
	[LanguageId],
	[LanguageName],
	[LanguageIndigenous],
	[ToLanguageId],
	[ToLanguageName],
	[ToEnglish],
	[EnteredDate],
	[AccreditationMethodDescription],
	[AccreditationLevel],
	[AccreditationCategoryDescription],
	[ReceivingOfficeName],
	[Status],
	[StatusDate],
	[StatusReason],
	[EligibilityMeetsRequirements],
	[EligibilityJustification],
	[IncompleteApplication],
	[LanguageToBoth],
	[Direction],
	[InviteToTesting],
	[PreferredTestCentreName],
	[MaxOpportunities],
	[Prerequisite1],
	[Prerequisite2],
	[Prerequisite3],
	[ForMigration],
	[CountryOfTrainingName],
	[ProjectName],
	[EligibilityListedOnApplication],
	[SponsorInstitutionName],
	[ForCommunityLanguagePoints],
	[ForEducationalAndSkillEmployment],
	[SkillAssessmentResult],
	[FailureReason],
	[CourseCompletedDateInWizard]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

            // Move the data
            this.ExecuteSql(@"
UPDATE [ApplicationHistory] 
SET [FailureReason] = ac.[FailureReason]
FROM [AccreditationHistory] ac
LEFT JOIN  [ApplicationHistory] ap ON ac.ApplicationId = ap.ApplicationId");

            // drop the index
            this.ExecuteSql(@"DROP INDEX [dbo].[AccreditationHistory].[IX_Accreditation_ObsoletedDate]");

            // delete the column
            Delete.Column("FailureReason").FromTable("AccreditationHistory");

            // re-create the index
            this.ExecuteSql(@"CREATE NONCLUSTERED INDEX [IX_Accreditation_ObsoletedDate] ON [dbo].[AccreditationHistory]
(
	[ObsoletedDate] ASC
)
INCLUDE ( 	[AccreditationResultId],
	[ApplicationId],
	[PersonId],
	[ResultDate],
	[Days],
	[CertificateNumber],
	[IncludeInPD],
	[AccreditationLevel],
	[ExpiryDate],
	[Notes],
	[Result]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

            // Delete rows where the result is not a pass or conceded pass
            this.ExecuteSql(@"
DELETE FROM [AccreditationHistory]
WHERE [Result] != 'Passed' and [Result] != 'Conceded Pass'");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
