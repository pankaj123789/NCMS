using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialTypeDowngradePathScriptGenerator : BaseScriptGenerator
    {
        public CredentialTypeDowngradePathScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblCredentialTypeDowngradePath";

        public override IList<string> Columns => new[] {
            "CredentialTypeDowngradePathId",
            "CredentialTypeFromId",
            "CredentialTypeToId",
        };

        public override void RunScripts()
        {
            //TFS189634
            DeleteTableRow("1");

            CreateOrUpdateTableRow(new[] { "2", "21", "20" });
            CreateOrUpdateTableRow(new[] { "3", "27", "26" });
            CreateOrUpdateTableRow(new[] { "4", "3", "2" });
        }
    }
}