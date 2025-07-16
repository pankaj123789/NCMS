
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180716_RemoveIncludePdColumnFromAddressTable
{
    [NaatiMigration(201807161510)]
    public class RemoveIncludePdColumnFromAddressTable : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("DROP STATISTICS [dbo].[tblAddress].[_dta_stat_2009058193_9_10]");
            Execute.Sql("DROP STATISTICS [dbo].[tblAddress].[_dta_stat_2009058193_10_4]");
            Execute.Sql("DROP STATISTICS [dbo].[tblAddress].[_dta_stat_2009058193_10_5_4]");
            Execute.Sql("DROP STATISTICS [dbo].[tblAddress].[_dta_stat_2009058193_10_9_5_4]");
            Execute.Sql("DROP STATISTICS [dbo].[tblAddress].[_dta_stat_2009058193_2_10_5_4_9]");

            Execute.Sql("DROP INDEX [_dta_index_tblAddress_9_2009058193__K10_1_2_3_4_5] ON [dbo].[tblAddress]");
            Execute.Sql("DROP INDEX [_dta_index_tblAddress_9_2009058193__K9_2_4_5] ON [dbo].[tblAddress]");
            
            Delete.Column("IncludeInPd").FromTable("tblAddress");
        }
    }
}
