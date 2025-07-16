using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class ExternalAccountingOperationTypeScriptGenerator : BaseScriptGenerator
    {
        public ExternalAccountingOperationTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tluExternalAccountingOperationType";
        public override IList<string> Columns => new[] {
                                                           "ExternalAccountingOperationTypeId",
                                                           "Name",
                                                           "DisplayName"
                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "CreateInvoice", "Create invoice" });
            CreateOrUpdateTableRow(new[] { "2", "CreatePayment", "Create payment" });
            CreateOrUpdateTableRow(new[] { "3", "CreateContact", "Create contact" });
            CreateOrUpdateTableRow(new[] { "4", "UpdateContact", "Update contact" });
            CreateOrUpdateTableRow(new[] { "5", "CreateCreditNote", "Create Credit Note" });
        }
    }
}