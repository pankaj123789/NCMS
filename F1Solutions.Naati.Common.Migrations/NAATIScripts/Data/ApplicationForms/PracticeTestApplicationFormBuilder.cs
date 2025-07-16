using System.Linq;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms.Base;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms
{
    public class PracticeTestFormApplicationFormBuilder : IFormBuilderHelper
    {
        private readonly SharedBuilders _sharedBuilders;

        public PracticeTestFormApplicationFormBuilder(SharedBuilders sharedBuilder)
        {
            _sharedBuilders = sharedBuilder;
        }

        public void CreateForm()
        {
            var userType = _sharedBuilders.UserTypes.First(x => x.Name == "LoggedInUser");
            var application = userType.AddForm(17, "Practice Test", "Practice Test", false, "practice");

            Sections(application);
        }

        private void Sections(IApplicationFormBuilder application)
        {

            var yourDetailsSectionSection = YourDetailsSection(application);
            var question1 = Question1(yourDetailsSectionSection);
            var question2 = Question2(yourDetailsSectionSection);

            var credentialsSectionSection = CredentialsSection(application);
            var question3 = Question3(credentialsSectionSection);
            var question4 = Question4(credentialsSectionSection);

            var testLocationSectionSection = TestLocationSection(application);
            var question5 = Question5(testLocationSectionSection);
            var question6 = Question6(testLocationSectionSection);

            var attachmentsSectionSection = AttachmentsSection(application);
            var question7 = Question7(attachmentsSectionSection);
            var question10 = Question10(attachmentsSectionSection);

            //this actually means that if English to Chinese is selected 
            //or Chinese to English then show an additional question
            //asking to select Simplified or Traditional

            question4.ShowOnlyIf()
                .Skill(5063).IsSelected()
                .Or()
                .Skill(5064).IsSelected();
        }

        private IFormSectionBuilder YourDetailsSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Your Details", string.Empty);
            return section;
        }
        private IFormSectionBuilder CredentialsSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Credentials", string.Empty);
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
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "CredentialSelector");
            var questionType = answerType.CreateQuestionType(@"Please select the practice test(s) you would like to sit.", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question4(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Please specify your preferred form of written Chinese for this test.", @"");


            var option4OneA = Option4OneA(questionType);
            var option4OneB = Option4OneB(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option4OneA).StoreOption(4).OnField(123);
            question.WhenSelectedOption(option4OneB).StoreOption(5).OnField(123);
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
            return questionType.AddOption(@"Credential Community Language", @"");
        }
        private IFormAnswerOptionBuilder Option3B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Certified Translator", @"");
        }
        private IFormAnswerOptionBuilder Option3C(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Certified Interpreter", @"");
        }
        private IFormAnswerOptionBuilder Option10A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes <strong>(This is a practice test. It does not lead to credential or certification. You will receive results as an indication of performance only.)<strong>", @"");
        }
        private IFormAnswerOptionBuilder Option10B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @" Click <b>Finish</b>  to terminate the application process.  Data associated with this application will be deleted.");
        }

        private IFormAnswerOptionBuilder Option4OneA(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Simplified Chinese", @"");
        }
        private IFormAnswerOptionBuilder Option4OneB(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Traditional Chinese", @"");
        }
    }
}
