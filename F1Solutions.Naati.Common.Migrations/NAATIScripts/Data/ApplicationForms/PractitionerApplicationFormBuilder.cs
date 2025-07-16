using System.Linq;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms.Base;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms
{
    public class PractitionerApplicationFormBuilder : IFormBuilderHelper
    {
        private readonly SharedBuilders _sharedBuilders;

        public PractitionerApplicationFormBuilder(SharedBuilders sharedBuilder)
        {
            _sharedBuilders = sharedBuilder;
        }

        public void CreateForm()
        {
            var userType = _sharedBuilders.UserTypes.First(x => x.Name == "PractitionerUser");
            var application = userType.AddForm(6, "Additional Certifications", "Practitioner Certification", false, "certification-practitioner");
            application.DisplayOnlyCredentialTypes(1, 2, 5, 6, 7, 12, 13, 20, 21, 10, 8, 9, 24, 22, 23, 3, 36, 37);
            Sections(application);
        }

        private void Sections(IApplicationFormBuilder application)
        {

            var yourDetailsSectionSection = YourDetailsSection(application);
            var question1 = Question1(yourDetailsSectionSection);
            var question2 = Question2(yourDetailsSectionSection);

            var qualificationsSectionSection = QualificationsSection(application);
            var question3 = Question3(qualificationsSectionSection);

            var credentialsSectionSection = CredentialsSection(application);
            var question4 = Question4(credentialsSectionSection);

            var additionalInformationSection = AdditionalInformationSection(application);
            var question4One = Question4One(additionalInformationSection);

            var prerequisitesSectionSection = PrerequisitesSection(application);
            var question5 = Question5(prerequisitesSectionSection);
            var question6 = Question6(prerequisitesSectionSection);
            var question7 = Question7(prerequisitesSectionSection);
            var question8 = Question8(prerequisitesSectionSection);
            var question9 = Question9(prerequisitesSectionSection);
            var question10 = Question10(prerequisitesSectionSection);
            var question11 = Question11(prerequisitesSectionSection);
            var question12 = Question12(prerequisitesSectionSection);
            var question13 = Question13(prerequisitesSectionSection);
            var question14 = Question14(prerequisitesSectionSection);

            var testLocationSectionSection = TestLocationSection(application);
            var question15 = Question15(testLocationSectionSection);

            var sponsorSectionSection = SponsorSection(application);
            var question16 = Question16(sponsorSectionSection);
            var question17 = Question17(sponsorSectionSection);
            var question18 = Question18(sponsorSectionSection);
            var question19 = Question19(sponsorSectionSection);

            var attachmentsSectionSection = AttachmentsSection(application);
            var question20 = Question20(attachmentsSectionSection);
            var question21 = Question21(attachmentsSectionSection);

            var option5B = question5.Options.ElementAt(1);
            var option6B = question6.Options.ElementAt(1);
            var option16A = question16.Options.ElementAt(0);

            question4One.ShowOnlyIf()
                .Skill(607).IsSelected()
                .Or()
                .Skill(608).IsSelected();

            question5.ShowOnlyIf()
                .CredentialRequestPathType(3).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(8).IsSelected()
                .Or()
                .CredentialType(22).IsSelected();

            question6.ShowOnlyIf()
                .CredentialRequestPathType(3).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(9).IsSelected()
                .Or()
                .CredentialType(23).IsSelected();

            question7.ShowOnlyIf()
                .CredentialRequestPathType(3).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(7).IsSelected()
                .Or()
                .CredentialType(21).IsSelected()
                .Or()
                .CredentialType(3).IsSelected()
                .Or()
                .CredentialType(10).IsSelected()
                .Or()
                .CredentialType(24).IsSelected();

            question8.ShowOnlyIf()
                .CredentialRequestPathType(3).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(8).IsSelected()
                .Or()
                .CredentialType(22).IsSelected()
                .AndGroupFormedBy()
                .Option(option5B).IsSelected();

            question9.ShowOnlyIf()
                .CredentialRequestPathType(3).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(9).IsSelected()
                .Or()
                .CredentialType(23).IsSelected()
                .AndGroupFormedBy()
                .Option(option6B).IsSelected();

            question10.ShowOnlyIf()
                .CredentialRequestPathType(3).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(7).IsSelected()
                .Or()
                .CredentialType(21).IsSelected()
                .Or()
                .CredentialType(3).IsSelected()
                .Or()
                .CredentialType(10).IsSelected()
                .Or()
                .CredentialType(24).IsSelected();

            question11.ShowOnlyIf()
                .CredentialRequestPathType(3).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(8).IsSelected()
                .Or()
                .CredentialType(22).IsSelected()
                .AndGroupFormedBy()
                .Option(option5B).IsSelected();

            question12.ShowOnlyIf()
                .CredentialRequestPathType(3).IsSelected()
                .AndGroupFormedBy()
                .CredentialType(9).IsSelected()
                .Or()
                .CredentialType(23).IsSelected()
                .AndGroupFormedBy()
                .Option(option6B).IsSelected();

            question13.ShowOnlyIf()
                .CredentialType(1).IsSelected()
                .Or()
                .CredentialType(5).IsSelected()
                .Or()
                .CredentialType(12).IsSelected()
                .Or()
                .CredentialType(19).IsSelected();

            question14.ShowOnlyIf()
                .CredentialRequestPathType(2).IsSelected();

            question17.ShowOnlyIf()
                .Option(option16A).IsSelected();

            question18.ShowOnlyIf()
                .Option(option16A).IsSelected();

            question19.ShowOnlyIf()
                .Option(option16A).IsSelected();


        }

        private IFormSectionBuilder YourDetailsSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Your Details", string.Empty);
            return section;
        }
        private IFormSectionBuilder QualificationsSection(IApplicationFormBuilder application)
        {
            var section = application.AddSection(@"Qualifications", string.Empty);
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
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Have you completed a <a href=""[[ExternalUrl_NAATI_URL]]/services/endorsed-qualification/"" target=""_blank"">NAATI Endorsed Qualification</a> (or another qualification) in translating or interpreting which is related to the new certification you are applying for?", @"");
            var option3A = Option3A(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_Applications_URL]]/MyNAATI/Apply/certification");
            var option3B = Option3B(questionType).RequestDocumentWhenSelected(9);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option3A).Store("True").OnField(48);
            question.WhenSelectedOption(option3B).Store("False").OnField(48);

            return question;
        }
        private IFormQuestionBuilder Question4(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "CredentialSelectorUpgradeAndSameLevel");
            var questionType = answerType.CreateQuestionType(@"Please select the Credentials you are seeking.", @"If the Credential or Language you require is not available here, you may need to submit a Certification application instead.");

            var question = section.AddQuestion(questionType);

            return question;
        }

        private IFormQuestionBuilder Question4One(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Please specify your preferred form of written Chinese for this test.", @"");


            var option4OneA = Option4OneA(questionType);
            var option4OneB = Option4OneB(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option4OneA).StoreOption(4).OnField(121);
            question.WhenSelectedOption(option4OneB).StoreOption(5).OnField(121);
            return question;
        }
        private IFormQuestionBuilder Question5(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you hold a bachelors degree (or higher) in a health related field? (e.g. Medicine, Health Science)", @"");
            var option5A = Option5A(questionType);
            var option5B = Option5B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option5A).Store("True").OnField(105);
            question.WhenSelectedOption(option5B).Store("False").OnField(105);

            return question;
        }
        private IFormQuestionBuilder Question6(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you hold a bachelors degree (or higher) in law?", @"");
            var option6A = Option6A(questionType);
            var option6B = Option6B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option6A).Store("True").OnField(106);
            question.WhenSelectedOption(option6B).Store("False").OnField(106);

            return question;
        }
        private IFormQuestionBuilder Question7(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you have work experience in the higher level you are applying for?", @"Practitioners seeking credentials at a higher level need to demonstrate evidence of three year’s work practice (at the appropriate level).");
            var option7A = Option7A(questionType).RequestDocumentWhenSelected(10);
            var option7B = Option7B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/become-certified/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option7A).Store("True").OnField(51);
            question.WhenSelectedOption(option7B).Store("False").OnField(51);

            return question;
        }
        private IFormQuestionBuilder Question8(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you have work experience in the higher level you are applying for?", @"Practitioners seeking credentials at a higher level need to demonstrate evidence of three year’s work practice (at the appropriate level).");
            var option8A = Option8A(questionType).RequestDocumentWhenSelected(10);
            var option8B = Option8B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/certification/the-certification-system/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option8A).Store("True").OnField(51);
            question.WhenSelectedOption(option8B).Store("False").OnField(51);

            return question;
        }
        private IFormQuestionBuilder Question9(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you have work experience in the higher level you are applying for?", @"Practitioners seeking credentials at a higher level need to demonstrate evidence of three year’s work practice (at the appropriate level).");
            var option9A = Option9A(questionType).RequestDocumentWhenSelected(10);
            var option9B = Option9B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/certification/the-certification-system/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option9A).Store("True").OnField(51);
            question.WhenSelectedOption(option9B).Store("False").OnField(51);

            return question;
        }
        private IFormQuestionBuilder Question10(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Can you demonstrate evidence of professional development?", @"Practitioners seeking credentials at a higher level also need to demonstrate evidence of professional development activities to support advanced practice");
            var option10A = Option10A(questionType).RequestDocumentWhenSelected(32);
            var option10B = Option10B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/become-certified/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option10A).Store("True").OnField(54);
            question.WhenSelectedOption(option10B).Store("False").OnField(54);

            return question;
        }
        private IFormQuestionBuilder Question11(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Can you demonstrate evidence of professional development?", @"Practitioners seeking credentials at a higher level also need to demonstrate evidence of professional development activities to support advanced practice");
            var option11A = Option11A(questionType).RequestDocumentWhenSelected(32);
            var option11B = Option11B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/become-certified/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option11A).Store("True").OnField(54);
            question.WhenSelectedOption(option11B).Store("False").OnField(54);

            return question;
        }
        private IFormQuestionBuilder Question12(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Can you demonstrate evidence of professional development?", @"Practitioners seeking credentials at a higher level also need to demonstrate evidence of professional development activities to support advanced practice");
            var option12A = Option12A(questionType).RequestDocumentWhenSelected(32);
            var option12B = Option12B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/certification/the-certification-system/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option12A).Store("True").OnField(54);
            question.WhenSelectedOption(option12B).Store("False").OnField(54);

            return question;
        }
        private IFormQuestionBuilder Question13(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you have work experience in the new language combination you are applying for?", @"Practitioners seeking certification in another language combination need to demonstrate evidence of three year’s work practice (at the appropriate level).");
            var option13A = Option13A(questionType).RequestDocumentWhenSelected(10);
            var option13B = Option13B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option13A).Store("True").OnField(52);
            question.WhenSelectedOption(option13B).Store("False").OnField(52);

            return question;
        }
        private IFormQuestionBuilder Question14(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you meet the translating or interpreting  <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">training requirements</a>?", @"");
            var option14A = Option14A(questionType).RequestDocumentWhenSelected(12);
            var option14B = Option14B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option14A).Store("True").OnField(53);
            question.WhenSelectedOption(option14B).Store("False").OnField(53);

            return question;
        }
        private IFormQuestionBuilder Question15(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "TestLocation");
            var questionType = answerType.CreateQuestionType(@"Please select your preferred test location.", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question16(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Will a third-party organisation be <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/submit-an-application/"" target=""_blank"">sponsoring</a> your application?", @"You will need to attach a Purchase Order from your employer or another document from your employer approving this transaction.");
            var option16A = Option16A(questionType).RequestDocumentWhenSelected(24);
            var option16B = Option16B(questionType);

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option16A).Store("True").OnField(55);
            question.WhenSelectedOption(option16B).Store("False").OnField(55);

            return question;
        }
        private IFormQuestionBuilder Question17(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter the name of the sponsoring organisation.", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(56);

            return question;
        }
        private IFormQuestionBuilder Question18(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Input");
            var questionType = answerType.CreateQuestionType(@"Please enter the name of a contact person from the sponsoring organisation.", @"Please note this contact person will be sent the invoice.");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(57);

            return question;
        }
        private IFormQuestionBuilder Question19(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "Email");
            var questionType = answerType.CreateQuestionType(@"Please enter the email address of the contact person from the sponsoring organisation.", @"");

            var question = section.AddQuestion(questionType);
            question.StoreReponseOnField(58);

            return question;
        }
        private IFormQuestionBuilder Question20(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "DocumentUpload");
            var questionType = answerType.CreateQuestionType(@"", @"");

            var question = section.AddQuestion(questionType);

            return question;
        }
        private IFormQuestionBuilder Question21(IFormSectionBuilder section)
        {
            var answerType = _sharedBuilders.AnswerTypes.First(x => x.Name == "RadioOptions");
            var questionType = answerType.CreateQuestionType(@"Do you agree to the <a href=""[[ExternalUrl_NAATI_URL]]/policies/terms-and-conditions/"" target=""_blank"">Terms and Conditions</a>?", @"");
            var option21A = Option21A(questionType);
            var option21B = Option21B(questionType).ExecuteActionWhenSelected(_sharedBuilders.ActionTypes.First(x => x.Name == "Delete"), @"[[ExternalUrl_Applications_URL]]");

            var question = section.AddQuestion(questionType);
            question.WhenSelectedOption(option21A).DoNotStoreAnswer();
            question.WhenSelectedOption(option21B).DoNotStoreAnswer();

            return question;
        }


        private IFormAnswerOptionBuilder Option3A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to apply using the Certification application form instead.  Click <b>Finish</b> to be redirected to the correct form.");
        }
        private IFormAnswerOptionBuilder Option3B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }

        private IFormAnswerOptionBuilder Option4OneA(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Simplified Chinese", @"");
        }
        private IFormAnswerOptionBuilder Option4OneB(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Traditional Chinese", @"");
        }

        private IFormAnswerOptionBuilder Option5A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"");
        }
        private IFormAnswerOptionBuilder Option5B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option6A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"");
        }
        private IFormAnswerOptionBuilder Option6B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option7A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach evidence of your <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">work experience</a> or attach a document stating why you should be exempt at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option7B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you cannot provide evidence of work experience, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option8A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach evidence of your <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">work experience</a> or attach a document stating why you should be exempt at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option8B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you cannot provide evidence of work experience, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option9A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach evidence of your <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">work experience</a> or attach a document stating why you should be exempt at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option9B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you cannot provide evidence of work experience, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option10A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach evidence of your <a href=""[[ExternalUrl_NAATI_URL]]/certification/certification-prerequisites/training/"" target=""_blank"">professional development</a> or attach a document stating why you should be exempt at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option10B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you cannot provide evidence of professional development, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option11A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach evidence of your <a href=""[[ExternalUrl_NAATI_URL]]/certification/certification-prerequisites/training/"" target=""_blank"">professional development</a> or attach a document stating why you should be exempt at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option11B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you cannot provide evidence of professional development, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option12A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach evidence of your <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">professional development</a> or attach a document stating why you should be exempt at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option12B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you cannot provide evidence of professional development, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option13A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach evidence of your <a href=""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"" target=""_blank"">work experience</a> or attach a document stating why you should be exempt at the end of this application form.
");
        }
        private IFormAnswerOptionBuilder Option13B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you cannot provide evidence of work experience, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option14A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"You will need to attach evidence of your <a href=""""[[ExternalUrl_NAATI_URL]]/become-certified/how-do-i-become-certified/"""" target=""""_blank"""">training</a> or attach a document stating why you should be exempt at the end of this application form.");
        }
        private IFormAnswerOptionBuilder Option14B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"If you cannot provide evidence of training, you will not be able to submit this application.  Click <b>Finish</b> to terminate the application process and be redirected to the website containing more information.");
        }
        private IFormAnswerOptionBuilder Option16A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"");
        }
        private IFormAnswerOptionBuilder Option16B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"");
        }
        private IFormAnswerOptionBuilder Option21A(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"Yes", @"");
        }
        private IFormAnswerOptionBuilder Option21B(IFormQuestionTypeBuilder questionType)
        {
            return questionType.AddOption(@"No", @"Click <b>Finish</b> to terminate the application process.  Data associated with this application will be deleted.");
        }


    }
}
