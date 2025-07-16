using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class MaterialRequestRoundPanelMembershipTypeScriptGenerator : BaseScriptGenerator
    {
        public MaterialRequestRoundPanelMembershipTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblMaterialRequestPanelMembershipType";
        public override IList<string> Columns => new[] {
            "MaterialRequestPanelMembershipTypeId",
            "Name",
            "DisplayName"
        };
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Coordinator", "Coordinator" });
            CreateOrUpdateTableRow(new[] { "2", "PanelCollaborator", "Panel Collaborator" });
 
        }
    }
}
