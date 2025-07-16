using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20200102_AddMinPayReviewOverallToStandardMarking
{
    [NaatiMigration(202001021400)]
    public class AddMinPayReviewOverallToStandardMarking : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblTestSpecificationStandardMarkingScheme").AddColumn("MinOverallMarkForPaidReview").AsInt32().Nullable();
            Update.Table("tblTestSpecificationStandardMarkingScheme").Set(new { MinOverallMarkForPaidReview = 0 }).AllRows();
            Alter.Column("MinOverallMarkForPaidReview").OnTable("tblTestSpecificationStandardMarkingScheme").AsInt32().Nullable();
        }
    }
}
