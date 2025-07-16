using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class RolePlayerStatusTypeGenerator : BaseScriptGenerator
    {
        public RolePlayerStatusTypeGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblRolePlayerStatusType";
        public override IList<string> Columns => new[] {
														   "RolePlayerStatusTypeId",
                                                           "Name",
                                                           "DisplayName",
                                                       };

        public override void RunScripts()
        {
			CreateOrUpdateTableRow(new[] { "1", "Pending", "Pending" });
			CreateOrUpdateTableRow(new[] { "2", "Accepted", "Accepted" });
			CreateOrUpdateTableRow(new[] { "3", "Rejected", "Rejected" });
			CreateOrUpdateTableRow(new[] { "4", "Rehearsed", "Rehearsed" });
			CreateOrUpdateTableRow(new[] { "5", "Attended", "Attended" });
			CreateOrUpdateTableRow(new[] { "6", "NoShow", "No Show" });
        }
    }
}
