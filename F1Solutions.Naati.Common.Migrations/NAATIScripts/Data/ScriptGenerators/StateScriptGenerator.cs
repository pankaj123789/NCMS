using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class StateScriptGenerator : BaseScriptGenerator
    {
        public StateScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tluState";

        public override IList<string> Columns => new[]
        {
            "StateId",
            "State",
            "Name"
        };
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "ACT", "Australian Capital Territory"});
            CreateOrUpdateTableRow(new[] { "2", "NSW", "New South Wales" });
            CreateOrUpdateTableRow(new[] { "3", "NT ", "Northern Territory" });
            CreateOrUpdateTableRow(new[] { "4", "QLD", "Queensland" });
            CreateOrUpdateTableRow(new[] { "5", "SA", "South Australia" });
            CreateOrUpdateTableRow(new[] { "6", "TAS", "Tasmania" });
            CreateOrUpdateTableRow(new[] { "7", "VIC", "Victoria" });
            CreateOrUpdateTableRow(new[] { "8", "WA", "Western Australia" });
            CreateOrUpdateTableRow(new[] { "9", "OS", "Overseas" });
            CreateOrUpdateTableRow(new[] { "10", "NZ", "New Zealand" });
            CreateOrUpdateTableRow(new[] { "109", "NAT", "National Office - ACT" });
            CreateOrUpdateTableRow(new[] { "117", "NIL", "OBSOLETE USE" });
        }
    }
}
