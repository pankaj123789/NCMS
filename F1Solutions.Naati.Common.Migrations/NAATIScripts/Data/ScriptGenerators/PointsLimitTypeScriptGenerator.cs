using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class PdPointsLimitTypeScriptGenerator : BaseScriptGenerator
    {
        public PdPointsLimitTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblPdPointsLimitType";

        public override IList<string> Columns => new[]
        {
            "PdPointsLimitTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "MaxPointsPerApplication", "Max. Points per Application" });
            CreateOrUpdateTableRow(new[] { "2", "MaxPointsPerYear", "Max. Points Per Year" });
        }
    }
}
