using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialPreRequisiteScriptGenerator : BaseScriptGenerator
    {
        public CredentialPreRequisiteScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblCredentialPreRequisite";
        public override IList<string> Columns => new[] {
            "CredentialPreRequisiteId",
            "CredentialTypeId",
            "CredentialApplicationTypeId",
            "CredentialTypePrerequisiteId",
            "SkillMatch",
            "ApplicationTypePrerequisiteId"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "6", "2" , "16", "0","4" });
            CreateOrUpdateTableRow(new[] { "2", "6", "2", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "3", "7", "2", "6", "1", "2" });
            CreateOrUpdateTableRow(new[] { "4", "8", "2", "7", "1", "2" });
            CreateOrUpdateTableRow(new[] { "5", "8", "2", "32", "0", "15" });
            CreateOrUpdateTableRow(new[] { "6", "9", "2", "7", "1", "2" });
            CreateOrUpdateTableRow(new[] { "7", "9", "2", "31", "0", "14" });

            CreateOrUpdateTableRow(new[] { "8", "6", "6", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "9", "6", "6", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "10", "7", "6", "6", "1", "6" });
            CreateOrUpdateTableRow(new[] { "11", "8", "6", "7", "1", "6" });
            CreateOrUpdateTableRow(new[] { "12", "8", "6", "32", "0", "15" });
            CreateOrUpdateTableRow(new[] { "13", "9", "6", "7", "1", "6" });
            CreateOrUpdateTableRow(new[] { "14", "9", "6", "31", "0", "14" });
            CreateOrUpdateTableRow(new[] { "15", "20", "2", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "16", "20", "2", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "17", "21", "2", "20", "1", "2" });
            CreateOrUpdateTableRow(new[] { "18", "22", "2", "21", "1", "2" });
            CreateOrUpdateTableRow(new[] { "19", "22", "2", "32", "0", "15" });
            CreateOrUpdateTableRow(new[] { "20", "23", "2", "21", "1", "2" });
            CreateOrUpdateTableRow(new[] { "21", "23", "2", "31", "0", "14" });
            CreateOrUpdateTableRow(new[] { "22", "20", "6", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "23", "20", "6", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "24", "21", "6", "20", "1", "6" });
            CreateOrUpdateTableRow(new[] { "25", "22", "6", "21", "1", "6" });
            CreateOrUpdateTableRow(new[] { "26", "22", "6", "32", "0", "15" });
            CreateOrUpdateTableRow(new[] { "27", "23", "6", "21", "1", "6" });
            CreateOrUpdateTableRow(new[] { "28", "23", "6", "31", "0", "14" });
            CreateOrUpdateTableRow(new[] { "29", "5", "2", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "30", "5", "2", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "31", "5", "6", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "32", "5", "6", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "33", "1", "2", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "34", "1", "2", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "35", "1", "6", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "36", "1", "6", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "37", "12", "2", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "38", "12", "2", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "39", "12", "6", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "40", "12", "6", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "41", "19", "2", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "42", "19", "2", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "43", "19", "6", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "44", "19", "6", "17", "0", "5" });
            //TFS 216642
            CreateOrUpdateTableRow(new[] { "45", "25", "10", "29", "0", "11" });
            CreateOrUpdateTableRow(new[] { "46", "25", "10", "30", "0", "12" });
            CreateOrUpdateTableRow(new[] { "47", "25", "6", "29", "0", "11" });
            CreateOrUpdateTableRow(new[] { "48", "25", "6", "30", "0", "12" });
            CreateOrUpdateTableRow(new[] { "49", "28", "10", "29", "0", "11" });
            CreateOrUpdateTableRow(new[] { "50", "28", "10", "30", "0", "12" });
            CreateOrUpdateTableRow(new[] { "51", "28", "6", "29", "0", "11" });
            CreateOrUpdateTableRow(new[] { "52", "28", "6", "30", "0", "12" });

            CreateOrUpdateTableRow(new[] { "53", "2", "2", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "54", "2", "2", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "55", "2", "6", "16", "0", "4" });
            CreateOrUpdateTableRow(new[] { "56", "2", "6", "17", "0", "5" });
            CreateOrUpdateTableRow(new[] { "57", "3", "2", "2", "0", "2" });
            CreateOrUpdateTableRow(new[] { "58", "3", "6", "2", "0", "6" });

            //TFS 216642
            CreateOrUpdateTableRow(new[] { "59", "26", "10", "29", "0", "11" });
            CreateOrUpdateTableRow(new[] { "60", "26", "10", "30", "0", "12" });
            CreateOrUpdateTableRow(new[] { "61", "27", "10", "26", "0", "10" });
        }
    }
}
