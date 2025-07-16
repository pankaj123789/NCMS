using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class RefundMethodTypeScriptGenerator : BaseScriptGenerator
    {
        public RefundMethodTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblRefundMethodType";

        public override IList<string> Columns => new[]
        {
            "RefundMethodTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Undefined", "Undefined" });
            CreateOrUpdateTableRow(new[] { "2", "CreditCard", "Credit Card" });
            CreateOrUpdateTableRow(new[] { "3", "DirectDeposit", "Direct Deposit" });
            CreateOrUpdateTableRow(new[] { "4", "PayPal", "PayPal" });
        }
    }
}
