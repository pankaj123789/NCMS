using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialApplicationFieldOptionScriptGenerator : BaseScriptGenerator
    {

        public CredentialApplicationFieldOptionScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblCredentialApplicationFieldOption";
        public override IList<string> Columns => new[] {
            "CredentialApplicationFieldOptionId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "CreditCardOnline", "Credit Card (Online)" });
            CreateOrUpdateTableRow(new[] { "2", "DirectDeposit", "Direct Deposit" });
            CreateOrUpdateTableRow(new[] { "3", "CashCheque", "Cash/Cheque" });

            CreateOrUpdateTableRow(new[] { "4", "Simplified", "Simplified Chinese" });
            CreateOrUpdateTableRow(new[] { "5", "Traditional", "Traditional Chinese" });
        }
    }
    }
