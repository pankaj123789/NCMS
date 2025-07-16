using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160816_UpdatesForNewETL
{
    [NaatiMigration(201608160923)]
    public class IncreaseColumnSizes : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("ALTER TABLE [Internal].[Accreditation] ALTER COLUMN [FailureReason] VARCHAR(500) NULL");
            Execute.Sql("ALTER TABLE [Internal].[Person] ALTER COLUMN [FullName] VARCHAR(300) NULL");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
