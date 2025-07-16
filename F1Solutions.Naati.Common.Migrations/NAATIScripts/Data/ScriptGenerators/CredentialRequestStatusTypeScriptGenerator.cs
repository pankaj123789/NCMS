using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialRequestStatusTypeScriptGenerator : BaseScriptGenerator
    {
        public CredentialRequestStatusTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner)
        {
        }

        public override string TableName => "tblCredentialRequestStatusType";

        public override IList<string> Columns => new[]
        {
            "CredentialRequestStatusTypeId",
            "Name",
            "DisplayName",
            "DisplayOrder"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] {"1", "Draft", "Draft", "1",});
            CreateOrUpdateTableRow(new[] {"2", "Rejected", "Application Rejected", "28",});
            CreateOrUpdateTableRow(new[] {"3", "RequestEntered", "Request Entered", "6",});
            CreateOrUpdateTableRow(new[] {"4", "ReadyForAssessment", "Ready for Assessment", "7"});
            CreateOrUpdateTableRow(new[] {"5", "BeingAssessed", "Being Assessed", "8"});
            CreateOrUpdateTableRow(new[] {"6", "Pending", "Pending", "9"});
            CreateOrUpdateTableRow(new[] {"7", "AssessmentFailed", "Assessment Failed", "10"});
            CreateOrUpdateTableRow(new[] {"8", "AssessmentPaidReview", "Assessment Paid Review", "11"});
            CreateOrUpdateTableRow(new[] {"9", "AssessmentComplete", "Assessment Complete", "12"});
            CreateOrUpdateTableRow(new[] {"10", "EligibleForTesting", "Eligible for Testing", "13"});
            CreateOrUpdateTableRow(new[] {"11", "TestFailed", "Test Failed", "21"});
            CreateOrUpdateTableRow(new[] {"12", "CertificationIssued", "Certification Issued", "24"});
            CreateOrUpdateTableRow(new[] {"13", "Cancelled", "Cancelled", "4"});
            CreateOrUpdateTableRow(new[] {"14", "Deleted", "Deleted", "3"});
            CreateOrUpdateTableRow(new[] {"15", "AwaitingTestPayment", "Awaiting Test Payment", "14"});
            CreateOrUpdateTableRow(new[] {"16", "Withdrawn", "Withdrawn", "27"});
            CreateOrUpdateTableRow(new[] {"17", "TestAccepted", "Test Accepted", "16"});
            CreateOrUpdateTableRow(new[] {"19", "TestSessionAccepted", "Test Session Accepted", "18"});
            CreateOrUpdateTableRow(new[] {"20", "CheckedIn", "Checked In", "19"});
            CreateOrUpdateTableRow(new[] {"21", "TestSat", "Test Sat", "20"});
            CreateOrUpdateTableRow(new[] {"22", "IssuedPassResult", "Issued Pass Result", "25"});
            CreateOrUpdateTableRow(new[] {"23", "UnderPaidReview", "Under Paid Review", "26"});
            CreateOrUpdateTableRow(new[] {"24", "AwaitingSupplementaryTestPayment", "Awaiting Supplementary Test Payment", "15"});
            CreateOrUpdateTableRow(new[] {"25", "AwaitingApplicationPayment", "Awaiting Application Payment", "5" });
            CreateOrUpdateTableRow(new[] {"26", "ProcessingSubmission", "Processing Submission", "2" });
            CreateOrUpdateTableRow(new[] {"27", "ProcessingPaidReviewInvoice", "Processing Paid Review Invoice", "22" });
            CreateOrUpdateTableRow(new[] {"28", "AwaitingPaidReviewPayment", "Awaiting Paid Review Payment", "23" });
            CreateOrUpdateTableRow(new[] {"29", "ProcessingSupplementaryTestInvoice", "Processing Supplementary Test Invoice", "29" });
            CreateOrUpdateTableRow(new[] {"30", "TestInvalidated", "Test Invalidated", "30" });
            CreateOrUpdateTableRow(new[] {"31", "ToBeIssued", "To Be Issued", "31" });
            CreateOrUpdateTableRow(new[] {"32", "ProcessingTestInvoice", "Processing Test Invoice", "32" });
            CreateOrUpdateTableRow(new[] {"33", "ProcessingRequest", "Processing Request", "33" });
            CreateOrUpdateTableRow(new[] {"34", "RefundRequested", "Refund Requested", "34" });
            CreateOrUpdateTableRow(new[] {"35", "ProcessingCreditNote", "Processing Credit Note", "35" });
            CreateOrUpdateTableRow(new[] {"36", "AwaitingCreditNotePayment", "Awaiting Credit Note Payment", "36" });
            CreateOrUpdateTableRow(new[] {"37", "RefundRequestApproved", "Refund Request Approved", "37" });
            CreateOrUpdateTableRow(new[] {"38", "RefundFailed", "Refund Failed", "38" });
            CreateOrUpdateTableRow(new[] {"39", "CertificationOnHold", "Certification On Hold", "39" });
            CreateOrUpdateTableRow(new[] {"40", "IssuePracticeTestResults", "Issued Practice Test Results", "40" });
            CreateOrUpdateTableRow(new[] { "41", "OnHoldToBeIssued", "On Hold To Be Issued", "41" });
        }

        public override void RunDescendantOrderScripts()
        {
            DeleteTableRow("18");
            DeleteTableRow("42");
            DeleteTableRow("43");
        }
    }
}