using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class TestComponentBaseTypeScriptGenerator : BaseScriptGenerator
    {
        public TestComponentBaseTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblTestComponentBaseType";

        public override IList<string> Columns => new[]
        {
            "TestComponentBaseTypeId", "Name", "DisplayName"
        };
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Language", "Language" });
            CreateOrUpdateTableRow(new[] { "2", "Skill", "Skill" });
        }
    }
}
