using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class FeeTypeScriptGenerator : BaseScriptGenerator
    {
        public FeeTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblFeeType";
        public override IList<string> Columns => new[] {
                                                           "FeeTypeId",
                                                           "Name",
                                                           "DisplayName",
                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Application", "Application Fee" });
            CreateOrUpdateTableRow(new[] { "2", "ApplicationAssessment", "Assessment Fee" });
            CreateOrUpdateTableRow(new[] { "3", "Test", "Test Fee" });
            CreateOrUpdateTableRow(new[] { "4", "SupplementaryTest", "Supplementary Test Fee" });
            CreateOrUpdateTableRow(new[] { "5", "PaidTestReview", "Paid Test Review Fee" });

        }
    }
}
