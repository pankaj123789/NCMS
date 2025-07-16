using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class EmailTemplateScriptGenerator : BaseScriptGenerator
    {
        public EmailTemplateScriptGenerator(NaatiScriptRunner runner) 
            : base(runner) { }

        public override string TableName => "tblEmailTemplate";

        public override IList<string> Columns => new[]
        {
            "EmailTemplateId",
            "Name",
            "Subject",
            "Content",
            "FromAddress",
            "Active",
        };

        public override void RunScripts()
        {
            const string noReplyEmail = "noreply@naati.com.au";
            const string transitionEmail = "transition@naati.com.au";
            CreateOrUpdateEmailTemplate(41, "CCL Application Rejected", "Application Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclApplicationRejectedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(42, "CCL Application Submitted", "Your application has been submitted to NAATI – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclApplicationSubmittedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(43, "CCL Credential Request Cancelled", "Cancellation Request – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclCredentialRequestCancelledTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(44, "Certification Application Rejected", "Application Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationApplicationRejectedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(45, "Certification Application Submitted", "Your application has been submitted to NAATI  – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationApplicationSubmittedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(46, "Certification Assessment Failed", "Assessment Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationAssessmentFailedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(47, "Certification Assessment Passed", "Eligibility for Testing – Application reference: [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationAssessmentPassedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(48, "Certification Assessment Pending", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationAssessmentPendingTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(49, "Certification Credential Request Cancelled", "Cancellation Request – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationCredentialRequestCancelledTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(50, "Ethics Application Submitted", "Eligibility for Testing – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsApplicationSubmittedTemplate, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(51, "Prerequisite Tests Application Submitted", "Eligibility for Testing – Application [[Application Reference]]", EmailTemplates.EmailTemplates.InterculturalApplicationSubmittedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(52, "Transition/Accreditation Application Rejected", "Application Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.TransitionAccreditationApplicationRejectedTemplate, transitionEmail, 1);
            CreateOrUpdateEmailTemplate(53, "Transition/Accreditation Application Submitted", "Application Submitted – Application [[Application Reference]]", EmailTemplates.EmailTemplates.TransitionAccreditationApplicationSubmittedTemplate, transitionEmail, 1);
            CreateOrUpdateEmailTemplate(54, "Transition/Accreditation Assessment Failed", "Assessment Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.TransitionAccreditationAssessmentFailedTemplate, transitionEmail, 1);
            CreateOrUpdateEmailTemplate(55, "Transition/Accreditation Assessment Pending", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.TransitionAccreditationAssessmentPendingTemplate, transitionEmail, 1);
            CreateOrUpdateEmailTemplate(56, "Transition Credential Issued", "Congratulations! You have been awarded a NAATI Credential", EmailTemplates.EmailTemplates.TransitionCredentialIssuedTemplate, transitionEmail, 1);
            CreateOrUpdateEmailTemplate(57, "Transition Credential Reissued", "Congratulations! You have been awarded a NAATI Credential", EmailTemplates.EmailTemplates.TransitionCredentialIssuedTemplate, transitionEmail, 1);
            CreateOrUpdateEmailTemplate(58, "Transition/Accreditation Credential Request Cancelled", "Cancellation Request – Application [[Application Reference]]", EmailTemplates.EmailTemplates.TransitionAccreditationCredentialRequestCancelledTemplate, transitionEmail, 1);
            CreateOrUpdateEmailTemplate(59, "Transition/Accreditation Pending Assessment Failed", "Assessment Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.TransitionAccreditationPendingAssessmentFailedTemplate, transitionEmail, 1);
            CreateOrUpdateEmailTemplate(60, "Certification Finish Checking Applicant Invoice", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationFinishCheckingInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(61, "Certification Finish Checking Sponsor Invoice", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationFinishCheckingInvoiceSponsorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(62, "Certification Finish Checking Applicant", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationFinishCheckingNoInvoiceApplicantTemplate, noReplyEmail,1);
            CreateOrUpdateEmailTemplate(63, "CCL V1 Invite for Testing Applicant Invoice", "You are invited to sit a CCL Test – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclInviteForTestingInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(64, "CCL V1 Invite for Testing Sponsor Invoice", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclInviteForTestingInvoiceSponsorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(65, "CCL V1 Invite for Testing Applicant", "You are invited to sit a CCL Test – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclInviteForTestingNoInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(66, "CCL Test Invoice Paid", "Test fee paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclTestInvoicePaidTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(67, "CCL Credential Request Withdrawn", "Application Withdrawn – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclWithdrawRequestTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(68, "Certification Assessment Invoice Paid", "Assessment fee paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationAssessmentInvoicePaidTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(69, "Certification Application Rejected After Invoice", "Application Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationApplicationRejectedAfterInvoiceTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(70, "Certification Credential Issued", "Congratulations! You have passed a NAATI Certification test", EmailTemplates.EmailTemplates.CertificationCredentialIssuedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(71, "Ethics Invite for Testing Applicant Invoice", "You are invited to sit an Ethical Competency Test – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInviteForTestingInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(72, "Ethics Invite for Testing Sponsor Invoice", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInviteForTestingInvoiceSponsorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(73, "Ethics Invite for Testing Applicant", "You are invited to sit an Ethical Competency Test – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInviteForTestingNoInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(74, "Ethics Test Invoice Paid", "Test fee paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsTestInvoicePaidTemplate, noReplyEmail, 0);   
            CreateOrUpdateEmailTemplate(75, "Ethics Credential Request Withdrawn", "Application Withdrawn – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalWithdrawRequestTemplate, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(76, "Intercultural Invite for Testing Applicant Invoice", "You are invited to sit an Intercultural Competency Test – Application [[Application Reference]]", EmailTemplates.EmailTemplates.InterculturalInviteForTestingInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(77, "Intercultural Invite for Testing Sponsor Invoice", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.InterculturalInviteForTestingInvoiceSponsorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(78, "Intercultural Invite for Testing Applicant", "You are invited to sit an Intercultural Competency Test – Application [[Application Reference]]", EmailTemplates.EmailTemplates.InterculturalInviteForTestingNoInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(79, "Prerequisite Tests Invoice Paid", "Test fee paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.InterculturalTestInvoicePaidTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(80, "Prerequisite Tests Credential Request Withdrawn", "Application Withdrawn – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalWithdrawRequestTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(81, "Certification Invite for Testing Applicant Invoice", "You are invited to sit a Certification Test – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationInviteForTestingInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(82, "Certification Invite for Testing Sponsor Invoice", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationInviteForTestingInvoiceSponsorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(83, "Certification Invite for Testing Applicant", "You are invited to sit a Certification Test – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationInviteForTestingNoInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(84, "Certification Test Invoice Paid", "Test fee paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationTestInvoicePaidTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(85, "Certification Credential Request Withdrawn", "Application Withdrawn – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationWithdrawRequestTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(86, "Certification Credential Reissued", "Congratulations! You have passed a NAATI Certification test", EmailTemplates.EmailTemplates.CertificationCredentialIssuedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(87, "Certification Allocate Test Session", "NAATI Certification Test scheduled – Please confirm your place – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationTestSessionAllocatedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(88, "Certification Accept Test Session NCMS", "Your test place has been confirmed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationTestSessionAcceptedNcmsTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(89, "Certification Accept Test Session myNAATI", "Your test place has been confirmed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationTestSessionAcceptedMyNaatiTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(90, "Certification Reject Test Session NCMS", "Your test place has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationTestSessionRejectedNcmsTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(91, "Certification Reject Test Session myNAATI", "Your test place has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationTestSessionRejectedMyNaatiTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(92, "CCL Allocate Test Session", "NAATI CCL Test scheduled – Please confirm your place – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclTestSessionAllocatedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(93, "CCL Accept Test Session NCMS", "Your test place has been confirmed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclTestSessionAcceptedNcmsTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(94, "CCL Accept Test Session myNAATI", "Your test place has been confirmed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclTestSessionAcceptedMyNaatiTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(95, "CCL Reject Test Session NCMS", "Your test place has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclTestSessionRejectedNcmsTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(96, "CCL Reject Test Session myNAATI", "Your test place has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclTestSessionRejectedMyNaatiTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(97, "Ethics/Intercultural Allocate Test Session", "NAATI [[Application Type]] scheduled – Please confirm your place – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalTestSessionAllocatedTemplate, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(98, "Ethics Accept Test Session NCMS", "Your test place has been confirmed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalTestSessionAcceptedNcmsTemplate, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(99, "Ethics Accept Test Session myNAATI", "Your test place has been confirmed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalTestSessionAcceptedMyNaatiTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(100, "Ethics Reject Test Session NCMS", "Your test place has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalTestSessionRejectedNcmsTemplate, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(101, "Ethics Reject Test Session myNAATI", "Your test place has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalTestSessionRejectedMyNaatiTemplate, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(102, "Prerequisite Tests Allocate Test Session", "NAATI [[Application Type]] scheduled – Please confirm your place – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalTestSessionAllocatedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(103, "Intercultural Accept Test Session NCMS", "Your test place has been confirmed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalTestSessionAcceptedNcmsTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(104, "Prerequisite Tests Accept Test Session via myNAATI", "Your test place has been confirmed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalTestSessionAcceptedMyNaatiTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(105, "Prerequisite Tests Reject Test Session via NCMS", "Your test place has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalTestSessionRejectedNcmsTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(106, "Prerequisite Tests Reject Test Session via myNAATI", "Your test place has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalTestSessionRejectedMyNaatiTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(107, "CCL Issue Fail", "Your NAATI CCL Test Results – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclTestFailedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(108, "CCL Issue Pass", "Your NAATI CCL Test Results – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclIssuedPassResultTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(109, "CCL Issue Credential", "Congratulations! You have passed a NAATI CCL test.", EmailTemplates.EmailTemplates.CclIssueCredentialTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(110, "CCL Issue Credential", "Congratulations! You have passed a NAATI CCL test.", EmailTemplates.EmailTemplates.CclReIssueCredentialTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(111, "CCL Paid Review", "Review Created – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclPaidRevieInvoiceProcessedTrustedSponsoredAppilcantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(112, "CCL Issue Fail After Review", "Your NAATI CCL Test Review Results – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclFailPaidReviewTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(113, "CCL Issue Pass After Review", "Your NAATI CCL Test Review Results – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclPassPaidReviewTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(115, "CCL V2 Application submitted", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclV2ApplicationSubmittedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(116, "CCL V2 Application submitted with CC", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclV2ApplicationSubmittedWithCreditCardTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(117, "CCL V2 Application Invoice Paid", "Your application has been submitted to NAATI – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclV2ApplicationInvoicePaidTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(118, "CCL V2 Application Checking Finished", "Please select a test session – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclV2ApplicationCheckingFinishedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(119, "CCL Test Session Selected", "Your test place has been confirmed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclV2TestSessionAllocatedAndAcceptedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(120, "CCL Test Session Rejected", "Your test place has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclV2TestSessionRejectedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(121, "Certification Supplementary Test Applicant Invoice", "NAATI Supplementary Certification Test – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationCreateSupplementaryTestInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(122, "Certification Supplementary Test Sponsor Invoice", "Supplementary Certification Test: Sponsor Invoice [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationCreateSupplementaryTestInvoiceSponsorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(123, "Certification Supplementary Test Applicant", "NAATI Supplementary Certification Test – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationCreateSupplementaryTestNoInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(124, "Certification Widthdraw Supplementary Test", "Withdraw Supplementary Certification Test [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationWithdrawSupplementaryTestTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(125, "Certification Supplementary Test Invoice Paid", "Supplementary Certification Test Invoice Paid [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationSupplementaryTestInvoicePaidTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(126, "CCL V2 Application Submission Processed", "Your application has been submitted to NAATI – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclV2ApplicationSubmissionProcessedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(127, "CCL V2 Paid Application Submission Processed", "Your application has been submitted to NAATI – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclV2PaidApplicationSubmissionProcessedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(128, "Certification Cancel Test Invitation", "Your invitation to sit a test has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationCancelTestInvitationTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(129, "Ethical Competency Cancel Test Invitation", "Your invitation to sit a test has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicalCompetencyCancelTestInvitationTemplate, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(130, "Prerequisite Tests Cancel Test Invitation", "Your invitation to sit a test has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.InterculturalCompetencyCancelTestInvitationTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(131, "Transition/Accreditation Finish Checking Applicant Invoice", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.TransitionAccreditationFinishCheckingInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(132, "Transition/Accreditation Assessment Invoice Paid", "Application fee paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.TransitionAccreditationAssessmentInvoicePaidTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(133, "Certification Issue Fail", "Your NAATI Certification Test result", EmailTemplates.EmailTemplates.CertificationIssueFailTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(134, "Certification Issue Pass", "Your NAATI Certification Test result", EmailTemplates.EmailTemplates.CertificationIssuePassTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(136, "Ethics Issue Fail", "Your Ethical Competency Screening Test result", EmailTemplates.EmailTemplates.EthicsIssueFailTemplate, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(137, "Ethics Issue Pass", "Your Ethical Competency Screening Test result", EmailTemplates.EmailTemplates.EthicsIssuePassTemplate, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(138, "Ethics Issue Credential", "Congratulations! You have passed a NAATI prerequisite test", EmailTemplates.EmailTemplates.EthicsInterculturalIssueCredentialTemplate, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(139, "Prerequisite Tests Issue Fail", "Your Prerequisite Test result", EmailTemplates.EmailTemplates.InterculturalIssueFailTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(140, "Prerequisite Tests Issue Pass", "Your Prerequisite Test result", EmailTemplates.EmailTemplates.InterculturalIssuePassTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(141, "Prerequisite Tests Issue Credential", "Congratulations! You have passed a NAATI prerequisite test", EmailTemplates.EmailTemplates.EthicsInterculturalIssueCredentialTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(142, "Certification/Prerequisite Tests Issue Fail After Review", "NAATI Certification/Prerequisite Test Review result", EmailTemplates.EmailTemplates.CertificationIssueFailAfterReviewTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(144, "Certification/Prerequisite Tests Issue Pass After Review", "NAATI Certification/Prerequisite Test Review result", EmailTemplates.EmailTemplates.CertificationIssuePassAfterReviewTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(145, "Transition/Accreditation Finish Checking Sponsor Invoice", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.TransitionAccreditationFinishCheckingInvoiceSponsorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(146, "Transition/Accreditation Finish Checking Applicant", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.TransitionAccreditationFinishCheckingNoInvoiceApplicantTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(147, "Recertification Application Submitted", "Application Submitted – Application [[Application Reference]]", EmailTemplates.EmailTemplates.RecertificationApplicationSubmittedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(148, "Recertification Application Rejected", "Application Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.RecertificationApplicationRejectedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(149, "Recertification Assessment Invoice Paid", "Application fee paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.RecertificationAssessmentInvoicePaidTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(150, "Recertification Assessment Pending", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.RecertificationAssessmentPendingTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(151, "Recertification Assessment Failed", "Your NAATI Recertification Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.RecertificationAssessmentFailedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(152, "Recertification Pending Assessment Failed", "Your NAATI Recertification Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.RecertificationPendingAssessmentFailedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(153, "Recertification Credential Issued", "Congratulations! You’ve recertified your NAATI credential", EmailTemplates.EmailTemplates.RecertificationCredentialIssuedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(154, "Recertification Credential Request Cancelled", "Cancellation Request – Application [[Application Reference]]", EmailTemplates.EmailTemplates.RecertificationCredentialRequestCancelledTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(155, "Recertification Credential Reissued", "Congratulations! You’ve recertified your NAATI credential", EmailTemplates.EmailTemplates.RecertificationCredentialIssuedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(156, "Recertification Finish Checking Applicant Invoice", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.RecertificationFinishCheckingInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(157, "Recertification Finish Checking Sponsor Invoice", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.RecertificationFinishCheckingInvoiceSponsorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(158, "Recertification Finish Checking Applicant", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.RecertificationFinishCheckingNoInvoiceApplicantTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(159, "CLA Application Submitted", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaApplicationSubmittedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(160, "CLA Application Rejected", "Application Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaApplicationRejectedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(161, "CLA Finish Checking Applicant Invoice", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaFinishCheckingInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(162, "CLA Finish Checking Sponsor Invoice", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaFinishCheckingInvoiceSponsorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(163, "CLA Finish Checking Applicant", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaFinishCheckingNoInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(164, "CLA Application Rejected", "Application Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaApplicationRejectedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(165, "CLA Application Invoice Paid", "Your application has been submitted to NAATI – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaApplicationInvoicePaidTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(166, "CLA Issue Credential", "Congratulations! You have passed a NAATI [[Credential Request Type]] Test.", EmailTemplates.EmailTemplates.ClaIssueCredentialTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(167, "CLA Credential Request Cancelled", "Cancellation Request – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaCredentialRequestCancelledTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(168, "CLA Issue Credential", "Congratulations! You have passed a NAATI [[Credential Request Type]] Test.", EmailTemplates.EmailTemplates.ClaReIssueCredentialTemplate, noReplyEmail, 1);
            
            CreateOrUpdateEmailTemplate(172, "CLA Credential Request Withdrawn", "Application Withdrawn – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaWithdrawRequestTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(173, "CLA Allocate Test Session", "NAATI CLA Test scheduled – Please confirm your place – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaTestSessionAllocatedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(174, "CLA Accept Test Session NCMS", "Your test place has been confirmed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaTestSessionAcceptedNcmsTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(175, "CLA Accept Test Session myNAATI", "Your test place has been confirmed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaTestSessionAcceptedMyNaatiTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(176, "CLA Test Session Rejected", "Your test place has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaTestSessionRejectedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(177, "CLA Test Session Rejected", "Your test place has been cancelled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaTestSessionRejectedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(178, "CLA Issue Fail", "Your NAATI [[Credential Request Type]] Test Results – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaTestFailedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(179, "CLA Issue Pass", "Your NAATI [[Credential Request Type]] Test Results – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaIssuedPassResultTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(183, "CLA Test Session Selected", "Your test place has been confirmed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.ClaTestSessionAllocatedAndAcceptedTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(184, "Certification/Prerequisite Tests Paid Review Being Processed", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationCreatePaidTestReviewInvoiceBeingProcessed, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(185, "Certification/Prerequisite Tests Paid Review Being Processed Sponsored", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationCreatePaidTestReviewSponsoredBeingProcessed, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(186, "Certification/Prerequisite Tests Paid Review Being Processed Credit Card", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationCreatePaidTestReviewCreditCardPaymentBeingProcessed, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(187, "Certification/Prerequisite Tests Paid Review Fee Applicant Invoice", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationPaidReviewInvoiceProcessedInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(188, "Certification/Prerequisite Tests Paid Review Fee Sponsor Invoice", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationPaidReviewInvoiceProcessedInvoiceSponsorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(189, "Certification/Prerequisite Tests Paid Review Fee Sponsored Applicant", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationPaidReviewInvoiceProcessedNoInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(190, "Certification/Prerequisite Tests Paid Review Card Payment Received", "Paid Test Review Paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationPaidReviewInvoiceProcessedCreditCardReceivedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(191, "Certification/Prerequisite Tests Paid Review Invoice Paid", "Paid Test Review Paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationPaidTestReviewPaidTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(192, "Certification/Prerequisite Tests Paid Review Withdrawn", "Paid Test Review Withdrawn – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationPaidReviewWithdrawnTemplate, noReplyEmail, 1);
                                                                 
            CreateOrUpdateEmailTemplate(193, "CCL Paid Review Being Processed", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclCreatePaidTestReviewInvoiceBeingProcessed, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(194, "CCL Paid Review Being Processed Sponsored", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclCreatePaidTestReviewSponsoredBeingProcessed, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(195, "CCL Paid Review Being Processed Credit Card", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclCreatePaidTestReviewCreditCardPaymentBeingProcessed, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(196, "CCL Paid Review Fee Applicant Invoice", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclPaidReviewInvoiceProcessedInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(197, "CCL Paid Review Fee Sponsor Invoice", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclPaidReviewInvoiceProcessedInvoiceSponsorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(198, "CCL Paid Review Fee Sponsored Applicant", "NAATI Application Update – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclPaidReviewInvoiceProcessedNoInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(199, "CCL Paid Review Card Payment Received", "Paid Test Review Paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclPaidReviewInvoiceProcessedCreditCardReceivedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(200, "CCL Paid Review Invoice Paid", "Paid Test Review Paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclPaidTestReviewPaidTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(201, "CCL Paid Review Withdrawn", "Paid Test Review Withdrawn – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclPaidReviewWithdrawnTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(202, "Certification/Prerequisite Tests Paid Review Being Processed Trusted Sponsor", "Review Created – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationPaidRevieInvoiceProcessedTrustedSponsoredApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(203, "Certification Issue Fail Supplementary Offered", "Supplementary Test Option – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationIssueFailAfterReviewEligibleForSupplementaryTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(204, "Certification Issue Fail Conceded Pass Offered", "Conceded Credential granted – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationIssueFailConcededPassOfferedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(205, "Certification Pending Assessment Failed", "Assessment Outcome – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationPendingAssessmentFailedTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(206, "Indigenous Certification Issue Fail", "Your NAATI Certification Test result", EmailTemplates.EmailTemplates.IndigenousCertificationIssueFailTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(207, "Indigenous Certification Issue Pass", "Your NAATI Certification Test result", EmailTemplates.EmailTemplates.IndigenousCertificationIssuePassTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(208, "Indigenous Certification Issue Fail After Review", "NAATI Certification Test Review result", EmailTemplates.EmailTemplates.IndigenousCertificationIssueFailAfterReviewTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(209, "Indigenous Certification Issue Pass After Review", "NAATI Certification Test Review result", EmailTemplates.EmailTemplates.IndigenousCertificationIssuePassAfterReviewTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(210, "Indigenous Certification Issue Fail Supplementary Offered", "Supplementary Test Option – Application [[Application Reference]]", EmailTemplates.EmailTemplates.IndigenousCertificationIssueFailEligibleForSupplementaryTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(211, "Indigenous Certification Issue Fail Conceded Pass Offered", "Conceded Credential granted – Application [[Application Reference]]", EmailTemplates.EmailTemplates.IndigenousCertificationIssueFailConcededPassOfferedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(212, "Indigenous Certification Credential Issued", "Congratulations! You have passed a NAATI Certification test", EmailTemplates.EmailTemplates.IndigenousCertificationCredentialIssuedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(213, "Indigenous Certification Credential Reissued", "Congratulations! You have passed a NAATI Certification test", EmailTemplates.EmailTemplates.IndigenousCertificationCredentialIssuedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(214, "Indigenous Ethics Issue Fail", "Your Ethical Competency Screening Test result", EmailTemplates.EmailTemplates.IndigenousEthicsIssueFailTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(215, "Indigenous Ethics Issue Pass", "Your Ethical Competency Screening Test result", EmailTemplates.EmailTemplates.IndigenousEthicsIssuePassTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(216, "Indigenous Ethics Issue Credential", "Congratulations! You have passed a NAATI prerequisite test", EmailTemplates.EmailTemplates.IndigenousEthicsInterculturalIssueCredentialTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(217, "Indigenous Intercultural Issue Fail", "Your Intercultural Competency Screening Test result", EmailTemplates.EmailTemplates.IndigenousInterculturalIssueFailTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(218, "Indigenous Intercultural Issue Pass", "Your Intercultural Competency Screening Test result", EmailTemplates.EmailTemplates.IndigenousInterculturalIssuePassTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(219, "Indigenous Intercultural Issue Credential", "Congratulations! You have passed a NAATI prerequisite test", EmailTemplates.EmailTemplates.IndigenousEthicsInterculturalIssueCredentialTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(220, "Indigenous  Certification Issue Fail After Review Supplementary Offered", "Supplementary Test Option – Application [[Application Reference]]", EmailTemplates.EmailTemplates.IndigenousCertificationIssueFailAfterReviewEligibleForSupplementaryTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(221, "Indigenous  Certification Issue Fail After Review Conceded Pass Offered", "Conceded Credential granted – Application [[Application Reference]]", EmailTemplates.EmailTemplates.IndigenousCertificationIssueFailAfterReviewConcededPassOfferedTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(222, "Certification Issue Fail After Review Supplementary Offered", "Supplementary Test Option – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationIssueFailAfterReviewEligibleForSupplementaryTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(223, "Certification Issue Fail After Review Conceded Pass Offered", "Conceded Credential granted – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationIssueFailAfterReviewConcededPassOfferedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(224, "Notify Test Session Details", "Important – your test details have changed", EmailTemplates.EmailTemplates.CertificationNotifyTestSessionDetailsTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(225, "Notify Test Session Details", "Important – your test details have changed", EmailTemplates.EmailTemplates.CclNotifyTestSessionDetailsTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(226, "Notify Test Session Details", "Important – your test details have changed", EmailTemplates.EmailTemplates.ClaNotifyTestSessionDetailsTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(227, "Notify Test Session Details", "Important – your test details have changed", EmailTemplates.EmailTemplates.EthicsNotifyTestSessionDetailsTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(228, "Certification Supplementary Test Being Processed", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationCreateSupplemementaryTestInvoiceBeingProcessed, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(229, "Certification Supplementary Test Being Processed Sponsored", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationCreateSupplementaryTestSponsoredBeingProcessed, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(230, "Certification Supplementary Test Being Processed Credit Card", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationCreateSupplementaryTestCreditCardPaymentBeingProcessed, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(231, "Certification Supplementary Test Being Processed Trusted Sponsor", "Your NAATI application is being processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationSupplementaryTestInvoiceProcessedTrustedSponsoredApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(232, "Certification Supplementary Card Payment Received", "Supplementary Certification Test: Card Payment Received [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationSupplementaryTestPaidTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(233, "Role-player Allocate Test Session", "You are selected as a Role-Player for [[Test Session ID]]", EmailTemplates.EmailTemplates.RolePlayerTestSessionAllocatedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(234, "Role-player Rehearsal details Changed", "Change of Test Session Details for [[Test Session ID]]", EmailTemplates.EmailTemplates.RolePlayerTestSessionChangedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(235, "Role-player Removed", "You have been removed from [[Test Session ID]] as a Role-Player", EmailTemplates.EmailTemplates.RolePlayerRemovedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(236, "Role-player Allocation Details Changed", "Change of Role-player Details for [[Test Session Id]]", EmailTemplates.EmailTemplates.RolePlayerTestSessionAllocationChangedTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(237, "Certification Send Candidate Brief", "Brief for your Test Session", EmailTemplates.EmailTemplates.CertificationSendCandidateBrief, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(238, "Failed Test Reverted", "Your Test Result has been overturned - Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclFailedTestResultReverted, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(239, "Passed Test Reverted", "Your Test Result has been overturned - Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclPassedTestResultReverted, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(240, "Issued Credential Reverted", "Your Credential has been overturned - Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclIssuedCredentialReverted, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(241, "Test Invalidated", "Test outcome - Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclTestInvalidatedEmailTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(242, "Invalidated Test Reverted", "Your Test Result has been overturned- Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclInvalidatedTestRevertedEmailTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(243, "CCL Allocate Test Session - Credit Card", "NAATI CCL Test scheduled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclAllocateTestSessionCreditCardPayment, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(244, "CCL Allocate Test Session - Direct Debit", "NAATI CCL Test scheduled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclAllocateTestSessionDirectDebit, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(245, "CCL Test Invoice Processed - Applicant Invoice - Credit Card", "Your NAATI application test invoice has been processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclTestInvoiceProcessedCreditCardPayment, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(246, "CCL Test Invoice Processed - Applicant Invoice - Direct Debit", "Your NAATI application test invoice has been processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclTestInvoiceProcessed, noReplyEmail, 0);

            CreateOrUpdateEmailTemplate(247, "Certification Allocate Test Session - Credit Card", "NAATI Certification Test scheduled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationAllocateTestSessionCreditCard, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(248, "Certification Allocate Test Session - Direct Debit", "NAATI Certification Test scheduled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationAllocateTestSessionDirectDebit, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(249, "Certification Allocate Test Session - Applicant with Sponsor ", "NAATI Certification Test scheduled - Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationAllocateTestSessionUntrustedSponsorApplicantTemplate, noReplyEmail, 1);


            CreateOrUpdateEmailTemplate(251, "Prerequisite Tests Allocate Test Session - Credit Card", "NAATI [[Application Type]] Test scheduled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterCulturalAllocateTestSessionDirectCreditCard, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(252, "Prerequisite Tests Allocate Test Session - Direct Debit", "NAATI [[Application Type]] Test scheduled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterCulturalAllocateTestSessionDirectDebit, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(253, "Prerequisite Tests Allocate Test Session - Applicant with Sponsor", "NAATI [[Application Type]] Test scheduled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalAllocateTestSessionUntrustedSponsorApplicantTemplate, noReplyEmail, 1);


            CreateOrUpdateEmailTemplate(254, "Certification Test Invoice Processed - Applicant Invoice - Credit Card", "Your NAATI application test invoice has been processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationTestInvoiceProcessedCreditCard, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(255, "Certification Test Invoice Processed - Applicant Invoice - Direct Debit", "Your NAATI application test invoice has been processed", EmailTemplates.EmailTemplates.CertificationTestInvoiceProcessedDirectDebit, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(256, "Certification Sponsored Test invoice Processed", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationTestInvoiceProcessedSponsorTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(257, "Prerequisite Tests Invoice Processed - Applicant Invoice - Credit Card", "Your NAATI application test invoice has been processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterCulturalTestInvoiceProcessedCreditCard, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(258, "Prerequisite Tests invoice Processed - Direct Debit", "NAATI Certification Test scheduled- Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterCulturalTestInvoiceProcessedDirectDebit, noReplyEmail, 0);
            CreateOrUpdateEmailTemplate(259, "Prerequisite Tests Invoice Processed - Sponsor Invoice", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalTestInvoiceProcessedSponsorTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(260, "CCL Test Invoice Paid", "Test fee paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CclTestInvoicePaidAfterSelectSession, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(261, "Certification Test Invoice Paid", "Test fee paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.CertificationTestInvoicePaidAfterSelectTestSession, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(262, "Prerequisite Tests Invoice Paid", ": Test fee paid – Application [[Application Reference]]", EmailTemplates.EmailTemplates.EthicsInterculturalTestInvoicePaidAfterSelectTestSesession, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(263, "Test Material Request - Approved - Coordinator", "Your submission for the creation of Test Material [[Test Material ID]] has been approved", EmailTemplates.EmailTemplates.TestMaterialRequestApprovedCoordinatorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(264, "Test Material Request - Assign Coordinator", "You are selected as the Coordinator for the creation of Test Material [[Test Material ID]]", EmailTemplates.EmailTemplates.TestMaterialRequestAssignCoordinatorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(265, "Test Material Request - Cancel - Coordinator", "Test Material Creation [[Test Material ID]] for [[Credential Type]] - [[Test Task Description]] in [[Language]] has been cancelled", EmailTemplates.EmailTemplates.TestMaterialRequestCancelCoordinatorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(266, "Test Material Request - Rejected - Coordinator", "Your submission for the creation of Test Material [[Test Material ID]] has been rejected", EmailTemplates.EmailTemplates.TestMaterialRequestRejectedCoordinatorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(267, "Test Material Request - Revert - Coordinator", "The approval decision of Test Material [[Test Material ID]] has been reverted", EmailTemplates.EmailTemplates.TestMaterialRequestRevertCoordinatorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(268, "Test Material Request - Submit For Approval - Coordinator", "Test Material Request was submitted for approval - Test Material [[Test Material ID]] – Round [[Test Material Round]]", EmailTemplates.EmailTemplates.TestMaterialRequestSubmitForApprovalCoordinatorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(269, "Test Material Request - Submit For Approval - Requestor", "Approval required for Test Material [[Test Material ID]] – Round [[Test Material Round]]", EmailTemplates.EmailTemplates.TestMaterialRequestSubmitForApprovalRequestorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(270, "Test Material Request - Update Due Date", "Due date is changed for Test Material Creation [[Test Material ID]] [[Credential Type]] - [[Test Task Description]] in [[Language]]", EmailTemplates.EmailTemplates.TestMaterialRequestUpdateDueDateTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(271, "Test Material Request - Update Request Details - Coordinator", "Information to Test Material Creation [[Test Material ID]] [[Credential Type]] - [[Test Task Description]] in [[Language]] has changed", EmailTemplates.EmailTemplates.TestMaterialRequestUpdateRequestDetailsCoordinatorTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(272, "Test Material Request - Assign panel Collaborator", "You are selected as a Panel Collaborator for the creation of Test Material [[Test Material ID]] for [[Credential Type]] - [[Test Task Description]] in [[Language]]", EmailTemplates.EmailTemplates.TestMaterialRequestAssignPanelCollaborator, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(273, "Test Material Request - Remove panel Collaborator", "You are no longer a Panel Collaborator for the creation of Test Material [[Test Material ID]] for [[Credential Type]] - [[Test Task Description]] in [[Language]]", EmailTemplates.EmailTemplates.TestMaterialRequestRemovePanelCollaborator, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(274, "Accreditation Credential Issued", "Congratulations! You have been awarded a NAATI Credential", EmailTemplates.EmailTemplates.AccreditationCredentialIssuedTemplate, transitionEmail, 1);
            CreateOrUpdateEmailTemplate(275, "Accreditation Credential Reissued", "Congratulations! You have been awarded a NAATI Credential", EmailTemplates.EmailTemplates.AccreditationCredentialIssuedTemplate, transitionEmail, 1);
            
            CreateOrUpdateEmailTemplate(276, "Email Reminder - Recertification Due", "Your certification is expiring soon", EmailTemplates.EmailTemplates.RecertificationReminderTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(277, "Email Reminder - Upcoming Test", "You have an upcoming test with NAATI", EmailTemplates.EmailTemplates.TestSessionReminderTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(278, "Email Reminder - Certification Test Available", "Available test session for you on [[Test Session Date]]", EmailTemplates.EmailTemplates.TestSessionAvailabilityNoticeTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(279, "Refund - Request Submitted", "Your refund request for [[Application Reference]] has been submitted", EmailTemplates.EmailTemplates.RefundRequestedEmailTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(280, "Refund - Request Rejected", "Your refund request for [[Application Reference]] has been rejected", EmailTemplates.EmailTemplates.RefundRejectedEmailTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(281, "Refund - Refund Processed", "Your refund for [[Application Reference]] has been processed", EmailTemplates.EmailTemplates.CreditNoteProcessed, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(282, "Refund - Request Approved", "Your refund request for [[Application Reference]] has been approved", EmailTemplates.EmailTemplates.RefundApprovedEmailTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(283, "Application Assessment Payment (myNAATI)", "Your application assessment fee for [[Application Reference]]", EmailTemplates.EmailTemplates.MakeApplicationPaymentTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(284, "Test Material Not Assigned to Test Sessions Reminder", "Test Material Not Assigned to Test Sessions Reminder", EmailTemplates.EmailTemplates.TestMaterialReminderEmailTemplate, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(285, "Practice Test Invitation", "Practice Test Invitation", EmailTemplates.EmailTemplates.PracticeTestInviteForTestingInvoiceApplicantTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(286, "Practice Test Issue Result", "Practice Test Issue Result", EmailTemplates.EmailTemplates.PracticeTestIssueResultTemplate, noReplyEmail, 1);

            // TFS 205073
            CreateOrUpdateEmailTemplate(287, "Practice Test Invoice Processed - Applicant Invoice - Credit Card", "Your NAATI application practice test invoice has been processed – Application [[Application Reference]]", EmailTemplates.EmailTemplates.PracticeTestInvoiceProcessedApplicationInvoiceCreditCardTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(288, "Practice Test Allocate Test Session - Credit Card", "NAATI Practice Test scheduled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.PracticeTestAllocateTestSessionCreditCardTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(289, "Practice Test Allocate Test Session - Applicant with Sponsor", "NAATI Practice Test scheduled – Application [[Application Reference]]", EmailTemplates.EmailTemplates.PracticeTestAllocateTestSessionApplicantWithSponsorTemplate, noReplyEmail, 1);
            CreateOrUpdateEmailTemplate(290, "Sponsored Test invoice Processed", "Invoice for applicant [[Given Name]] [[Family Name]] – Application [[Application Reference]]", EmailTemplates.EmailTemplates.PracticeTestSponsoredTestInvoiceProcessed, noReplyEmail, 1);

            CreateOrUpdateEmailTemplate(291, "Generic Email", "NAATI correspondence", EmailTemplates.EmailTemplates.Generic, noReplyEmail, 1);
        }

        public override void RunDescendantOrderScripts()
        {
            DeleteTableRow("135");
            DeleteTableRow("143");
        }

        private void CreateOrUpdateEmailTemplate(int id, string name, string subject, string content, string fromAddress,
            int active)
        {
            const string emailHeadToken = "[[EmailTemplateHead]]";
            const string emailFooterToken = "[[EmailTemplateFooter]]";
            const string materialRequestEmailFooterToken = "[[MaterialRequestEmailTemplateFooter]]";
            const string materialRequestCollaboratorEmailFooterToken = "[[MaterialRequestCollaboratorEmailTemplateFooter]]";
            const string automatedEmailFooterToken = "[[AutomatedEmailTemplateFooter]]";
            const string naatiOperationTeamEmailTemplateFooter = "[[NaatiOperationTeamEmailTemplateFooter]]";
            const string cclEmailFooterToken = "[[CclEmailTemplateFooter]]";
            const string claEmailFooterToken = "[[ClaEmailTemplateFooter]]";
            const string emailFontFamily = "[[EmailTemplateFontFamily]]";
            const string commonClosingToken = "[[CommonClosing]]";
            const string furtherQuestionsToken = "[[FurtherQuestions]]";
            const string yourCustomerNumberToken = "[[YourCustomerNumber]]";
            const string paymentOptionsToken = "[[PaymentOptions]]";

            var emailBody = content
                .Replace(emailHeadToken, EmailTemplates.EmailTemplates.EmailTemplateHead)
                .Replace(emailFooterToken, EmailTemplates.EmailTemplates.EmailTemplateFooter)
                .Replace(materialRequestEmailFooterToken, EmailTemplates.EmailTemplates.MaterialRequestEmailTemplateFooter)
                .Replace(materialRequestCollaboratorEmailFooterToken, EmailTemplates.EmailTemplates.MaterialRequestCollaboratorEmailTemplateFooter)
                .Replace(automatedEmailFooterToken, EmailTemplates.EmailTemplates.AutomatedEmailTemplateFooter)
                .Replace(naatiOperationTeamEmailTemplateFooter, EmailTemplates.EmailTemplates.NaatiOperationTeamEmailTemplateFooter)
                .Replace(cclEmailFooterToken, EmailTemplates.EmailTemplates.CclEmailTemplateFooter)
                .Replace(claEmailFooterToken, EmailTemplates.EmailTemplates.ClaEmailTemplateFooter)
                .Replace(commonClosingToken, EmailTemplates.EmailTemplates.EmailTemplateCommonClosing)
                .Replace(emailFontFamily, EmailTemplates.EmailTemplates.FontFamily)
                .Replace(furtherQuestionsToken, EmailTemplates.EmailTemplates.FurtherQuestions)
                .Replace(yourCustomerNumberToken, EmailTemplates.EmailTemplates.YourCustomerNumber)
                .Replace(paymentOptionsToken, EmailTemplates.EmailTemplates.EmailTemplatePaymentOptions);

            var emailAddress = fromAddress;
            if (ScriptRunner.CurrentEnvironment != ScriptSEnvironmentName.Prod && !string.IsNullOrEmpty(fromAddress))
            {
                var emailDomain =
                    ScriptRunner.CurrentEnvironment == ScriptSEnvironmentName.Uat
                        ? "@naati.com.au"
                        : "@altf4solutions.onmicrosoft.com";

                var index = fromAddress.IndexOf("@", StringComparison.InvariantCulture);

                emailAddress = $"{ScriptRunner.CurrentEnvironment}.{fromAddress.Substring(0, index) + emailDomain}";
            }

            CreateOrUpdateTableRow(
                new[]
                {
                    id.ToString(),
                    name,
                    subject,
                    emailBody,
                    emailAddress,
                    active.ToString()
                });
        }
    }
}
