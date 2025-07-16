using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class TestMaterialTypeScriptGenerator : BaseScriptGenerator
    {
        public TestMaterialTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblTestMaterialType";
        public override IList<string> Columns => new[] {
            "TestMaterialTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Test", "Test Material" });
            CreateOrUpdateTableRow(new[] { "2", "Source", "Source Test Material" });
        }
    }
}
