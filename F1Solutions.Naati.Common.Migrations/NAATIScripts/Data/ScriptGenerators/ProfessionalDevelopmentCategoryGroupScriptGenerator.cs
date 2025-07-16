using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    class ProfessionalDevelopmentCategoryGroupScriptGenerator:BaseScriptGenerator
    {
        public ProfessionalDevelopmentCategoryGroupScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblProfessionalDevelopmentCategoryGroup";
        public override IList<string> Columns => new[] {
            "ProfessionalDevelopmentCategoryGroupId",
            "Name",
            "Description",
            "RequiredPointsPerYear"
        };
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "ETHICS", "ETHICS","3.3334" });
        }
    }
}

