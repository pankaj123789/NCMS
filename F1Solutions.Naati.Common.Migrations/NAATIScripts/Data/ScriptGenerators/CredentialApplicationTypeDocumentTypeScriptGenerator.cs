using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialApplicationTypeDocumentTypeScriptGenerator: BaseScriptGenerator
    {
        public CredentialApplicationTypeDocumentTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblCredentialApplicationTypeDocumentType";
        public override IList<string> Columns => new[] {
            "CredentialApplicationTypeDocumentTypeId",
            "CredentialApplicationTypeId",
            "DocumentTypeId",
            "Mandatory",
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "1", "10", "0" });
            CreateOrUpdateTableRow(new[] { "2", "1", "17", "0" });
            CreateOrUpdateTableRow(new[] { "3", "1", "18", "0" });
            CreateOrUpdateTableRow(new[] { "4", "1", "21", "1" });
            CreateOrUpdateTableRow(new[] { "5", "2", "9", "1" });
            CreateOrUpdateTableRow(new[] { "6", "2", "10", "0" });
            CreateOrUpdateTableRow(new[] { "7", "2", "11", "0" });
            CreateOrUpdateTableRow(new[] { "8", "2", "12", "0" });
            CreateOrUpdateTableRow(new[] { "9", "2", "13", "0" });
            CreateOrUpdateTableRow(new[] { "10", "2", "14", "0" });
            CreateOrUpdateTableRow(new[] { "11", "2", "15", "0" });
            CreateOrUpdateTableRow(new[] { "12", "2", "16", "0" });
            CreateOrUpdateTableRow(new[] { "13", "2", "17", "0" });
            CreateOrUpdateTableRow(new[] { "14", "2", "24", "0" });
            CreateOrUpdateTableRow(new[] { "15", "3", "9", "1" });
            CreateOrUpdateTableRow(new[] { "16", "3", "24", "0" });
            CreateOrUpdateTableRow(new[] { "17", "3", "17", "0" });
            CreateOrUpdateTableRow(new[] { "18", "4", "17", "0" });
            CreateOrUpdateTableRow(new[] { "19", "5", "17", "0" });
            CreateOrUpdateTableRow(new[] { "20", "2", "25", "0" });
            CreateOrUpdateTableRow(new[] { "21", "2", "26", "0" });
        
            CreateOrUpdateTableRow(new[] { "22", "6", "9", "1" });
            CreateOrUpdateTableRow(new[] { "23", "6", "27", "0" });
            CreateOrUpdateTableRow(new[] { "24", "6", "12", "0" });
            CreateOrUpdateTableRow(new[] { "25", "6", "28", "0" });
            CreateOrUpdateTableRow(new[] { "26", "6", "24", "0" });
            CreateOrUpdateTableRow(new[] { "27", "6", "8", "0" });
            CreateOrUpdateTableRow(new[] { "28", "7", "9", "1" });
            CreateOrUpdateTableRow(new[] { "29", "7", "24", "0" });
            CreateOrUpdateTableRow(new[] { "30", "7", "17", "0" });

            CreateOrUpdateTableRow(new[] { "31", "8", "32", "0" });
            CreateOrUpdateTableRow(new[] { "32", "8", "10", "0" });

            CreateOrUpdateTableRow(new[] { "33", "9", "24", "0" });
            CreateOrUpdateTableRow(new[] { "34", "9", "9", "1" });
            CreateOrUpdateTableRow(new[] { "35", "8", "24", "0" });

			CreateOrUpdateTableRow(new[] { "36", "10", "9", "0" });
			CreateOrUpdateTableRow(new[] { "37", "10", "11", "0" });
			CreateOrUpdateTableRow(new[] { "38", "10", "15", "0" });
			CreateOrUpdateTableRow(new[] { "39", "10", "16", "0" });
			CreateOrUpdateTableRow(new[] { "40", "10", "24", "0" });
			CreateOrUpdateTableRow(new[] { "41", "10", "17", "0" });
            CreateOrUpdateTableRow(new[] { "42", "13", "9", "1" });
            CreateOrUpdateTableRow(new[] { "43", "13", "24", "0" });
            CreateOrUpdateTableRow(new[] { "44", "13", "17", "0" });

            CreateOrUpdateTableRow(new[] { "45", "16", "9", "0" });
            CreateOrUpdateTableRow(new[] { "46", "16", "11", "0" });
            CreateOrUpdateTableRow(new[] { "47", "16", "17", "0" });
            CreateOrUpdateTableRow(new[] { "48", "16", "24", "0" });
        }
    }
}
