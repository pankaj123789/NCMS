using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialApplicationStatusTypeScriptGenerator : BaseScriptGenerator
    {
        public CredentialApplicationStatusTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblCredentialApplicationStatusType";
        public override IList<string> Columns => new[] {
                                                           "CredentialApplicationStatusTypeId",
                                                           "Name",
                                                           "DisplayName",
                                                           "DisplayOrder"
                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1","Draft","Draft","1"});
            CreateOrUpdateTableRow(new[] { "2","Entered","Entered", "5" });
            CreateOrUpdateTableRow(new[] { "3","BeingChecked","Being Checked", "6" });
            CreateOrUpdateTableRow(new[] { "4","Rejected","Rejected", "10" });
            CreateOrUpdateTableRow(new[] { "5", "InProgress", "In Progress", "8" });
            CreateOrUpdateTableRow(new[] { "6","Completed","Completed", "9" });
            CreateOrUpdateTableRow(new[] { "7","Deleted","Deleted", "3" });
            CreateOrUpdateTableRow(new[] { "8","AwaitingAssessmentPayment","Awaiting Assessment Payment", "7" });
            CreateOrUpdateTableRow(new[] { "9", "AwaitingApplicationPayment", "Awaiting Application Payment", "4" });
            CreateOrUpdateTableRow(new[] { "10", "ProcessingSubmission", "Processing Submission", "2" });
            CreateOrUpdateTableRow(new[] { "11", "ProcessingApplicationInvoice", "Processing Application Invoice", "11" });
        }
    }
}