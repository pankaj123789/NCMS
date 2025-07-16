using System.Linq;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms.Base;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms
{
    public class RecertificationApplicationFormBuilder : IFormBuilderHelper
    {
        private readonly SharedBuilders _sharedBuilders;

        public RecertificationApplicationFormBuilder(SharedBuilders sharedBuilder)
        {
            _sharedBuilders = sharedBuilder;
        }

        public void CreateForm()
        {
            var userType = _sharedBuilders.UserTypes.First(x => x.Name == "RecertificationUser");
            var application = userType.AddForm(8, "Recertification", "Recertification", false, "recertification");

            Sections(application);
        }

        private void Sections(IApplicationFormBuilder application)
        {

            var yourDetailsSectionSection = YourDetailsSection(application);
            var question1 = Question1(yourDetailsSectionSection);
            var question2 = Question2(yourDetailsSectionSection);

            var credentialsSectionSection = CredentialsSection(application);
            var question3 = Question3(credentialsSectionSection);

            var pDandWorkPracticeSectionSection = PDandWorkPracticeSection(application);
            var question4 = Question4(pDandWorkPracticeSectionSection);
            var question5 = Question5(pDandWorkPracticeSectionSection);
            var question6 = Question6(pDandWorkPracticeSectionSection);

            var productsSectionSection = ProductsSection(application);
            var question7 = Question7(productsSectionSection);
            var question8 = Question8(productsSectionSection);

            var sponsorSectionSection = SponsorSection(application);
            var question9 = Question9(sponsorSectionSection);
            var question10 = Question10(sponsorSectionSection);
            var question11 = Question11(sponsorSectionSection);
            var question12 = Question12(sponsorSectionSection);

            var attachmentsSectionSection = AttachmentsSection(application);
            var question13 = Question13(attachmentsSectionSection);
            var question14 = Question14(attachmentsSectionSection);

            var option9A = question9.Options.ElementAt(0);

            question5.ShowOnlyIf()
                .PdPointsMet().IsNotSelected();

            question6.ShowOnlyIf()
                .WorkPracticeMet().IsNotSelected();

            question7.ShowOnlyIf()
                .CredentialType(5).IsSelected()
                .Or()
                .CredentialType(6).IsSelected()
                .Or()
                .CredentialType(20).IsSelected()
                .Or()
                .CredentialType(21).IsSelected()
                .Or()
                .CredentialType(22).IsSelected()
                .Or()
                .CredentialType(23).IsSelected()
                .Or()
                .CredentialType(24).IsSelected()
                .Or()
                .CredentialType(25).IsSelected()
                .Or()
                .CredentialType(26).IsSelected()
                .Or()
                .CredentialType(27).IsSelected()
                .Or()
                .CredentialType(7).IsSelected()
                .Or()
                .CredentialType(8).IsSelected()
                .Or()
                .CredentialType(9).IsSelected()
                .Or()
                .CredentialType(10).IsSelected()
                .Or()
                .CredentialType(11).IsSelected()
                .Or()
                .CredentialType(12).IsSelected()
                .Or()
                .CredentialType(13).IsSelected()
                .Or()
                .CredentialType(19).IsSelected();

            question8.ShowOnlyIf()
                .CredentialType(1).IsSelected()
                .Or()
                .CredentialType(2).IsSelected()
                .Or()
                .CredentialType(3).IsSelected()
                .Or()
                .CredentialType(4).IsSelected()
                .Or()
                .CredentialType(28).IsSelected();

            question10.ShowOnlyIf()
                .Option(option9A).IsSelected();

            question11.ShowOnlyIf()
                .Option(option9A).IsSelected();

            question12.ShowOnlyIf()
                .Option(option9A).IsSelected();


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
        private IFormSectionBuilder PDandWorkPracticeSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"PD and Work Practice", string.Empty);
            return section;
        }
        private IFormSectionBuilder ProductsSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Products", string.Empty);
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
            var questionType = answerType.CreateQuestionType(@"", @"");

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
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RecertificationCredentialSelector");
            var questionType = answerType.CreateQuestionType(@"List of Credentials to submit for recertification", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question4(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Please note that all LogBook activities on myNAATI which match the criteria for this recertification application will be submitted with this application.", @"Go to the LogBook if you wish to modify any of the activities prior to submission.  You will not be able to modify these activities once the recertification application is submitted. [[Pd Points Summary]]");
            var option4A = Option4A(questionType);
            var option4B = Option4B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/Logbook#dashboard");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option4A).DoNotStoreAnswer();
            question.WhenSelectedOption(option4B).DoNotStoreAnswer();

            return question;
        }
        private IFormQuestionBuilder Question5(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"It appears you have not met all of the Professional Development requirements in your LogBook for recertification.  Please attach evidence to support why you should be exempt from the Professional Development requirements.", @"");
            var option5A = Option5A(questionType).RequestDocumentWhenSelected(28);
            var option5B = Option5B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/practitioners/recertification/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option5A).Store("True").OnField(61);
            question.WhenSelectedOption(option5B).Store("False").OnField(61);

            return question;
        }
        private IFormQuestionBuilder Question6(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"It appears you do not have enough Work Practice evidence in your LogBook for recertification.  Please attach evidence to support why you should be exempt from the work practice requirements.", @"");
            var option6A = Option6A(questionType).RequestDocumentWhenSelected(27);
            var option6B = Option6B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/practitioners/recertification/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option6A).Store("True").OnField(62);
            question.WhenSelectedOption(option6B).Store("False").OnField(62);

            return question;
        }
        private IFormQuestionBuilder Question7(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Practitioners who are granted recertification are entitled to an ID card (free of charge).  Upon recertification would you like to to receive the ID Card?", @"");
            var option7A = Option7A(questionType);
            var option7B = Option7B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option7A).Store("True").OnField(63);
            question.WhenSelectedOption(option7B).Store("False").OnField(63);

            return question;
        }
        private IFormQuestionBuilder Question8(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Practitioners who are granted recertification are entitled to a stamp (free of charge).  Upon recertification would you like to to receive the stamp?", @"");
            var option8A = Option8A(questionType);
            var option8B = Option8B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option8A).Store("True").OnField(64);
            question.WhenSelectedOption(option8B).Store("False").OnField(64);

            return question;
        }
        private IFormQuestionBuilder Question9(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Will a third-party organisation be <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">sponsoring</a> your application?", @"You will need to attach a Purchase Order from your employer or another document from your employer approving this transaction.");
            var option9A = Option9A(questionType).RequestDocumentWhenSelected(24);
            var option9B = Option9B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option9A).Store("True").OnField(69);
            question.WhenSelectedOption(option9B).Store("False").OnField(69);

            return question;
        }
        private IFormQuestionBuilder Question10(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter the name of the sponsoring organisation.", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(70);

            return question;
        }
        private IFormQuestionBuilder Question11(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter the name of a contact person from the sponsoring organisation.", @"Please note this contact person will be sent the invoice.");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(71);

            return question;
        }
        private IFormQuestionBuilder Question12(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Email");
            var questionType = answerType.CreateQuestionType(@"Please enter the email address of the contact person from the sponsoring organisation.", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(72);

            return question;
        }
        private IFormQuestionBuilder Question13(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "DocumentUpload");
            var questionType = answerType.CreateQuestionType(@"", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question14(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you agree to the following <a href=""[[ExternalUrl_NAATI_URL]]/policies/terms-and-conditions/"" target=""_blank"">Terms and Conditions</a>?", @"");
            var option14A = Option14A(questionType);
            var option14B = Option14B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option14A).DoNotStoreAnswer();
            question.WhenSelectedOption(option14B).DoNotStoreAnswer();

            return question;
        }


        private IFormAnswerOptionBuilder Option4A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"OK", @"");
        }
        private IFormAnswerOptionBuilder Option4B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No, I want to modify my logbook before submission.", @"");
        }
        private IFormAnswerOptionBuilder Option5A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"OK, I will attach evidence.", @"");
        }
        private IFormAnswerOptionBuilder Option5B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"I do not have evidence to attach at this time.", @"As you have not met the Profesisonal Development requirements on the LogBook, and have not attached evidence of PD, you will not be able to submit this application.");
        }
        private IFormAnswerOptionBuilder Option6A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"OK, I will attach evidence.", @"");
        }
        private IFormAnswerOptionBuilder Option6B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"I do not have evidence to attach at this time.", @"As you have not met the Work Practice requirements on the LogBook, and have not attached evidence of Work Practice, you will not be able to submit this application.");
        }
        private IFormAnswerOptionBuilder Option7A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"Please note that the ID Card will be posted to the Primary address (displayed at the top of this application form) once the application has been assessed.");
        }
        private IFormAnswerOptionBuilder Option7B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option8A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"Please note that the Translator Stamps will be posted to the Primary address (displayed at the top of this application form) once the application has been assessed.");
        }
        private IFormAnswerOptionBuilder Option8B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option9A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"");
        }
        private IFormAnswerOptionBuilder Option9B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option14A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"Click <b>Finish</b> to submit the application to NAATI.");
        }
        private IFormAnswerOptionBuilder Option14B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @" Click <b>Finish</b>  to terminate the application process.  Data associated with this application will be deleted.");
        }


    }
}
