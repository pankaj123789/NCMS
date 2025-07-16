using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialTypeCrossSkillScriptGenerator : BaseScriptGenerator
    {
        public CredentialTypeCrossSkillScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblCredentialTypeCrossSkill";

        public override IList<string> Columns => new string[]
        {
            "CredentialTypeCrossSkillId",
            "CredentialTypeFromId",
            "CredentialTypeToId",

        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "3", "6" });
            CreateOrUpdateTableRow(new[] { "2", "3", "5" });
            CreateOrUpdateTableRow(new[] { "3", "3", "12" });
            CreateOrUpdateTableRow(new[] { "4", "3", "13" });
            CreateOrUpdateTableRow(new[] { "5", "3", "19" });
            CreateOrUpdateTableRow(new[] { "6", "3", "20" });
            CreateOrUpdateTableRow(new[] { "7", "2", "6" });
            CreateOrUpdateTableRow(new[] { "8", "2", "5" });
            CreateOrUpdateTableRow(new[] { "9", "2", "12" });
            CreateOrUpdateTableRow(new[] { "10", "2", "13" });
            CreateOrUpdateTableRow(new[] { "11", "2", "19" });
            CreateOrUpdateTableRow(new[] { "12", "2", "20" });
            CreateOrUpdateTableRow(new[] { "13", "10", "2" });
            CreateOrUpdateTableRow(new[] { "14", "10", "1" });
            CreateOrUpdateTableRow(new[] { "15", "10", "12" });
            CreateOrUpdateTableRow(new[] { "16", "10", "13" });
            CreateOrUpdateTableRow(new[] { "17", "10", "19" });
            CreateOrUpdateTableRow(new[] { "18", "10", "20" });
            CreateOrUpdateTableRow(new[] { "19", "7", "2" });
            CreateOrUpdateTableRow(new[] { "20", "7", "1" });
            CreateOrUpdateTableRow(new[] { "21", "7", "12" });
            CreateOrUpdateTableRow(new[] { "22", "7", "13" });
            CreateOrUpdateTableRow(new[] { "23", "7", "19" });
            CreateOrUpdateTableRow(new[] { "24", "7", "20" });
            CreateOrUpdateTableRow(new[] { "25", "6", "2" });
            CreateOrUpdateTableRow(new[] { "26", "6", "1" });
            CreateOrUpdateTableRow(new[] { "27", "6", "12" });
            CreateOrUpdateTableRow(new[] { "28", "6", "13" });
            CreateOrUpdateTableRow(new[] { "29", "6", "19" });
            CreateOrUpdateTableRow(new[] { "30", "6", "20" });
            CreateOrUpdateTableRow(new[] { "31", "13", "2" });
            CreateOrUpdateTableRow(new[] { "32", "13", "1" });
            CreateOrUpdateTableRow(new[] { "33", "13", "12" });
            CreateOrUpdateTableRow(new[] { "34", "13", "13" });
            CreateOrUpdateTableRow(new[] { "35", "13", "19" });
            CreateOrUpdateTableRow(new[] { "36", "13", "20" });
            CreateOrUpdateTableRow(new[] { "37", "21", "2" });
            CreateOrUpdateTableRow(new[] { "38", "21", "1" });
            CreateOrUpdateTableRow(new[] { "39", "21", "12" });
            CreateOrUpdateTableRow(new[] { "40", "21", "13" });
            CreateOrUpdateTableRow(new[] { "41", "21", "6" });
            CreateOrUpdateTableRow(new[] { "42", "21", "5" });
            CreateOrUpdateTableRow(new[] { "43", "20", "2" });
            CreateOrUpdateTableRow(new[] { "44", "20", "1" });
            CreateOrUpdateTableRow(new[] { "45", "20", "12" });
            CreateOrUpdateTableRow(new[] { "46", "20", "13" });
            CreateOrUpdateTableRow(new[] { "47", "20", "19" });
            CreateOrUpdateTableRow(new[] { "48", "20", "20" });
            CreateOrUpdateTableRow(new[] { "49", "21", "36" });
            CreateOrUpdateTableRow(new[] { "50", "21", "37" });
        }
    }
}
