using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class EmailSendStatusTypeScriptGenerator : BaseScriptGenerator
    {
        public EmailSendStatusTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblEmailSendStatusType";
        public override IList<string> Columns => new[] {
            "EmailSendStatusTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Requested", "Requested" });
            CreateOrUpdateTableRow(new[] { "2", "Sending", "Sending" });
            CreateOrUpdateTableRow(new[] { "3", "Retry", "Failed - pending retry" });
            CreateOrUpdateTableRow(new[] { "4", "Failed", "Failed permanently" });
            CreateOrUpdateTableRow(new[] { "5", "Successful", "Successful" });
        }
    }
}
