using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class MaterialRequestStatusTypeScripGenerator : BaseScriptGenerator
    {
        public MaterialRequestStatusTypeScripGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblMaterialRequestStatusType";
        public override IList<string> Columns => new[] {
            "MaterialRequestStatusTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "InProgress", "In Progress" });
            CreateOrUpdateTableRow(new[] { "2", "AwaitingFinalisation", "Awaiting Finalisation" });
            CreateOrUpdateTableRow(new[] { "3", "Finalised", "Finalised" });
            CreateOrUpdateTableRow(new[] { "4", "Cancelled", "Cancelled" });
        }
    }
}
