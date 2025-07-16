using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialApplicationRefundPolicyScriptGenerator : BaseScriptGenerator
    {
        public CredentialApplicationRefundPolicyScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblCredentialApplicationRefundPolicy";

        public override IList<string> Columns => new[] {
                                                           "CredentialApplicationRefundPolicyId",
                                                           "Name",
                                                           "Description",
                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Default", "<p>To read NAATI's full cancellation policy, please visit the <a href=\"https://www.naati.com.au/policies/terms-and-conditions/\" target=\"_blank\">terms and conditions</a> of our website.</p>" });

            CreateOrUpdateTableRow(new[] { "2", "General",
                "<p>Test bookings can be rescheduled up to and including 8 days before your test date.</p>" +
                "<ul style=\"margin-top:4px; margin-bottom:4px; padding-left:20px;\">" +
                "<li>Changes 7 days or less before test date are not possible and are considered a cancellation. You will receive a 75% refund upon request via your myNAATI account.</li>" +
                "<li>Cancellations can be requested any time before the test is due to start.</li>" +
                "<li>Non-attendance on the test date or cancellations requested after the scheduled start time will result in no refund being offered.</li>" +
                "</ul>" +
                "<p style=\"margin-top:4px;\">To read NAATI''s full cancellation policy, please visit the " +
                "<a href=\"https://www.naati.com.au/policies/terms-and-conditions/\" target=\"_blank\">terms and conditions</a> of our website.</p>"
            });

            CreateOrUpdateTableRow(new[] { "3", "General",
            "<p>Test bookings can be rescheduled up to and including 8 days before your test date.</p>" +
            "<ul style=\"margin-top:4px; margin-bottom:4px; padding-left:20px;\">" +
            "<li>Changes 7 days or less before test date are not possible and are considered a cancellation. You will receive a 75% refund upon request via your myNAATI account.</li>" +
            "<li>Cancellations can be requested any time before the test is due to start.</li>" +
            "<li>Non-attendance on the test date or cancellations requested after the scheduled start time will result in no refund being offered.</li>" +
            "</ul>" +
            "<p style=\"margin-top:4px;\">To read NAATI''s full cancellation policy, please visit the " +
            "<a href=\"https://www.naati.com.au/policies/terms-and-conditions/\" target=\"_blank\">terms and conditions</a> of our website.</p>"
            });

        }
    }
}
