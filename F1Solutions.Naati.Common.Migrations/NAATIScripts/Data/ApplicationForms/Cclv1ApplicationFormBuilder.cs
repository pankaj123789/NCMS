using System.Linq;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms.Base;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms
{
    public class Cclv1ApplicationFormBuilder : IFormBuilderHelper
    {
        private readonly SharedBuilders _sharedBuilders;

        public Cclv1ApplicationFormBuilder(SharedBuilders sharedBuilder)
        {
            _sharedBuilders = sharedBuilder;
        }

        public void CreateForm()
        {
            var userType = _sharedBuilders.UserTypes.First(x => x.Name == "NonPractitionerUser");
            var application = userType.AddForm(3, "Credentialed Community Language (CCL) Testing", "CCL Application", true, "cclv1");

            Sections(application);
        }

        private void Sections(IApplicationFormBuilder application)
        {

            var yourDetailsSectionSection = YourDetailsSection(application);
            var question1 = Question1(yourDetailsSectionSection);
            var question2 = Question2(yourDetailsSectionSection);

            var applicationSelectionSectionSection = ApplicationSelectionSection(application);
            var question3 = Question3(applicationSelectionSectionSection);

            var languageSectionSection = LanguageSection(application);
            var question4 = Question4(languageSectionSection);

            var testLocationSectionSection = TestLocationSection(application);
            var question5 = Question5(testLocationSectionSection);
            var question6 = Question6(testLocationSectionSection);

            var sponsorSectionSection = SponsorSection(application);
            var question7 = Question7(sponsorSectionSection);
            var question8 = Question8(sponsorSectionSection);
            var question9 = Question9(sponsorSectionSection);
            var question10 = Question10(sponsorSectionSection);

            var attachmentsSectionSection = AttachmentsSection(application);
            var question11 = Question11(attachmentsSectionSection);
            var question12 = Question12(attachmentsSectionSection);
            var question13 = Question13(attachmentsSectionSection);

            var option7A = question7.Options.ElementAt(0);

            question8.ShowOnlyIf()
                .Option(option7A).IsSelected();

            question9.ShowOnlyIf()
                .Option(option7A).IsSelected();

            question10.ShowOnlyIf()
                .Option(option7A).IsSelected();


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
        private IFormSectionBuilder LanguageSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Language", string.Empty);
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
            var questionType = answerType.CreateQuestionType(@"", @"You will need to upload a copy of an identification document (such as a passport or Australian driver''s licence) at the end of this form.  Please <a href=""[[ExternalUrl_NAATI_URL]]/resources/forms-fees/preparing-application-evidence/"" target=""_blank"">click here</a> to see a list of other identity evidence we will accept.");

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
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Why are you taking this test?", @"");
            var option3A = Option3A(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Redirect"), @"[[ExternalUrl_Applications_URL]]/certification");
            var option3B = Option3B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Redirect"), @"[[ExternalUrl_Applications_URL]]/certification");
            var option3C = Option3C(questionType).RequestDocumentWhenSelected(9);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option3A).DoNotStoreAnswer();
            question.WhenSelectedOption(option3B).DoNotStoreAnswer();
            question.WhenSelectedOption(option3C).Store("True").OnField(32);

            return question;
        }
        private IFormQuestionBuilder Question4(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "LanguageSelector");
            var questionType = answerType.CreateQuestionType(@"What Language Other Than English (LOTE) are you seeking to sit the test in?", @"If your language is not listed, please <a href=""[[ExternalUrl_NAATI_URL]]/get-in-touch/get-in-touch/"" target=""_blank"">contact us</a>.");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question5(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "TestLocation");
            var questionType = answerType.CreateQuestionType(@"Please select your preferred test location.", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question6(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "TestSessions");
            var questionType = answerType.CreateQuestionType(@"Below is a list of indicative Test Sessions.", @"You will be able to select the Test Session after your application has been submitted and checked by NAATI.");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question7(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Will a third-party organisation be <a href=""[[ExternalUrl_NAATI_URL]]/resources/forms-fees/preparing-application-evidence/"" target=""_blank"">sponsoring</a> your application?", @"");
            var option7A = Option7A(questionType).RequestDocumentWhenSelected(24);
            var option7B = Option7B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option7A).Store("True").OnField(33);
            question.WhenSelectedOption(option7B).Store("False").OnField(33);

            return question;
        }
        private IFormQuestionBuilder Question8(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter the name of the sponsoring organisation.", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(34);

            return question;
        }
        private IFormQuestionBuilder Question9(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter the name of a contact person from the sponsoring organisation.", @"Please note this contact person will be sent the invoice.");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(35);

            return question;
        }
        private IFormQuestionBuilder Question10(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Email");
            var questionType = answerType.CreateQuestionType(@"Please enter the email address of the contact person from the sponsoring organisation.", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(36);

            return question;
        }
        private IFormQuestionBuilder Question11(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "PersonPhoto");
            var questionType = answerType.CreateQuestionType(@"If you have not already done so, please upload an <a href=""[[ExternalUrl_NAATI_URL]]/resources/forms-fees/preparing-application-evidence/"" target=""_blank"">Australian passport size</a> photo.", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question12(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "DocumentUpload");
            var questionType = answerType.CreateQuestionType(@"", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question13(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you agree to the following <a href=""[[ExternalUrl_NAATI_URL]]/resources/our-policies/terms-and-conditions"" target=""_blank"">Terms and Conditions</a>?", @"");
            var option13A = Option13A(questionType);
            var option13B = Option13B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option13A).DoNotStoreAnswer();
            question.WhenSelectedOption(option13B).DoNotStoreAnswer();

            return question;
        }


        private IFormAnswerOptionBuilder Option3A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"To work as a translator or interpreter", @"If you wish to work as a translator or interpreter, you will need to apply for certification instead.  Click <b>Finish</b> to be redirected to the correct application.");
        }
        private IFormAnswerOptionBuilder Option3B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"To obtain a skills assessment for migration purposes", @"To obtain a Skill Assessment, you will need to sit a Certification test.  Click <b>Finish</b> to be redirected to the correct application.");
        }
        private IFormAnswerOptionBuilder Option3C(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"To obtain Credentialed Community Language Points", @"");
        }
        private IFormAnswerOptionBuilder Option7A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach a Purchase Order from your employer or another document from your employer approving this transaction.");
        }
        private IFormAnswerOptionBuilder Option7B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option13A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"<strong>Cancellation Policy:<strong/> If you cancel your test with NAATI, you will be charged a cancellation fee.<br />
•	If a cancellation request is received more than 5 weeks before the test date, the cancellation fee is 25% of the test fee. <br />
•	If a cancellation request is received within five weeks of the test date (35 days or fewer), the cancellation fee is 100% of the test fee. There is no refund.<br /> 
•	Non-attendance on the test date is considered a cancellation and no test fees will be refunded.<br />
To read NAATI''s full cancellation policy, please visit the <a href=""[[ExternalUrl_NAATI_URL]]/resources/our-policies/terms-and-conditions/"" target=""_blank"">terms and conditions</a> on our website.");
        }
        private IFormAnswerOptionBuilder Option13B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @" Click <b>Finish</b>  to terminate the application process.  Data associated with this application will be deleted.");
        }


    }
}
