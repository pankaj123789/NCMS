using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20170508_AddPdRenewalToPersonTable
{
    [NaatiMigration(201705081656)]
    public class AddPdRenewalToPersonTabel : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
ALTER TABLE [PersonHistory]
ADD [LatestPdListingRenewalFy] VARCHAR(10) NULL");
            Execute.Sql(@"
ALTER TABLE [PersonHistory]
ADD [LatestPdListingRenewalDate] DATETIME NULL");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
