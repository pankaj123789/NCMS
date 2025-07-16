using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialTypeTemplateScriptGenerator : BaseScriptGenerator
    {
        public CredentialTypeTemplateScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblCredentialTypeTemplate";

        public override IList<string> Columns => new[]
        {
            "CredentialTypeTemplateId",
            "CredentialTypeId",
            "StoredFileId",
            "DocumentNameTemplate",
        };

        public override void RunScripts()
        {
            // we can only use this script generator when we already know the StoredFileId

            CreateOrUpdateTableRow(new[] { "51", "29", "101995", "[[NAATI No]] [[Credential Type]] [[Document Number]] Certificate" });
            CreateOrUpdateTableRow(new[] { "52", "30", "101995", "[[NAATI No]] [[Credential Type]] [[Document Number]] Certificate" });

            CreateOrUpdateTableRow(new[] { "53", "31", "101995", "[[NAATI No]] [[Credential Type]] [[Document Number]] Certificate" });
            CreateOrUpdateTableRow(new[] { "54", "32", "101995", "[[NAATI No]] [[Credential Type]] [[Document Number]] Certificate" });

            CreateOrUpdateTableRow(new[] { "11", "6", "14895", "[[Practitioner Number]] [[Credential Type]] [[Skill]] [[Document Number]] Letter" });


        }
    }
}