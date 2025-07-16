using System;
using System.Collections.Generic;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Security;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    // NOT REQUIRED. DELETE AFTER DEVELOPMENT.
    public class SecurityVerbScriptGenerator : BaseScriptGenerator
    {
        public SecurityVerbScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblSecurityVerb";

        public override IList<string> Columns => new[]
        {
            "SecurityVerbId",
            "Name",
            "DisplayName",
            "Value"
        };

        public override void RunScripts()
        {
            var names = Enum.GetNames(typeof(SecurityVerbName));
            var values = Enum.GetValues(typeof(SecurityVerbName));
            for (int i = 0; i < values.Length; i++)
            {
                CreateOrUpdateTableRow(new[]
                {
                    (i + 1).ToString(),
                    names[i],
                    ((SecurityVerbName)values.GetValue(i)).GetDescription(),
                    ((long) values.GetValue(i)).ToString()
                });
            }
        }
    }
}