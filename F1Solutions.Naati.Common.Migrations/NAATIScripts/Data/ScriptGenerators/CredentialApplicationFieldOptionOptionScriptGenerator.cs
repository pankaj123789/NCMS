using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialApplicationFieldOptionOptionScriptGenerator : BaseScriptGenerator
    {

        public CredentialApplicationFieldOptionOptionScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblCredentialApplicationFieldOptionOption";
        public override IList<string> Columns => new[] {
            "CredentialApplicationFieldOptionOptionId",
            "CredentialApplicationFieldId",
            "CredentialApplicationFieldOptionId"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "60", "1" });
            CreateOrUpdateTableRow(new[] { "2", "60", "2" });
            CreateOrUpdateTableRow(new[] { "3", "60", "3" });

            CreateOrUpdateTableRow(new[] { "4", "120", "4" });
            CreateOrUpdateTableRow(new[] { "5", "120", "5" });
            CreateOrUpdateTableRow(new[] { "6", "121", "4" });
            CreateOrUpdateTableRow(new[] { "7", "121", "5" });
            CreateOrUpdateTableRow(new[] { "8", "122", "4" });
            CreateOrUpdateTableRow(new[] { "9", "122", "5" });
            CreateOrUpdateTableRow(new[] { "10", "123", "4" });
            CreateOrUpdateTableRow(new[] { "11", "123", "5" });
        }
    }
    }
