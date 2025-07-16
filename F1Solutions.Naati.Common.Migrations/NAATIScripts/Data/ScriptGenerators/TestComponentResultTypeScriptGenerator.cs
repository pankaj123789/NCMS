using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class MarkingResultTypeScriptGenerator : BaseScriptGenerator
    {
        public MarkingResultTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner)
        {
        }

        public override string TableName => "tblMarkingResultType";

        public override IList<string> Columns => new[]
        {
            "MarkingResultTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] {"1", "Original", "Original" });
            CreateOrUpdateTableRow(new[] {"2", "EligableForSupplementary", "Eligible For Supplementary" });
            CreateOrUpdateTableRow(new[] {"3", "FromOriginal", "FromOriginal" });
        }
    }
}
