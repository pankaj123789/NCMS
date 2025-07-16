using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20170728_ImproveTestAttendanceAssetViewPerformance
{
    [NaatiMigration(201707281000)]
    public class ImproveTestAttendanceAssetViewPerformance : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.Indexes);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
