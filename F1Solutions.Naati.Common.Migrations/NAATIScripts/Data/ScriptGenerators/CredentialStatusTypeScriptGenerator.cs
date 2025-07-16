using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialStatusTypeScriptGenerator: BaseScriptGenerator
    {
        public CredentialStatusTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblCredentialStatusType";
        public override IList<string> Columns => new[] {
            "CredentialStatusTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Unknown", "Unknown" });
            CreateOrUpdateTableRow(new[] { "2", "Terminated", "Terminated" });
            CreateOrUpdateTableRow(new[] { "3", "Expired", "Expired" });
            CreateOrUpdateTableRow(new[] { "4", "Active", "Active" });
            CreateOrUpdateTableRow(new[] { "5", "Future", "Future" });
        }
    }
}
