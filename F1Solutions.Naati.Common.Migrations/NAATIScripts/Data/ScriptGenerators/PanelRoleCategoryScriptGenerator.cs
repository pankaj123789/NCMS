using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class PanelRoleCategoryScriptGenerator : BaseScriptGenerator
    {
        public PanelRoleCategoryScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblPanelRoleCategory";

        public override IList<string> Columns => new[]
        {
            "PanelRoleCategoryId",
            "Name",
            "DisplayName",
            "MembershipDurationMonths"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] {"1", "Examiner", "Examiner", "60"});
            CreateOrUpdateTableRow(new[] {"2", "RolePlayer", "Role-player", "60"});
        }
    }
}
