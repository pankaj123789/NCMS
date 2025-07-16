using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class LanguageGroupScriptGenerator : BaseScriptGenerator
    {
        public LanguageGroupScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblLanguageGroup";
        public override IList<string> Columns  => new[] {
            "LanguageGroupId",
            "Name"
          
        };
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Indigenous" });
            CreateOrUpdateTableRow(new[] { "2", "Chin languages" });
            CreateOrUpdateTableRow(new[] { "3", "Prerequisites" });
        }
    }
}
