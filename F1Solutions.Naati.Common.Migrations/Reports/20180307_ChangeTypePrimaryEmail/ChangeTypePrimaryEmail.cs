using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20180307_ChangeTypePrimaryEmail
{
    [NaatiMigration(201803071000)]
    public class ChangeTypePrimaryEmail : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
                            DROP INDEX [IX_Person_ObsoletedDate] ON [dbo].[PersonHistory]
                            GO

                            alter table PersonHistory alter column PrimaryAddress  nvarchar(500)  null 
                            Go
                        
                            CREATE NONCLUSTERED INDEX [IX_Person_ObsoletedDate] ON [dbo].[PersonHistory]
                            (
	                            [ObsoletedDate] ASC
                            )
                            INCLUDE ( 	[PersonId],
	                            [GivenName],
	                            [OtherNames],
	                            [FamilyName],
	                            [Title],
	                            [Gender],
	                            [DateOfBirth],
	                            [CountryOfBirth],
	                            [PrimaryAddress],
	                            [Country],
	                            [PrimaryEmail],
	                            [PrimaryPhone],
	                            [SecondaryAddress],
	                            [SecondaryPhone],
	                            [SecondaryEmail]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            GO");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
