using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.EmailTemplates.ActionBuilder
{
    public class CredentialApplicationActionBuilderHelper : IEmailTemplateBuilderHelper
    {
        public IEnumerable<ISystemActionEmailTemplateScriptBuilder> GetActionBuilders()
        {
            var builders = new List<ISystemActionEmailTemplateScriptBuilder>();


            var submitApplicationNoneEvent42 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SubmitApplication)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(42)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(submitApplicationNoneEvent42);

            var submitApplicationNoneEvent45 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SubmitApplication).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(45)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(submitApplicationNoneEvent45);

            //var submitApplicationNoneEvent50 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.SubmitApplication).WithEvent(SystemActionEventTypeName.None)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(50)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(submitApplicationNoneEvent50);

            var submitApplicationNoneEvent51 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SubmitApplication).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(51)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(submitApplicationNoneEvent51);

            var submitApplicationNoneEvent53 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SubmitApplication).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Transition, CredentialApplicationTypeName.Accreditation)
                .ThenUseEmailTemplate(53)
                .To(EmailTemplateDetailTypeName.SendToApplicant);
            
            builders.Add(submitApplicationNoneEvent53);

            var submitApplicationInvoiceCreatedToApplicantEvent115 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SubmitApplication).WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV2)
                .ThenUseEmailTemplate(115)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(submitApplicationInvoiceCreatedToApplicantEvent115);

            var submitApplicationCreditCardPaymentReceivedEvent116 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SubmitApplication).WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV2)
                .ThenUseEmailTemplate(116)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(submitApplicationCreditCardPaymentReceivedEvent116);

            var submitApplicationNoneEvent147 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SubmitApplication).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Recertification)
                .ThenUseEmailTemplate(147)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(submitApplicationNoneEvent147);

            var submitApplicationNoneEvent159 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SubmitApplication).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(159)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(submitApplicationNoneEvent159);

            var rejectApplicationNoneEvent41 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectApplication).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(41)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectApplicationNoneEvent41);

            var rejectApplicationNoneEvent44 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectApplication).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(44)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectApplicationNoneEvent44);

            var rejectApplicationNoneEvent52 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectApplication).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Transition, CredentialApplicationTypeName.Accreditation)
                .ThenUseEmailTemplate(52)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectApplicationNoneEvent52);

            var rejectApplicationNoneEvent148 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectApplication).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Recertification)
                .ThenUseEmailTemplate(148)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectApplicationNoneEvent148);

            var rejectApplicationNoneEvent160 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectApplication).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(160)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectApplicationNoneEvent160);

            var finishCheckingInvoiceCreatedToApplicantEvent60 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(60)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(finishCheckingInvoiceCreatedToApplicantEvent60);

            var finishCheckingInvoiceCreatedToUntrustedSponsorEvent61 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(61)
                .To(EmailTemplateDetailTypeName.SendToSponsor, EmailTemplateDetailTypeName.AttachInoviceToSponsor);

            builders.Add(finishCheckingInvoiceCreatedToUntrustedSponsorEvent61);

            var finishCheckingInvoiceCreatedToUntrustedSponsorEvent62 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(62)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(finishCheckingInvoiceCreatedToUntrustedSponsorEvent62);

            var finishCheckingNoneEvent118 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(118)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(finishCheckingNoneEvent118);

            var finishCheckingInvoiceCreatedToApplicantEvent131 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Accreditation, CredentialApplicationTypeName.Transition)
                .ThenUseEmailTemplate(131)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(finishCheckingInvoiceCreatedToApplicantEvent131);

            var finishCheckingInvoiceCreatedToUntrustedSponsorEvent145 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Transition, CredentialApplicationTypeName.Accreditation)
                .ThenUseEmailTemplate(145)
                .To(EmailTemplateDetailTypeName.SendToSponsor, EmailTemplateDetailTypeName.AttachInoviceToSponsor);

            builders.Add(finishCheckingInvoiceCreatedToUntrustedSponsorEvent145);

            var finishCheckingInvoiceCreatedToUntrustedSponsorEvent146 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Accreditation, CredentialApplicationTypeName.Transition)
                .ThenUseEmailTemplate(146)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(finishCheckingInvoiceCreatedToUntrustedSponsorEvent146);

            var finishCheckingInvoiceCreatedToApplicantEvent156 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Recertification)
                .ThenUseEmailTemplate(156)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(finishCheckingInvoiceCreatedToApplicantEvent156);

            var finishCheckingInvoiceCreatedToUntrustedSponsorEvent157 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Recertification)
                .ThenUseEmailTemplate(157)
                .To(EmailTemplateDetailTypeName.SendToSponsor, EmailTemplateDetailTypeName.AttachInoviceToSponsor);

            builders.Add(finishCheckingInvoiceCreatedToUntrustedSponsorEvent157);

            var finishCheckingInvoiceCreatedToUntrustedSponsorEvent158 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Recertification)
                .ThenUseEmailTemplate(158)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(finishCheckingInvoiceCreatedToUntrustedSponsorEvent158);

            var finishCheckingInvoiceCreatedToApplicantEvent161 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(161)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(finishCheckingInvoiceCreatedToApplicantEvent161);

            var finishCheckingInvoiceCreatedToUntrustedSponsorEvent162 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(162)
                .To(EmailTemplateDetailTypeName.SendToSponsor, EmailTemplateDetailTypeName.AttachInoviceToSponsor);

            builders.Add(finishCheckingInvoiceCreatedToUntrustedSponsorEvent162);

            var finishCheckingInvoiceCreatedToUntrustedSponsorEvent163 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(163)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(finishCheckingInvoiceCreatedToUntrustedSponsorEvent163);

            var finishCheckingInvoiceCreatedToUntrustedSponsorEvent164 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FinishChecking).WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousCertification)
                .ThenUseEmailTemplate(61)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(finishCheckingInvoiceCreatedToUntrustedSponsorEvent164);

            var assessmentInvoicePaidNoneEvent68 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AssessmentInvoicePaid).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(68)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(assessmentInvoicePaidNoneEvent68);

            var assessmentInvoicePaidNoneEvent132 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AssessmentInvoicePaid).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Transition, CredentialApplicationTypeName.Accreditation, CredentialApplicationTypeName.IndigenousCertification)
                .ThenUseEmailTemplate(132)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(assessmentInvoicePaidNoneEvent132);

            var assessmentInvoicePaidNoneEvent149 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AssessmentInvoicePaid).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Recertification)
                .ThenUseEmailTemplate(149)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(assessmentInvoicePaidNoneEvent149);

            var rejectApplicationAfterInvoiceNoneEvent41 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectApplicationAfterInvoice).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV2)
                .ThenUseEmailTemplate(41)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectApplicationAfterInvoiceNoneEvent41);

            var rejectApplicationAfterInvoiceNoneEvent69 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectApplicationAfterInvoice).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(69)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectApplicationAfterInvoiceNoneEvent69);

            var rejectApplicationAfterInvoiceNoneEvent164 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectApplicationAfterInvoice).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(164)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectApplicationAfterInvoiceNoneEvent164);

            var applicationSubmissionProcessedInvoiceCreatedToApplicantEvent126 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ApplicationSubmissionProcessed).WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV2)
                .ThenUseEmailTemplate(126)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(applicationSubmissionProcessedInvoiceCreatedToApplicantEvent126);

            var applicationSubmissionProcessedCreditCardPaymentReceivedEvent127 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ApplicationSubmissionProcessed).WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV2)
                .ThenUseEmailTemplate(127)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(applicationSubmissionProcessedCreditCardPaymentReceivedEvent127);

            var applicationInvoicePaidNoneEvent117 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ApplicationInvoicePaid).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV2)
                .ThenUseEmailTemplate(117)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(applicationInvoicePaidNoneEvent117);

            var applicationInvoicePaidNoneEvent165 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AssessmentInvoicePaid).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(165)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(applicationInvoicePaidNoneEvent165);

            var sendRecertificationReminderNoneEvent276 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SendRecertificationReminder).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Transition, CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Recertification, CredentialApplicationTypeName.IndigenousCertification)
                .ThenUseEmailTemplate(276)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(sendRecertificationReminderNoneEvent276);

            var sendMakeApplicationPaymentAdviceEvent283 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.MakeApplicationPayment).WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.Transition, CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Recertification, CredentialApplicationTypeName.IndigenousCertification)
                .ThenUseEmailTemplate(283)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(sendMakeApplicationPaymentAdviceEvent283);

            var submitPracticeTestApplicationNoneEvent285 = EmailTemplateBuilderExtension
            .WhenAction(SystemActionTypeName.SubmitApplication).WithEvent(SystemActionEventTypeName.None)
            .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.PracticeTest)
            .ThenUseEmailTemplate(285)
            .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(submitPracticeTestApplicationNoneEvent285);

            //TFS 200215
            var progressApplicationToEligibleForTestingCcl = EmailTemplateBuilderExtension
            .WhenAction(SystemActionTypeName.ProgressCredentialToEligibleForTesting).WithEvent(SystemActionEventTypeName.None)
            .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3)
            .ThenUseEmailTemplate(118)
            .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(progressApplicationToEligibleForTestingCcl);

            var progressApplicationToEligibleForTestingCertification = EmailTemplateBuilderExtension
            .WhenAction(SystemActionTypeName.ProgressCredentialToEligibleForTesting).WithEvent(SystemActionEventTypeName.None)
            .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
            .ThenUseEmailTemplate(47)
            .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(progressApplicationToEligibleForTestingCertification);

            var sendEmail = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SendEmail)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnAnyApplicationType()
                .ThenUseEmailTemplate(291)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(sendEmail);

            return builders;
        }
    }
}