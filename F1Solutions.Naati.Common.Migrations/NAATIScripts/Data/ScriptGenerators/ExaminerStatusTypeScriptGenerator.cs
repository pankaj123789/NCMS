using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class ExaminerStatusTypeScriptGenerator : BaseScriptGenerator
    {
        public ExaminerStatusTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblExaminerStatusType";
        public override IList<string> Columns => new[] {
            "ExaminerStatusTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "InProgress", "In Progress" });
            CreateOrUpdateTableRow(new[] { "2", "Submitted", "Submitted" });
            CreateOrUpdateTableRow(new[] { "3", "Overdue", "Overdue" });
        }
    }
}
