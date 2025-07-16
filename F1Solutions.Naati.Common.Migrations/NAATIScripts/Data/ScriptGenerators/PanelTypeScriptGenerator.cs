using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class PanelTypeScriptGenerator : BaseScriptGenerator
    {
        public PanelTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tluPanelType";
        public override IList<string> Columns => new[] {
            "PanelTypeId",
            "Name",
            "Description",
            "AllowCredentialTypeLink",
           
        };
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Panel Examiners", "Language panels", "1"});
        }
    }
}
