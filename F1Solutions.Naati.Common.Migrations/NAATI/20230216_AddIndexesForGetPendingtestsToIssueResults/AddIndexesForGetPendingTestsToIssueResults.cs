using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20230216_AddIndexesForGetPendingtestsToIssueResults
{
    [NaatiMigration(202302161500)]
    public class AddIndexesForGetPendingTestsToIssueResults : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddIndexesForGetPendingTestsToIssueResultQuery);
        }
    }
}
