using System;
using System.Collections.Generic;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Security;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class SecurityNounScriptGenerator : BaseScriptGenerator
    {
        public SecurityNounScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblSecurityNoun";

        public override IList<string> Columns => new[]
        {
            "SecurityNounId",
            "Name",
            "DisplayName",
        };

        public override void RunScripts()
        {
            var names = Enum.GetNames(typeof(SecurityNounName));
            var values = Enum.GetValues(typeof(SecurityNounName));
            for (int i = 0; i < values.Length; i++)
            {
                CreateOrUpdateTableRow(new[]
                {
                    ((int) values.GetValue(i)).ToString(),
                    names[i],
                    ((SecurityNounName)values.GetValue(i)).GetDescription(),
                });
            }
        }
    }
}