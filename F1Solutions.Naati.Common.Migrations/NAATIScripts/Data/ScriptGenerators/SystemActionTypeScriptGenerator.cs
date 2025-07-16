using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class SystemActionTypeScriptGenerator : BaseScriptGenerator
    {
        public override string TableName => "tblSystemActionType";
        public override IList<string> Columns  => new[]
        {
            "SystemActionTypeId",
            "Name",
            "DisplayName",
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(SystemActionTypeName.CreateApplication, new [] { "Create Application" });
            CreateOrUpdateTableRow(SystemActionTypeName.NewCredentialRequest, new [] { "New Credential Request" });
            CreateOrUpdateTableRow(SystemActionTypeName.SubmitApplication, new [] { "Submit Application" });
            CreateOrUpdateTableRow(SystemActionTypeName.StartChecking, new [] { "Start Checking Application" });
            CreateOrUpdateTableRow(SystemActionTypeName.CancelChecking, new [] { "Cancel Checking Application" });
            CreateOrUpdateTableRow(SystemActionTypeName.RejectApplication, new [] { "Reject Application" });
            CreateOrUpdateTableRow(SystemActionTypeName.FinishChecking, new [] { "Finish Checking Application" });
            CreateOrUpdateTableRow(SystemActionTypeName.FinaliseApplication, new [] { "Finalise Application" });
            CreateOrUpdateTableRow(SystemActionTypeName.DeleteApplication, new [] { "Delete Application" });
            CreateOrUpdateTableRow(SystemActionTypeName.AssessmentInvoicePaid, new [] { "Application Assessment Invoice Paid" });
            CreateOrUpdateTableRow(SystemActionTypeName.RejectApplicationAfterInvoice, new [] { "Reject Application After Invoice" });
            CreateOrUpdateTableRow(SystemActionTypeName.ApplicationSubmissionProcessed, new [] { "Application Submission Processed" });
            CreateOrUpdateTableRow(SystemActionTypeName.ApplicationInvoicePaid, new [] { "Application Invoice Paid" });
            CreateOrUpdateTableRow(SystemActionTypeName.ReactivateApplication, new [] { "Reactivate Application" });
            CreateOrUpdateTableRow(SystemActionTypeName.CloseApplication, new [] { "Close Application" });
            CreateOrUpdateTableRow(SystemActionTypeName.IssueRecertificationCredentials, new [] { "Issue Recertification Credentials" });
            CreateOrUpdateTableRow(SystemActionTypeName.AssessCredentials, new [] { "Assess Credentials" });
            CreateOrUpdateTableRow(SystemActionTypeName.SendRecertificationReminder, new [] { "Send Recertification Reminder" });

            CreateOrUpdateTableRow(SystemActionTypeName.StartAssessing, new [] { "Start Assessing" });
            CreateOrUpdateTableRow(SystemActionTypeName.CancelAssessment, new [] { "Cancel Assessment" });
            CreateOrUpdateTableRow(SystemActionTypeName.PassAssessment, new [] { "Pass Assessment" });
            CreateOrUpdateTableRow(SystemActionTypeName.PendAssessment, new [] { "Pend Assessment" });
            CreateOrUpdateTableRow(SystemActionTypeName.FailAssessment, new [] { "Fail Assessment" });
            CreateOrUpdateTableRow(SystemActionTypeName.FailPendingAssessment, new [] { "Fail Pending Assessment" });
            CreateOrUpdateTableRow(SystemActionTypeName.CreatePaidReview, new [] { "Create Paid Review" });
            CreateOrUpdateTableRow(SystemActionTypeName.PassReview, new [] { "Pass Review" });
            CreateOrUpdateTableRow(SystemActionTypeName.FailReview, new [] { "Fail Review" });
            CreateOrUpdateTableRow(SystemActionTypeName.IssueCredential, new [] { "Issue Credential" });
            CreateOrUpdateTableRow(SystemActionTypeName.CancelRequest, new [] { "Cancel Request" });
            CreateOrUpdateTableRow(SystemActionTypeName.DeleteRequest, new [] { "Delete Request" });
            CreateOrUpdateTableRow(SystemActionTypeName.ReissueCredential, new [] { "Reissue Credential" });
           
            CreateOrUpdateTableRow(SystemActionTypeName.TestInvoicePaid, new [] { "Test Invoice Paid" });
            CreateOrUpdateTableRow(SystemActionTypeName.WithdrawRequest, new [] { "Withdraw Request" });
            CreateOrUpdateTableRow(SystemActionTypeName.AllocateTestSession, new [] { "Allocate Test Session" });
            //CreateOrUpdateTableRow(new[] { "1018, new [] { "AcceptTestSession, new [] { "Accept Test Session" });
            //CreateOrUpdateTableRow(new[] { "1019, new [] { "AcceptTestSessionFromMyNaati, new [] { "Accept Test Session (myNAATI)" });
            CreateOrUpdateTableRow(SystemActionTypeName.RejectTestSessionFromMyNaati, new [] { "Reject Test Session (myNAATI)" });
            CreateOrUpdateTableRow(SystemActionTypeName.RejectTestSession, new [] { "Reject Test Session" });
            CreateOrUpdateTableRow(SystemActionTypeName.CheckIn, new [] { "Check In" });
            CreateOrUpdateTableRow(SystemActionTypeName.MarkAsSat, new [] { "Mark As Sat" });
            CreateOrUpdateTableRow(SystemActionTypeName.UndoCheckIn, new [] { "Undo Check In" });
            CreateOrUpdateTableRow(SystemActionTypeName.UndoMarkAsSat, new [] { "Undo Mark As Sat" });
            CreateOrUpdateTableRow(SystemActionTypeName.IssueFail, new [] { "Issue Fail" });
            CreateOrUpdateTableRow(SystemActionTypeName.IssuePass, new [] { "Issue Pass" });
            CreateOrUpdateTableRow(SystemActionTypeName.CreatePaidTestReview, new [] { "Create Paid Test Review" });
            CreateOrUpdateTableRow(SystemActionTypeName.IssueFailAfterReview, new [] { "Issue Fail (post-review)" });
            CreateOrUpdateTableRow(SystemActionTypeName.IssuePassAfterReview, new [] { "Issue Pass (post-review)" });
           
            CreateOrUpdateTableRow(SystemActionTypeName.CreateSupplementaryTest, new [] { "Create Supplementary Test" });
            CreateOrUpdateTableRow(SystemActionTypeName.WithdrawSupplementaryTest, new [] { "Withdraw Supplementary" });
            CreateOrUpdateTableRow(SystemActionTypeName.SupplementaryTestInvoicePaid, new [] { "Supplementary Test Invoice Paid" });
            CreateOrUpdateTableRow(SystemActionTypeName.AllocateTestSessionFromMyNaati, new [] { "Allocate Test Session (myNAATI)" });
            CreateOrUpdateTableRow(SystemActionTypeName.CancelTestInvitation, new [] { "Cancel Test Invitation" });
            CreateOrUpdateTableRow(SystemActionTypeName.AssignTestMaterial, new [] { "Assign Test Material" });
            CreateOrUpdateTableRow(SystemActionTypeName.Reassess, new [] { "Reassess" });
            CreateOrUpdateTableRow(SystemActionTypeName.PaidReviewInvoiceProcessed, new [] { "Paid Review Invoice Processed" });
            CreateOrUpdateTableRow(SystemActionTypeName.PaidReviewInvoicePaid, new [] { "Paid Review Invoice Paid" });
            CreateOrUpdateTableRow(SystemActionTypeName.WithdrawPaidReview, new [] { "Withdraw Paid Review" });
            CreateOrUpdateTableRow(SystemActionTypeName.NotifyTestSessionDetails, new [] { "Notify Test Session Details" });
            CreateOrUpdateTableRow(SystemActionTypeName.SupplementaryTestInvoiceProcessed, new [] { "Supplementary Test Invoice Processed" });
            CreateOrUpdateTableRow(SystemActionTypeName.SendCandidateBrief, new [] { "Send Candidate Brief" });
            CreateOrUpdateTableRow(SystemActionTypeName.RevertResults, new [] { "Revert Results" });
            CreateOrUpdateTableRow(SystemActionTypeName.ComputeFinalMarks, new [] { "Compute Final Marks" });
            CreateOrUpdateTableRow(SystemActionTypeName.InvalidateTest, new [] { "Invalidate Test" });
            CreateOrUpdateTableRow(SystemActionTypeName.MarkForAssessment, new [] { "Mark For Assessment" });
            CreateOrUpdateTableRow(SystemActionTypeName.TestInvoiceProcessed, new [] { "Test Invoice Processed" });
            CreateOrUpdateTableRow(SystemActionTypeName.SendTestSessionReminder, new [] { "Send Test Session Reminder" });
            CreateOrUpdateTableRow(SystemActionTypeName.SendSessionAvailabilityNotice, new [] { "Send Session Availability Notice" });
            CreateOrUpdateTableRow(SystemActionTypeName.RequestRefund, new [] { "Request For Refund" });
            CreateOrUpdateTableRow(SystemActionTypeName.CreditNoteProcessed, new [] { "Credit Note Processed" });
            CreateOrUpdateTableRow(SystemActionTypeName.CreditNotePaid, new [] { "Credit Note Paid" });
            CreateOrUpdateTableRow(SystemActionTypeName.ProcessRefund, new [] { "Process Refund" });
            CreateOrUpdateTableRow(SystemActionTypeName.ApproveRefund, new [] { "Approve Refund" });
            CreateOrUpdateTableRow(SystemActionTypeName.RejectRefund, new [] { "Reject Refund" });
            CreateOrUpdateTableRow(SystemActionTypeName.SendTestSessionSittingMaterialReminder, new[] { "Send TestSessionSitting Material Reminder" });
            CreateOrUpdateTableRow(SystemActionTypeName.CertificationOnHold, new[] { "On Hold" });

            CreateOrUpdateTableRow(SystemActionTypeName.AllocateRolePlayer, new [] { "Allocate Role-player" });
            CreateOrUpdateTableRow(SystemActionTypeName.RolePlayerMarkAsRejected, new [] { "Role-player Mark as Rejected" });
            CreateOrUpdateTableRow(SystemActionTypeName.RolePlayerMarkAsAccepted, new [] { "Role-player Mark as Accepted" });
            CreateOrUpdateTableRow(SystemActionTypeName.NotifyTestSessionRehearsalDetails, new [] { "Notify Test Session Rehearsal Details" });
            CreateOrUpdateTableRow(SystemActionTypeName.RolePlayerAcceptTestSessionFromMyNaati, new [] { "Role-player Accept Test Session (myNAATI)" });
            CreateOrUpdateTableRow(SystemActionTypeName.RolePlayerRejectTestSessionFromMyNaati, new [] { "Role-player Reject Test Session (myNAATI)" });
            CreateOrUpdateTableRow(SystemActionTypeName.RolePlayerRemoveFromTestSession, new [] { "Role-player Remove from Test Session" });
            CreateOrUpdateTableRow(SystemActionTypeName.RolePlayerNotifyAllocationUpdate, new [] { "Role-player Notify Allocation Update" });
            CreateOrUpdateTableRow(SystemActionTypeName.RolePlayerMarkAsAttendedRehearsal, new [] { "Role-player Mark as Attended Rehearsal" });
            CreateOrUpdateTableRow(SystemActionTypeName.RolePlayerMarkAsAttendedTest, new [] { "Role-player Mark as Attended Test" });
            CreateOrUpdateTableRow(SystemActionTypeName.RolePlayerMarkAsNoShow, new [] { "Role-player Mark as No Show" });
            CreateOrUpdateTableRow(SystemActionTypeName.RolePlayerMarkAsPending, new [] { "Role-player Mark as Pending" });
            CreateOrUpdateTableRow(SystemActionTypeName.RolePlayerMarkAsRemoved, new [] { "Role-player Mark as Removed" });

            CreateOrUpdateTableRow(SystemActionTypeName.CreateMaterialRequest, new [] { "Create Material Request" });
            CreateOrUpdateTableRow(SystemActionTypeName.CloneMaterialRequest, new [] { "Clone Material Request" });
            CreateOrUpdateTableRow(SystemActionTypeName.CreateMaterialRequestRound, new [] { "Create Material Request Round" });
            CreateOrUpdateTableRow(SystemActionTypeName.UploadFinalMaterialDocuments, new [] { "Upload Final Material Documents" });
            CreateOrUpdateTableRow(SystemActionTypeName.RevertMaterialRequest, new [] { "Revert Material Request" });
            CreateOrUpdateTableRow(SystemActionTypeName.UpdateMaterialRequest, new [] { "Update Material Request" });
            CreateOrUpdateTableRow(SystemActionTypeName.CancelMaterialRequest, new [] { "Cancel Material Request" });
            CreateOrUpdateTableRow(SystemActionTypeName.ApproveMaterialRequestPayment, new [] { "Approve Material Request Payment" });
            CreateOrUpdateTableRow(SystemActionTypeName.MarkMaterialRequestMemberAsPaid, new [] { "Mark Material Request Member as Paid" });
            CreateOrUpdateTableRow(SystemActionTypeName.MarkMaterialRequestMemberAsPaid, new [] { "Mark Material Request Member as Paid" });
            CreateOrUpdateTableRow(SystemActionTypeName.UnApproveMaterialRequestPayment, new [] { "UnApprove MaterialRequest Payment" });
            CreateOrUpdateTableRow(SystemActionTypeName.UpdateMaterialRequestMembers, new [] { "Update Material Request Members" });
       
            CreateOrUpdateTableRow(SystemActionTypeName.SubmitRoundForApproval, new [] { "Submit Round For Approval" });
            CreateOrUpdateTableRow(SystemActionTypeName.ApproveMaterialRequestRound, new [] { "Approve Material Request Round" });
            CreateOrUpdateTableRow(SystemActionTypeName.RejectMaterialRequestRound, new [] { "Reject Material Request Round" });
            CreateOrUpdateTableRow(SystemActionTypeName.RevertMaterialRequestRound, new [] { "Revert Material Request Round" });
            CreateOrUpdateTableRow(SystemActionTypeName.UpdateMaterialRequestRound, new [] { "Update Round Details" });
            CreateOrUpdateTableRow(SystemActionTypeName.MakeApplicationPayment, new[] { "Make Application Payment" });
            CreateOrUpdateTableRow(SystemActionTypeName.CreatePreRequisiteApplications, new[] { "Create PreRequisite Applications" });
            CreateOrUpdateTableRow(SystemActionTypeName.IssuePracticeTestResults, new[] { "Issue Practice Test Results" });
            CreateOrUpdateTableRow(SystemActionTypeName.ProgressCredentialToEligibleForTesting, new[] { "Progress Credential To Eligible For Testing" });

            CreateOrUpdateTableRow(SystemActionTypeName.SendEmail, new[] { "Send Email" });
        }

        public SystemActionTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override void RunDescendantOrderScripts()
        {
            DeleteTableRow("1031");
            DeleteTableRow("1032");
            DeleteTableRow("1014");
            DeleteTableRow("1018");
            DeleteTableRow("1019");
            DeleteTableRow("13");
            DeleteTableRow("14");
        }
    }
}
