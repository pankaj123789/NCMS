using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class TestMaterialLinkTypeScriptGenerator : BaseScriptGenerator
    {
        public TestMaterialLinkTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblTestMaterialLinkType";
        public override IList<string> Columns => new[] {
            "TestMaterialLinkTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Child", "Child" });
            CreateOrUpdateTableRow(new[] { "2", "Duplicated", "Duplicated" });
        }
    }
}
