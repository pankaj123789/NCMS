using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.EmailTemplates.ActionBuilder
{
    public class CredentialRequestActionBuilderHelper : IEmailTemplateBuilderHelper
    {
        public IEnumerable<ISystemActionEmailTemplateScriptBuilder> GetActionBuilders()
        {
            var builders = new List<ISystemActionEmailTemplateScriptBuilder>();

            var passAssessmentNoneEvent47 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PassAssessment)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(47)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(passAssessmentNoneEvent47);

            var generatePreRequisiteEvent47 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreatePreRequisiteApplications)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(47)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(generatePreRequisiteEvent47);

            var generatePreRequisiteEvent51 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreatePreRequisiteApplications)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest)
                .ThenUseEmailTemplate(51)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(generatePreRequisiteEvent51);

            var pendAssessmentNoneEvent48 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PendAssessment)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(48)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(pendAssessmentNoneEvent48);

            var pendAssessmentNoneEvent55 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PendAssessment)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Transition, CredentialApplicationTypeName.Accreditation)
                .ThenUseEmailTemplate(55)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(pendAssessmentNoneEvent55);

            var pendAssessmentNoneEvent150 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PendAssessment)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Recertification)
                .ThenUseEmailTemplate(150)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(pendAssessmentNoneEvent150);

            var failAssessmentNoneEvent46 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FailAssessment)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(46)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(failAssessmentNoneEvent46);

            var failAssessmentNoneEvent54 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FailAssessment)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Transition, CredentialApplicationTypeName.Accreditation)
                .ThenUseEmailTemplate(54)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(failAssessmentNoneEvent54);

            var failAssessmentNoneEvent151 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FailAssessment)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Recertification)
                .ThenUseEmailTemplate(151)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(failAssessmentNoneEvent151);

            var failPendingAssessmentNoneEvent59 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FailPendingAssessment)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Accreditation, CredentialApplicationTypeName.Transition)
                .ThenUseEmailTemplate(59)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(failPendingAssessmentNoneEvent59);

            var failPendingAssessmentNoneEvent152 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.FailPendingAssessment)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Recertification)
                .ThenUseEmailTemplate(152)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(failPendingAssessmentNoneEvent152);

            var issueCredentialNoneEvent56 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Transition)
                .ThenUseEmailTemplate(56)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueCredentialNoneEvent56);

            var issueCredentialNoneEvent70 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(70)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueCredentialNoneEvent70);

            var issueCredentialNoneEvent109 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(109)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueCredentialNoneEvent109);

            //var issueCredentialNoneEvent138 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.IssueCredential)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(138)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            //builders.Add(issueCredentialNoneEvent138);

            var issueCredentialNoneEvent141 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(141)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueCredentialNoneEvent141);

            var issueCredentialNoneEvent153 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Recertification)
                .ThenUseEmailTemplate(153)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueCredentialNoneEvent153);

            var issueCredentialNoneEvent166 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(166)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueCredentialNoneEvent166);

            var issueCredentialNoneEvent212 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousCertification)
                .ThenUseEmailTemplate(212)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueCredentialNoneEvent212);

            var issueCredentialNoneEvent216 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousEthics)
                .ThenUseEmailTemplate(216)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueCredentialNoneEvent216);

            var issueCredentialNoneEvent219 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousIntercultural)
                .ThenUseEmailTemplate(219)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueCredentialNoneEvent219);

            var issueCredentialNoneEvent274 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Accreditation)
                .ThenUseEmailTemplate(274)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueCredentialNoneEvent274);

            var cancelRequestNoneEvent43 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CancelRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(43)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(cancelRequestNoneEvent43);

            var cancelRequestNoneEvent49 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CancelRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(49)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(cancelRequestNoneEvent49);

            var cancelRequestNoneEvent58 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CancelRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Transition, CredentialApplicationTypeName.Accreditation)
                .ThenUseEmailTemplate(58)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(cancelRequestNoneEvent58);

            var cancelRequestNoneEvent154 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CancelRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Recertification)
                .ThenUseEmailTemplate(154)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(cancelRequestNoneEvent154);

            var cancelRequestNoneEvent167 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CancelRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(167)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(cancelRequestNoneEvent167);

            var reissueCredentialNoneEvent57 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ReissueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Transition)
                .ThenUseEmailTemplate(57)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(reissueCredentialNoneEvent57);

            var reissueCredentialNoneEvent86 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ReissueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(86)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(reissueCredentialNoneEvent86);

            var reissueCredentialNoneEvent110 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ReissueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(110)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(reissueCredentialNoneEvent110);

            var reissueCredentialNoneEvent141 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ReissueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural)
                .ThenUseEmailTemplate(141)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(reissueCredentialNoneEvent141);

            var reissueCredentialNoneEvent155 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ReissueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Recertification)
                .ThenUseEmailTemplate(155)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(reissueCredentialNoneEvent155);

            var reissueCredentialNoneEvent168 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ReissueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(168)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(reissueCredentialNoneEvent168);

            var reissueCredentialNoneEvent213 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ReissueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousCertification)
                .ThenUseEmailTemplate(213)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(reissueCredentialNoneEvent213);

            var reissueCredentialNoneEvent216 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ReissueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousEthics)
                .ThenUseEmailTemplate(216)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(reissueCredentialNoneEvent216);

            var reissueCredentialNoneEvent219 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ReissueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousIntercultural)
                .ThenUseEmailTemplate(219)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(reissueCredentialNoneEvent219);

            var reissueCredentialNoneEvent275 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ReissueCredential)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Accreditation)
                .ThenUseEmailTemplate(275)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(reissueCredentialNoneEvent275);

            var testInvoicePaidNoneEvent66 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoicePaid)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(66)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(testInvoicePaidNoneEvent66);

            //var testInvoicePaidNoneEvent74 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.TestInvoicePaid)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(74)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(testInvoicePaidNoneEvent74);

            var testInvoicePaidNoneEvent79 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoicePaid)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest)
                .ThenUseEmailTemplate(79)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(testInvoicePaidNoneEvent79);

            var testInvoicePaidNoneEvent84 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoicePaid)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(84)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(testInvoicePaidNoneEvent84);

            var testInvoicePaidTestSessionConfirmedEvent260 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoicePaid)
                .WithEvent(SystemActionEventTypeName.TestSessionConfirmed)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(260)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(testInvoicePaidTestSessionConfirmedEvent260);

            var testInvoicePaidTestSessionConfirmedEvent261 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoicePaid)
                .WithEvent(SystemActionEventTypeName.TestSessionConfirmed)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(261)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(testInvoicePaidTestSessionConfirmedEvent261);

            var testInvoicePaidTestSessionConfirmedEvent262 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoicePaid)
                .WithEvent(SystemActionEventTypeName.TestSessionConfirmed)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(262)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(testInvoicePaidTestSessionConfirmedEvent262);

            var withdrawRequestNoneEvent67 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.WithdrawRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(67)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(withdrawRequestNoneEvent67);

            //var withdrawRequestNoneEvent75 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.WithdrawRequest)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(75)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(withdrawRequestNoneEvent75);

            var withdrawRequestNoneEvent80 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.WithdrawRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(80)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(withdrawRequestNoneEvent80);

            var withdrawRequestNoneEvent85 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.WithdrawRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(85)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(withdrawRequestNoneEvent85);

            var withdrawRequestNoneEvent172 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.WithdrawRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(172)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(withdrawRequestNoneEvent172);

            var allocateTestSessionNoneEvent87 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSession)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(87)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionNoneEvent87);

            var allocateTestSessionNoneEvent92 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSession)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(92)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionNoneEvent92);

            //var allocateTestSessionNoneEvent97 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.AllocateTestSession)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(97)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(allocateTestSessionNoneEvent97);

            var allocateTestSessionNoneEvent102 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSession)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(102)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionNoneEvent102);

            var allocateTestSessionNoneEvent119 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSession)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(119)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionNoneEvent119);

            var allocateTestSessionNoneEvent173 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSession)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(173)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionNoneEvent173);

            var allocateTestSessionCreditCardPaymentReceivedEvent243 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSession)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(243)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionCreditCardPaymentReceivedEvent243);

            //var allocateTestSessionInvoiceCreatedToApplicantEvent244 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.AllocateTestSession)
            //    .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3)
            //    .ThenUseEmailTemplate(244)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(allocateTestSessionInvoiceCreatedToApplicantEvent244);

            var allocateTestSessionCreditCardPaymentReceivedEvent247 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSession)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(247)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionCreditCardPaymentReceivedEvent247);

            //var allocateTestSessionInvoiceCreatedToApplicantEvent248 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.AllocateTestSession)
            //    .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
            //    .ThenUseEmailTemplate(248)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(allocateTestSessionInvoiceCreatedToApplicantEvent248);

            var allocateTestSessionInvoiceCreatedToUntrustedSponsorEvent249 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSession)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(249)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionInvoiceCreatedToUntrustedSponsorEvent249);

            var allocateTestSessionCreditCardPaymentReceivedEvent251 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSession)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(251)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionCreditCardPaymentReceivedEvent251);

            //var allocateTestSessionInvoiceCreatedToApplicantEvent252 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.AllocateTestSession)
            //    .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(252)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(allocateTestSessionInvoiceCreatedToApplicantEvent252);

            var allocateTestSessionInvoiceCreatedToUntrustedSponsorEvent253 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSession)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(253)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionInvoiceCreatedToUntrustedSponsorEvent253);

            var rejectTestSessionFromMyNaatiNoneEvent91 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(91)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectTestSessionFromMyNaatiNoneEvent91);

            var rejectTestSessionFromMyNaatiNoneEvent96 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(96)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectTestSessionFromMyNaatiNoneEvent96);

            //var rejectTestSessionFromMyNaatiNoneEvent101 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.RejectTestSessionFromMyNaati)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(101)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(rejectTestSessionFromMyNaatiNoneEvent101);

            var rejectTestSessionFromMyNaatiNoneEvent106 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest)
                .ThenUseEmailTemplate(106)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectTestSessionFromMyNaatiNoneEvent106);

            var rejectTestSessionFromMyNaatiNoneEvent120 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.CCLV2)
                .ThenUseEmailTemplate(120)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectTestSessionFromMyNaatiNoneEvent120);

            var rejectTestSessionFromMyNaatiNoneEvent176 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(176)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectTestSessionFromMyNaatiNoneEvent176);

            var rejectTestSessionNoneEvent90 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectTestSession)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(90)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectTestSessionNoneEvent90);

            var rejectTestSessionNoneEvent95 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectTestSession)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(95)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectTestSessionNoneEvent95);

            //var rejectTestSessionNoneEvent100 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.RejectTestSession)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(100)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(rejectTestSessionNoneEvent100);

            var rejectTestSessionNoneEvent105 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectTestSession)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(105)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectTestSessionNoneEvent105);

            var rejectTestSessionNoneEvent120 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectTestSession)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.CCLV2)
                .ThenUseEmailTemplate(120)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectTestSessionNoneEvent120);

            var rejectTestSessionNoneEvent177 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectTestSession)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(177)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(rejectTestSessionNoneEvent177);

            var issueFailNoneEvent107 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFail)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(107)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailNoneEvent107);

            var issueFailNoneEvent133 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFail)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(133)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailNoneEvent133);

            //var issueFailNoneEvent136 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.IssueFail)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(136)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(issueFailNoneEvent136);

            var issueFailNoneEvent139 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFail)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(139)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailNoneEvent139);

            var issueFailNoneEvent178 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFail)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(178)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailNoneEvent178);

            var issueFailSupplementaryTestOfferedEvent203 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFail)
                .WithEvent(SystemActionEventTypeName.SupplementaryTestOffered)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(203)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailSupplementaryTestOfferedEvent203);

            var issueFailConcededPassOfferedEvent204 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFail)
                .WithEvent(SystemActionEventTypeName.ConcededPassOffered)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(204)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueFailConcededPassOfferedEvent204);

            var issueFailNoneEvent206 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFail)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousCertification)
                .ThenUseEmailTemplate(206)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailNoneEvent206);

            var issueFailSupplementaryTestOfferedEvent210 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFail)
                .WithEvent(SystemActionEventTypeName.SupplementaryTestOffered)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousCertification)
                .ThenUseEmailTemplate(210)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailSupplementaryTestOfferedEvent210);

            var issueFailConcededPassOfferedEvent211 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFail)
                .WithEvent(SystemActionEventTypeName.ConcededPassOffered)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousCertification)
                .ThenUseEmailTemplate(211)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueFailConcededPassOfferedEvent211);

            var issueFailNoneEvent214 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFail)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousEthics)
                .ThenUseEmailTemplate(214)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailNoneEvent214);

            var issueFailNoneEvent217 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFail)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousIntercultural)
                .ThenUseEmailTemplate(217)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailNoneEvent217);

            var issuePassNoneEvent108 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssuePass)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2)
                .ThenUseEmailTemplate(108)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issuePassNoneEvent108);

            var issuePassNoneEvent134 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssuePass)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(134)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issuePassNoneEvent134);

            //var issuePassNoneEvent137 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.IssuePass)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(137)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(issuePassNoneEvent137);

            var issuePassNoneEvent140 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssuePass)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest)
                .ThenUseEmailTemplate(140)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issuePassNoneEvent140);

            var issuePassNoneEvent179 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssuePass)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(179)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issuePassNoneEvent179);

            var issuePassNoneEvent207 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssuePass)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousCertification)
                .ThenUseEmailTemplate(207)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issuePassNoneEvent207);

            var issuePassNoneEvent215 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssuePass)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousEthics)
                .ThenUseEmailTemplate(215)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issuePassNoneEvent215);

            var issuePassNoneEvent218 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssuePass)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousIntercultural)
                .ThenUseEmailTemplate(218)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issuePassNoneEvent218);

            var createPaidTestReviewNoneEvent111 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreatePaidTestReview)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(111)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(createPaidTestReviewNoneEvent111);

            var createPaidTestReviewInvoiceCreatedToApplicantEvent184 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreatePaidTestReview)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(184)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(createPaidTestReviewInvoiceCreatedToApplicantEvent184);

            var createPaidTestReviewInvoiceCreatedToUntrustedSponsorEvent185 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreatePaidTestReview)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.Ethics)
                .ThenUseEmailTemplate(185)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(createPaidTestReviewInvoiceCreatedToUntrustedSponsorEvent185);

            var createPaidTestReviewCreditCardPaymentReceivedEvent186 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreatePaidTestReview)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(186)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(createPaidTestReviewCreditCardPaymentReceivedEvent186);

            var createPaidTestReviewInvoiceCreatedToApplicantEvent193 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreatePaidTestReview)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(193)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(createPaidTestReviewInvoiceCreatedToApplicantEvent193);

            var createPaidTestReviewInvoiceCreatedToUntrustedSponsorEvent194 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreatePaidTestReview)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(194)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(createPaidTestReviewInvoiceCreatedToUntrustedSponsorEvent194);

            var createPaidTestReviewCreditCardPaymentReceivedEvent195 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreatePaidTestReview)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(195)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(createPaidTestReviewCreditCardPaymentReceivedEvent195);

            var createPaidTestReviewNoneEvent202 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreatePaidTestReview)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest)
                .ThenUseEmailTemplate(202)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(createPaidTestReviewNoneEvent202);

            var issueFailAfterReviewNoneEvent112 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFailAfterReview)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(112)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailAfterReviewNoneEvent112);

            var issueFailAfterReviewNoneEvent142 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFailAfterReview)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(142)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailAfterReviewNoneEvent142);

            var issueFailAfterReviewNoneEvent208 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFailAfterReview)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousIntercultural, CredentialApplicationTypeName.IndigenousCertification, CredentialApplicationTypeName.IndigenousEthics)
                .ThenUseEmailTemplate(208)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailAfterReviewNoneEvent208);

            var issueFailAfterReviewSupplementaryTestOfferedEvent220 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFailAfterReview)
                .WithEvent(SystemActionEventTypeName.SupplementaryTestOffered)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousCertification)
                .ThenUseEmailTemplate(220)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailAfterReviewSupplementaryTestOfferedEvent220);

            var issueFailAfterReviewConcededPassOfferedEvent221 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFailAfterReview)
                .WithEvent(SystemActionEventTypeName.ConcededPassOffered)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousCertification)
                .ThenUseEmailTemplate(221)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueFailAfterReviewConcededPassOfferedEvent221);

            var issueFailAfterReviewSupplementaryTestOfferedEvent222 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFailAfterReview)
                .WithEvent(SystemActionEventTypeName.SupplementaryTestOffered)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(222)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueFailAfterReviewSupplementaryTestOfferedEvent222);

            var issueFailAfterReviewConcededPassOfferedEvent223 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssueFailAfterReview)
                .WithEvent(SystemActionEventTypeName.ConcededPassOffered)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(223)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant);

            builders.Add(issueFailAfterReviewConcededPassOfferedEvent223);

            var issuePassAfterReviewNoneEvent113 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssuePassAfterReview)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(113)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issuePassAfterReviewNoneEvent113);

            var issuePassAfterReviewNoneEvent144 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssuePassAfterReview)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Intercultural)
                .ThenUseEmailTemplate(144)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issuePassAfterReviewNoneEvent144);

            var issuePassAfterReviewNoneEvent209 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssuePassAfterReview)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousIntercultural, CredentialApplicationTypeName.IndigenousCertification, CredentialApplicationTypeName.IndigenousEthics)
                .ThenUseEmailTemplate(209)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issuePassAfterReviewNoneEvent209);

            var createSupplementaryTestInvoiceCreatedToApplicantEvent228 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreateSupplementaryTest)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(228)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(createSupplementaryTestInvoiceCreatedToApplicantEvent228);

            var createSupplementaryTestInvoiceCreatedToUntrustedSponsorEvent229 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreateSupplementaryTest)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(229)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(createSupplementaryTestInvoiceCreatedToUntrustedSponsorEvent229);

            var createSupplementaryTestCreditCardPaymentReceivedEvent230 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreateSupplementaryTest)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(230)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(createSupplementaryTestCreditCardPaymentReceivedEvent230);

            var createSupplementaryTestNoneEvent231 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreateSupplementaryTest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(231)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(createSupplementaryTestNoneEvent231);

            var withdrawSupplementaryTestNoneEvent124 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.WithdrawSupplementaryTest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(124)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(withdrawSupplementaryTestNoneEvent124);

            var supplementaryTestInvoicePaidNoneEvent125 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SupplementaryTestInvoicePaid)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(125)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(supplementaryTestInvoicePaidNoneEvent125);

            var allocateTestSessionFromMyNaatiNoneEvent89 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(89)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionFromMyNaatiNoneEvent89);

            var allocateTestSessionFromMyNaatiNoneEvent94 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(94)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionFromMyNaatiNoneEvent94);

            //var allocateTestSessionFromMyNaatiNoneEvent98 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(98)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(allocateTestSessionFromMyNaatiNoneEvent98);

            var allocateTestSessionFromMyNaatiNoneEvent104 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(104)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionFromMyNaatiNoneEvent104);

            var allocateTestSessionFromMyNaatiNoneEvent119 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.CCLV2)
                .ThenUseEmailTemplate(119)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionFromMyNaatiNoneEvent119);

            var allocateTestSessionFromMyNaatiNoneEvent183 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(183)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionFromMyNaatiNoneEvent183);

            var allocateTestSessionFromMyNaatiCreditCardPaymentReceivedEvent243 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(243)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionFromMyNaatiCreditCardPaymentReceivedEvent243);

            //var allocateTestSessionFromMyNaatiInvoiceCreatedToApplicantEvent244 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
            //    .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3)
            //    .ThenUseEmailTemplate(244)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(allocateTestSessionFromMyNaatiInvoiceCreatedToApplicantEvent244);

            var allocateTestSessionFromMyNaatiCreditCardPaymentReceivedEvent247 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(247)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionFromMyNaatiCreditCardPaymentReceivedEvent247);

            //var allocateTestSessionFromMyNaatiInvoiceCreatedToApplicantEvent248 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
            //    .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
            //    .ThenUseEmailTemplate(248)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(allocateTestSessionFromMyNaatiInvoiceCreatedToApplicantEvent248);

            var allocateTestSessionFromMyNaatiInvoiceCreatedToUntrustedSponsorEvent249 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(249)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionFromMyNaatiInvoiceCreatedToUntrustedSponsorEvent249);

            var allocateTestSessionFromMyNaatiCreditCardPaymentReceivedEvent251 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(251)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionFromMyNaatiCreditCardPaymentReceivedEvent251);

            //var allocateTestSessionFromMyNaatiInvoiceCreatedToApplicantEvent252 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
            //    .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(252)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(allocateTestSessionFromMyNaatiInvoiceCreatedToApplicantEvent252);

            var allocateTestSessionFromMyNaatiInvoiceCreatedToUntrustedSponsorEvent253 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(253)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionFromMyNaatiInvoiceCreatedToUntrustedSponsorEvent253);

            var cancelTestInvitationNoneEvent120 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CancelTestInvitation)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(120)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(cancelTestInvitationNoneEvent120);

            var cancelTestInvitationNoneEvent128 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CancelTestInvitation)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(128)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(cancelTestInvitationNoneEvent128);

            //var cancelTestInvitationNoneEvent129 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.CancelTestInvitation)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics)
            //    .ThenUseEmailTemplate(129)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant);

            //builders.Add(cancelTestInvitationNoneEvent129);

            var cancelTestInvitationNoneEvent130 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CancelTestInvitation)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(130)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(cancelTestInvitationNoneEvent130);

            var paidReviewInvoiceProcessedInvoiceCreatedToApplicantEvent187 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PaidReviewInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(187)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(paidReviewInvoiceProcessedInvoiceCreatedToApplicantEvent187);

            var paidReviewInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent188 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PaidReviewInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(188)
                .To(EmailTemplateDetailTypeName.SendToSponsor, EmailTemplateDetailTypeName.AttachInoviceToSponsor);

            builders.Add(paidReviewInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent188);

            var paidReviewInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent189 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PaidReviewInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(189)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(paidReviewInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent189);

            var paidReviewInvoiceProcessedCreditCardPaymentReceivedEvent190 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PaidReviewInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest)
                .ThenUseEmailTemplate(190)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(paidReviewInvoiceProcessedCreditCardPaymentReceivedEvent190);

            var paidReviewInvoiceProcessedInvoiceCreatedToApplicantEvent196 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PaidReviewInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(196)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(paidReviewInvoiceProcessedInvoiceCreatedToApplicantEvent196);

            var paidReviewInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent197 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PaidReviewInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(197)
                .To(EmailTemplateDetailTypeName.SendToSponsor, EmailTemplateDetailTypeName.AttachInoviceToSponsor, EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(paidReviewInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent197);

            var paidReviewInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent198 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PaidReviewInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCL)
                .ThenUseEmailTemplate(198)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(paidReviewInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent198);

            var paidReviewInvoiceProcessedCreditCardPaymentReceivedEvent199 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PaidReviewInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(199)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(paidReviewInvoiceProcessedCreditCardPaymentReceivedEvent199);

            var paidReviewInvoicePaidNoneEvent191 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PaidReviewInvoicePaid)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(191)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(paidReviewInvoicePaidNoneEvent191);

            var paidReviewInvoicePaidNoneEvent200 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.PaidReviewInvoicePaid)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(200)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(paidReviewInvoicePaidNoneEvent200);

            var withdrawPaidReviewNoneEvent192 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.WithdrawPaidReview)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(192)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(withdrawPaidReviewNoneEvent192);

            var withdrawPaidReviewNoneEvent201 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.WithdrawPaidReview)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(201)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(withdrawPaidReviewNoneEvent201);

            var notifyTestSessionDetailsNoneEvent224 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.NotifyTestSessionDetails)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(224)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(notifyTestSessionDetailsNoneEvent224);

            var notifyTestSessionDetailsNoneEvent225 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.NotifyTestSessionDetails)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(225)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(notifyTestSessionDetailsNoneEvent225);

            var notifyTestSessionDetailsNoneEvent226 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.NotifyTestSessionDetails)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(226)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(notifyTestSessionDetailsNoneEvent226);

            var notifyTestSessionDetailsNoneEvent227 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.NotifyTestSessionDetails)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural)
                .ThenUseEmailTemplate(227)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(notifyTestSessionDetailsNoneEvent227);

            var supplementaryTestInvoiceProcessedInvoiceCreatedToApplicantEvent121 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SupplementaryTestInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(121)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(supplementaryTestInvoiceProcessedInvoiceCreatedToApplicantEvent121);

            var supplementaryTestInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent122 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SupplementaryTestInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(122)
                .To(EmailTemplateDetailTypeName.SendToSponsor, EmailTemplateDetailTypeName.AttachInoviceToSponsor);

            builders.Add(supplementaryTestInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent122);

            var supplementaryTestInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent123 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SupplementaryTestInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(123)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(supplementaryTestInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent123);

            var supplementaryTestInvoiceProcessedCreditCardPaymentReceivedEvent232 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SupplementaryTestInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
                .ThenUseEmailTemplate(232)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(supplementaryTestInvoiceProcessedCreditCardPaymentReceivedEvent232);

            var sendCandidateBriefNoneEvent237 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SendCandidateBrief)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(237)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachCandidateBriefToApplicant);

            builders.Add(sendCandidateBriefNoneEvent237);

            var revertResultsFailedTestRevertedEvent238 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RevertResults)
                .WithEvent(SystemActionEventTypeName.FailedTestReverted)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.IndigenousEthics, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.IndigenousIntercultural, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(238)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(revertResultsFailedTestRevertedEvent238);

            var revertResultsPassedTestRevertedEvent239 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RevertResults)
                .WithEvent(SystemActionEventTypeName.PassedTestReverted)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousIntercultural, CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.IndigenousEthics, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(239)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(revertResultsPassedTestRevertedEvent239);

            var revertResultsIssuedCredentialRevertedEvent240 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RevertResults)
                .WithEvent(SystemActionEventTypeName.IssuedCredentialReverted)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.IndigenousEthics, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.IndigenousIntercultural)
                .ThenUseEmailTemplate(240)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(revertResultsIssuedCredentialRevertedEvent240);

            var revertResultsInvalidatedTestRevertedEvent242 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RevertResults)
                .WithEvent(SystemActionEventTypeName.InvalidatedTestReverted)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.IndigenousIntercultural, CredentialApplicationTypeName.IndigenousCertification, CredentialApplicationTypeName.IndigenousEthics, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(242)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(revertResultsInvalidatedTestRevertedEvent242);

            var invalidateTestNoneEvent241 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.InvalidateTest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.IndigenousEthics, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.IndigenousCertification, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.IndigenousIntercultural, CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(241)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(invalidateTestNoneEvent241);

            var testInvoiceProcessedCreditCardPaymentReceivedEvent245 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3)
                .ThenUseEmailTemplate(245)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(testInvoiceProcessedCreditCardPaymentReceivedEvent245);

            //var testInvoiceProcessedInvoiceCreatedToApplicantEvent246 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.TestInvoiceProcessed)
            //    .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCLV3)
            //    .ThenUseEmailTemplate(246)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            //builders.Add(testInvoiceProcessedInvoiceCreatedToApplicantEvent246);

            var testInvoiceProcessedCreditCardPaymentReceivedEvent254 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(254)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(testInvoiceProcessedCreditCardPaymentReceivedEvent254);

            //var testInvoiceProcessedInvoiceCreatedToApplicantEvent255 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.TestInvoiceProcessed)
            //    .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner)
            //    .ThenUseEmailTemplate(255)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            //builders.Add(testInvoiceProcessedInvoiceCreatedToApplicantEvent255);

            var testInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent256 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(256)
                .To(EmailTemplateDetailTypeName.SendToSponsor, EmailTemplateDetailTypeName.AttachInoviceToSponsor);

            builders.Add(testInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent256);

            var testInvoiceProcessedCreditCardPaymentReceivedEvent257 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest)
                .ThenUseEmailTemplate(257)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(testInvoiceProcessedCreditCardPaymentReceivedEvent257);

            //var testInvoiceProcessedInvoiceCreatedToApplicantEvent258 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.TestInvoiceProcessed)
            //    .WithEvent(SystemActionEventTypeName.InvoiceCreatedToApplicant)
            //    .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CSLIKnowledgeTest, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural)
            //    .ThenUseEmailTemplate(258)
            //    .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            //builders.Add(testInvoiceProcessedInvoiceCreatedToApplicantEvent258);

            var testInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent259 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoiceProcessed)
                .WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.CSHIKnowledgeTest, CredentialApplicationTypeName.CSLIKnowledgeTest)
                .ThenUseEmailTemplate(259)
                .To(EmailTemplateDetailTypeName.SendToSponsor, EmailTemplateDetailTypeName.AttachInoviceToSponsor);

            builders.Add(testInvoiceProcessedInvoiceCreatedToUntrustedSponsorEvent259);

            var sendTestSessionReminderNoneEvent277 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SendTestSessionReminder)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.IndigenousIntercultural, CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Recertification, CredentialApplicationTypeName.IndigenousEthics, CredentialApplicationTypeName.IndigenousCertification, CredentialApplicationTypeName.Cla, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.Ethics, CredentialApplicationTypeName.Intercultural, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Transition, CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.Certification)
                .ThenUseEmailTemplate(277)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(sendTestSessionReminderNoneEvent277);

            var sendSessionAvailabilityNoticeNoneEvent278 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SendSessionAvailabilityNotice)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.Certification,
                CredentialApplicationTypeName.CertificationPractitioner,
                CredentialApplicationTypeName.Ethics,
                CredentialApplicationTypeName.Intercultural,
                CredentialApplicationTypeName.CSHIKnowledgeTest,
                CredentialApplicationTypeName.CSLIKnowledgeTest,
                CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(278)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(sendSessionAvailabilityNoticeNoneEvent278);

            var sendRefundRequested = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RequestRefund)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(279)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(sendRefundRequested);

            var sendRefundRejected = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectRefund)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(280)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(sendRefundRejected);

            var sendCreditNoteProcessed = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreditNoteProcessed)
                .WithEvent(SystemActionEventTypeName.CreditCardRefundIssuedToApplicant)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(281)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(sendCreditNoteProcessed);

            var sendCreditNotePaid = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreditNotePaid)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(281)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(sendCreditNotePaid);

            var sendRefundApproved = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ApproveRefund)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.CCL, CredentialApplicationTypeName.CCLV2, CredentialApplicationTypeName.CCLV3, CredentialApplicationTypeName.Certification, CredentialApplicationTypeName.CertificationPractitioner, CredentialApplicationTypeName.Cla)
                .ThenUseEmailTemplate(282)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(sendRefundApproved);

            var issueResultPracticeTestCclResultApplicationNoneEvent286 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.IssuePracticeTestResults).WithEvent(SystemActionEventTypeName.None)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.PracticeTest)
                .ThenUseEmailTemplate(286)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(issueResultPracticeTestCclResultApplicationNoneEvent286);

            var testInvoiceProcessedPracticeTestOnCreditCardPaymentReceived = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoiceProcessed).WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.PracticeTest)
                .ThenUseEmailTemplate(287)
                .To(EmailTemplateDetailTypeName.SendToApplicant, EmailTemplateDetailTypeName.AttachInoviceToApplicant);

            builders.Add(testInvoiceProcessedPracticeTestOnCreditCardPaymentReceived);

            var allocateTestSessionCreditCardPracticeTestOnCreditCardPaymentReceived = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSession).WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.PracticeTest)
                .ThenUseEmailTemplate(288)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionCreditCardPracticeTestOnCreditCardPaymentReceived);

            var allocateTestSessionFromMyNaatiCreditCardPracticeTestOnCreditCardPaymentReceived = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati).WithEvent(SystemActionEventTypeName.CreditCardPaymentReceived)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.PracticeTest)
                .ThenUseEmailTemplate(288)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionFromMyNaatiCreditCardPracticeTestOnCreditCardPaymentReceived);

            var allocateTestSessionApplicationWithSponsorOnEventInvoiceCreatedToUnTrustedSponsor = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSession).WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.PracticeTest)
                .ThenUseEmailTemplate(289)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionApplicationWithSponsorOnEventInvoiceCreatedToUnTrustedSponsor);

            var allocateTestSessionFromMyNaatiApplicationWithSponsorOnEventInvoiceCreatedToUnTrustedSponsor = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.AllocateTestSessionFromMyNaati).WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.PracticeTest)
                .ThenUseEmailTemplate(289)
                .To(EmailTemplateDetailTypeName.SendToApplicant);

            builders.Add(allocateTestSessionFromMyNaatiApplicationWithSponsorOnEventInvoiceCreatedToUnTrustedSponsor);

            var sponsoredTestInvoiceProcessedOnEventInvoiceCreatedToUntrustedSponsor = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.TestInvoiceProcessed).WithEvent(SystemActionEventTypeName.InvoiceCreatedToUntrustedSponsor)
                .IsExecutedOnApplicationTypes(CredentialApplicationTypeName.PracticeTest)
                .ThenUseEmailTemplate(290)
                .To(EmailTemplateDetailTypeName.SendToSponsor, EmailTemplateDetailTypeName.AttachInoviceToSponsor);

            builders.Add(sponsoredTestInvoiceProcessedOnEventInvoiceCreatedToUntrustedSponsor);

            return builders;
        }
    }
}