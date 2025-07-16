using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class TestResultEligibilityTypeScriptGenerator : BaseScriptGenerator
    {
        public TestResultEligibilityTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner)
        { }

        public override string TableName => "tblTestResultEligibilityType";

        public override IList<string> Columns => new[]
        {
            "TestResultEligibilityTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Pass", "Pass" });
            CreateOrUpdateTableRow(new[] { "2", "ConcededPass", "Conceded Pass" });
            CreateOrUpdateTableRow(new[] { "3", "SupplementaryTest", "Supplementary Test" });
        }
    }
}
