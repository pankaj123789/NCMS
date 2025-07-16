using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialRequestAssociationTypeScriptGenerator : BaseScriptGenerator
    {
        public CredentialRequestAssociationTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblCredentialRequestAssociationType";
        public override IList<string> Columns => new[] {
            "CredentialRequestAssociationTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "ConcededPass", "Conceded Pass" });
        }
    }
}
