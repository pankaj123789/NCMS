using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class MaterialRequestTaskTypeScriptGenerator : BaseScriptGenerator
    {
        public MaterialRequestTaskTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblMaterialRequestTaskType";
        public override IList<string> Columns => new[]
        {
            "MaterialRequestTaskTypeId",
            "Name",
            "DisplayName",
        };
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Creating", "Creating" });
            CreateOrUpdateTableRow(new[] { "2", "Checking", "Checking" });
            CreateOrUpdateTableRow(new[] { "3", "Trialler", "Trialler" });
        }
    }
}
