using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class NotificationTypeScriptGenerator : BaseScriptGenerator
    {
        public NotificationTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblNotificationType";

        public override IList<string> Columns => new[]
        {
            "NotificationTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "DownloadTestMaterial", "Download Test Material"});
            CreateOrUpdateTableRow(new[] { "2", "ErrorMessage", "Error Message"});
        }
    }
}
