using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class RolePlayerRoleTypeGenerator : BaseScriptGenerator
    {
        public RolePlayerRoleTypeGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblRolePlayerRoleType";
        public override IList<string> Columns => new[] {
														   "RolePlayerRoleTypeId",
                                                           "Name",
                                                           "DisplayName",
                                                       };

        public override void RunScripts()
        {
			CreateOrUpdateTableRow(new[] { "1", "PrimaryRolePlayer", "Primary" });
			CreateOrUpdateTableRow(new[] { "2", "BackupRolePlayer", "Backup" });
		}
    }
}
