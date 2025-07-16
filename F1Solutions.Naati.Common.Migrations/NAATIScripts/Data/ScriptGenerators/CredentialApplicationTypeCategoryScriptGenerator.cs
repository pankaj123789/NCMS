using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialApplicationTypeCategoryScriptGenerator: BaseScriptGenerator  
    {
        public CredentialApplicationTypeCategoryScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblCredentialApplicationTypeCategory";

        public override IList<string> Columns => new[]
        {
            "CredentialApplicationTypeCategoryId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "None", "None"});
            CreateOrUpdateTableRow(new[] { "2", "Transition", "Transition" });
            CreateOrUpdateTableRow(new[] { "3", "Recertification", "Recertification" });
        }
    }
}
