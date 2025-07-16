using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class ProfessionalDevelopmentCategoryRequirementScriptGenerator:BaseScriptGenerator
    {
        public ProfessionalDevelopmentCategoryRequirementScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblProfessionalDevelopmentCategoryRequirement";
        public override IList<string> Columns => new[] {
            "ProfessionalDevelopmentCategoryRequirementId",
            "ProfessionalDevelopmentCategoryId",
            "ProfessionalDevelopmentRequirementId",
            "Points",
        };
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "1", "1", "60" });
            CreateOrUpdateTableRow(new[] { "2", "2", "2", "40" });
            CreateOrUpdateTableRow(new[] { "3", "3", "2", "20" });
            CreateOrUpdateTableRow(new[] { "4", "4", "37", "20" });
            CreateOrUpdateTableRow(new[] { "5", "4", "4", "10" });
            CreateOrUpdateTableRow(new[] { "6", "5", "3", "20" });
            CreateOrUpdateTableRow(new[] { "7", "5", "5", "10" });
            CreateOrUpdateTableRow(new[] { "8", "6", "6", "10" });
            CreateOrUpdateTableRow(new[] { "9", "7", "7", "10" });
            CreateOrUpdateTableRow(new[] { "10", "8", "8", "20" });
            CreateOrUpdateTableRow(new[] { "11", "9", "6", "10" });
            CreateOrUpdateTableRow(new[] { "12", "10", "9", "10" });
            CreateOrUpdateTableRow(new[] { "13", "11", "10", "10" });
            CreateOrUpdateTableRow(new[] { "14", "12", "11", "20" });
            CreateOrUpdateTableRow(new[] { "15", "13", "12", "20" });
            CreateOrUpdateTableRow(new[] { "16", "14", "13", "20" });
            CreateOrUpdateTableRow(new[] { "17", "15", "14", "20" });
            CreateOrUpdateTableRow(new[] { "18", "16", "15", "20" });
            CreateOrUpdateTableRow(new[] { "19", "17", "16", "40" });
            CreateOrUpdateTableRow(new[] { "20", "18", "17", "20" });
            CreateOrUpdateTableRow(new[] { "21", "19", "8", "20" });
            CreateOrUpdateTableRow(new[] { "22", "20", "18", "10" });
            CreateOrUpdateTableRow(new[] { "23", "20", "19", "20" });
            CreateOrUpdateTableRow(new[] { "24", "21", "20", "10" });
            CreateOrUpdateTableRow(new[] { "25", "22", "9", "10" });
            CreateOrUpdateTableRow(new[] { "26", "23", "21", "10" });
            CreateOrUpdateTableRow(new[] { "27", "24", "9", "10" });
            CreateOrUpdateTableRow(new[] { "28", "25", "22", "10" });
            CreateOrUpdateTableRow(new[] { "29", "26", "23", "10" });
            CreateOrUpdateTableRow(new[] { "30", "27", "10", "10" });
            CreateOrUpdateTableRow(new[] { "31", "28", "16", "30" });
            CreateOrUpdateTableRow(new[] { "32", "29", "4", "10" });
            CreateOrUpdateTableRow(new[] { "33", "29", "3", "20" });
            CreateOrUpdateTableRow(new[] { "34", "30", "24", "10" });
             // CreateOrUpdateTableRow(new[] { "35", "31", "12", "20" });
            CreateOrUpdateTableRow(new[] { "36", "32", "4", "10" });
            CreateOrUpdateTableRow(new[] { "37", "32", "3", "20" });
            CreateOrUpdateTableRow(new[] { "38", "33", "10", "10" });
            CreateOrUpdateTableRow(new[] { "39", "34", "25", "40" });
            CreateOrUpdateTableRow(new[] { "40", "34", "26", "60" });
            CreateOrUpdateTableRow(new[] { "41", "35", "27", "10" });
            CreateOrUpdateTableRow(new[] { "42", "35", "28", "20" });
            CreateOrUpdateTableRow(new[] { "43", "36", "29", "10" });
            CreateOrUpdateTableRow(new[] { "44", "36", "16", "20" });
            CreateOrUpdateTableRow(new[] { "45", "37", "30", "10" });
            CreateOrUpdateTableRow(new[] { "46", "37", "31", "20" });
            CreateOrUpdateTableRow(new[] { "47", "38", "30", "10" });
            CreateOrUpdateTableRow(new[] { "48", "38", "31", "20" });
            CreateOrUpdateTableRow(new[] { "49", "39", "32", "10" });
            CreateOrUpdateTableRow(new[] { "50", "40", "32", "10" });
            CreateOrUpdateTableRow(new[] { "51", "41", "33", "10" });
            CreateOrUpdateTableRow(new[] { "52", "42", "33", "10" });
            CreateOrUpdateTableRow(new[] { "53", "43", "34", "10" });
            CreateOrUpdateTableRow(new[] { "54", "44", "9", "10" });
            CreateOrUpdateTableRow(new[] { "55", "45", "32", "10" });
            CreateOrUpdateTableRow(new[] { "56", "46", "10", "10" });
            CreateOrUpdateTableRow(new[] { "57", "4", "35", "40" });
            CreateOrUpdateTableRow(new[] { "58", "47", "36", "40" });


        }

        
        public override void RunDescendantOrderScripts()
        {
            DeleteTableRow("35");
        }
    }
}
