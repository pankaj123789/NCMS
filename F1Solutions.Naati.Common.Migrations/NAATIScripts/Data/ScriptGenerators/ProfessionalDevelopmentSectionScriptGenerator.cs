using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class ProfessionalDevelopmentSectionScriptGenerator : BaseScriptGenerator
    {
        public ProfessionalDevelopmentSectionScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblProfessionalDevelopmentSection";
        public override IList<string> Columns => new[] {
            "ProfessionalDevelopmentSectionId",
            "Name",
            "Description",
            "RequiredPointsPerYear"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "CATEGORY 1: SKILLS DEVELOPMENT AND KNOWLEDGE", "CATEGORY 1: SKILLS DEVELOPMENT AND KNOWLEDGE","10" });
            CreateOrUpdateTableRow(new[] { "2", "CATEGORY 2: INDUSTRY ENGAGEMENT", "CATEGORY 2: INDUSTRY ENGAGEMENT","10" });
            CreateOrUpdateTableRow(new[] { "3", "CATEGORY 3: MAINTENANCE OF LANGUAGE", "CATEGORY 3: MAINTENANCE OF LANGUAGE","10" });
        }
    }
}
