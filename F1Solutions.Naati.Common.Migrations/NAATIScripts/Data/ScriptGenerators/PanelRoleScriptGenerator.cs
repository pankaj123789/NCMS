using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class PanelRoleScriptGenerator : BaseScriptGenerator
    {
        public PanelRoleScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblPanelRole";

        public override IList<string> Columns => new[]
                                                {
                                                    "PanelRoleId",
                                                    "Name",
                                                    "Description",
                                                    "PanelRoleCategoryId",
                                                    "Chair"
                                                }; 
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Examiner - Contract (Chair)", "CHR", "1", "1" });
            CreateOrUpdateTableRow(new[] { "2", "Role-player", "RP", "2", "0" });
            CreateOrUpdateTableRow(new[] { "4", "Examiner - Contract", "EXMCON", "1", "0" });
            CreateOrUpdateTableRow(new[] { "307", "Examiner - Interim", "EXMINT", "1", "0" });
            CreateOrUpdateTableRow(new[] { "612", "Examiner - Casual", "EXMCAS", "1", "0" });
        }
    }
}
