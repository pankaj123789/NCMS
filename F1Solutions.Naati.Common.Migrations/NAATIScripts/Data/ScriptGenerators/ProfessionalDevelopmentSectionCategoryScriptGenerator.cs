using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class ProfessionalDevelopmentSectionCategoryScriptGenerator : BaseScriptGenerator
    {
        public ProfessionalDevelopmentSectionCategoryScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblProfessionalDevelopmentSectionCategory";

        public override IList<string> Columns => new[]
        {
            "ProfessionalDevelopmentSectionCategoryId",
            "ProfessionalDevelopmentSectionId",
            "ProfessionalDevelopmentCategoryId",
            "PdPointsLimitTypeId",
            "PointsLimit"
        };
        public override void RunScripts()
        {

            CreateOrUpdateTableRow(new[] { "1", "1", "1", null,null});
            CreateOrUpdateTableRow(new[] { "2", "1", "2", null,null});
            CreateOrUpdateTableRow(new[] { "3", "1", "3", "1", "20" });
            CreateOrUpdateTableRow(new[] { "4", "1", "4" ,"1","60"});
            CreateOrUpdateTableRow(new[] { "5", "1", "5", "1", "20" });
            CreateOrUpdateTableRow(new[] { "6", "1", "6", "1", "10" });
            CreateOrUpdateTableRow(new[] { "7", "1", "7", "2", "20" });
            CreateOrUpdateTableRow(new[] { "8", "1", "8", "1", "20" });
            CreateOrUpdateTableRow(new[] { "9", "1", "9", "1", "10" });
            CreateOrUpdateTableRow(new[] { "10", "1","10","2", "10" });
            CreateOrUpdateTableRow(new[] { "11", "1","11","2", "10" });
            CreateOrUpdateTableRow(new[] { "12", "2","12", null, null });
            CreateOrUpdateTableRow(new[] { "13", "2","13", "1", "20" });
            CreateOrUpdateTableRow(new[] { "14", "2","14",null,null});
            CreateOrUpdateTableRow(new[] { "15", "2","15",null,null});
            CreateOrUpdateTableRow(new[] { "16", "2","16","1","40"});
            CreateOrUpdateTableRow(new[] { "17", "2","17","1","80"});
            CreateOrUpdateTableRow(new[] { "18", "2","18","2","40"});
            CreateOrUpdateTableRow(new[] { "19", "2","19",null,null});
            CreateOrUpdateTableRow(new[] { "20", "2","20",null,null});
            CreateOrUpdateTableRow(new[] { "21", "2","21","2","10"});
            CreateOrUpdateTableRow(new[] { "22", "2","22","2","10"});
            CreateOrUpdateTableRow(new[] { "23", "2","23","1","20"});
            CreateOrUpdateTableRow(new[] { "24", "2","24","2","10"});
            CreateOrUpdateTableRow(new[] { "25", "2","25","1","10"});
            CreateOrUpdateTableRow(new[] { "26", "2","26",null,null});
            CreateOrUpdateTableRow(new[] { "27", "2","27","1","10"});
            CreateOrUpdateTableRow(new[] { "28", "2","28",null,null});
            CreateOrUpdateTableRow(new[] { "29", "2","29","2","20"});
            CreateOrUpdateTableRow(new[] { "30", "2","30","2","10"});
                                                   
            CreateOrUpdateTableRow(new[] { "32", "2","32","2","20"});
            CreateOrUpdateTableRow(new[] { "33", "2","33","1","10"});
            CreateOrUpdateTableRow(new[] { "34", "3","34",null,null});
            CreateOrUpdateTableRow(new[] { "35", "3","35",null,null});
            CreateOrUpdateTableRow(new[] { "36", "3","36",null,null});
            CreateOrUpdateTableRow(new[] { "37", "3","37","2","20"});
            CreateOrUpdateTableRow(new[] { "38", "3","38","2","20"});
            CreateOrUpdateTableRow(new[] { "39", "3","39","2","10"});
            CreateOrUpdateTableRow(new[] { "40", "3","40","2","10"});
            CreateOrUpdateTableRow(new[] { "41", "3","41","2","10"});
            CreateOrUpdateTableRow(new[] { "42", "3","42","2","10"});
            CreateOrUpdateTableRow(new[] { "43", "3","43","2","10"});
            CreateOrUpdateTableRow(new[] { "44", "3","44","2","10"});
            CreateOrUpdateTableRow(new[] { "45", "3","45","2","10"});
            CreateOrUpdateTableRow(new[] { "46", "3","46","1","10"});
            CreateOrUpdateTableRow(new[] { "47", "1", "47", "1", "40" });
            CreateOrUpdateTableRow(new[] { "48", "2", "47", "1", "40" });
        }
    }
}
