
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180228_ChangeTypeForStreetDetailsColumn
{
    [NaatiMigration(201802281000)]
    public class ChangeTypeForStreetDetailsColumn : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
                            DROP INDEX [_dta_index_tblAddress_9_2009058193__K10_1_2_3_4_5] ON [dbo].[tblAddress]

                            alter table tblAddress
                            alter column StreetDetails nvarchar(500) not null  

                            CREATE NONCLUSTERED INDEX [_dta_index_tblAddress_9_2009058193__K10_1_2_3_4_5] ON [dbo].[tblAddress]
                            (
	                            [IncludeInPD] ASC
                            )
                            INCLUDE ( 	[AddressId],
	                            [EntityId],
	                            [StreetDetails],
	                            [PostcodeId],
	                            [CountryId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            GO"
                        );
        }
    }
}
