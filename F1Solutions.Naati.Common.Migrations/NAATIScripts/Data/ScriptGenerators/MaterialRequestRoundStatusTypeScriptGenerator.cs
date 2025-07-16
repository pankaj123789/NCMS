using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class MaterialRequestRoundStatusTypeScriptGenerator : BaseScriptGenerator
    {
        public MaterialRequestRoundStatusTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblMaterialRequestRoundStatusType";
        public override IList<string> Columns => new[] {
            "MaterialRequestRoundStatusTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "SentForDevelopment", "Sent For Development" });
            CreateOrUpdateTableRow(new[] { "2", "AwaitingApproval", "Awaiting Approval" });
            CreateOrUpdateTableRow(new[] { "3", "Rejected", "Rejected" });
            CreateOrUpdateTableRow(new[] { "4", "Approved", "Approved" });
            CreateOrUpdateTableRow(new[] { "5", "Cancelled", "Cancelled" });
        }
    }
}
