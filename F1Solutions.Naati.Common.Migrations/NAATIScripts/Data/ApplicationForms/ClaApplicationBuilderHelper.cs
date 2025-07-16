using System.Linq;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms.Base;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms
{
    public class ClaApplicationBuilderHelper : IFormBuilderHelper
    {
        private readonly SharedBuilders _sharedBuilders;

        public ClaApplicationBuilderHelper(SharedBuilders sharedBuilder)
        {
            _sharedBuilders = sharedBuilder;
        }

        public void CreateForm()
        {
            var userType = _sharedBuilders.UserTypes.First(x => x.Name == "LoggedInUser");
            var application = userType.AddForm(9, "Community Language Aide (CLA)", "Community Language Aide (CLA)", false, "cla");

            Sections(application);
        }

        private void Sections(IApplicationFormBuilder application)
        {

            var yourDetailsSectionSection = YourDetailsSection(application);
            var question1 = Question1(yourDetailsSectionSection);
            var question2 = Question2(yourDetailsSectionSection);

            var languageSectionSection = LanguageSection(application);
            var question3 = Question3(languageSectionSection);
            var question4 = Question4(languageSectionSection);

            var testLocationSectionSection = TestLocationSection(application);
            var question5 = Question5(testLocationSectionSection);

            var sponsorSectionSection = SponsorSection(application);
            var question6 = Question6(sponsorSectionSection);
            var question7 = Question7(sponsorSectionSection);
            var question8 = Question8(sponsorSectionSection);
            var question9 = Question9(sponsorSectionSection);

            var attachmentsSectionSection = AttachmentsSection(application);
            var question10 = Question10(attachmentsSectionSection);
            var question11 = Question11(attachmentsSectionSection);
            var question12 = Question12(attachmentsSectionSection);

            var option6A = question6.Options.ElementAt(0);

            question7.ShowOnlyIf()
                .Option(option6A).IsSelected();

            question8.ShowOnlyIf()
                .Option(option6A).IsSelected();

            question9.ShowOnlyIf()
                .Option(option6A).IsSelected();


        }

        private IFormSectionBuilder YourDetailsSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Your Details", string.Empty);
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
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Why are you taking this test?", @"");
            var option3A = Option3A(questionType);
            var option3B = Option3B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_Applications_URL]]/certification");
            var option3C = Option3C(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_Applications_URL]]/ccl");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option3A).DoNotStoreAnswer();
            question.WhenSelectedOption(option3B).DoNotStoreAnswer();
            question.WhenSelectedOption(option3C).DoNotStoreAnswer();

            return question;
        }
        private IFormQuestionBuilder Question4(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "LanguageSelector");
            var questionType = answerType.CreateQuestionType(@"What Language Other Than English (LOTE) are you seeking to sit the test in?", @"If your language is not listed, please <a href=""[[ExternalUrl_NAATI_URL]]/contact/"" target=""_blank"">contact us</a>.");

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
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Will a third-party organisation be <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">sponsoring</a> your application?", @"");
            var option6A = Option6A(questionType).RequestDocumentWhenSelected(9).RequestDocumentWhenSelected(24);
            var option6B = Option6B(questionType).RequestDocumentWhenSelected(9);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option6A).Store("True").OnField(65);
            question.WhenSelectedOption(option6B).Store("False").OnField(65);

            return question;
        }
        private IFormQuestionBuilder Question7(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter the name of the sponsoring organisation.", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(66);

            return question;
        }
        private IFormQuestionBuilder Question8(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter the name of a contact person from the sponsoring organisation.", @"Please note this contact person will be sent the invoice.");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(67);

            return question;
        }
        private IFormQuestionBuilder Question9(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Email");
            var questionType = answerType.CreateQuestionType(@"Please enter the email address of the contact person from the sponsoring organisation.", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(68);

            return question;
        }
        private IFormQuestionBuilder Question10(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "PersonPhoto");
            var questionType = answerType.CreateQuestionType(@"If you have not already done so, please upload an <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">Australian passport size</a> photo.", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question11(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "DocumentUpload");
            var questionType = answerType.CreateQuestionType(@"", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question12(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you agree to the <a href=""[[ExternalUrl_NAATI_URL]]/policies/terms-and-conditions/"" target=""_blank"">Terms and Conditions</a>?", @"");
            var option12A = Option12A(questionType);
            var option12B = Option12B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option12A).DoNotStoreAnswer();
            question.WhenSelectedOption(option12B).DoNotStoreAnswer();

            return question;
        }


        private IFormAnswerOptionBuilder Option3A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"To be eligible to claim a language allowance", @"");
        }
        private IFormAnswerOptionBuilder Option3B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"To work as a translator or interpreter", @"If you wish to work as a translator or interpreter, you will need to apply for certification instead. Click <b>Finish</b> to be redirected to the correct application.");
        }
        private IFormAnswerOptionBuilder Option3C(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"To obtain Credentialed Community Language Points", @"If you wish to obtain 5 points for immigration purposes, you will need to apply for a CCL test instead. Click <b>Finish</b> to be redirected to the correct application.");
        }
        private IFormAnswerOptionBuilder Option6A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach a Purchase Order from your employer or another document from your employer approving this transaction.");
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
            return questionType.AddOption(@"No", @" Click <b>Finish</b>  to terminate the application process.  Data associated with this application will be deleted.");
        }


    }
}
