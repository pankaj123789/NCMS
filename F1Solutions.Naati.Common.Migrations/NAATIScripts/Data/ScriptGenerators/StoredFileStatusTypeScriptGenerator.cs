using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class StoredFileStatusTypeScriptGenerator : BaseScriptGenerator
    {
        public StoredFileStatusTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner)
        {
        }

        public override string TableName => "tblStoredFileStatusType";

        public override IList<string> Columns => new[]
        {
            "StoredFileStatusTypeId",
            "Name",
            "DisplayName",
            "DisplayOrder"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Current", "Current", "1", });
            CreateOrUpdateTableRow(new[] { "2", "Queued", "Queued", "2", });
            CreateOrUpdateTableRow(new[] { "3", "SoftDeleted", "Soft Deleted", "3", });
        }

        public override void RunDescendantOrderScripts()
        {
        }
    }
}