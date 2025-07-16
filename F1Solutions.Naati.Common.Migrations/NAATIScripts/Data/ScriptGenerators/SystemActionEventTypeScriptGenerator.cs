using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class SystemActionEventTypeScriptGenerator: BaseScriptGenerator
    {
        public SystemActionEventTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblSystemActionEventType";

        public override IList<string> Columns => new[]
        {
            "SystemActionEventTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(SystemActionEventTypeName.None, new [] { "-" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.InvoiceCreatedToApplicant, new [] { "Invoice Created To Applicant" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor, new [] { "Invoice Created To Untrusted Sponsor" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.CreditCardPaymentReceived, new [] { "Credit Card Payment Received" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.SupplementaryTestOffered, new [] { "Supplementary Test Offered" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.ConcededPassOffered, new [] { "Conceded Pass Offered" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.FailedTestReverted, new [] { "Failed Test Reverted" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.PassedTestReverted, new [] { "Passed Test Reverted" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.IssuedCredentialReverted, new [] { "Issued Credential Reverted" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.InvalidatedTestReverted, new [] { "Invalidated Test Reverted" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.TestSessionConfirmed, new [] { "Test Session Confirmed" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.CoordinatorChanged, new [] { "Coordinator Changed" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.CollaboratorAdded, new [] { "Collaborator Added" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.CollaboratorRemoved, new [] { "Collaborator Removed" });
            CreateOrUpdateTableRow(SystemActionEventTypeName.CreditCardRefundIssuedToApplicant, new [] { "Credit Card Refund Issued To Applicant" });
        }
    }
}
