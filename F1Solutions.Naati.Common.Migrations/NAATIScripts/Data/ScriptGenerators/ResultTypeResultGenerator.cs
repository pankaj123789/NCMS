using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class ResultTypeResultGenerator : BaseScriptGenerator
    {
        public ResultTypeResultGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tluResultType";
        public override IList<string> Columns => new[] {
            "ResultTypeId",
            "Result"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Awaiting Results" });
            CreateOrUpdateTableRow(new[] { "2", "Passed" });
            CreateOrUpdateTableRow(new[] { "3", "Issue Practice Test Results" });
            CreateOrUpdateTableRow(new[] { "4", "Failed" });
            CreateOrUpdateTableRow(new[] { "5", "Test Invalidated" });
        }

        public override void RunDescendantOrderScripts()
        {

        }
    }
}

