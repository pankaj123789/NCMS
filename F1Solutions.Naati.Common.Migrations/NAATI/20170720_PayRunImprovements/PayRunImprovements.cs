using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20170720_PayRunImprovements
{
    [NaatiMigration(201707201341)]
    public class PayRunImprovements : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"alter table tblPerson add ExaminerTrackingCategory varchar(255)");
            Execute.Sql(@"alter table tblProductSpecification add TrackingActivity varchar(255)");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
