using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialTypeUpgradePathScriptGenerator : BaseScriptGenerator
    {
        public CredentialTypeUpgradePathScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblCredentialTypeUpgradePath";
        public override IList<string> Columns => new[] {
                                                           "CredentialTypeUpgradePathId",
                                                           "CredentialTypeFromId",
                                                           "CredentialTypeToId",
                                                           "MatchDirection"

                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "1", "2", "1" });
            CreateOrUpdateTableRow(new[] { "2", "2", "3", "1" });
            CreateOrUpdateTableRow(new[] { "3", "5", "6", "1" });
            CreateOrUpdateTableRow(new[] { "4", "6", "7", "1" });
            CreateOrUpdateTableRow(new[] { "5", "7", "8", "1" });
            CreateOrUpdateTableRow(new[] { "6", "7", "9", "1" });
            CreateOrUpdateTableRow(new[] { "7", "7", "10", "0" });
            CreateOrUpdateTableRow(new[] { "8", "12", "13", "1" });
            CreateOrUpdateTableRow(new[] { "9", "19", "20", "1" });
            CreateOrUpdateTableRow(new[] { "10", "20", "21", "1" });
            CreateOrUpdateTableRow(new[] { "11", "21", "22", "1" });
            CreateOrUpdateTableRow(new[] { "12", "21", "23", "1" });
            CreateOrUpdateTableRow(new[] { "13", "21", "24", "0" });
            CreateOrUpdateTableRow(new[] { "14", "25", "26", "1" });
            CreateOrUpdateTableRow(new[] { "15", "26", "27", "1" });
            CreateOrUpdateTableRow(new[] { "16", "21", "36", "1" });
            CreateOrUpdateTableRow(new[] { "17", "21", "37", "1" });
        }
    }
}
