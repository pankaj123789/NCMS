using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialRequestPathTypeScriptGenerator : BaseScriptGenerator
    {
        public CredentialRequestPathTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblCredentialRequestPathType";
        public override IList<string> Columns => new[] {
            "CredentialRequestPathTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "New", "New" });
            CreateOrUpdateTableRow(new[] { "2", "NewSkill", "New Skill" });
            CreateOrUpdateTableRow(new[] { "3", "Upgrade", "Upgrade" });
            CreateOrUpdateTableRow(new[] { "4", "Recertify", "Recertify" });
            CreateOrUpdateTableRow(new[] { "5", "Conceded", "Conceded" });
        }
    }
}
