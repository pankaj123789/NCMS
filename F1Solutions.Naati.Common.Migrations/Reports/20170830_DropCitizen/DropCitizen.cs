using System;
using FluentMigrator;

namespace F1Solutions.NAATI.SAM.Migrations.Reports._20170830_DropCitizen
{
    [Migration(201708081656)]
    public class DropCitizen : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"DROP INDEX [PersonHistory].[IX_Person_ObsoletedDate]");

            Execute.Sql(@"
            ALTER TABLE [PersonHistory]
            DROP COLUMN [Citizen]");

            Execute.Sql(@"
            CREATE NONCLUSTERED INDEX [IX_Person_ObsoletedDate] ON [dbo].[PersonHistory]
            (
	            [ObsoletedDate] ASC
            )
            INCLUDE ( 	[PersonId],
	            [EntityId],
	            [GivenName],
	            [OtherNames],
	            [Surname],
	            [Title],
	            [Gender],
	            [BirthDate],
	            [BirthCountry],
	            [Deceased],
	            [HighestEducationLevel],
	            [ReleaseDetails],
	            [DoNotInviteToDirectory],
	            [EnteredDate],
	            [ExpertiseFreeText],
	            [NameOnAccreditationProduct],
	            [DoNotSendCorrespondence],
	            [ScanRequired],
	            [IsEportalActive],
	            [PersonalDetailsLastUpdatedOnEportal],
	            [WebAccountCreateDate],
	            [AllowVerifyOnline],
	            [ABN],
	            [Note],
	            [NAATINumber],
	            [NAATINumberDisplay],
	            [EntityType],
	            [StreetDetails],
	            [Postcode],
	            [Country],
	            [State],
	            [MostRecentArchiveDate],
	            [MostRecentInvoiceDate],
	            [MostRecentApplicationDate],
	            [Email],
	            [Phone],
	            [Mobile],
	            [FullName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
