using System.Linq;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms.Base;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms
{
    public class CclApplicationFormBuilder : IFormBuilderHelper
    {
        private readonly SharedBuilders _sharedBuilders;

        public CclApplicationFormBuilder(SharedBuilders sharedBuilder)
        {
            _sharedBuilders = sharedBuilder;
        }

        public void CreateForm()
        {
            var userType = _sharedBuilders.UserTypes.First(x => x.Name == "LoggedInUser");
            var application = userType.AddForm(13, "Credentialed Community Language (CCL) Testing", "Credentialed Community Language (CCL) Testing", false, "ccl");

            Sections(application);
        }

        private void Sections(IApplicationFormBuilder application)
        {

            var yourDetailsSectionSection = YourDetailsSection(application);
            var question1 = Question1(yourDetailsSectionSection);
            var question2 = Question2(yourDetailsSectionSection);
            var question3 = Question3(yourDetailsSectionSection);

            var languageSectionSection = LanguageSection(application);
            var question4 = Question4(languageSectionSection);

            var testLocationSectionSection = TestLocationSection(application);
            var question5 = Question5(testLocationSectionSection);
            var question6 = Question6(testLocationSectionSection);

            var attachmentsSectionSection = AttachmentsSection(application);
            var question7 = Question7(attachmentsSectionSection);
            var question8 = Question8(attachmentsSectionSection);
            var question10 = Question10(attachmentsSectionSection);


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
            var option3A = Option3A(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Redirect"), @"[[ExternalUrl_Applications_URL]]/certification");
            var option3B = Option3B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Redirect"), @"[[ExternalUrl_Applications_URL]]/certification");
            var option3C = Option3C(questionType).RequestDocumentWhenSelected(9);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option3A).DoNotStoreAnswer();
            question.WhenSelectedOption(option3B).DoNotStoreAnswer();
            question.WhenSelectedOption(option3C).Store("True").OnField(107);

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
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "TestSessions");
            var questionType = answerType.CreateQuestionType(@"Below is a list of indicative Test Sessions.", @"You will only be able to select the Test Session after your application has been assessed by NAATI.");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question7(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "PersonPhoto");
            var questionType = answerType.CreateQuestionType(@"If you have not already done so, please upload an <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">Australian passport size</a> photo.", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question8(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "DocumentUpload");
            var questionType = answerType.CreateQuestionType(@"", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }

        private IFormQuestionBuilder Question10(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you agree to the <a href=""[[ExternalUrl_NAATI_URL]]/policies/terms-and-conditions/"" target=""_blank"">Terms and Conditions</a>?", @"");
            var option10A = Option10A(questionType);
            var option10B = Option10B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option10A).DoNotStoreAnswer();
            question.WhenSelectedOption(option10B).DoNotStoreAnswer();

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
        private IFormAnswerOptionBuilder Option10A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"");
        }
        private IFormAnswerOptionBuilder Option10B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @" Click <b>Finish</b>  to terminate the application process.  Data associated with this application will be deleted.");
        }


    }
}
