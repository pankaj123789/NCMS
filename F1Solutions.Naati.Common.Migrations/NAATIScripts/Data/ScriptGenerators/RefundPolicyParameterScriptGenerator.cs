using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class RefundPolicyParameterScriptGenerator : BaseScriptGenerator
    {
        public RefundPolicyParameterScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblRefundPolicyParameter";

        public override IList<string> Columns => new[] {
                                                           "RefundPolicyParameterId",
                                                           "CredentialApplicationRefundPolicyId",
                                                           "Name",
                                                           "Value",
                                                       };

        //if you change these, also change CredentialApplicationRefundPolicyScriptGenerator
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "2", "RefundPercentage", "0.75" });
            CreateOrUpdateTableRow(new[] { "2", "2", "HoursBeforeTestSitting", "0" });
            CreateOrUpdateTableRow(new[] { "3", "3", "RefundPercentage", "0.75" });
            CreateOrUpdateTableRow(new[] { "4", "3", "HoursBeforeTestSitting", "0" });
        }
    }
}
