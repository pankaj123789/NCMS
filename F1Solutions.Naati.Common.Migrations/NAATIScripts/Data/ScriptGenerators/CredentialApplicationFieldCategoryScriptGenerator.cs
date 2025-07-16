using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialApplicationFieldCategoryScriptGenerator : BaseScriptGenerator
    {
        public CredentialApplicationFieldCategoryScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblCredentialApplicationFieldCategory";
        public override IList<string> Columns => new[] {
            "CredentialApplicationFieldCategoryId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "None", "None" });
            CreateOrUpdateTableRow(new[] { "2", "Claim", "Claim" });
        }
    }
}
    

