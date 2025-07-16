using System.Linq;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms.Base;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms
{
    public class CertificationApplicationFormBuilder : IFormBuilderHelper
    {
        private readonly SharedBuilders _sharedBuilders;

        public CertificationApplicationFormBuilder(SharedBuilders sharedBuilder)
        {
            _sharedBuilders = sharedBuilder;
        }

        public void CreateForm()
        {
            var userType = _sharedBuilders.UserTypes.First(x => x.Name == "LoggedInUser");
            var application = userType.AddForm(2, "Certification & Recognised Practising", "Certification Application", false, "certification");
            application.DisplayOnlyCredentialTypes(1, 2, 5, 6, 7, 12, 13, 19, 20, 21, 26, 27);
            Sections(application);
        }

        private void Sections(IApplicationFormBuilder application)
        {

            var yourDetailsSectionSection = YourDetailsSection(application);
            var question1 = Question1(yourDetailsSectionSection);
            var question2 = Question2(yourDetailsSectionSection);

            var applicationSelectionSectionSection = ApplicationSelectionSection(application);
            var question3 = Question3(applicationSelectionSectionSection);
            var question4 = Question4(applicationSelectionSectionSection);

            var credentialsSectionSection = CredentialsSection(application);
            var question5 = Question5(credentialsSectionSection);

            var additionalInformationSection = AdditionalInformationSection(application);
            var question5One = Question5One(additionalInformationSection);

            var endorsedQualificationSectionSection = EndorsedQualificationSection(application);
            var question6 = Question6(endorsedQualificationSectionSection);
            var question7_1 = Question7_1(endorsedQualificationSectionSection);
            var question7_2 = Question7_2(endorsedQualificationSectionSection);
            var question7_3 = Question7_3(endorsedQualificationSectionSection);
            var question9 = Question9(endorsedQualificationSectionSection);
            var question10 = Question10(endorsedQualificationSectionSection);
            var question11 = Question11(endorsedQualificationSectionSection);
            var question12 = Question12(endorsedQualificationSectionSection);
            var question13 = Question13(endorsedQualificationSectionSection);

            var membershipsSectionSection = MembershipsSection(application);
            var question14 = Question14(membershipsSectionSection);
            var question15 = Question15(membershipsSectionSection);

            var nonEndorsedQualificationSectionSection = NonEndorsedQualificationSection(application);
            var question16 = Question16(nonEndorsedQualificationSectionSection);
            var question17 = Question17(nonEndorsedQualificationSectionSection);
            var question18 = Question18(nonEndorsedQualificationSectionSection);
            var question19 = Question19(nonEndorsedQualificationSectionSection);
            var question20 = Question20(nonEndorsedQualificationSectionSection);
            var question21 = Question21(nonEndorsedQualificationSectionSection);
            var question22 = Question22(nonEndorsedQualificationSectionSection);
            var question23 = Question23(nonEndorsedQualificationSectionSection);

            var prerequisitesSectionSection = PrerequisitesSection(application);
            var question24 = Question24(prerequisitesSectionSection);
            var question25 = Question25(prerequisitesSectionSection);
            var question26 = Question26(prerequisitesSectionSection);
            var question27 = Question27(prerequisitesSectionSection);
            var question28 = Question28(prerequisitesSectionSection);
            var question29 = Question29(prerequisitesSectionSection);

            var testLocationSectionSection = TestLocationSection(application);
            var question30 = Question30(testLocationSectionSection);

            var sponsorSectionSection = SponsorSection(application);
            var question31 = Question31(sponsorSectionSection);
            var question32 = Question32(sponsorSectionSection);
            var question33 = Question33(sponsorSectionSection);
            var question34 = Question34(sponsorSectionSection);

            var attachmentsSectionSection = AttachmentsSection(application);
            var question35 = Question35(attachmentsSectionSection);
            var question36 = Question36(attachmentsSectionSection);
            var question37 = Question37(attachmentsSectionSection);

            var option3C = question3.Options.ElementAt(2);
            var option6A = question6.Options.ElementAt(0);
            var option6B = question6.Options.ElementAt(1);
            var option13B = question13.Options.ElementAt(1);
            var option14A = question14.Options.ElementAt(0);
            var option14B = question14.Options.ElementAt(1);
            var option15A = question15.Options.ElementAt(0);
            var option15B = question15.Options.ElementAt(1);
            var option12B = question12.Options.ElementAt(1);
            var option16A = question16.Options.ElementAt(0);
            var option17A = question17.Options.ElementAt(0);
            var option18A = question18.Options.ElementAt(0);
            var option19A = question19.Options.ElementAt(0);
            var option16B = question16.Options.ElementAt(1);
            var option12A = question12.Options.ElementAt(0);
            var option31A = question31.Options.ElementAt(0);

            question4.ShowOnlyIf()
                .Option(option3C).IsSelected();

            //this actually means that if English to Chinese is selected 
            //or Chinese to English then show an additional question
            //asking to seklect Simplified or Traditional

            question5One.ShowOnlyIf()
                .Skill(607).IsSelected()
                .Or()
                .Skill(608).IsSelected();

            question7_1.ShowOnlyIf()
                .Option(option6A).IsSelected();

            question7_2.ShowOnlyIf()
                .Option(option6A).IsSelected();

            question7_3.ShowOnlyIf()
                .Option(option6A).IsSelected();

            question9.ShowOnlyIf()
                .Option(option6A).IsSelected();

            question10.ShowOnlyIf()
                .Option(option6A).IsSelected();

            question11.ShowOnlyIf()
                .Option(option6A).IsSelected();

            question12.ShowOnlyIf()
                .Option(option6A).IsSelected();

            question13.ShowOnlyIf()
                .Option(option6B).IsSelected();

            question14.ShowOnlyIf()
                .Option(option13B).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(3).IsSelected()
                .Or()
                .CredentialType(4).IsSelected();

            question15.ShowOnlyIf()
                .Option(option13B).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(10).IsSelected()
                .Or()
                .CredentialType(11).IsSelected()
                .Or()
                .CredentialType(24).IsSelected();

            question16.ShowOnlyIf()
                .Option(option13B).IsSelected()
                .And()
                .Option(option14A).IsNotSelected()
                .And()
                .CredentialType(21).IsNotSelected()
                .And()
                .Option(option14B).IsNotSelected()
                .And()
                .Option(option15A).IsNotSelected()
                .And()
                .Option(option15B).IsNotSelected()
                .And()
                .CredentialType(8).IsNotSelected()
                .And()
                .CredentialType(9).IsNotSelected()
                .And()
                .CredentialType(22).IsNotSelected()
                .And()
                .CredentialType(23).IsNotSelected()
                .And()
                .CredentialType(7).IsNotSelected();

            question17.ShowOnlyIf()
                .Option(option14B).IsSelected()
                .Or()
                .Option(option15B).IsSelected();

            question18.ShowOnlyIf()
                .Option(option12B).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(8).IsSelected()
                .Or()
                .CredentialType(9).IsSelected()
                .Or()
                .CredentialType(22).IsSelected()
                .Or()
                .CredentialType(23).IsSelected();

            question19.ShowOnlyIf()
                .Option(option13B).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(7).IsSelected()
                .Or()
                .CredentialType(21).IsSelected();

            question20.ShowOnlyIf()
                .Option(option16A).IsSelected()
                .Or()
                .Option(option17A).IsSelected()
                .Or()
                .Option(option18A).IsSelected()
                .Or()
                .Option(option19A).IsSelected();

            question21.ShowOnlyIf()
                .Option(option16A).IsSelected()
                .Or()
                .Option(option17A).IsSelected()
                .Or()
                .Option(option18A).IsSelected()
                .Or()
                .Option(option19A).IsSelected();

            question22.ShowOnlyIf()
                .Option(option16A).IsSelected()
                .Or()
                .Option(option17A).IsSelected()
                .Or()
                .Option(option18A).IsSelected()
                .Or()
                .Option(option19A).IsSelected();

            question23.ShowOnlyIf()
                .Option(option16A).IsSelected()
                .Or()
                .Option(option17A).IsSelected()
                .Or()
                .Option(option18A).IsSelected()
                .Or()
                .Option(option19A).IsSelected();

            question24.ShowOnlyIf()
                .Option(option16B).IsSelected();

            question25.ShowOnlyIf()
                .Option(option6B).IsSelected()
                .Or()
                .Option(option12A).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(12).IsSelected()
                .Or()
                .CredentialType(13).IsSelected();

            question26.ShowOnlyIf()
                .Option(option6B).IsSelected()
                .Or()
                .Option(option12A).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(1).IsSelected()
                .Or()
                .CredentialType(2).IsSelected()
                .Or()
                .CredentialType(5).IsSelected()
                .Or()
                .CredentialType(6).IsSelected()
                .Or()
                .CredentialType(7).IsSelected()
                .Or()
                .CredentialType(19).IsSelected()
                .Or()
                .CredentialType(20).IsSelected()
                .Or()
                .CredentialType(21).IsSelected()
                .Or()
                .CredentialType(26).IsSelected()
                .Or()
                .CredentialType(27).IsSelected();

            question27.ShowOnlyIf()
                .Option(option6B).IsSelected()
                .Or()
                .Option(option12A).IsSelected();

            question28.ShowOnlyIf()
                .Option(option6B).IsSelected()
                .Or()
                .Option(option12A).IsSelected()
                .AndGroupFormedBy()
                .Option(option14A).IsNotSelected()
                .And()
                .Option(option15A).IsNotSelected();

            question29.ShowOnlyIf()
                .CredentialType(1).IsSelected()
                .Or()
                .CredentialType(5).IsSelected()
                .Or()
                .CredentialType(12).IsSelected()
                .Or()
                .CredentialType(19).IsSelected()
                .Or()
                .CredentialType(25).IsSelected();

            question32.ShowOnlyIf()
                .Option(option31A).IsSelected();

            question33.ShowOnlyIf()
                .Option(option31A).IsSelected();

            question34.ShowOnlyIf()
                .Option(option31A).IsSelected();


        }

        private IFormSectionBuilder YourDetailsSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Your Details", string.Empty);
            return section;
        }
        private IFormSectionBuilder ApplicationSelectionSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Application Selection", string.Empty);
            return section;
        }
        private IFormSectionBuilder CredentialsSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Credentials", string.Empty);
            return section;
        }
        private IFormSectionBuilder AdditionalInformationSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Additional Information", string.Empty);
            return section;
        }
        private IFormSectionBuilder EndorsedQualificationSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Endorsed Qualification", string.Empty);
            return section;
        }
        private IFormSectionBuilder MembershipsSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Memberships", string.Empty);
            return section;
        }
        private IFormSectionBuilder NonEndorsedQualificationSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Non-Endorsed Qualification", string.Empty);
            return section;
        }
        private IFormSectionBuilder PrerequisitesSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Prerequisites", string.Empty);
            return section;
        }
        private IFormSectionBuilder TestLocationSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Test Location", string.Empty);
            return section;
        }
        private IFormSectionBuilder SponsorSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Sponsor", string.Empty);
            return section;
        }
        private IFormSectionBuilder AttachmentsSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Attachments", string.Empty);
            return section;
        }

        private IFormQuestionBuilder Question1(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "PersonVerification");
            var questionType = answerType.CreateQuestionType(@"", @"You will need to upload a copy of an identification document (such as a passport or Australian driver''s licence) at the end of this form.  Please <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">click here</a> to see a list of other identity evidence we will accept.");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question2(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "PersonDetails");
            var questionType = answerType.CreateQuestionType(@"", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question3(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "CheckOptions");
            var questionType = answerType.CreateQuestionType(@"Why are you seeking a NAATI credential?", @"You may select more than one option.");
            var option3A = Option3A(questionType).RequestDocumentWhenSelected(9);
            var option3B = Option3B(questionType).RequestDocumentWhenSelected(9);
            var option3C = Option3C(questionType).RequestDocumentWhenSelected(9);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option3A).Store("True").OnField(18);
            question.WhenSelectedOption(option3B).Store("True").OnField(19);
            question.WhenSelectedOption(option3C).Store("True").OnField(20);

            return question;
        }
        private IFormQuestionBuilder Question4(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you wish to take a CCL Test (without a Certification)?", @"");
            var option4A = Option4A(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_Applications_URL]]/ccl");
            var option4B = Option4B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option4A).DoNotStoreAnswer();
            question.WhenSelectedOption(option4B).DoNotStoreAnswer();

            return question;
        }
        private IFormQuestionBuilder Question5(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "CredentialSelector");
            var questionType = answerType.CreateQuestionType(@"Please select the Credentials you are seeking.", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }

        private IFormQuestionBuilder Question5One(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Please specify your preferred form of written Chinese for this test.",@"");


            var option5OneA = Option5OneA(questionType);
            var option5OneB = Option5OneB(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option5OneA).StoreOption(4).OnField(120);
            question.WhenSelectedOption(option5OneB).StoreOption(5).OnField(120);
            return question;
        }

        
        private IFormQuestionBuilder Question6(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Have you completed (or do you expect to complete) a <a href=""[[ExternalUrl_NAATI_URL]]/services/endorsed-qualification/"" target=""_blank"">NAATI Endorsed Qualification</a> in translating or interpreting?", @"");
            var option6A = Option6A(questionType).RequestDocumentWhenSelected(41);
            var option6B = Option6B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option6A).Store("True").OnField(21);
            question.WhenSelectedOption(option6B).Store("False").OnField(21);

            return question;
        }

        private IFormQuestionBuilder Question7_1(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "EndorsedQualificationInstitution");
            var questionType = answerType.CreateQuestionType(@"Please enter the Institution", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question7_2(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "EndorsedQualificationLocation");
            var questionType = answerType.CreateQuestionType(@"Please enter the Location", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question7_3(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "EndorsedQualification");
            var questionType = answerType.CreateQuestionType(@"Please enter the qualification name", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(117);

            return question;
        }
        private IFormQuestionBuilder Question9(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter you Student ID", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(26);

            return question;
        }
        private IFormQuestionBuilder Question10(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Date");
            var questionType = answerType.CreateQuestionType(@"Please enter the date you started the qualification", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(27);

            return question;
        }
        private IFormQuestionBuilder Question11(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Date");
            var questionType = answerType.CreateQuestionType(@"Please enter the date you completed (or intend to complete) the qualification", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(28);

            return question;
        }
        private IFormQuestionBuilder Question12(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Has it been more than 3 years since you completed the qualification?", @"");
            var option12A = Option12A(questionType);
            var option12B = Option12B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option12A).Store("True").OnField(29);
            question.WhenSelectedOption(option12B).Store("False").OnField(29);

            return question;
        }
        private IFormQuestionBuilder Question13(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Have you completed a NAATI Approved Course? ", @"");
            var option13A = Option13A(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/services/endorsed-qualification/");
            var option13B = Option13B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option13A).Store("True").OnField(23);
            question.WhenSelectedOption(option13B).Store("False").OnField(23);

            return question;
        }
        private IFormQuestionBuilder Question14(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you hold a membership with AITC?", @"");
            var option14A = Option14A(questionType).RequestDocumentWhenSelected(26);
            var option14B = Option14B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option14A).Store("True").OnField(31);
            question.WhenSelectedOption(option14B).Store("False").OnField(31);

            return question;
        }
        private IFormQuestionBuilder Question15(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you hold a membership with AIIC?", @"");
            var option15A = Option15A(questionType).RequestDocumentWhenSelected(25);
            var option15B = Option15B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option15A).Store("True").OnField(30);
            question.WhenSelectedOption(option15B).Store("False").OnField(30);

            return question;
        }
        private IFormQuestionBuilder Question16(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Have you completed a non-NAATI endorsed translating or interpreting qualification (at the <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">appropriate level</a>)?", @"");
            var option16A = Option16A(questionType).RequestDocumentWhenSelected(11);
            var option16B = Option16B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option16A).Store("True").OnField(22);
            question.WhenSelectedOption(option16B).Store("False").OnField(22);

            return question;
        }
        private IFormQuestionBuilder Question17(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Have you completed a non-NAATI endorsed translating or interpreting qualification (at the <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">appropriate level</a>)?", @"");
            var option17A = Option17A(questionType).RequestDocumentWhenSelected(11);
            var option17B = Option17B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option17A).Store("True").OnField(47);
            question.WhenSelectedOption(option17B).Store("False").OnField(47);

            return question;
        }
        private IFormQuestionBuilder Question18(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Have you completed a non-NAATI endorsed translating or interpreting qualification (with specialised units and at the <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">appropriate level</a>)?", @"");
            var option18A = Option18A(questionType).RequestDocumentWhenSelected(11);
            var option18B = Option18B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option18A).Store("True").OnField(47);
            question.WhenSelectedOption(option18B).Store("False").OnField(47);

            return question;
        }
        private IFormQuestionBuilder Question19(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Have you completed a non-NAATI endorsed translating or interpreting qualification (at the <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">appropriate level</a>)?", @"");
            var option19A = Option19A(questionType).RequestDocumentWhenSelected(11);
            var option19B = Option19B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option19A).Store("True").OnField(47);
            question.WhenSelectedOption(option19B).Store("False").OnField(47);

            return question;
        }
        private IFormQuestionBuilder Question20(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter the Institution", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(41);

            return question;
        }
        private IFormQuestionBuilder Question21(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter the qualification name", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(42);

            return question;
        }
        private IFormQuestionBuilder Question22(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter you Student ID", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(44);

            return question;
        }
        private IFormQuestionBuilder Question23(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Date");
            var questionType = answerType.CreateQuestionType(@"Please enter the date you completed the qualification", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(43);

            return question;
        }
        private IFormQuestionBuilder Question24(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you meet the translating or interpreting <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">training requirements</a>?", @"");
            var option24A = Option24A(questionType).RequestDocumentWhenSelected(12);
            var option24B = Option24B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option24A).Store("True").OnField(13);
            question.WhenSelectedOption(option24B).Store("False").OnField(13);

            return question;
        }
        private IFormQuestionBuilder Question25(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you meet the <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">Auslan proficiency requirements</a>?", @"");
            var option25A = Option25A(questionType).RequestDocumentWhenSelected(13);
            var option25B = Option25B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option25A).Store("True").OnField(14);
            question.WhenSelectedOption(option25B).Store("False").OnField(14);

            return question;
        }
        private IFormQuestionBuilder Question26(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you meet the <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">English proficiency requirements</a>?", @"");
            var option26A = Option26A(questionType).RequestDocumentWhenSelected(14);
            var option26B = Option26B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option26A).Store("True").OnField(15);
            question.WhenSelectedOption(option26B).Store("False").OnField(15);

            return question;
        }
        private IFormQuestionBuilder Question27(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"<a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">Ethical competency</a> is a prerequisite for certification testing, can you provide evidence to be exempt from testing?", @"");
            var option27A = Option27A(questionType).RequestDocumentWhenSelected(15);
            var option27B = Option27B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option27A).Store("True").OnField(16);
            question.WhenSelectedOption(option27B).Store("False").OnField(16);

            return question;
        }
        private IFormQuestionBuilder Question28(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"<a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">Intercultural competency</a> is a prerequisite for certification testing, can you provide evidence to be exempt from testing?", @"");
            var option28A = Option28A(questionType).RequestDocumentWhenSelected(16);
            var option28B = Option28B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option28A).Store("True").OnField(17);
            question.WhenSelectedOption(option28B).Store("False").OnField(17);

            return question;
        }
        private IFormQuestionBuilder Question29(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"You have applied for a Recognised Practising credential.  Do you have work experience in the language combinations you are applying for?", @"");
            var option29A = Option29A(questionType).RequestDocumentWhenSelected(10);
            var option29B = Option29B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option29A).Store("True").OnField(46);
            question.WhenSelectedOption(option29B).Store("False").OnField(46);

            return question;
        }
        private IFormQuestionBuilder Question30(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "TestLocation");
            var questionType = answerType.CreateQuestionType(@"Please select your preferred test location.", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question31(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Will a third-party organisation be <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">sponsoring</a> your application?", @"");
            var option31A = Option31A(questionType).RequestDocumentWhenSelected(24);
            var option31B = Option31B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option31A).Store("True").OnField(37);
            question.WhenSelectedOption(option31B).Store("False").OnField(37);

            return question;
        }
        private IFormQuestionBuilder Question32(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter the name of the sponsoring organisation.", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(38);

            return question;
        }
        private IFormQuestionBuilder Question33(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter the name of a contact person from the sponsoring organisation.", @"Please note this contact person will be sent the invoice.");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(39);

            return question;
        }
        private IFormQuestionBuilder Question34(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Email");
            var questionType = answerType.CreateQuestionType(@"Please enter the email address of the contact person from the sponsoring organisation.", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(40);

            return question;
        }
        private IFormQuestionBuilder Question35(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "PersonPhoto");
            var questionType = answerType.CreateQuestionType(@"If you have not already done so, please upload an <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">Australian passport size</a> photo.", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question36(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "DocumentUpload");
            var questionType = answerType.CreateQuestionType(@"", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question37(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you agree to the <a href=""[[ExternalUrl_NAATI_URL]]/policies/terms-and-conditions/"" target=""_blank"">Terms and Conditions</a>?", @"");
            var option37A = Option37A(questionType);

            var option37B = Option37B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option37A).DoNotStoreAnswer();
            question.WhenSelectedOption(option37B).DoNotStoreAnswer();

            return question;
        }


        private IFormAnswerOptionBuilder Option3A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"To work as a translator or interpreter", @"");
        }
        private IFormAnswerOptionBuilder Option3B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"To obtain a skills assessment for migration purposes", @"");
        }
        private IFormAnswerOptionBuilder Option3C(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"To obtain Credentialed Community Language Points", @"");
        }
        private IFormAnswerOptionBuilder Option4A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @" Click <b>Finish</b> to be redirected to the correct application form.");
        }
        private IFormAnswerOptionBuilder Option4B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option5OneA(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Simplified Chinese", @"");
        }
        private IFormAnswerOptionBuilder Option5OneB(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Traditional Chinese", @"");
        }
        private IFormAnswerOptionBuilder Option6A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach a copy of your <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">Transcript or Proof of Enrolment</a>, at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option6B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option12A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"");
        }
        private IFormAnswerOptionBuilder Option12B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option13A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"If you have completed a NAATI Approved Course, you will need to submit a B form. Click <b>Finish</b> to be redirected to the correct application form.");
        }
        private IFormAnswerOptionBuilder Option13B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option14A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach <a href=""[[ExternalUrl_NAATI_URL]/become-certified/how-do-i-become-certified/submit-an-application/]"" target=""_blank"">AITC Membership Evidence</a>, at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option14B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option15A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">AIIC Membership Evidence</a>, at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option15B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option16A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach a copy of your <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">transcript</a>, at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option16B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option17A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach a copy of your <a href=""[[ExternalUrl_NAATI_URL]]become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">transcript</a>, at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option17B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you have not completed a non-NAATI Endorsed Advanced Qualification, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option18A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach a copy of your <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">transcript</a>, at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option18B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you have not completed a non-NAATI Endorsed Advanced Qualification, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option19A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach a copy of your <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">transcript</a>, at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option19B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you have not completed a non-NAATI Endorsed Advanced Qualification, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option24A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach evidence of your <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">training</a> or attach a document stating why you should be exempt at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option24B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you cannot provide evidence of training, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option25A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach evidence of your <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">Auslan proficiency</a> or attach a document stating why you should be exempt at the end of this application form");
        }
        private IFormAnswerOptionBuilder Option25B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you cannot provide evidence, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option26A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach evidence of your <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">English proficiency</a> or attach a document stating why you should be exempt at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option26B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you cannot provide evidence, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option27A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">evidence</a> showing why you should be exempt at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option27B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"NAATI will contact you to arrange an Ethical Competency test after you application has been submitted.  Click <b>Next</b> to continue.");
        }
        private IFormAnswerOptionBuilder Option28A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">evidence</a> showing why you should be exempt at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option28B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"NAATI will contact you to arrange an Intercultural Competency test after you application has been submitted.  Click <b>Next</b> to continue.");
        }
        private IFormAnswerOptionBuilder Option29A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">Evidence</a>, at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option29B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you does not have work expirience in this language combination, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option31A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach a Purchase Order from your employer or another document from your employer approving this transaction.");
        }
        private IFormAnswerOptionBuilder Option31B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option37A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"");
        }
        private IFormAnswerOptionBuilder Option37B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @" Click <b>Finish</b>  to terminate the application process.  Data associated with this application will be deleted.");
        }


    }
}
