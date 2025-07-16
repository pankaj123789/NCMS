using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class TestStatusTypeScriptGenerator : BaseScriptGenerator
    {
        public TestStatusTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblTestStatusType";
        public override IList<string> Columns => new[] {
            "TestStatusTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] {"1", "Sat", "Sat" });
            CreateOrUpdateTableRow(new[] {"2", "InProgress", "In Progress" });
            CreateOrUpdateTableRow(new[] {"3", "UnderReview", "Under Review" });
            CreateOrUpdateTableRow(new[] {"4", "UnderPaidReview", "Under Paid Review" });
            CreateOrUpdateTableRow(new[] {"5", "Finalised", "Finalised" });
            CreateOrUpdateTableRow(new[] {"6", "PendingSupplementaryTest", "Pending Supplementary Test" });
            CreateOrUpdateTableRow(new[] {"7", "TestInvalidated", "Test Invalidated" });
        }
    }
}
