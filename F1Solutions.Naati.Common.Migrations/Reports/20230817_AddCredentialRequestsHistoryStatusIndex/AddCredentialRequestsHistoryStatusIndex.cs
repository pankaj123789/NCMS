using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20230817_AddCredentialRequestsHistoryStatusIndex
{
    [NaatiMigration(202308171717)]
    public class AddCredentialRequestsHistoryStatusIndex: NaatiMigration
    {
        public override void Up()
        {
	        Execute.Sql(@"
			DROP INDEX IF EXISTS [IX_CredentialRequestsHistory_Status_IncludeAll] ON [CredentialRequestsHistory]
			GO
			CREATE NONCLUSTERED INDEX [IX_CredentialRequestsHistory_Status_IncludeAll] 
				ON [CredentialRequestsHistory] ([Status])
				INCLUDE (
					[PersonId],
					[Title],
					[GivenName],
					[OtherNames],
					[FamilyName],
					[PrimaryAddress],
					[Country],
					[PrimaryEmail],
					[PrimaryPhone],
					[ApplicationId],
					[ApplicationType],
					[ApplicationReference],
					[ApplicationOwner],
					[CredentialTypeInternalName],
					[CredentialTypeExternalName],
					[Certification],
					[Language1],
					[Language2],
					[DirectionDisplayName],
					[ModifiedDate],
					[ObsoletedDate],
					[StatusDateModified],
					[StatusModifiedUser],
					[NAATINumber],
					[PractitionerNumber],
					[Language1Code],
					[Language1Group],
					[Language2Code],
					[Language2Group],
					[State],
					[Postcode],
					[CredentialId],
					[LinkedCredentialRequestId],
					[LinkedCredentialRequestReason],
					[RowStatus],
					[AutoCreated])");
        }
    }
}