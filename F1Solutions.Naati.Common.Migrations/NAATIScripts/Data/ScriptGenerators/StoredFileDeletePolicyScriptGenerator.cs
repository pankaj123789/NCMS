using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class StoredFileDeletePolicyScriptGenerator : BaseScriptGenerator
    {
        public StoredFileDeletePolicyScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }
        public override string TableName => "tblStoredFileDeletePolicy";

        public override IList<string> Columns => new[] {
            "StoredFileDeletePolicyId",
            "PolicyExecutionOrder",
            "PolicyDescription",
            "DaysToKeep",

        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "1", "Delete after 1 year from Application Completed.","183"});
            CreateOrUpdateTableRow(new[] { "2", "2", "Delete after 3 years from Application Completed.", "1095" });
            CreateOrUpdateTableRow(new[] { "3", "3", "Delete 3 years after the document was uploaded.", "1095" });
            CreateOrUpdateTableRow(new[] { "4", "4", "If the user is not a Practitioner, delete after 3 years.", "1095" });
            CreateOrUpdateTableRow(new[] { "5", "5", "Delete after 3 years from the Test Material Request being Finalised.", "1095" });
            CreateOrUpdateTableRow(new[] { "6", "6", "Delete after 6 months from Application Completed.", "183" });
            CreateOrUpdateTableRow(new[] { "7", "7", "Delete 5 years after the document was uploaded.", "1825" });
            CreateOrUpdateTableRow(new[] { "8", "8", "Delete after 4 years from the test result issued", "1460" });
            CreateOrUpdateTableRow(new[] { "9", "9", "Delete after 1 year of the recertification application completed", "365" });
        }
    }
}
