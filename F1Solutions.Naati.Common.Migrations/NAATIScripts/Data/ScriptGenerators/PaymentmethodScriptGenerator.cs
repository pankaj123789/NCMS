using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class PaymentmethodScriptGenerator : BaseScriptGenerator
    {
        public PaymentmethodScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblPaymentMethodType";
        public override IList<string> Columns => new[] {
            "PaymentMethodTypeId",
            "Name",
            "DisplayName",
            "ExternalReferenceId"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "CASH", "Cash payment", "fd546e2b-987c-eb11-b857-000d3a6a46a3" });
            CreateOrUpdateTableRow(new[] { "2", "CHEQUE", "Cheque payment", "01556e2b-987c-eb11-b857-000d3a6a46a3" });
            CreateOrUpdateTableRow(new[] { "3", "CARD", "Card payment", "4f7290b0-5081-eb11-b855-000d3a6a4611" });
            CreateOrUpdateTableRow(new[] { "5", "PAYPAL", "PayPal payment", "b252122b-5a80-eb11-b855-000d3a6a4611" });
            CreateOrUpdateTableRow(new[] { "6", "DD", "Direct Deposit", "a6e168b2-3d7c-eb11-b854-000d3ad1a7ea" });
            CreateOrUpdateTableRow(new[] { "7", "SECUREPAY", "Secure pay", "4f7290b0-5081-eb11-b855-000d3a6a4611" });
            CreateOrUpdateTableRow(new[] { "8", "BPAY", "BPAY- Manual", "fb546e2b-987c-eb11-b857-000d3a6a46a3" });
            CreateOrUpdateTableRow(new[] { "9", "BANK", "Bank Transfer", "fd546e2b-987c-eb11-b857-000d3a6a46a3" });
            CreateOrUpdateTableRow(new[] { "10", "CC", "Credit Card", "4f7290b0-5081-eb11-b855-000d3a6a4611" });
            CreateOrUpdateTableRow(new[] { "11", "EFT", "Electronic Funds Transfer", "02556e2b-987c-eb11-b857-000d3a6a46a3" });
            CreateOrUpdateTableRow(new[] { "12", "BPAY-BATCH", "BPAY Batch Generator", "9ee168b2-3d7c-eb11-b854-000d3ad1a7ea" });
        }
    }
}
