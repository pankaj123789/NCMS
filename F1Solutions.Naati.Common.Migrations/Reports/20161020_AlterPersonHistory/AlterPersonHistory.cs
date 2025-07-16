using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20161020_AlterPersonHistory
{
    [NaatiMigration(201610201756)]
    public class AlterPersonHistory : NaatiMigration
    {
        public override void Up()
        {
            this.ExecuteSql("DROP INDEX IX_Person_ObsoletedDate ON PersonHistory");

            Create.Column("Email").OnTable("PersonHistory").AsString(200).Nullable();
            Create.Column("Phone").OnTable("PersonHistory").AsString(60).Nullable();
            Create.Column("Mobile").OnTable("PersonHistory").AsString(60).Nullable();

            this.ExecuteSql(@"
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
